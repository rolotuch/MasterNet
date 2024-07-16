using Microsoft.EntityFrameworkCore;

namespace MasterNet.Aplicacion.Core
{
    public class PagedList<T>
    {
        //constructor de los parametros qeu se le devolveran al cliente.
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber; //pagina actual
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); //total de registro / tamañao de la pagina = numero de paginas
            PageSize = pageSize; // tamaño de la pagina en registros
            TotalCount = count; // total de registros
            Items = items; //
        }
        //esta sera la data que se le devolvera al cliente
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new List<T>();

        //este metodo me permite disparar la paginacion contra la base de datos
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize) //el iquerable representa la consulta en terminos de expresion fuction
        {
            var count = await source.CountAsync(); //aca ejecutamos la consulta contra la base de datos y lo guardamos en count, devuelve la cantidad de registros quehay en la bd
            //ahora le indicamos que me envie los records por ejemplo de la pagina 1 y de alli inicia a contar los records

            var items = await source
                        .Skip((pageNumber - 1) * pageSize) // el skip me permite obtener el indice de lapagina que inicio a contar, este depende de dos parametros el numero de pagina y el tamaño dela pagina
                        .Take(pageSize)  //toma la pagina y me devuelve los registros dependiendo del tamaño de la pagina
                        .ToListAsync(); //ejecutamos la consulta

            return new PagedList<T>(items, count, pageNumber, pageSize); // devolvemos el valor
        }

    }
}
