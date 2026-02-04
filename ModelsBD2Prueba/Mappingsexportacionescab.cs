using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Mappingsexportacionescab
    {
        public Mappingsexportacionescab()
        {
            Mappingsexportacioneslins = new HashSet<Mappingsexportacioneslin>();
        }

        public int Idexportacion { get; set; }
        public int? Idmap { get; set; }
        public DateTime? Fecha { get; set; }

        public virtual ICollection<Mappingsexportacioneslin> Mappingsexportacioneslins { get; set; }
    }
}
