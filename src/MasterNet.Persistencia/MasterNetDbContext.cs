using Bogus;
using MasterNet.Dominio;
using MasterNet.Persistencia.Modelos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// tareas a realizar 
// estas primera son obligatorias
// 1  crear la cadena de conexion
// 2  mapear los modelos -- sobreescribir onModelCreating
// estas primera son opcionales
// 3 carga de data de prueba , agregar data en modelos
// 4 Declarar modelos como propiedades del context.

namespace MasterNet.Persistencia
{
    //public class MasterNetDbContext : DbContext //este herea desde dbcontext
    public class MasterNetDbContext : IdentityDbContext<AppUser>// cambiamos esto para utilizar identity, pero para personalizar mis tablas debemos de especificar esto <AppUser>
    {

        public MasterNetDbContext(DbContextOptions<MasterNetDbContext> options) : base(options)
        {
            
        }
        // 4 Declarar modelos como propiedades del context.
        public DbSet<Curso>? Cursos { get; set; }
        public DbSet<Instructor>? Instructores { get; set; }
        public DbSet<Precio>? Precios { get; set; }
        public DbSet<Calificacion>? Calificaciones { get; set; }

        
        // tarea 1 crear la cadena de conexion -- sobreescribir onconfiguring
        // la cadena de conexion aca ya no funciona ya uqe se creo en persistencia el dependencyinyection
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    optionsBuilder.UseSqlite("Data Source=LocalDatabase.db") // utilizamos sqlite entre comillas le damos la cadena de conexion "localdatabase.db" es el nombre del archivo de la base de datos
        //                                                             // este se crea en el contexto donde se esta llamando en este caso en persistencia
        //    .LogTo( //esto me crea un log de tipo writeline
        //        Console.WriteLine,
        //        new[] { DbLoggerCategory.Database.Command.Name },
        //        Microsoft.Extensions.Logging.LogLevel.Information
        //        ).EnableSensitiveDataLogging();

        //}

        //2  mapear los modelos -- sobreescribir onModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Curso>().ToTable("cursos");  //el nombre que va entre <> es el nombre de la clase que definimos en dominio, lo que va entre "" seria el nombre que tiene la tabla en la base de datos.
            modelBuilder.Entity<Instructor>().ToTable("instructores");
            modelBuilder.Entity<CursoInstructor>().ToTable("cursos_instructores");
            modelBuilder.Entity<Precio>().ToTable("precios");
            modelBuilder.Entity<CursoPrecio>().ToTable("cursos_precios");
            modelBuilder.Entity<Calificacion>().ToTable("calificaciones");
            modelBuilder.Entity<Photo>().ToTable("Imagenes");

            modelBuilder.Entity<Precio>()
                .Property(b => b.PrecioActual)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Precio>()
                .Property(b => b.PrecioPromocion)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Precio>()
                .Property(b => b.Nombre)
                .HasColumnType("VARCHAR")
                .HasMaxLength(250);


            modelBuilder.Entity<Curso>()
                .HasMany(m => m.Photos) //relacion uno a  muchos
                .WithOne(m => m.Curso) // cada foto tiene un curso, lo relacionamos al objeto
                .HasForeignKey(m => m.CursoId) // esta relacion se basa en una clave foranea
                //.IsRequired() //es requerido, // le vamos a quitar el requerido
                .OnDelete(DeleteBehavior.Cascade); // si eliminamos un curso se elimia las fotos relacionadas en ella.

            modelBuilder.Entity<Curso>()
                .HasMany(m => m.Calificaciones)
                .WithOne(m => m.Curso)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Restrict); // se elimina el curso pero dentro de calificacion el cursoId sera nullo.  antes de la eliminacion el id representba al curos

            modelBuilder.Entity<Curso>()
                .HasMany(m => m.Precios) // relacion muchos a muchos
                .WithMany(m => m.Cursos) // el modelo precio contiene un conjunto de cursos
                .UsingEntity<CursoPrecio>( // como es de muchos a muchos necesitamos un componente, un modelo que me haga la relacicion
                    j => j //relacionamos con el precio
                        .HasOne(p => p.Precio) // aca estamos hablando a nivel de la tabla intermedia. aca le pasamos el objeto el cual representa al modelo
                        .WithMany(p => p.CursoPrecios) // un precio tien una coleccion de cursos precios
                        .HasForeignKey(p => p.PrecioId), // amarramos a la llave foranea.
                    j => j // relacionamos contra el curso
                        .HasOne(p => p.Curso) // representacion del objeto en el modelo
                        .WithMany(p => p.CursoPrecios) // un curso tiene un conjunto de precios y un curso tiene un conjunto de cursoprecio
                        .HasForeignKey(p => p.CursoId), //la llave foranea
                    j => // estas llaves compuestas tienen llaves foreaneas y llaves primarias
                    {
                        j.HasKey(t => new { t.PrecioId, t.CursoId }); // aca definimos las llaves primarias.
                    }

                );


