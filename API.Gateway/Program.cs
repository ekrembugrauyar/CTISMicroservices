using Ocelot.DependencyInjection; // Provides extension methods for adding Ocelot services
using Ocelot.Middleware;         // Provides middleware to enable Ocelot request routing

var builder = WebApplication.CreateBuilder(args);

// Load the Ocelot configuration file, which defines routes and downstream services
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("ocelot.Swagger.json", optional: false, reloadOnChange: true);


// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Ocelot services into the DI container
builder.Services.AddOcelot();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

// Configure Swagger
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
    opt.ReConfigureUpstreamSwaggerJson = (context, path) => path;
    opt.ServerOcelot = "/swagger/docs";
});

// Enables Ocelot's middleware to intercept and forward incoming HTTP requests
await app.UseOcelot();

// Optional: Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Start the application
app.Run();
