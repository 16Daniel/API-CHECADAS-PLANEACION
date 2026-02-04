using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Modificadoresidioma
    {
        public int Codmodificador { get; set; }
        public int Codidioma { get; set; }
        public string? Descripcion { get; set; }
        public byte[]? Version { get; set; }

        public virtual Idioma CodidiomaNavigation { get; set; } = null!;
        public virtual Modificadorescab CodmodificadorNavigation { get; set; } = null!;
    }
}
