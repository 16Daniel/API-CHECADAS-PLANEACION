using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class CommerceVersione
    {
        public int Id { get; set; }
        public long? Version { get; set; }
        public DateTime? Fechamodificado { get; set; }
    }
}
