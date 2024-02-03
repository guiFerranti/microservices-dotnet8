namespace GeekShopping.Email.Messages;

public class UpdatePaymentResultMessage
{
    public long OrderId { get; set; }

    public string Email { get; set; }

    public bool Status { get; set; }
}
