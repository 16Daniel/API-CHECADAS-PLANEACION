using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class Notificacione
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public bool Success { get; set; }
    }
}
