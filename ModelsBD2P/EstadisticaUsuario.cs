using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class EstadisticaUsuario
    {
        public int Idusuario { get; set; }
        public int Tipo { get; set; }
        public int Idinforme { get; set; }
        public string? Favorito { get; set; }

        public virtual Estadistica Estadistica { get; set; } = null!;
    }
}
