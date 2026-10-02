using Microsoft.EntityFrameworkCore;
using SalesWebMvc;
using SalesWebMvc.Models; // Ajuste se seu Context estiver aqui
using SalesWebMvc.Services; // Pasta que você vai criar para os serviços

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados (Equivalente ao ConfigureServices do PDF)
var connectionString = builder.Configuration.GetConnectionString("SalesWebMvcContext");
builder.Services.AddDbContext<SalesWebMvcContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Registro de Serviços (Injeção de Dependência)
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();

var app = builder.Build();

// 3. Execução do SeedingService (Equivalente ao método Configure do PDF)
// Como não temos mais o método Configure para injetar o serviço direto, abrimos um escopo:
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();
        seedingService.Seed();
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// 4. Configuração de Rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();