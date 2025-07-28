using Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class InviteTokenDto
{
    [Required]
    public Role AssignedRole { get; set; }
}
