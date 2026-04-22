using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Library
{
    public class ActionStatus
    {
        public ActionStatus()
        {
            Success = false;
            Message = string.Empty;
            MessageDetail = string.Empty;
            Result = null;
        }

        public ActionStatus(bool success, string message, string messageDetail, object result)
        {
            Success = success;
            Message = message;
            MessageDetail = messageDetail;
            Result = result;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public string MessageDetail { get; set; }
        public object Result { get; set; }
    }
}