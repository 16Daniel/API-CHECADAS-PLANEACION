using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class EstadisticactrlConfig
    {
        public int Idinforme { get; set; }
        public int Codusuario { get; set; }
        public byte[]? Report { get; set; }
        public byte[]? Columns { get; set; }
        public byte[]? Styles { get; set; }
        public string? Datosadicionales { get; set; }
    }
}
