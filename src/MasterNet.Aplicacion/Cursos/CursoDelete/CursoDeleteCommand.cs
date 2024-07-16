using FluentValidation;
using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Persistencia;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterNet.Aplicacion.Cursos.CursoDelete
{
    public class CursoDeleteCommand
    {
        public record CursoDeleteCommandRequest(Guid? CursoId) : IRequest<ResponseResult<Unit>>;

        internal class CursoDeleteCommandHandler : IRequestHandler<CursoDeleteCommandRequest, ResponseResult<Unit>>
        {
            private readonly MasterNetDbContext _context;

            public CursoDeleteCommandHandler(MasterNetDbContext context)
            {
                _context = context;
            }

            public async Task<ResponseResult<Unit>> Handle(CursoDeleteCommandRequest request, CancellationToken cancellationToken)
            {
                //primero verificamos si existe el elemento en la bd
                var curso = await _context.Cursos!
                    .Include(x => x.Instructores) //se eliminan en la tabla intermedia la que tiene la relacion de muchos a muchos
                    .Include(x => x.Precios)
                    .Include(x => x.Calificaciones)
                    .Include(x => x.Photos)
                    .FirstOrDefaultAsync(x => x.Id == request.CursoId);

                if (curso == null)
                {
                    return ResponseResult<Unit>.Failure("El curso que intenta eliminar no existe");
                }
                _context.Cursos!.Remove(curso);
                var resultado = await _context.SaveChangesAsync(cancellationToken) > 0;
                return resultado ? ResponseResult<Unit>.Success(Unit.Value) : ResponseResult<Unit>.Failure("Error al intentar eliminar el curso");
            }
        }

        public class CursoDeleteCommandRequestValidator : AbstractValidator<CursoDeleteCommandRequest>
        {
            public CursoDeleteCommandRequestValidator()
            {
                RuleFor(x => x.CursoId).NotNull().WithMessage("no tiene curso id");

            }
        }
    }
}
