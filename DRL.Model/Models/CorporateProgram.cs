using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CorporateProgram
    {
        public long CorporateProgramId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public string Plan { get; set; }
        public string Year { get; set; }
        public DateTime Effective { get; set; }
        public int Payment1Request { get; set; }
        public int Payment1CheckNo { get; set; }
        public decimal Payment1Amount { get; set; }
        public DateTime Payment1Date { get; set; }
        public int Payment2Request { get; set; }
        public int Payment2CheckNo { get; set; }
        public decimal Payment2Amount { get; set; }
        public DateTime Payment2Date { get; set; }
        public int NetPoints { get; set; }
        public string R { get; set; }
        public string MarketArea { get; set; }
        public string Awards { get; set; }
        public string Next { get; set; }
        public int NeededPoint { get; set; }
        public string Tp { get; set; }
        public string TpLevel { get; set; }
        public int EarnedPoint { get; set; }
        public int BonusPoint { get; set; }
        public string TravelGoal { get; set; }
        public string Meeting { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string TravelersName { get; set; }
        public string TraveleresTitle { get; set; }
        public long AccountId { get; set; }
        public string Vrip { get; set; }
        public string Level { get; set; }
        public int Pd { get; set; }
        public int CsLyr { get; set; }
        public int Target { get; set; }
        public double CsYtd { get; set; }
        public int CsNeeded { get; set; }
        public string Qualified { get; set; }
        public decimal Rebate { get; set; }
        public long ContactId { get; set; }
        public string StrretAddress { get; set; }
        public string Zip { get; set; }
        public string VripYear { get; set; }
        public DateTime UpdateDate { get; set; }
        public string SugarCorporateProgramId { get; set; }
    }
}
