using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Services; // Adicionado para acessar o SellerService e DepartmentService

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SalesWebMvcContext") ?? throw new InvalidOperationException("Connection string 'SalesWebMvcContext' not found.");
builder.Services.AddDbContext<SalesWebMvcContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Mensagens de model binding em português (ex.: campo numérico com texto inválido)
    var messages = options.ModelBindingMessageProvider;
    messages.SetValueMustNotBeNullAccessor(field => $"O campo {field} é obrigatório.");
    messages.SetMissingBindRequiredValueAccessor(field => $"O campo {field} é obrigatório.");
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"O valor '{value}' não é válido para {field}.");
    messages.SetValueIsInvalidAccessor(value => $"O valor '{value}' é inválido.");
    messages.SetValueMustBeANumberAccessor(field => $"O campo {field} deve ser um número.");
    messages.SetUnknownValueIsInvalidAccessor(field => $"O valor informado para {field} é inválido.");
});

// REGISTRO DOS SERVIÇOS (Injeção de Dependência)
builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<SalesRecordService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

// EXECUÇÃO DO SEEDING SERVICE (Apenas em ambiente de desenvolvimento)
// Aplica as migrations pendentes e popula o banco se estiver vazio.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();
        seedingService.Seed();
    }
}

// Cultura fixa en-US para o model binding: inputs type="number" sempre enviam "1234.56".
// A exibição em pt-BR (R$, dd/MM/yyyy) é feita explicitamente pelo helper Format.
var enUS = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = [enUS],
    SupportedUICultures = [enUS]
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
