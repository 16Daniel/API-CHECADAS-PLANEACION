using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2
{
    public partial class Regimenret
    {
        public Regimenret()
        {
            Codarticulos = new HashSet<Articulo>();
            CodarticulosNavigation = new HashSet<Articulo>();
        }

        public int Codigo { get; set; }
        public string Descripcion { get; set; } = null!;
        public string? Claveretarticulo { get; set; }

        public virtual ICollection<Articulo> Codarticulos { get; set; }
        public virtual ICollection<Articulo> CodarticulosNavigation { get; set; }
    }
}
