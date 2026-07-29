using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Indicadore
    {
        public int Id { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Condicion1 { get; set; }
        public string? Condicion2 { get; set; }
        public string? Parametro { get; set; }
        public string? Tipo { get; set; }
        public string? Periodo { get; set; }
        public string? PorcentajeGlobal { get; set; }
        public string? Operacion { get; set; }
        public string? Codalmacen { get; set; }
        public string? Version { get; set; }
        public int? Idsql { get; set; }
    }
}
