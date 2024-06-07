using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Gruposalmacencriterio
    {
        public Gruposalmacencriterio()
        {
            Gruposalmacenlincriterios = new HashSet<Gruposalmacenlincriterio>();
        }

        public int Idgrupo { get; set; }
        public int Idcriterio { get; set; }
        public string? Titulocolumna { get; set; }

        public virtual ICollection<Gruposalmacenlincriterio> Gruposalmacenlincriterios { get; set; }
    }
}
