using Bogus;
using MasterNet.Dominio;
using MasterNet.Persistencia;
using MasterNet.Persistencia.Modelos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.WebApi.Extensiones
{
    public static class DataSeed
    {
        public static async Task SeddDataAuthentication(
            this IApplicationBuilder app)

        {
            using var scope = app.ApplicationServices.CreateScope(); //esto es com oun contenedor de objetos
            var service = scope.ServiceProvider; // creamos el service provider
            var loggerFactory = service.GetRequiredService<ILoggerFactory>(); //creamos el objeto loggerfactory para almacenar los logs le debemos pasar la interfaz o la clase
            // creamos el objeto de tipo context y el usser manager.  para poder realizar la migracion. lo metemos en un try catch para capturar los posibles errres
            try
            {
                var context = service.GetRequiredService<MasterNetDbContext>(); //creola session de tipo objeto, session del context
                await context.Database.MigrateAsync(); // teniendo el cotext le indico que realice la migracion qeu sea asincrona 
                var userManager = service.GetRequiredService<UserManager<AppUser>>(); // instanciamos el usser manager utilizando el servicio esta la parseamos contra la clase modelo es decir la tabla que maneja el usuario AppUser

                if (!userManager.Users.Any()) //logica para que no me duplique la creacion de los usuarios
                {
                    //creamos el usuario administrado

                    var userAdmin = new AppUser
                    {
                        NombreCompleto = "Rolando tubac",
                        UserName = "rolando",
                        Email = "tubacrolando@gmail.com"
                    };

                    await userManager.CreateAsync(userAdmin, "Password123"); //se envia al metodo  createAsync el userAdmin y el password seteado
                    await userManager.AddToRoleAsync(userAdmin, RolesPersonalizados.ADMIN); //se asignan los roles
                    
                    //creamos el usuario cliente
                    var userClient = new AppUser
                    {
                        NombreCompleto = "Juan Perez",
                        UserName = "juanperez",
                        Email = "juan.perez@gmail.com"
                    };

                    await userManager.CreateAsync(userClient, "Password123"); 
                    await userManager.AddToRoleAsync(userClient, RolesPersonalizados.CLIENT); //se le asigna el rol de cliente
                }


                var cursos = await context.Cursos!.Take(10).Skip(0).ToListAsync(); //me toma los 10 primero datos de la tabla cursos
                //ahora trataremos de llenar las tablas que aun no tienen datos
                // asginamos un curso a un istructor
                if (!context.Set<CursoInstructor>().Any()) 
                {
                    var instructores = await context.Instructores!.Take(10).Skip(0).ToListAsync(); //traemos todos los instructores
                    //recorremos todos los cursos cuyo intructor en el curso sea igual a instructor de la tabla instructores
                    foreach (var curso in cursos)
                    {
                        curso.Instructores = instructores;
                    }
                }

                //await context.SaveChangesAsync(); //aca terminamos pero pasa lo mismo con cursoprecio entonces vamos a terminar y de ultimo acemos el savechanges
                if (!context.Set<CursoPrecio>().Any())
                {
                    var precios = await context.Precios!.ToListAsync();
                    foreach (var curso in cursos)
                    {
                        curso.Precios = precios;
                    }
                }

                if (!context.Set<Calificacion>().Any())
                {
                    foreach (var curso in cursos)
                    {
                        var fakerCalificacion = new Faker<Calificacion>()
                            .RuleFor(c => c.Id, _ => Guid.NewGuid())
                            .RuleFor(c => c.Alumno, f => f.Name.FullName())
                            .RuleFor(c => c.Comentario, f => f.Commerce.ProductDescription())
                            .RuleFor(c => c.Puntaje, 5)
                            .RuleFor(c => c.CursoId, curso.Id);

                        var calificaciones = fakerCalificacion.Generate(10);
                        context.AddRange(calificaciones);
                    }
                }


                await context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<MasterNetDbContext>();
                logger.LogError(e.Message);
            }


        }
    }
}
