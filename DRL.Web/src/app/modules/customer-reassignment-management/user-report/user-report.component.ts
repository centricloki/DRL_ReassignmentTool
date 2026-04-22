import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CustomerimportService, UserReportHierarchyNode } from '../customerimport.service';

export interface UserReportFlatRow {
  node: UserReportHierarchyNode;
  depth: number;
}

/** Mirrors prototype ROLES — colors + hierarchy level for collapse defaults & column rules */
const ROLE_STYLE: { [k: string]: { color: string; bg: string; level: number } } = {
  'AVP': { color: '#d97706', bg: '#fffbeb', level: 1 },
  'Zone Manager': { color: '#e306e7', bg: '#fee8fa', level: 2 },
  'Region Manager': { color: '#16a34a', bg: '#f0fdf4', level: 3 },
  'BD Manager': { color: '#7c3aed', bg: '#f5f3ff', level: 3 },
  'Territory Manager': { color: '#0284c7', bg: '#f0f9ff', level: 4 }
};

@Component({
  selector: 'app-user-report',
  templateUrl: './user-report.component.html',
  styleUrls: ['./user-report.component.css']
})
export class UserReportComponent implements OnInit, OnDestroy {

  rootTree: UserReportHierarchyNode[] = [];
  visibleRows: UserReportFlatRow[] = [];
  visibleCount = 0;
  selectedNode: UserReportHierarchyNode | null = null;
  detailPanelOpen = false;
  isLoading = false;

  collapsedIds: { [nodeId: string]: boolean } = {};
  private nodeById: { [nodeId: string]: UserReportHierarchyNode } = {};

  /** Text filters */
  filterName = '';
  filterTerritory = '';
  /** Exact-match dropdowns (empty = all) */
  filterRole = '';
  filterAvp = '';
  filterZone = '';
  filterRegion = '';
  filterBd = '';

  optionsRole: string[] = [];
  optionsAvp: string[] = [];
  optionsZone: string[] = [];
  optionsRegion: string[] = [];
  optionsBd: string[] = [];

  private unsubscribe$ = new Subject<void>();

  constructor(private customerimportService: CustomerimportService) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.loadHierarchy();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  loadHierarchy(): void {
    this.customerimportService.getUserReportHierarchyData().pipe(
      takeUntil(this.unsubscribe$)
    ).subscribe({
      next: (res) => {
        this.rootTree = res.data || [];
        this.rebuildNodeIndex();
        this.populateFilterOptions();
        this.applyDefaultExpandAll();
        this.recomputeView();
        this.isLoading = false;
        this.selectedNode = null;
        this.detailPanelOpen = false;
      },
      error: (err) => {
        console.error('User report hierarchy load failed', err);
        this.isLoading = false;
      }
    });
  }

  private rebuildNodeIndex(): void {
    this.nodeById = {};
    this.walkIndex(this.rootTree);
  }

  private walkIndex(nodes: UserReportHierarchyNode[]): void {
    for (const n of nodes) {
      this.nodeById[n.nodeId] = n;
      if (n.children && n.children.length) {
        this.walkIndex(n.children);
      }
    }
  }

  /** Collect distinct values for column dropdowns from full tree */
  private populateFilterOptions(): void {
    const roles = new Set<string>();
    const avps = new Set<string>();
    const zones = new Set<string>();
    const regions = new Set<string>();
    const bds = new Set<string>();

    const visit = (nodes: UserReportHierarchyNode[]) => {
      for (const n of nodes) {
        if (n.roleName) {
          roles.add(n.roleName.trim());
        }
        if (n.avpFilter && n.avpFilter.trim()) {
          avps.add(n.avpFilter.trim());
        }
        if (n.zoneName) {
          const zoneNames = n.zoneName.split(',').map((s) => s.trim()).filter(Boolean);
          for (const zoneName of zoneNames) {
            zones.add(zoneName);
          }
        }
        if (n.regionName && n.regionName.trim()) {
          const regionNames = n.regionName.split(',').map((s) => s.trim()).filter(Boolean);
          for (const regionName of regionNames) {
            regions.add(regionName);
          }
        }
        if (n.bdFilter && n.bdFilter.trim()) {
          bds.add(n.bdFilter.trim());
        }
        if (n.children && n.children.length) {
          visit(n.children);
        }
      }
    };
    visit(this.rootTree);

    // Sort all options alphabetically by name
    this.optionsRole = Array.from(roles).sort((a, b) => a.localeCompare(b));
    this.optionsAvp = Array.from(avps).sort((a, b) => a.localeCompare(b));
    this.optionsZone = Array.from(zones).sort((a, b) => a.localeCompare(b));
    this.optionsRegion = Array.from(regions).sort((a, b) => a.localeCompare(b));
    this.optionsBd = Array.from(bds).sort((a, b) => a.localeCompare(b));
  }

