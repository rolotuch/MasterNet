using MasterNet.Dominio;

namespace MasterNet.Aplicacion.Interfaces
{
    public interface IServicioReporte <T> where T : BaseEntity  //este se llama where T : BaseEntity un constrain
    {
        Task<MemoryStream> GetCsvReport(List<T> registros);
    }
}
