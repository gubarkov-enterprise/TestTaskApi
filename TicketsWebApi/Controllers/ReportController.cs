using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsWebApi.Models;

namespace TicketsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Auditor")]
    public class ReportController : ControllerBase
    {
        private AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public Report Get()
        {
            return new Report(_context.Tickets
                .Where(ticket => ticket.LastActionDate >= DateTime.Now.Subtract(TimeSpan.FromDays(7))).ToArray());
        }

        [Route("details/{id:int}")]
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var ticket = _context.Tickets.Find(id);
            return Ok(new
            {
                ticket.Title,
                ticket.Content,
                created = ticket.DateReg.ToString("F"),
                completed = ticket.LastActionDate.ToString("F"),
                responseTime = ticket.ExecutionTime
            });
        }
    }
}