  /** Collapse all items on landing */
  private applyDefaultExpandAll(): void {
    this.collapsedIds = {};
    const walk = (nodes: UserReportHierarchyNode[]) => {
      for (const n of nodes) {
        if (n.children && n.children.length) {
          this.collapsedIds[n.nodeId] = false;
          walk(n.children);
        }
      }
    };
    walk(this.rootTree);
  }

  recomputeView(): void {
    this.visibleRows = this.flattenWithVisibility(this.rootTree, 0);
    this.visibleCount = this.visibleRows.length;
  }

  private hasActiveFilters(): boolean {
    return !!(this.filterName || this.filterTerritory || this.filterRole || this.filterAvp
      || this.filterZone || this.filterRegion || this.filterBd);
  }

  onFilterChange(): void {
    this.recomputeView();
  }

  clearFilters(): void {
    this.filterName = '';
    this.filterTerritory = '';
    this.filterRole = '';
    this.filterAvp = '';
    this.filterZone = '';
    this.filterRegion = '';
    this.filterBd = '';
    this.recomputeView();
  }

  private matchesFilters(n: UserReportHierarchyNode): boolean {
    if (!this.hasActiveFilters()) {
      return true;
    }
    const name = (n.fullName || '').toLowerCase();
    if (this.filterName && name.indexOf(this.filterName.toLowerCase()) < 0) {
      return false;
    }
    if (this.filterRole && (n.roleName || '').trim() !== this.filterRole) {
      return false;
    }
    if (this.filterAvp && (n.avpFilter || '').trim() !== this.filterAvp) {
      return false;
    }
    if (this.filterZone) {
      const zones = (n.zoneName || '').split(',').map((s) => s.trim()).filter(Boolean);
      if (zones.indexOf(this.filterZone) < 0) {
        return false;
      }
    }
    if (this.filterRegion) {
      const regions = (n.regionName || '').split(',').map((s) => s.trim()).filter(Boolean);
      if (regions.indexOf(this.filterRegion) < 0) {
        return false;
      }
    }
    if (this.filterBd && (n.bdFilter || '').trim() !== this.filterBd) {
      return false;
    }
    if (this.filterTerritory) {
      const q = this.filterTerritory.toLowerCase();
      const ids = n.territoryIds || [];
      const names = n.territoryNames || [];
      const raw = (n.territoryName || '').toLowerCase();
      let hit = raw.indexOf(q) >= 0;
      for (const t of ids) {
        if (t && t.toLowerCase().indexOf(q) >= 0) {
          hit = true;
          break;
        }
      }
      if (!hit) {
        for (const t of names) {
          if (t && t.toLowerCase().indexOf(q) >= 0) {
            hit = true;
            break;
          }
        }
      }
      if (!hit) {
        return false;
      }
    }
    return true;
  }

  /** Check if row should be visible based on filters and collapse state */
  private isRowVisible(n: UserReportHierarchyNode): boolean {
    if (!this.matchesFilters(n)) {
      return false;
    }
    // When filters are active, only check collapse state of visible ancestors
    // When no filters, check all ancestors
    return !this.hasVisibleCollapsedAncestor(n);
  }

  private hasVisibleCollapsedAncestor(n: UserReportHierarchyNode): boolean {
    let pid: string | null = n.parentNodeId;
    while (pid) {
      const p = this.nodeById[pid];
      if (!p) {
        break;
      }
      // If this ancestor is collapsed AND visible (matches filters), then hide children
      if (this.collapsedIds[pid] && this.matchesFilters(p)) {
        return true;
      }
      pid = p.parentNodeId || null;
    }
    return false;
  }

