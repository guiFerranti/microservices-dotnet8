using GeekShopping.MessageBus;
using RabbitMQ.Client;

namespace GeekShopping.CartAPI.RabbitMQSender;

public interface IRabbitMQMessageSender
{

    void SendMessage(BaseMessage baseMessage, string queueName);

}
