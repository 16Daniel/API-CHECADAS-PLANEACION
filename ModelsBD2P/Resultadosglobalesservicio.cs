﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Resultadosglobalesservicio
    {
        public Resultadosglobalesservicio()
        {
            Resultadosgeneranservicios = new HashSet<Resultadosgeneranservicio>();
        }

        public int Idtipoasunto { get; set; }
        public int Codservicio { get; set; }
        public int Codresultado { get; set; }
        public string? Nomresultado { get; set; }

        public virtual Serviciosglobale CodservicioNavigation { get; set; } = null!;
        public virtual Tipoasunto IdtipoasuntoNavigation { get; set; } = null!;
        public virtual ICollection<Resultadosgeneranservicio> Resultadosgeneranservicios { get; set; }
    }
}
