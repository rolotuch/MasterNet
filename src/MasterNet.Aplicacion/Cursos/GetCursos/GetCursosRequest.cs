using MasterNet.Aplicacion.Core;

namespace MasterNet.Aplicacion.Cursos.GetCursos
{
    public class GetCursosRequest : PagingParams //heredamos de PagingParams con ello tenemos todos los parametros de pagin params pero adicional si queremos realizar busquedas podemos agregar mas aca
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
    }
}
