using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Tarifashotelcalendario
    {
        public DateTime Dia { get; set; }
        public int Codcliente { get; set; }
        public int Codtarifa { get; set; }
        public int Idtemporada { get; set; }
        public byte[]? Version { get; set; }

        public virtual Tarifashotel CodtarifaNavigation { get; set; } = null!;
        public virtual Temporadashotel IdtemporadaNavigation { get; set; } = null!;
    }
}
