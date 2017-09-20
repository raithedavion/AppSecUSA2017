using System;
using System.Collections.Generic;

namespace WebAPI.Model
{
    public partial class ServiceLoginLog
    {
        public int LogId { get; set; }
        public string UserName { get; set; }
        public string Pass { get; set; }
        public int ReturnCode { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
