using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemIncidencia
    {
        public int Idincidencia { get; set; }
        public int Tipo { get; set; }
        public string? Serie { get; set; }
        public int? Numero { get; set; }
        public DateTime? Fechadoc { get; set; }
        public int? Codproveedor { get; set; }
        public int? Idincidenciaorig { get; set; }
    }
}
