using Microsoft.AspNetCore.Identity;
namespace MasterNet.Persistencia.Modelos
{
    public class AppUser: IdentityUser
    {
        public string? NombreCompleto { get; set; }
        public string? Carrera { get; set; }
    }
}