  private flattenWithVisibility(nodes: UserReportHierarchyNode[], depth: number): UserReportFlatRow[] {
    const rows: UserReportFlatRow[] = [];
    for (const n of nodes) {
      if (this.isRowVisible(n)) {
        rows.push({ node: n, depth });
      }
      if (n.children && n.children.length) {
        rows.push(...this.flattenWithVisibility(n.children, depth + 1));
      }
    }
    return rows;
  }

  toggleExpand(node: UserReportHierarchyNode, ev: MouseEvent): void {
    ev.stopPropagation();
    if (!node.children || !node.children.length) {
      return;
    }
    if (this.collapsedIds[node.nodeId]) {
      delete this.collapsedIds[node.nodeId];
    } else {
      this.collapsedIds[node.nodeId] = true;
    }
    this.recomputeView();
  }

  expandAll(): void {
    this.collapsedIds = {};
    this.recomputeView();
  }

  collapseAll(): void {
    const bag: { [id: string]: boolean } = {};
    if (this.hasActiveFilters()) {
      // When filters are active, only collapse visible nodes that have children
      // This prevents non-visible ancestors from hiding the filtered results
      for (const row of this.visibleRows) {
        if (row.node.children && row.node.children.length) {
          bag[row.node.nodeId] = true;
        }
      }
    } else {
      // When no filters, collapse all expandable nodes in the full tree
      this.collectExpandableIds(this.rootTree, bag);
    }
    this.collapsedIds = bag;
    this.recomputeView();
  }

  private collectExpandableIds(nodes: UserReportHierarchyNode[], bag: { [id: string]: boolean }): void {
    for (const n of nodes) {
      if (n.children && n.children.length) {
        bag[n.nodeId] = true;
        this.collectExpandableIds(n.children, bag);
      }
    }
  }

  openDetail(node: UserReportHierarchyNode, ev?: MouseEvent): void {
    if (ev) {
      ev.stopPropagation();
    }
    this.selectedNode = node;
    this.detailPanelOpen = true;
  }

  closeDetail(): void {
    this.detailPanelOpen = false;
    this.selectedNode = null;
  }

  rowClick(node: UserReportHierarchyNode): void {
    this.openDetail(node);
  }

  selectDirectReport(node: UserReportHierarchyNode): void {
    this.openDetail(node);
  }

  parentNode(): UserReportHierarchyNode | null {
    if (!this.selectedNode || !this.selectedNode.parentNodeId) {
      return null;
    }
    return this.nodeById[this.selectedNode.parentNodeId] || null;
  }

