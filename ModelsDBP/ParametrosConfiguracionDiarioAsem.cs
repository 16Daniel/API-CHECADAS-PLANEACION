using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class ParametrosConfiguracionDiarioAsem
    {
        public int Id { get; set; }
        public string NombreClave { get; set; } = null!;
        public string ConfiguracionJson { get; set; } = null!;
        public DateTime? FechaModificacion { get; set; }
    }
}
