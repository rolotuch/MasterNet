////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////namespace MasterNet.Persistencia
////{
////    public class MainEntry
////    {
////        static void main()
////        {
////            Console.WriteLine("Hola mundo");
////        }
////    }
////}


////class MainEntry
////{
////    static void main(string[] args)
////    {
////        Console.WriteLine("Hola mundo");
////    }
////}

////recordemos que antes de realizar todo esto es necesario agregar esta linea en el proyecto de persistencia.
////< PropertyGroup >
////        < Nullable > enable </ Nullable >

////        < TargetFramework > net8.0 </ TargetFramework >
////        < ImplicitUsings > enable </ ImplicitUsings >
////        < OutputType > Exe </ OutputType > // esta linea es la que se agrega
////    </ PropertyGroup >

//using MasterNet.Dominio;
//using MasterNet.Persistencia;
//using Microsoft.EntityFrameworkCore;

////Console.WriteLine("Hola mundo");

//// utilizacion de linq
//using var context = new MasterNetDbContext(); // creamos un objeto de tipo dbcontest llamado context
////listar los cursoss

////agregar un curso
//var cursoNuevo = new Curso
//{
//    Id = Guid.NewGuid(),
//    Titulo = "progra para nerds",
//    Descripcion = "las bases de c#",
//    FechaPublicacion = DateTime.Now
//};
//// agregamos dentro de la session del context del ef
//context.Add(cursoNuevo); //solo en la session aun no se ha enviado a la bd
//await context.SaveChangesAsync(); //lo agrega a la bd

//var lista_cuross = await context.Cursos!.ToListAsync(); //el ! indica que no es nullable

//foreach (var curso in lista_cuross)
//{
//    Console.WriteLine($"Curso: {curso.Id} - {curso.Titulo}");
//}
