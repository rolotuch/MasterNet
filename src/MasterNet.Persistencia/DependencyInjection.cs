using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MasterNet.Persistencia
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistencia(
        this IServiceCollection services,
        IConfiguration configuration
    )
        {
            services.AddDbContext<MasterNetDbContext>(opt =>
            {
                // impresion en consola de cada transaccion que realice mi app
                opt.LogTo(Console.WriteLine, new[] {
                DbLoggerCategory.Database.Command.Name
            }, LogLevel.Information).EnableSensitiveDataLogging(); //
                //conexcion a la base de datos
                opt.UseSqlite(configuration.GetConnectionString("SqliteDatabase"));
            });


            return services;
        }
    }
}
