﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class BiInformesUsuario
    {
        public int Idusuario { get; set; }
        public int Idinforme { get; set; }

        public virtual BiInforme IdinformeNavigation { get; set; } = null!;
    }
}