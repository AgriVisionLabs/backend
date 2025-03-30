using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrivision.Backend.Domain.Interfaces.Identity;
public interface IApplicationUserRole
{
    public string UserId { get; set; }
    public string RoleId { get; set; }
    public string FarmId { get; set; }


}
