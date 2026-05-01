namespace ServiFlow.ViewModels
{
    public class RankingClienteItemVM
    {
        public int UsuarioId { get; set; }
        public int PosicionRanking { get; set; }

        public string NombreCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public string? TelefonoCliente { get; set; }

        public int TotalCitas { get; set; }
        public DateTime? ProximaCita { get; set; }
    }
}