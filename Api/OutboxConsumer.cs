using MassTransit;

namespace Api
{
  public class OutboxConsumer : IConsumer<OutboxMessage>
  {
    private readonly ILogger<OutboxConsumer> _logger;

    public OutboxConsumer(ILogger<OutboxConsumer> logger)
    {
      _logger = logger;
    }

    public Task Consume(ConsumeContext<OutboxMessage> context)
    {
      _logger.LogInformation("Consumed");
      return Task.CompletedTask;
    }
  }
}
