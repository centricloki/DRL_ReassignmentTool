namespace DRL.Entity
{
    public class ENTDerivedManagerResponse
    {
        public long ManagerId { get; set; }
        public string ManagerName { get; set; }
        public bool IsFallback { get; set; }
    }

    public class ENTDerivedManagerRequest
    {
        public int RoleId { get; set; }
        public int DefTerritoryId { get; set; }
    }
}