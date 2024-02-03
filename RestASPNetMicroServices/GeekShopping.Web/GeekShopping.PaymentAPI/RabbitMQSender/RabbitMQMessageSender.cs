﻿using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{

    private readonly string _hostName;
    private readonly string _password;
    private readonly string _userName;
    private IConnection _connection;
    private const string ExchangeName = "FanoutPaymentUpdateExchange";

    public RabbitMQMessageSender()
    {
        _hostName = "localhost";
        _password = "guest";
        _userName = "guest";
    }

    public void SendMessage(BaseMessage message)
    {
        if (ConnectionExists())
        {
        using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: false);

        byte[] body = GetMessageAsByArray(message);

            channel.BasicPublish(exchange: ExchangeName, "", basicProperties: null, body: body);
        }
    }

    private byte[] GetMessageAsByArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        var json = JsonSerializer.Serialize<UpdatePaymentResultMessage>((UpdatePaymentResultMessage)message, options);

        return Encoding.UTF8.GetBytes(json);
    }

    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };
            _connection = factory.CreateConnection();
        }
        catch (Exception)
        {

            throw;
        }
    }

    private bool ConnectionExists()
    {
        if (_connection != null) return true;
        CreateConnection();
        return _connection != null;
    }
}