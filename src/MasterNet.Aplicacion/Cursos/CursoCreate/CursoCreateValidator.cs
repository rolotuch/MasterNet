using FluentValidation;

namespace MasterNet.Aplicacion.Cursos.CursoCreate
{
    public class CursoCreateValidator : AbstractValidator<CursoCreateRequest> //validamos los datos que biene del request
    {
        //validar el contenido del request
        public CursoCreateValidator() {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El titulo no debe estar vacio");
                //.NotEqual()
            RuleFor(x => x.Descripcion).NotEmpty().WithMessage("La descripcion no debe estar vacio"); ;
        }
    }
}
