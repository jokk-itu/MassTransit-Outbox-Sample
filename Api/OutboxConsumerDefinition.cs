using MassTransit;

namespace Api
{
  public class OutboxConsumerDefinition : ConsumerDefinition<OutboxConsumer>
  {
    private readonly IServiceProvider _serviceProvider;

    public OutboxConsumerDefinition(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OutboxConsumer> consumerConfigurator)
    {
      base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
      endpointConfigurator.UseEntityFrameworkOutbox<OutboxContext>(_serviceProvider);
    }
  }
}
