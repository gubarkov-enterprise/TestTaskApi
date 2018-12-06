namespace TicketsWebApi.Helpers
{
    public class AppSettings
    {
        public bool Initializated { get; set; }

        public string ConnectionString { get; set; }
        public string AdminLogin { get; set; }
        public string AdminPassword { get; set; }
    }
}