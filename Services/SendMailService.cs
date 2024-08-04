using ShoppingOnline.Interfaces;
using System.Net.Mail;
using System.Net;

namespace ShoppingOnline.Services
{
    public class SendMailService : ISendMail
    {
        private readonly ILogger<SendMailService> _logger;
        private readonly string _passwordEmail;
        private readonly string _email;
        

        public SendMailService(IConfiguration configuration,ILogger<SendMailService> logger)
        {
            _passwordEmail = configuration["SendMail:Password"];
            _email = configuration["SendMail:Email"];
            _logger = logger;
        }

        public async Task<bool> SendMail(string name, string subject, string content, string toMail)
        {
            bool rs = false;
            try
            {
                MailMessage message = new MailMessage();
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // host name
                    Port = 587, // port number
                    EnableSsl = true, // whether your smtp server requires SSL
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential
                    {
                        UserName = _email,
                        Password = _passwordEmail
                    }
                };

                MailAddress fromAddress = new MailAddress(_email, name);
                message.From = fromAddress;
                message.To.Add(toMail);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;
                smtp.Send(message);
                rs = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                rs = false;
            }
            return rs;
        }
    }
}
