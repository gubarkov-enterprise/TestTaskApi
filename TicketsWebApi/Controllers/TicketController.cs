using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsWebApi.Models;
using TicketsWebApi.Services;

namespace TicketsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        public TicketController(AppDbContext context, ISmtpService smtp)
        {
            Context = context;
            Smtp = smtp;
        }

        private AppDbContext Context { get; }
        private ISmtpService Smtp { get; }

        [HttpPost, AllowAnonymous]
        public IActionResult Post([FromBody] Ticket ticket)
        {
            ticket.Status = TicketStatus.Unchecked;
            ticket.DateReg = DateTime.Now.ToLocalTime();
            Context.Tickets.Add(ticket);
            Context.SaveChanges();
            if (!string.IsNullOrEmpty(ticket.UserEmail))
                Smtp.SendNotify(ticket);

            return Ok();
        }

        [HttpGet, Authorize(Roles = "Administrator")]
        public Ticket[] Get()
        {
            return Context.Tickets.OrderByDescending(ticket => ticket.DateReg).ToArray();
        }

        [Route("{id:int}/ChangeStatus")]
        [HttpGet, Authorize(Roles = "Administrator")]
        public IActionResult ChangeStatus(int id, [FromQuery] TicketStatus status)
        {
            var ticket = Context.Tickets.Find(id);
            ticket.Status = status;
            ticket.LastActionDate = DateTime.Now;
            Context.SaveChanges();
            if (!string.IsNullOrEmpty(ticket.UserEmail))
                Smtp.SendNotify(ticket);
            return Ok();
        }
    }
}