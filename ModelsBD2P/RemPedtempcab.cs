using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemPedtempcab
    {
        public RemPedtempcab()
        {
            RemPedtemplins = new HashSet<RemPedtemplin>();
        }

        public int Idpedido { get; set; }
        public string? Supedido { get; set; }
        public int? Codcliente { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Fechaentrega { get; set; }
        public string? Enviopor { get; set; }
        public double? Totalneto { get; set; }

        public virtual ICollection<RemPedtemplin> RemPedtemplins { get; set; }
    }
}