  /** Template alias for parent row */
  get reportParent(): UserReportHierarchyNode | null {
    // First, try to get the manager information from the selected node
    if (this.selectedNode &&
      (this.selectedNode.managerFullName ||
        this.selectedNode.managerZoneName ||
        this.selectedNode.managerRegionName ||
        this.selectedNode.managerTerritoryName)) {

      // Determine the manager's role based on available data hierarchy
      // Priority: Use API-provided managerRoleName first, then infer from data
      let managerRoleName = this.selectedNode.managerRoleName;
      if (!managerRoleName) {
        // Try to extract from name first
        const nameLower = (this.selectedNode.managerFullName || '').toLowerCase();
        if (nameLower.includes('avp')) {
          managerRoleName = 'AVP';
        } else if (nameLower.includes('zone')) {
          managerRoleName = 'Zone Manager';
        } else if (nameLower.includes('region')) {
          managerRoleName = 'Region Manager';
        } else if (nameLower.includes('bd')) {
          managerRoleName = 'BD Manager';
        } else if (nameLower.includes('territory')) {
          managerRoleName = 'Territory Manager';
        } else {
          // Fallback: determine role based on available data fields
          // Check in reverse hierarchy order (lowest to highest)
          if (this.selectedNode.managerAvpName && this.selectedNode.managerAvpName.trim()) {
            managerRoleName = 'AVP';
          } else if (this.selectedNode.managerBdName && this.selectedNode.managerBdName.trim()) {
            managerRoleName = 'BD Manager';
          } else if (this.selectedNode.managerTerritoryName && this.selectedNode.managerTerritoryName.trim()) {
            managerRoleName = 'Territory Manager';
          } else if (this.selectedNode.managerRegionName && this.selectedNode.managerRegionName.trim()) {
            managerRoleName = 'Region Manager';
          } else if (this.selectedNode.managerZoneName && this.selectedNode.managerZoneName.trim()) {
            managerRoleName = 'Zone Manager';
          } else {
            managerRoleName = 'Manager';
          }
        }
      }

      // Create a virtual node representing the manager
      const managerNode: UserReportHierarchyNode = {
        nodeId: 'manager_' + this.selectedNode.nodeId,
        parentNodeId: null,
        userId: 0,
        userName: this.selectedNode.managerUserName || '',
        fullName: this.selectedNode.managerFullName || 'Vacant',
        roleName: managerRoleName,
        roleId: this.selectedNode.managerRoleId || '',
        zoneName: this.selectedNode.managerZoneName || '',
        zoneId: this.selectedNode.managerZoneId || '',
        regionName: this.selectedNode.managerRegionName || '',
        regionId: this.selectedNode.managerRegionId || '',
        territoryName: this.selectedNode.managerTerritoryName || '',
        territoryId: this.selectedNode.managerTerritoryId || '',
        territoryNames: this.selectedNode.managerTerritoryName ?
          this.selectedNode.managerTerritoryName.split(',').map(s => s.trim()).filter(Boolean) : [],
        territoryIds: this.selectedNode.managerTerritoryId ?
          this.selectedNode.managerTerritoryId.split(',').map(s => s.trim()).filter(Boolean) : [],
        territoryDetailIds: this.selectedNode.managerTerritoryId ?
          this.selectedNode.managerTerritoryId.split(',').map(s => s.trim()).filter(Boolean) : [],
        entityId: '',
        bdId: this.selectedNode.managerBdId || '',
        bdName: this.selectedNode.managerBdName || '',
        avpName: this.selectedNode.managerAvpName || '',
        avpId: this.selectedNode.managerAvpId || '',
        territoryFilterText: '',
        avpFilter: this.selectedNode.managerAvpName || '',
        zoneFilter: this.selectedNode.managerZoneName || '',
        regionFilter: this.selectedNode.managerRegionName || '',
        bdFilter: this.selectedNode.managerBdName || '',
        avpDetail: this.selectedNode.managerAvpName || '',
        zoneDetail: this.selectedNode.managerZoneName || '',
        regionDetail: this.selectedNode.managerRegionName || '',
        bdDetail: this.selectedNode.managerBdName || '',
        children: [],
        managerFullName: undefined,
        managerZoneName: undefined,
        managerZoneId: undefined,
        managerRegionName: undefined,
        managerRegionId: undefined,
        managerTerritoryName: undefined,
        managerTerritoryId: undefined
      };

      return managerNode;
    }

    // Fallback to the parent node if no manager info is available
    return this.parentNode();
  }

  indentArray(depth: number): number[] {
    const d = Math.max(0, depth);
    const arr: number[] = [];
    for (let i = 0; i < d; i++) {
      arr.push(i);
    }
    return arr;
  }

  /** Check if Reports To section has meaningful data to display */
  hasMeaningfulReportParent(): boolean {
    if (!this.selectedNode) {
      return false;
    }

    // Check if there's actual manager data from API (not just empty/vacant/TBD)
    // Must have a non-empty, non-TBD, non-Vacant manager name
    const hasValidManagerName = this.selectedNode.managerFullName &&
      this.selectedNode.managerFullName.trim().length > 0 &&
      this.selectedNode.managerFullName.trim().toLowerCase() !== 'tbd' &&
      this.selectedNode.managerFullName.trim().toLowerCase() !== 'vacant';

    return hasValidManagerName;
    // if (hasValidManagerName) {
    //   return true;
    // }

    // // If no manager data from API, check if there's a valid parent node in hierarchy
    // const parentNode = this.parentNode();
    // if (parentNode) {
    //   // Only show parent node if it has meaningful data (not vacant/TBD)
    //   const hasValidParentName = parentNode.fullName &&
    //                              parentNode.fullName.trim() &&
    //                              parentNode.fullName.trim().toLowerCase() !== 'tbd' &&
    //                              parentNode.fullName.trim().toLowerCase() !== 'vacant';
    //   if (hasValidParentName) {
    //     return true;
    //   }
    // }

    // return false;
  }

