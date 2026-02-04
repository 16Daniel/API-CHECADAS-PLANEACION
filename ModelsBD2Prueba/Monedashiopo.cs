using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Monedashiopo
    {
        public long Idmoneda { get; set; }
        public string? Descripcion { get; set; }
        public short? Decimales { get; set; }
        public string? Iniciales { get; set; }
        public bool? Inicialesdelante { get; set; }
        public byte[]? Imagen { get; set; }
        public bool? Descatalogada { get; set; }
        public byte[] Version { get; set; } = null!;
        public long? Versioninsert { get; set; }
    }
}
