using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Mappingsautomatizadosfile
    {
        public int Id { get; set; }
        public int Numfichero { get; set; }
        public string? Fichero { get; set; }
        public string? Ficherocopiadoa { get; set; }

        public virtual Mappingsautomatizado IdNavigation { get; set; } = null!;
    }
}