  directReports(): UserReportHierarchyNode[] {
    if (!this.selectedNode || !this.selectedNode.children) {
      return [];
    }
    return this.selectedNode.children;
  }

  territoryDisplay(n: UserReportHierarchyNode): string {
    if (n.territoryNames && n.territoryNames.length) {
      return n.territoryNames.join(', ');
    }
    return n.territoryName || '';
  }

  /** Territory codes for chips — prefer ids from API */
  territoryCodes(n: UserReportHierarchyNode): string[] {
    if (n.territoryIds && n.territoryIds.length) {
      return n.territoryIds.slice();
    }
    if (n.territoryNames && n.territoryNames.length) {
      return n.territoryNames.slice();
    }
    if (n.territoryName) {
      return n.territoryName.split(',').map((s) => s.trim()).filter(Boolean);
    }
    return [];
  }

  territoryVisibleCodes(n: UserReportHierarchyNode): string[] {
    return this.territoryCodes(n).slice(0, 3);
  }

  territoryRestCodes(n: UserReportHierarchyNode): string[] {
    return this.territoryCodes(n).slice(3);
  }

  territoryRestTooltip(n: UserReportHierarchyNode): string {
    return this.territoryRestCodes(n).join(', ');
  }

  /** Territory grid display - names only, no IDs */
  territoryGridDisplay(n: UserReportHierarchyNode): string[] {
    return n.territoryNames || [];
  }

  territoryGridVisible(n: UserReportHierarchyNode): string[] {
    return this.territoryGridDisplay(n).slice(0, 3);
  }

  territoryGridRest(n: UserReportHierarchyNode): string[] {
    return this.territoryGridDisplay(n).slice(3);
  }

  territoryGridRestTooltip(n: UserReportHierarchyNode): string {
    return this.territoryGridRest(n).join(', ');
  }

  /** Territory detail with IDs for detail panel */
  territoryFormatted(n: UserReportHierarchyNode): string[] {
    const names = n.territoryNames || [];
    const ids = n.territoryDetailIds || n.territoryIds || [];
    const result: string[] = [];
    for (let i = 0; i < names.length; i++) {
      const name = names[i];
      const id = i < ids.length ? ids[i] : '';
      if (id && id !== '0') {
        result.push(name + ' (' + id + ')');
      } else {
        result.push(name);
      }
    }
    return result;
  }

  territoryVisibleFormatted(n: UserReportHierarchyNode): string[] {
    return this.territoryFormatted(n).slice(0, 3);
  }

  territoryRestFormatted(n: UserReportHierarchyNode): string[] {
    return this.territoryFormatted(n).slice(3);
  }

  territoryRestTooltipFormatted(n: UserReportHierarchyNode): string {
    return this.territoryRestFormatted(n).join(', ');
  }

  hasChildren(node: UserReportHierarchyNode): boolean {
    return !!(node.children && node.children.length);
  }

  isCollapsed(node: UserReportHierarchyNode): boolean {
    return !!this.collapsedIds[node.nodeId];
  }

  getRoleLevel(roleName: string): number {
    const r = (roleName || '').trim();
    if (ROLE_STYLE[r]) {
      return ROLE_STYLE[r].level;
    }
    if (r === 'AVP' || r.indexOf('AVP') >= 0) {
      return 1;
    }
    if (r.indexOf('Zone') >= 0) {
      return 2;
    }
    if (r.indexOf('Region') >= 0) {
      return 3;
    }
    if (r.indexOf('BD') >= 0 && r.indexOf('Manager') >= 0) {
      return 3;
    }
    if (r.indexOf('Territory') >= 0) {
      return 4;
    }
    return 99;
  }

  getRoleStyle(roleName: string): { color: string; bg: string } {
    const r = (roleName || '').trim();
    if (ROLE_STYLE[r]) {
      const x = ROLE_STYLE[r];
      return { color: x.color, bg: x.bg };
    }
    return { color: '#888888', bg: '#f5f5f5' };
  }

  initials(name: string): string {
    if (!name || !name.trim()) {
      return '?';
    }
    const parts = name.trim().split(/\s+/).slice(0, 2);
    return parts.map((p) => p.charAt(0)).join('').toUpperCase();
  }

