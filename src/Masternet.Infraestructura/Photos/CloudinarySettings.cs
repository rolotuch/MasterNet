namespace Masternet.Infraestructura.Photos
{
    //creamos los campos segun lo que definmos en el appsettings, luego vamos al web api para inyectar en program, esto a nivel global
    public class CloudinarySettings
    {
       public string? CloudName { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
    
    }
}
