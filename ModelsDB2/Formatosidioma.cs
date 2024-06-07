using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Formatosidioma
    {
        public int Codformato { get; set; }
        public int Codidioma { get; set; }
        public string? Descripcion { get; set; }

        public virtual Formato CodformatoNavigation { get; set; } = null!;
        public virtual Idioma CodidiomaNavigation { get; set; } = null!;
    }
}
