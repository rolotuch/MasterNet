using Masternet.Infraestructura.Photos;
using Masternet.Infraestructura.Reportes;
using MasterNet.Aplicacion;
using MasterNet.Aplicacion.Interfaces;
using MasterNet.Persistencia;
using MasterNet.Persistencia.Modelos;
using MasterNet.WebApi.Extensiones;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddPersistencia(builder.Configuration);
//para manejar las photos
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings))); //esta parte lo podemos escribir asi GetSection("CloudinarySettings") pero se realizo de la otra manera para que compare el nombre con el nombre de la clase
// agregar el servicio para iservice photo
builder.Services.AddScoped<IPhotoService, PhotoService>();

//para manejar los reportes
//builder.Services.AddScoped<IServicioReporte, ServicioReporte>();
builder.Services.AddScoped(typeof(IServicioReporte<>), typeof(ServicioReporte<>));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentityCore<AppUser>(opt => {
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<MasterNetDbContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.SeddDataAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
