using System;
using System.Linq;
using Newtonsoft.Json;

namespace TicketsWebApi.Models
{
    public class Report
    {
        public Report(Ticket[] tickets)
        {
            if (tickets.Length == 0)
                return;
            Tickets = tickets.Select(ticket => new TicketResult
            {
                ExecutionTime = ticket.ExecutionTime,
                Report = this,
                Title = ticket.Title,
                Id = ticket.Id,
                Status = ticket.Status.ToString()
            }).ToArray();
            AverageExecutionTime = new TimeSpan(Convert.ToInt64(Tickets.Average(result => result.ExecutionTime.Ticks)));
        }

        public TicketResult[] Tickets { get; set; }

        public TimeSpan AverageExecutionTime { get; private set; }
    }

    public class TicketResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        [JsonIgnore] public Report Report { get; set; }
        public bool Warning => ExecutionTime > Report.AverageExecutionTime * 2;
        public string Status { get; set; }
    }
}