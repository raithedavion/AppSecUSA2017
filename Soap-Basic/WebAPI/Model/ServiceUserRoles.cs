using System;
using System.Collections.Generic;

namespace WebAPI.Model
{
    public partial class ServiceUserRoles
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public ServiceRole Role { get; set; }
        public ServiceUser User { get; set; }
    }
}
