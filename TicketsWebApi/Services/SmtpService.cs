using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TicketsWebApi.Helpers;
using TicketsWebApi.Models;

namespace TicketsWebApi.Services
{
    public interface ISmtpService
    {
        void SendNotify(Ticket ticket);
    }

    public class SmtpService : ISmtpService
    {
        public SmtpService(IOptions<SmtpData> data, AppDbContext context)
        {
            Data = data.Value;
            Admin = context.Users.SingleOrDefault(user => user.Role == UserRole.Administrator);
            Client = new SmtpClient(data.Value.Address, data.Value.Port)
            {
                Credentials = new NetworkCredential(data.Value.Login, data.Value.Password),
                EnableSsl = true
            };
        }

        private User Admin { get; set; }
        private SmtpClient Client { get; set; }
        private SmtpData Data { get; set; }

        public void SendNotify(Ticket ticket)
        {
            SendTo(ticket, Admin.Email);
            SendTo(ticket, ticket.UserEmail);
        }

        private void SendTo(Ticket ticket, string recieverAddress)
        {
            var message = new MailMessage(new MailAddress(Data.Login), new MailAddress(recieverAddress))
            {
                Subject = "Ticket status",
                Body = $"Your ticket {ticket.Title} status have changed to {ticket.Status.ToString()}"
            };
            Client.Send(message);
        }
    }
}