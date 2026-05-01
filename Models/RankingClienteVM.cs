using System.Collections.Generic;

namespace ServiFlow.ViewModels
{
    public class RankingClientesVM
    {
        public int EmprendimientoId { get; set; }
        public string NombreEmprendimiento { get; set; } = string.Empty;

        public List<RankingClienteItemVM> ClientesTop { get; set; } = new();
        public List<RankingClienteItemVM> ClientesPaginados { get; set; } = new();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }

        public string? SearchTerm { get; set; }
        public string SearchBy { get; set; } = "nombre";
    }
}