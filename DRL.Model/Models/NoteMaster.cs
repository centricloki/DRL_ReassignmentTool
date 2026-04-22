using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class NoteMaster
    {
        public long NoteId { get; set; }
        public string DeviceNoteId { get; set; }
        public long CustomerId { get; set; }
        public string Subject { get; set; }
        public string NoteDescription { get; set; }
        public string SugarNoteId { get; set; }
        public int ImportedFrom { get; set; }
        public bool IsExported { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long UpdatedBy { get; set; }
    }
}
