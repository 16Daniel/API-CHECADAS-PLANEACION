using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Procesosespecialesusu
    {
        public int Idproceso { get; set; }
        public int Codusuario { get; set; }

        public virtual Procesosespeciale IdprocesoNavigation { get; set; } = null!;
    }
}
