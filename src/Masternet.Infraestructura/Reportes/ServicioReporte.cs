using CsvHelper;
using MasterNet.Aplicacion.Interfaces;
using MasterNet.Dominio;
using System.Globalization;

namespace Masternet.Infraestructura.Reportes
{
    public class ServicioReporte<T> : IServicioReporte<T> where T : BaseEntity//recordemos que la interfaz es generica por eso le pasamos la T
    {
        public Task<MemoryStream> GetCsvReport(List<T> registros)
        {
            using var memoryStream = new MemoryStream();
            using var textWriter = new StreamWriter(memoryStream);
            using var csvWriter = new CsvWriter(textWriter, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords(registros);
            textWriter.Flush(); //limpiamos el objeto para no dejar nada en memoria
            memoryStream.Seek(0, SeekOrigin.Begin); //para resetear los bytes en memoria que reporesentan los memory stream

            return Task.FromResult(memoryStream);

        }
    }
}
