﻿using GeekShopping.Email.Messages;
using GeekShopping.Email.Model;
using GeekShopping.Email.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly DbContextOptions<MySQLContext> _context;

    public EmailRepository(DbContextOptions<MySQLContext> context)
    {
        _context = context;
    }
    
    public async Task LogEmail(UpdatePaymentResultMessage message)
    {
        EmailLog email = new()
        {
            Email = message.Email,
            SentDate = DateTime.UtcNow,
            Log = $"Order - {message.OrderId} has been created successfully!"
        };

        await using var _db = new MySQLContext(_context);

        _db.Emails.Add(email);
        _db.SaveChanges();
    }
}
