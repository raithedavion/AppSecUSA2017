using System;
using System.Collections.Generic;

namespace WebAPI.Model
{
    public partial class ServiceRole
    {
        public ServiceRole()
        {
            ServiceUserRoles = new HashSet<ServiceUserRoles>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public ICollection<ServiceUserRoles> ServiceUserRoles { get; set; }
    }
}
