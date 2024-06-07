using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class BiPermisosUsuario
    {
        public int Idusuario { get; set; }
        public string? Permisos { get; set; }
        public int? Idinformedefecto { get; set; }

        public virtual IeInforme? IdinformedefectoNavigation { get; set; }
    }
}
