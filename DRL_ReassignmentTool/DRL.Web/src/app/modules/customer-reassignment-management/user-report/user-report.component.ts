/** Check if Reports To section has meaningful data to display */
  hasMeaningfulReportParent(): boolean {
    if (!this.selectedNode) {
      return false;
    }
    
    // Check if there's actual manager data from API (not just empty/vacant/TBD)
    const managerFullName = this.selectedNode.managerFullName?.trim();
    if (managerFullName && 
        managerFullName !== 'TBD' && 
        managerFullName !== 'Vacant') {
      return true;
    }
    
    // If no manager data from API, check if there's a valid parent node in hierarchy
    const parentNode = this.parentNode();
    if (parentNode) {
      // Only show parent node if it has meaningful data (not vacant/TBD)
      const parentFullName = parentNode.fullName?.trim();
      return !!(parentFullName && 
                parentFullName !== 'TBD' && 
                parentFullName !== 'Vacant');
    }
    
    return false;
  }