  /** Zone display using formatted zoneFilter */
  zonePill(n: UserReportHierarchyNode): string {
    if (!n.zoneFilter || !n.zoneFilter.trim()) {
      return '';
    }
    return n.zoneFilter.trim();
  }

  /** Zone grid display as badges */
  zoneGridDisplay(n: UserReportHierarchyNode): string[] {
    return n.zoneName ? n.zoneName.split(',').map(s => s.trim()).filter(Boolean) : [];
  }

  zoneGridVisible(n: UserReportHierarchyNode): string[] {
    return this.zoneGridDisplay(n).slice(0, 3);
  }

  zoneGridRest(n: UserReportHierarchyNode): string[] {
    return this.zoneGridDisplay(n).slice(3);
  }

  zoneGridRestTooltip(n: UserReportHierarchyNode): string {
    return this.zoneGridRest(n).join(', ');
  }

  /** AVP column: level >= 1 and avpFilter present */
  showAvpCell(n: UserReportHierarchyNode): boolean {
    return this.getRoleLevel(n.roleName) >= 1 && !!(n.avpFilter && n.avpFilter.trim());
  }

  /** AVP display using formatted avpFilter */
  avpPill(n: UserReportHierarchyNode): string {
    if (!n.avpFilter || !n.avpFilter.trim()) {
      return '';
    }
    return n.avpFilter.trim();
  }

  /** BD column: BD Manager shows bdFilter; Territory Manager with bdFilter */
  bdPillText(n: UserReportHierarchyNode): string {
    const r = n.roleName || '';
    if ((r.indexOf('BD') >= 0 && r.indexOf('Manager') >= 0) || r.indexOf('Territory') >= 0) {
      if (n.bdFilter && n.bdFilter.trim()) {
        return n.bdFilter.trim();
      }
    }
    return '';
  }

  /** Region display using formatted regionFilter */
  regionPill(n: UserReportHierarchyNode): string {
    if (!n.regionFilter || !n.regionFilter.trim()) {
      return '';
    }
    return n.regionFilter.trim();
  }

  /** Region grid display as badges */
  regionGridDisplay(n: UserReportHierarchyNode): string[] {
    return n.regionName ? n.regionName.split(',').map(s => s.trim()).filter(Boolean) : [];
  }

  regionGridVisible(n: UserReportHierarchyNode): string[] {
    return this.regionGridDisplay(n).slice(0, 3);
  }

  regionGridRest(n: UserReportHierarchyNode): string[] {
    return this.regionGridDisplay(n).slice(3);
  }

  regionGridRestTooltip(n: UserReportHierarchyNode): string {
    return this.regionGridRest(n).join(', ');
  }

  /** Territory detail with IDs for detail panel */
  territoryDetail(n: UserReportHierarchyNode): string[] {
    const names = n.territoryNames || [];
    const ids = n.territoryDetailIds || n.territoryIds || [];
    const result: string[] = [];
    for (let i = 0; i < names.length; i++) {
      const name = names[i];
      const id = i < ids.length ? ids[i] : '';
      if (id && id !== '0') {
        result.push(name + ' (' + id + ')');
      } else {
        result.push(name);
      }
    }
    return result;
  }

  zoneDetailFormatted(n: UserReportHierarchyNode): string[] {
    const zoneNames = n.zoneName ? n.zoneName.split(',').map(s => s.trim()).filter(Boolean) : [];
    const zoneIds = n.zoneId ? n.zoneId.split(',').map(s => s.trim()).filter(Boolean) : [];
    const result: string[] = [];
    for (let i = 0; i < zoneNames.length; i++) {
      const name = zoneNames[i];
      const id = i < zoneIds.length ? zoneIds[i] : '';
      if (id && id !== '0') {
        result.push(name + ' (' + id + ')');
      } else {
        result.push(name);
      }
    }
    return result;
  }

  regionDetailFormatted(n: UserReportHierarchyNode): string[] {
    const regionNames = n.regionName ? n.regionName.split(',').map(s => s.trim()).filter(Boolean) : [];
    const regionIds = n.regionId ? n.regionId.split(',').map(s => s.trim()).filter(Boolean) : [];
    const result: string[] = [];
    for (let i = 0; i < regionNames.length; i++) {
      const name = regionNames[i];
      const id = i < regionIds.length ? regionIds[i] : '';
      if (id && id !== '0') {
        result.push(name + ' (' + id + ')');
      } else {
        result.push(name);
      }
    }
    return result;
  }

