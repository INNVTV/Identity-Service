using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Custodian.Models
{
    public class CustodialReport
    {
        public CustodialReport()
        {
            isSuccess = false;
            TaskCount = 0;
            TaskLog = new List<string>();
            Duration = 0;
        }

        public bool isSuccess;
        public int TaskCount;
        public string Message;

        public List<string> TaskLog { get; set; }
        public long Duration { get; set; }
    }
}
