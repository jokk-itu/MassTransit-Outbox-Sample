using Api;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OutboxContext>(options =>
{
  options.EnableServiceProviderCaching();
  options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});
builder.Services.AddMassTransit(busConfigurator =>
{
  busConfigurator.AddEntityFrameworkOutbox<OutboxContext>(outboxConfigurator =>
  {
    outboxConfigurator.UseSqlServer();
    outboxConfigurator.UseBusOutbox();
    outboxConfigurator.DisableInboxCleanupService();
    outboxConfigurator.QueryMessageLimit = 10;
    outboxConfigurator.QueryDelay = TimeSpan.FromSeconds(5);
    outboxConfigurator.QueryTimeout = TimeSpan.FromSeconds(2);
  });
  busConfigurator.AddConsumer<OutboxConsumer>(typeof(OutboxConsumerDefinition));
  busConfigurator.UsingRabbitMq((busContext, transportConfigurator) =>
  {
    transportConfigurator.Host("rabbitmq://eventbus:5672", hostConfigurator =>
      {
        hostConfigurator.Username("guest");
        hostConfigurator.Password("guest");
      });
    transportConfigurator.ConfigureEndpoints(busContext);
  });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<OutboxContext>().Database.EnsureDeleted();
scope.ServiceProvider.GetRequiredService<OutboxContext>().Database.EnsureCreated();
scope.Dispose();

app.MapControllers();

//WORKS
app.MapGet("/publish/outbox/ef", async ([FromServices] IPublishEndpoint endpoint, [FromServices] OutboxContext context) =>
{
  await context.Set<Test>().AddAsync(new Test { Name = "Hej" });
  await endpoint.Publish<OutboxMessage>(new(Guid.NewGuid()));
  await context.SaveChangesAsync();
  return Results.Ok();
});

//WORKS
app.MapGet("/send/outbox/ef", async ([FromServices] ISendEndpointProvider endpointProvider, [FromServices] OutboxContext context) =>
{
  await context.Set<Test>().AddAsync(new Test { Name = "Hej" });
  var endpoint = await endpointProvider.GetSendEndpoint(new Uri("exchange:Outbox"));
  await endpoint.Send<OutboxMessage>(new(Guid.NewGuid()));
  await context.SaveChangesAsync();
  return Results.Ok();
});


//WORKS
app.MapGet("/publish", async ([FromServices] IBusControl busControl, [FromServices] OutboxContext context) =>
{
  var endpoint = await busControl.GetPublishSendEndpoint<OutboxMessage>();
  await endpoint.Send<OutboxMessage>(new(Guid.NewGuid()));
  return Results.Ok();
});

//WORKS
app.MapGet("/send", async ([FromServices] IBusControl busControl, [FromServices] OutboxContext context) =>
{
  var endpoint = await busControl.GetSendEndpoint(new Uri("exchange:Outbox"));
  await endpoint.Send<OutboxMessage>(new(Guid.NewGuid()));
  return Results.Ok();
});

app.Run();