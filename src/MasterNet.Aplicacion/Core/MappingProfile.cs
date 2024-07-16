using AutoMapper;
using MasterNet.Aplicacion.Calificaciones.GetCalificaciones;
using MasterNet.Aplicacion.Cursos.GetCurso;
using MasterNet.Aplicacion.Instructores.GetInstructoresQuery;
using MasterNet.Aplicacion.Photos.GetPhotos;
using MasterNet.Aplicacion.Precios.GetPrecios;
using MasterNet.Dominio;

namespace MasterNet.Aplicacion.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Curso, CursoResponse>();
            CreateMap<Photo, PhotoResponse>();
            CreateMap<Precio, PrecioResponse>();
            //para mappear los campos de las tablas en dominio como en lores ponsese en aplication deben coincidir
            //en este caso en la tabla instructores en dominio el campo se llama apellidos pero aca en el response le colocamos solo apellido
            //para estos cassos y en los que no podemos ir al response a corregir podemos forzarlo de esta manera
            //mismo caso para calificaciones
            CreateMap<Instructor, InstructorResponse>()
                .ForMember(dest => dest.Apellido, src => src.MapFrom(doc => doc.Apellidos));

            CreateMap<Calificacion, CalificacionResponse>()
                .ForMember(dest => dest.NombreCurso, src => src.MapFrom(doc => doc.Curso!.Titulo));
        }
    }
}
