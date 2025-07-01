using System;
using System.Collections.Generic;

namespace CustorPortalAPI.Models;

public partial class Role
{
    public int RoleKey { get; set; }

    public string Role_Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User>? Users { get; set; } 
}
