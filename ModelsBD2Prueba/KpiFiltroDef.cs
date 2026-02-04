using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class KpiFiltroDef
    {
        public int Idkpi { get; set; }
        public int Idfiltro { get; set; }
        public int Iddetalle { get; set; }
        public string? Joinsql { get; set; }
        public string? Wheresql { get; set; }

        public virtual KpiFiltro IdfiltroNavigation { get; set; } = null!;
        public virtual Kpi IdkpiNavigation { get; set; } = null!;
    }
}
