using System;
using System.Collections.Generic;

namespace App_Azure_OpenId.Models;

public partial class Note
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public bool? IsActive { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int UserCreated { get; set; }

    public int UserUpdated { get; set; }

    public virtual User UserCreatedNavigation { get; set; } = null!;

    public virtual User UserUpdatedNavigation { get; set; } = null!;
}
