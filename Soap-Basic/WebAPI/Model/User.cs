using System;
using System.Collections.Generic;

namespace WebAPI.Model
{
    public partial class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public bool IsActive { get; set; }
        public bool? LockedOut { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
