using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Procesosespecialesexec
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Hora { get; set; }
        public int Codusuario { get; set; }

        public virtual Procesosespeciale IdNavigation { get; set; } = null!;
    }
}
