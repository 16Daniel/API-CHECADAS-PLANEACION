using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class ItModIncidencia
    {
        public ItModIncidencia()
        {
            ItPerfiles = new HashSet<ItPerfile>();
        }

        public int Id { get; set; }

        public virtual ICollection<ItPerfile> ItPerfiles { get; set; }
    }
}
