using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class PedidosMensual
    {
        public int Id { get; set; }
        public string Ubicacion { get; set; } = null!;
        public string Referencia { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public double ConsumoPromedio { get; set; }
        public double DesviacionEstandar { get; set; }
        public double NivelObjetivo { get; set; }
        public double StockFisico { get; set; }
        public int PedidoSugerido { get; set; }
        public int CodProveedor { get; set; }
        public int IdSucursal { get; set; }
        public string Nombreprov { get; set; } = null!;
        public string Udscaja { get; set; } = null!;
        public double Precio { get; set; }
        public int Tipoimpuesto { get; set; }
        public double Iva { get; set; }
        public string Estatus { get; set; } = null!;
        public DateTime? HoraCargaIcg { get; set; }
        public DateTime Fecha { get; set; }
        public string? Numpedido { get; set; }
        public int? Codarticulo { get; set; }
    }
}
