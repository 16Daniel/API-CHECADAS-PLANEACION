using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class BiInforme
    {
        public BiInforme()
        {
            BiInformesUsuarios = new HashSet<BiInformesUsuario>();
        }

        public int Idinforme { get; set; }
        public string? Titulo { get; set; }
        public string? Nombrecubo { get; set; }

        public virtual ICollection<BiInformesUsuario> BiInformesUsuarios { get; set; }
    }
}
