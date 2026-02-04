using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class BloqueosCancelado
    {
        public int Id { get; set; }
        public string? Info { get; set; }
        public DateTime? Fechacancelacion { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
