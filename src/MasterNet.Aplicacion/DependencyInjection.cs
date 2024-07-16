using FluentValidation;
using FluentValidation.AspNetCore;
using MasterNet.Aplicacion.Core;
using MasterNet.Aplicacion.Cursos.CursoCreate;
using Microsoft.Extensions.DependencyInjection;

namespace MasterNet.Aplicacion
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplication(
         this IServiceCollection services
     )
        {
            services.AddMediatR(configuration =>
            {
                configuration
                .RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CursoCreateCommand>();

            //inyectamos el autommaper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }
    }
}
