using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2
{
    public partial class Articuloscommerce
    {
        public int Codarticulo { get; set; }
        public int Codidioma { get; set; }
        public byte[]? Desccorta { get; set; }
        public byte[]? Desclarga { get; set; }
        public string? Desccortahtml { get; set; }
        public string? Desclargahtml { get; set; }

        public virtual Articulo CodarticuloNavigation { get; set; } = null!;
    }
}
