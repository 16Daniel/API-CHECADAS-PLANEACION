using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class CmrcInforme
    {
        public int Idinforme { get; set; }
        public int Ididioma { get; set; }
        public int? Tipo { get; set; }
        public byte[]? Xtrareport { get; set; }
        public string? Texto { get; set; }
        public string? Texto2 { get; set; }
        public string? Descripcion { get; set; }
    }
}
