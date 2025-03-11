using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;
public class FarmMember
{
    public string Email { get; set; }
    public FarmRoles Role { get; set; }
}
