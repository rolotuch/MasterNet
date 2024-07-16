using FluentValidation;

namespace MasterNet.Aplicacion.Cursos.CursoUpdate
{
    public class CursoUpdateValidate : AbstractValidator<CursoUpdateRequest>
    {
        public CursoUpdateValidate()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("El titulo no debe ser vacio");
            RuleFor(x => x.Descripcion).NotEmpty().WithMessage("La descripcion no debe ser vacio");
            RuleFor(x => x.FechaPublicacion).NotEmpty().WithMessage("La fecha de publicación no sebe ser vacia");
        }
    }

}
