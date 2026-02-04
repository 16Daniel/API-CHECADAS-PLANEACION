using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Galerium
    {
        public Galerium()
        {
            Galeriasecciones = new HashSet<Galeriaseccione>();
        }

        public int Idgaleria { get; set; }
        public byte[]? Foto { get; set; }
        public int? Colorfondo { get; set; }
        public int? Colortexto { get; set; }

        public virtual Galeriaidioma? Galeriaidioma { get; set; }
        public virtual ICollection<Galeriaseccione> Galeriasecciones { get; set; }
    }
}
