using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class IeCamposOrigenesRelacionale
    {
        public int IdCampoRelacional { get; set; }
        public int IdOrigenRelacional { get; set; }
        public int Orden { get; set; }

        public virtual IeCamposRelacionale IdCampoRelacionalNavigation { get; set; } = null!;
        public virtual IeOrigenesRelacionale IdOrigenRelacionalNavigation { get; set; } = null!;
    }
}
