using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Serviciosglobalescomentario
    {
        public int Codservicio { get; set; }
        public int Numcomentario { get; set; }
        public int? Codidioma { get; set; }
        public string? Comentario { get; set; }

        public virtual Serviciosglobale CodservicioNavigation { get; set; } = null!;
    }
}
