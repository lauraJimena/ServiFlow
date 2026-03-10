using Microsoft.EntityFrameworkCore;
using ServiFlow.Data;
using ServiFlow.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


//        db.Emprendimientos.AddRange(
//            new Emprendimiento
//            {
//                Nombre = "Studio Glam",
//                TipoServicio = "Uñas",
//                Descripcion = "Manicure profesional y semipermanente"
//            },
//            new Emprendimiento
//            {
//                Nombre = "Barber Pro",
//                TipoServicio = "Barbería",
//                Descripcion = "Corte moderno y perfilado de barba"
//            },
//            new Emprendimiento
//            {
//                Nombre = "Lash Room",
//                TipoServicio = "Pestañas",
//                Descripcion = "Extensiones clásicas y volumen ruso"
//            },
//            new Emprendimiento
//            {
//                Nombre = "Beauty Brow",
//                TipoServicio = "Cejas",
//                Descripcion = "Diseño y depilación de cejas profesional"
//            }
//        );

//        db.SaveChanges();

//}
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


//    var lista = db.Emprendimientos.ToList();

//    foreach (var e in lista)
//    {
//        if (e.Nombre == "Beauty Brow")
//            e.ImagenUrl = "/images/cejas.jpg";
//    }

//    db.SaveChanges();

//}
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


//    var lista = db.Emprendimientos.ToList();

//    foreach (var e in lista)
//    {
//        if (e.Nombre == "Lash Room")
//            e.ImagenUrl = "/images/pestanas.jpg";
//    }

//    db.SaveChanges();

//}

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//    var lista = db.Emprendimientos.ToList();

//    foreach (var e in lista)
//    {
//        if (e.Nombre == "Studio Glam")
//        {
//            e.EsPropio = true;
//        }
//        else
//        {
//            e.EsPropio = false;
//        }
//    }

//    db.SaveChanges();
//}

app.Run();
