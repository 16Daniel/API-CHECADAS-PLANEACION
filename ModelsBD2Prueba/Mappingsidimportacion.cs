using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Mappingsidimportacion
    {
        public int Idmap { get; set; }
        public int Posicion { get; set; }
        public string? Fieldname { get; set; }

        public virtual Mappingscab IdmapNavigation { get; set; } = null!;
    }
}
