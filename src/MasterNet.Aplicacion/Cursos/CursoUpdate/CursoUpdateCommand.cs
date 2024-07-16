using FluentValidation;
using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Persistencia;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MasterNet.Aplicacion.Cursos.CursoUpdate.CursoUpdateCommand;

namespace MasterNet.Aplicacion.Cursos.CursoUpdate
{
    public class CursoUpdateCommand
    {
        public record CursoUpdateCommandRequest(CursoUpdateRequest CursoUpdateRequest, Guid? CursoId) : IRequest<ResponseResult<Guid>>;
    }

    internal class CursoUpdateCommandHandler : IRequestHandler<CursoUpdateCommandRequest, ResponseResult<Guid>>
    {
        private readonly MasterNetDbContext _context;

        public CursoUpdateCommandHandler(MasterNetDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult<Guid>> Handle(CursoUpdateCommandRequest request, CancellationToken cancellationToken)
        {
            var cursoID = request.CursoId;
            var curso = await _context.Cursos!.FirstOrDefaultAsync(x => x.Id == cursoID);

            if (curso == null)
            {
                return ResponseResult<Guid>.Failure("El curso no existe");
            }

            curso.Descripcion = request.CursoUpdateRequest.Descripcion;
            curso.Titulo = request.CursoUpdateRequest.Titulo;
            curso.FechaPublicacion = request.CursoUpdateRequest?.FechaPublicacion;

            _context.Entry(curso).State = EntityState.Modified;
            var resultado = await _context.SaveChangesAsync() > 0;

            return resultado ? ResponseResult<Guid>.Success(curso.Id) : ResponseResult<Guid>.Failure("Error en el Update del curso");
        }
    }
    public class CursoUpdateCommandRequestValidator : AbstractValidator<CursoUpdateCommandRequest>
    {
        public CursoUpdateCommandRequestValidator()
        {
            //aca validamos que ambos tengan datos
            RuleFor(x => x.CursoUpdateRequest).SetValidator(new CursoUpdateValidate());
            RuleFor(x => x.CursoId).NotNull();
        }
    }
}