            modelBuilder.Entity<Curso>()
            .HasMany(m => m.Instructores)
            .WithMany(m => m.Cursos)
            .UsingEntity<CursoInstructor>( //tabla intermedia
                j => j //la definicion del modelo curosinstructor
                    .HasOne(p => p.Instructor)
                    .WithMany(p => p.CursoInstructores) //un instructor tiene muchos cursosinstructores
                    .HasForeignKey(p => p.InstructorId),
                j => j
                    .HasOne(p => p.Curso)
                    .WithMany(p => p.CursoInstructores)
                    .HasForeignKey(p => p.CursoId),
                j =>
                {
                    j.HasKey(t => new { t.InstructorId, t.CursoId });
                }
            );

            // logica para cargar la data de prueba
            modelBuilder.Entity<Curso>().HasData(CargarDataMaster().Item1); //hasdata espera un arreglo en este caso de tipo curso, item 1 porque curos es el primer elemento de la tupla
            modelBuilder.Entity<Precio>().HasData(CargarDataMaster().Item2);
            modelBuilder.Entity<Instructor>().HasData(CargarDataMaster().Item3);
            

            //llnear roles
            CargarDataSeguridad(modelBuilder);
        }

        //metodo para roles personalizados
        private void CargarDataSeguridad(ModelBuilder modelBuilder)
        {
            //iniciamos con los roles, cada rol tiene su propio id
            var adminId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            //llamamos al modelo que representa a los roles este es identityRole
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminId,
                    Name = RolesPersonalizados.ADMIN, //estos datos deberian ir en un archivo donde este todas las constantes
                    NormalizedName = RolesPersonalizados.ADMIN
                }
            );

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = clientId,
                    Name = RolesPersonalizados.CLIENT, //estos datos deberian ir en un archivo donde este todas las constantes
                    NormalizedName = RolesPersonalizados.CLIENT
                }
            );
            // los claims son permisos, en semantica de asp estas son policies para ello usamos la talba identityRoleClaim
            modelBuilder.Entity<IdentityRoleClaim<String>>().HasData(
                new IdentityRoleClaim<String>
                {
                    Id = 1,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.CURSO_READ,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 2,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.CURSO_UPDATE,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 3,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.CURSO_WRITE,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 4,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.CURSO_DELETE,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 5,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.INSTRUCTOR_CREATE,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 6,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.INSTRUCTOR_READ,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 7,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.INSTRUCTOR_UPDATE,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 8,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.COMENTARIO_READ,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 9,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.COMENTARIO_DELETE,
                    RoleId = adminId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 10,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.COMENTARIO_CREATE,
                    RoleId = adminId, //quien tendra este rol
                },

                //AHORA EL BLQUE DE CLIENTE
                new IdentityRoleClaim<String>
                {
                    Id = 11,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.CURSO_READ,
                    RoleId = clientId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 12,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.INSTRUCTOR_READ,
                    RoleId = clientId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 13,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.COMENTARIO_READ,
                    RoleId = clientId, //quien tendra este rol
                },
                new IdentityRoleClaim<String>
                {
                    Id = 14,
                    ClaimType = ClaimPersonalizados.POLICIES, //estos datos deberian ir en un archivo donde este todas las constantes
                    ClaimValue = PolicyMaster.COMENTARIO_CREATE,
                    RoleId = clientId, //quien tendra este rol
                }

            );
        }

        // 3 carga de data de prueba , agregar data en modelos para las tablas curso, precio, instructor utilizando bogo 
        private Tuple<Curso[], Precio[], Instructor[]> CargarDataMaster()  //aca le definimos para cada arreglo de curso, precio e instrucotr lo definimos como tupla Tuple<Curso[], Precio[], Instructor[]
        {
            var cursos = new List<Curso>(); //creamos una lista vacia de objetos cursos
            var faker = new Faker();  // creamos una istancia de bogus, esta se llama faker() hay que importarlo

            for (var i = 1; i < 10; i++) // creamos 10 elementos para la tabla cursos
            {
                var cursoId = Guid.NewGuid(); // este objeto depende de un id, del cursoId
                cursos.Add( //agregamos los datos a la lista
                    new Curso // creamos un nuevo elemento con los parametros o las tuplas que necesitamos.
                    {
                        Id = cursoId,
                        Descripcion = faker.Commerce.ProductDescription(),
                        Titulo = faker.Commerce.ProductName(),
                        FechaPublicacion = DateTime.UtcNow
                    }
                );
            }
            // ahora continuamos con el precio, este es mas sencillo solo utilizaremos un registro para el precio, aca no utilizamos bogu
            var precioId = Guid.NewGuid();
            var precio = new Precio
            {
                Id = precioId,
                PrecioActual = 10.0m,
                PrecioPromocion = 8.0m,
                Nombre = "Precio Regular"
            };
            var precios = new List<Precio>
        {
            precio
        };
            // para el instructor utilizamos bogo pero lo usamos de otra manera
            var fakerInstructor = new Faker<Instructor>() // llamamos al faker
                .RuleFor(t => t.Id, _ => Guid.NewGuid()) //generamos los id
                .RuleFor(t => t.Nombre, f => f.Name.FirstName()) // generamos el nombre
                .RuleFor(t => t.Apellidos, f => f.Name.LastName()) // generamos el apellido
                .RuleFor(t => t.Grado, f => f.Name.JobTitle()); // generamos el grado

            var instructores = fakerInstructor.Generate(10); // generamos 10 elementos por cada uno


            return Tuple.Create(cursos.ToArray(), precios.ToArray(), instructores.ToArray()); // retornamos la tupla y la convertimos en array
        }

    }
}
