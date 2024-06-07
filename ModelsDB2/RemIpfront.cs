using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class RemIpfront
    {
        public int Idfront { get; set; }
        public int Tipo { get; set; }
        public string? Ip { get; set; }
        public int? Puerto { get; set; }
        public string? Usuario { get; set; }
        public string? Passw { get; set; }
    }
}
