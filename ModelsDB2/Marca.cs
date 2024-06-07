using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Marca
    {
        public Marca()
        {
            Lineas = new HashSet<Linea>();
        }

        public int Codmarca { get; set; }
        public string? Descripcion { get; set; }
        public byte[]? Version { get; set; }
        public byte[]? Imagen { get; set; }

        public virtual ICollection<Linea> Lineas { get; set; }
    }
}
