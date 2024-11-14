using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Incidenciasnoautovalidable
    {
        public int Idincidencia { get; set; }
        public string? Problemas { get; set; }

        public virtual Incidencia IdincidenciaNavigation { get; set; } = null!;
    }
}
