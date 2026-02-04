using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class ItRpedido
    {
        public ItRpedido()
        {
            ItRpedidosLins = new HashSet<ItRpedidosLin>();
        }

        public string Numserie { get; set; } = null!;
        public int Numpedido { get; set; }
        public string N { get; set; } = null!;
        public int? Estatus { get; set; }
        public bool Factura { get; set; }
        public bool EditoPedido { get; set; }
        public int? NumpedidoReasignado { get; set; }
        public int? NumpedidoReasignadoXml { get; set; }
        public string? Xml { get; set; }
        public byte[]? Pdf { get; set; }
        public string? XmlFileName { get; set; }
        public string? PdfFileName { get; set; }
        public string? XmlUuid { get; set; }
        public bool Excel { get; set; }
        public byte[]? ArchivoExcel { get; set; }
        public string? ExcelFileName { get; set; }
        public bool Notificacion { get; set; }
        public int? Incidencia { get; set; }
        public int Autorizacion { get; set; }
        public int? CodvendedorRecibe { get; set; }
        public DateTime? Fechapedido { get; set; }
        public string? Xml2 { get; set; }
        public byte[]? Pdf2 { get; set; }
        public string? XmlFileName2 { get; set; }
        public string? PdfFileName2 { get; set; }
        public string? XmlUuid2 { get; set; }
        public string? Xml3 { get; set; }
        public byte[]? Pdf3 { get; set; }
        public string? XmlFileName3 { get; set; }
        public string? PdfFileName3 { get; set; }
        public string? XmlUuid3 { get; set; }
        public bool Factura3 { get; set; }

        public virtual ICollection<ItRpedidosLin> ItRpedidosLins { get; set; }
    }
}
