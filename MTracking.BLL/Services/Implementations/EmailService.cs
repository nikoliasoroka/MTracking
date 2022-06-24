using System;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;
using MTracking.BLL.Models.Abstractions;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;

namespace MTracking.BLL.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfigurationSection _emailConfiguration;

        public EmailService(IConfiguration configuration)
        {
            _emailConfiguration = configuration.GetSection("EmailConfiguration");
        }

        public async Task<IResult> SendEmailAsync(EmailMessage message)
        {
            var mailMessageResult = CreateEmailMessage(message);

            if (!mailMessageResult.Success)
                return Result.CreateFailed(ValidationFactory.EmailIsNotSent).AddError(mailMessageResult.ErrorInfo.Error);

            var sendResult = await SendAsync(mailMessageResult.Data);

            return !sendResult.Success
                ? Result.CreateFailed(sendResult.ErrorInfo.Error, sendResult.ErrorInfo.Exception)
                : Result.CreateSuccess();
        }

        private IResult<MimeMessage> CreateEmailMessage(EmailMessage message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_emailConfiguration["Title"], _emailConfiguration["From"]));
                emailMessage.To.AddRange(message.To);
                emailMessage.Subject = message.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

                return Result<MimeMessage>.CreateSuccess(emailMessage);
            }
            catch
            {
                return Result<MimeMessage>.CreateFailed(ValidationFactory.EmailMessageIsNotCreated);
            }
        }

        private async Task<IResult> SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfiguration["SmtpServer"], int.Parse(_emailConfiguration["Port"]), true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfiguration["Username"], _emailConfiguration["Password"]);
                await client.SendAsync(mailMessage);

                return Result.CreateSuccess();
            }
            catch(Exception ex)
            {
                return Result.CreateFailed(ValidationFactory.EmailIsNotSent, ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
