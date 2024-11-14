using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Estadostipodoc
    {
        public int Id { get; set; }
        public int Idtipodoc { get; set; }
        public string? Estado { get; set; }

        public virtual Tiposdoc IdtipodocNavigation { get; set; } = null!;
    }
}
