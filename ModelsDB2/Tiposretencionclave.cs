using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Tiposretencionclave
    {
        public Tiposretencionclave()
        {
            Tiposretencions = new HashSet<Tiposretencion>();
        }

        public int Id { get; set; }
        public string Clave { get; set; } = null!;
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<Tiposretencion> Tiposretencions { get; set; }
    }
}
