var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Swagger / OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS configuration (se necessário para o seu caso)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configurar CORS se necessário (aplicado globalmente)
app.UseCors("AllowAll");

// Redirecionar para HTTPS
app.UseHttpsRedirection();

// Autorizações (caso tenha algum tipo de autenticação)
app.UseAuthorization();

// Mapeamento das rotas dos controllers
app.MapControllers();

// Expor a aplicação para a porta que o Railway fornecer
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; // Porta dinâmica do Railway
app.Run($"http://0.0.0.0:{port}");  // Faz a aplicação escutar na porta configurada pelo Railway (0.0.0.0 para escutar todas as interfaces)

