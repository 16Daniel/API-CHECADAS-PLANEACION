using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Linea
    {
        public int Codmarca { get; set; }
        public int Codlinea { get; set; }
        public string? Descripcion { get; set; }

        public virtual Marca CodmarcaNavigation { get; set; } = null!;
    }
}
