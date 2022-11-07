using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WEB_APIContext>(obj => obj.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL")));

builder.Services.AddControllers().AddJsonOptions(opt =>
{ opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

var ReglasCors = "ReglasCors";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: ReglasCors, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors(ReglasCors);

app.MapGet("Hola", () => "Hola Mundo");

app.MapGet("Listar", (WEB_APIContext _context) =>
{
    try
    {
        return Results.Ok(_context.Contactos.ToList());
    }
    catch (Exception ex)
    {

        return Results.BadRequest(new { Mensaje = ex.Message });
    }


});

app.MapGet("Detail /{id}", (int id, WEB_APIContext _context) =>
{
    Contacto ObjContacto = _context.Contactos.Where(c => c.IdContacto == id).FirstOrDefault();

    if (ObjContacto == null)
    {
        return Results.BadRequest(new { mensaje = "No existe Datos" });
    }
    try
    {
        return Results.Ok(ObjContacto);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Mensaje = ex.Message });

    }
});

app.MapPost("Save", (Contacto objContacto, WEB_APIContext _context) =>
{
    try
    {
        _context.Contactos.Add(objContacto);
        var resultado = _context.SaveChanges();
        if (resultado > 0)
            return Results.Ok(new { Mesaje = "OK" });
        else
            return Results.BadRequest(new { Mensaje = "No es posible almacenar datos" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Mensaje = ex.Message });
    }

});

app.MapPut("Update", (Contacto objContacto, WEB_APIContext _context) =>
{
    Contacto _Contacto = _context.Contactos.Find(objContacto.IdContacto);

    if (_Contacto == null)
    {
        return Results.NotFound(new { Mensaje = "No existen datos" });
    }

    try
    {

        _Contacto.Nombre = string.IsNullOrEmpty(objContacto.Nombre) ? _Contacto.Nombre : objContacto.Nombre;
        _Contacto.Descripcion = objContacto.Descripcion is null ? _Contacto.Descripcion : objContacto.Descripcion;
        _Contacto.Telefono = objContacto.Telefono is null ? _Contacto.Telefono : objContacto.Telefono;
        _Contacto.IdTipo = objContacto.IdTipo is null ? _Contacto.IdTipo : objContacto.IdTipo;

        _context.Contactos.Update(_Contacto);
        var result = _context.SaveChanges();

        return Results.Ok(new { Mensaje = "OK", Response = "Datos Almacenados Correctamente"});
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { Mensaje = "No es posible almacenar los datos"});

    }
});

app.MapDelete("Delete /{id}", (int id, WEB_APIContext _context) =>
{
    Contacto ObjContacto = _context.Contactos.Find(id);

    if (ObjContacto == null)
    {
        return Results.NotFound(new { Mensaje = "Contacto no encontrado" });
    }

    try
    {
        _context.Contactos.Remove(ObjContacto);
        _context.SaveChanges();
        return Results.Ok(new { Mensaje = "OK", Response = "Contacto eliminado exitosamente" });
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { Mensaje = "No es posible eliminar la información" });

    }
});


app.Run();

