using GeekShopping.MessageBus;
using RabbitMQ.Client;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

public interface IRabbitMQMessageSender
{

    void SendMessage(BaseMessage baseMessage);

}
