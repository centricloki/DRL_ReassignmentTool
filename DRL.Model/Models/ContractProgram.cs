using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class ContractProgram
    {
        public long ContractProgramId { get; set; }
        public string Name { get; set; }
        public long? CustomerId { get; set; }
        public int? Number { get; set; }
        public string Plan { get; set; }
        public string Year { get; set; }
        public DateTime? Effective { get; set; }
        public int? Payment1Request { get; set; }
        public int? Payment1CheckNo { get; set; }
        public decimal? Payment1Amount { get; set; }
        public DateTime? Payment1Date { get; set; }
        public int? Payment2Request { get; set; }
        public int? Payment2CheckNo { get; set; }
        public decimal? Payment2Amount { get; set; }
        public DateTime? Payment2Date { get; set; }
        public string Description { get; set; }
        public long? UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string SugarId { get; set; }
        public bool IsExported { get; set; }
    }
}
