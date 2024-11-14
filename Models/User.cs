using System;
using System.Collections.Generic;

namespace App_Azure_OpenId.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Mail { get; set; }

    public string? Pass { get; set; }

    public bool? IsActive { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual ICollection<Note> NoteUserCreatedNavigations { get; set; } = new List<Note>();

    public virtual ICollection<Note> NoteUserUpdatedNavigations { get; set; } = new List<Note>();
}
