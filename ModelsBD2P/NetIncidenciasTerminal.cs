﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class NetIncidenciasTerminal
    {
        public int IdIncidencia { get; set; }
        public Guid? IdTerminal { get; set; }
        public DateTime? Fecha { get; set; }
        public int? GrupoIncidencia { get; set; }
        public int? TipoIncidencia { get; set; }
        public string? Descripcion { get; set; }
        public bool? EstaLeido { get; set; }

        public virtual NetTerminal? IdTerminalNavigation { get; set; }
    }
}