  trackRow(index: number, row: UserReportFlatRow): string {
    return row.node.nodeId + '-' + String(row.depth) + '-' + String(index);
  }

  exportCsv(): void {
    const headers = ['Name', 'Username', 'Role', 'AVP', 'Zone', 'Region', 'BD', 'Territories'];
    const lines: string[] = [];
    lines.push(headers.map((h) => this.escapeCsvField(h)).join(','));

    // Export all data instead of just visible rows
    this.walkAndExportNodes(this.rootTree, lines);

    const blob = new Blob([lines.join('\r\n')], { type: 'text/csv;charset=utf-8' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'user_report.csv';
    a.click();
    window.URL.revokeObjectURL(url);
  }

  private walkAndExportNodes(nodes: UserReportHierarchyNode[], lines: string[]): void {
    for (const n of nodes) {
      const row = [
        n.fullName,
        n.userName,
        n.roleName,
        n.avpFilter || '',
        n.zoneFilter || '',
        n.regionFilter || '',
        n.bdFilter || '',
        this.territoryDisplay(n)
      ];
      lines.push(row.map((c) => this.escapeCsvField(c == null ? '' : String(c))).join(','));

      // Process children recursively
      if (n.children && n.children.length) {
        this.walkAndExportNodes(n.children, lines);
      }
    }
  }

  private escapeCsvField(value: string): string {
    if (value.indexOf('"') >= 0 || value.indexOf(',') >= 0 || value.indexOf('\n') >= 0 || value.indexOf('\r') >= 0) {
      return '"' + value.replace(/"/g, '""') + '"';
    }
    return value;
  }

  getRoleDetailText(n: UserReportHierarchyNode): string {
    const role = n.roleName || '';
    if (role.indexOf('AVP') >= 0) {
      return n.avpName || n.zoneName || '';
    } else if (role.indexOf('Zone') >= 0) {
      return n.zoneName ? n.zoneName.split(',')[0].trim() : '';
    } else if (role.indexOf('Region') >= 0) {
      return n.regionName || '';
    } else if (role.indexOf('BD') >= 0) {
      return n.bdName || '';
    } else if (role.indexOf('Territory') >= 0) {
      return this.territoryNames(n).join(', ') || '';
    }
    // For generic "Manager" role (e.g., manager from API), try to show role-specific info
    if (n.avpName && n.avpName.trim()) {
      return n.avpName.trim();
    }
    if (n.bdName && n.bdName.trim()) {
      return n.bdName.trim();
    }
    if (n.zoneName && n.zoneName.trim()) {
      return n.zoneName.split(',')[0].trim();
    }
    if (n.regionName && n.regionName.trim()) {
      return n.regionName.trim();
    }
    return '';
  }

  territoryNames(n: UserReportHierarchyNode): string[] {
    return n.territoryNames || [];
  }

  getRoleDetailStyle(n: UserReportHierarchyNode): { color: string; bg?: string } {
    const role = n.roleName || '';
    if (role.indexOf('AVP') >= 0) {
      return { color: ROLE_STYLE['AVP'].color };
    } else if (role.indexOf('Zone') >= 0) {
      return { color: ROLE_STYLE['Zone Manager'].color };
    } else if (role.indexOf('Region') >= 0) {
      return { color: ROLE_STYLE['Region Manager'].color };
    } else if (role.indexOf('BD') >= 0) {
      return { color: ROLE_STYLE['BD Manager'].color };
    } else if (role.indexOf('Territory') >= 0) {
      return { color: ROLE_STYLE['Territory Manager'].color };
    }
    // For generic "Manager" role, use style based on available data (check in priority order)
    if (n.avpName && n.avpName.trim()) {
      return { color: ROLE_STYLE['AVP'].color };
    }
    if (n.bdName && n.bdName.trim()) {
      return { color: ROLE_STYLE['BD Manager'].color };
    }
    if (n.zoneName && n.zoneName.trim()) {
      return { color: ROLE_STYLE['Zone Manager'].color };
    }
    if (n.regionName && n.regionName.trim()) {
      return { color: ROLE_STYLE['Region Manager'].color };
    }
    return { color: '#888888' };
  }
}