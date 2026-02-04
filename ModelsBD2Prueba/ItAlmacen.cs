using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class ItAlmacen
    {
        public int Idaccount { get; set; }
        public string Codalmacen { get; set; } = null!;

        public virtual ItRebelAccount IdaccountNavigation { get; set; } = null!;
    }
}
