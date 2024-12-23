using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class ModificacionesPedSuc
    {
        public int Id { get; set; }
        public string Modificacion { get; set; } = null!;
        public string ValAntes { get; set; } = null!;
        public string ValDespues { get; set; } = null!;
        public string Justificacion { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public int Idusuario { get; set; }
        public int Idpedido { get; set; }
        public int Codart { get; set; }
        public bool Enviado { get; set; }
        public string? Comentario { get; set; }
    }
}
