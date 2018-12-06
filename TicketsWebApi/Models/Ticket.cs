using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TicketsWebApi.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UserEmail { get; set; }
        public string Content { get; set; }
        public DateTime DateReg { get; set; }
        public DateTime LastActionDate { get; set; }

        [NotMapped] public TimeSpan ExecutionTime => LastActionDate - DateReg;

        [JsonConverter(typeof(StringEnumConverter))]
        public TicketStatus Status { get; set; }
    }

    public enum TicketStatus
    {
        Approved,
        Rejected,
        Unchecked
    }
}