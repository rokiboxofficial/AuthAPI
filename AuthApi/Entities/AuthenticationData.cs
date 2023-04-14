using System.ComponentModel.DataAnnotations;

namespace AuthApi.Entities;

public sealed class AuthenticationData
{
    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    public string Login { get; set; } = null!;
    
    [Required]
    [MinLength(6)]
    [MaxLength(32)]
    public string Password { get; set; } = null!;
    
    [Required]
    public string Fingerprint { get; set; } = null!;
}