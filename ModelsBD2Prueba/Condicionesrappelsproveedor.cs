using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Condicionesrappelsproveedor
    {
        public int Codproveedor { get; set; }
        public int Codrappel { get; set; }
        public int Numlinea { get; set; }
        public string? Masmenos { get; set; }
        public int Dpto { get; set; }
        public int Seccion { get; set; }
        public int Familia { get; set; }
        public int Subfamilia { get; set; }
        public int Marca { get; set; }
        public int Linea { get; set; }
        public string? Referencia { get; set; }

        public virtual Rappelsproveedore Cod { get; set; } = null!;
    }
}
