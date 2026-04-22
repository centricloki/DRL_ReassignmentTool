using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Library
{
    public static class LookupCacheKeys
    {
        // Cache configuration
        public const int CACHE_MINUTES = 15;
        public const string ROLES_KEY = "lookup_roles";
        public const string REGIONS_KEY = "lookup_regions";
        public const string ZONES_KEY = "lookup_zones";
        public const string TERRITORIES_KEY = "lookup_territories";
        public const string STATES_KEY = "lookup_states";
        public const string CUST_REASSIGN_ROLES_KEY = "lookup_cust_reassign_roles";
        public const string AVPS_KEY = "lookup_avps";
        public const string BDS_KEY = "lookup_bds";
        public const string CITIES_KEY_PREFIX = "lookup_cities";
        public const string USER_REPORT_CACHE_PREFIX = "lookup_userreport_";
        /// <summary>Separate prefix so hierarchy response keys do not collide with per-node legacy cache keys.</summary>
        public const string USER_REPORT_HIERARCHY_CACHE_PREFIX = "lookup_userreport_hierarchy_";
    }
}
