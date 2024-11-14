using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class IeValoresFiltrosOrigen
    {
        public int IdCubo { get; set; }
        public int IdFiltroOrigen { get; set; }
        public int IdValorFiltroOrigen { get; set; }
        public string Valor { get; set; } = null!;

        public virtual IeFiltrosOrigen Id { get; set; } = null!;
    }
}
