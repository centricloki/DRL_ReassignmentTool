using System;

namespace DRL.Entity
{
    public class ENTLog
    {
        //public int Id { get; set; }
        public string ProgramId { get; set; }
        public string UserId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTime? Date { get; set; }
    }
}