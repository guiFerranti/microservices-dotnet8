using GeekShopping.MessageBus;
using RabbitMQ.Client;

namespace GeekShopping.OrderAPI.RabbitMQSender;

public interface IRabbitMQMessageSender
{

    void SendMessage(BaseMessage baseMessage, string queueName);

}
