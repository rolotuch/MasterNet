namespace MasterNet.Aplicacion.Core
{
    //estos son los datos que el cliente me debe enviar
    public abstract class PagingParams
    {
        public int PageNumber { get; set; } = 1;
        private const int MaxPageSize = 50;
        private int _pageSize = 10;  //valor por defecto si es que no me pide una tamaño de registros por paginas
        //creamos una validacion que me valida si me pides algo que no esta en la logica, por ejemplo si solo tenemos 5 paginas y el me pide la pagina 29 no lo voy a poder dar
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string? OrderBy { get; set; } //para ordenamiento
        public bool? OrderAsc { get; set; } = true; // verificar en que tipo de ordenamiento esta
    }
}
