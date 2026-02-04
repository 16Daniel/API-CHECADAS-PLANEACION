using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Tiposterminal
    {
        public Tiposterminal()
        {
            Favoritostiposterminals = new HashSet<Favoritostiposterminal>();
        }

        public int Idtipoterminal { get; set; }
        public string? Descripcion { get; set; }
        public byte[]? Version { get; set; }

        public virtual ICollection<Favoritostiposterminal> Favoritostiposterminals { get; set; }
    }
}
