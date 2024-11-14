using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Gruposocupante
    {
        public Gruposocupante()
        {
            Condicionesgruposocupantes = new HashSet<Condicionesgruposocupante>();
        }

        public int Idgrupo { get; set; }
        public string? Descripcion { get; set; }
        public byte[] Version { get; set; } = null!;

        public virtual ICollection<Condicionesgruposocupante> Condicionesgruposocupantes { get; set; }
    }
}
