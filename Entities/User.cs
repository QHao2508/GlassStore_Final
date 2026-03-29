using System;
using System.Collections.Generic;

namespace GlassStore.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
