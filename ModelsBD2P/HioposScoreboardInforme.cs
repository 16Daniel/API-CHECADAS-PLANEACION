﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class HioposScoreboardInforme
    {
        public int Id { get; set; }
        public int Idinforme { get; set; }
        public int Posicion { get; set; }

        public virtual HioposScoreboard IdNavigation { get; set; } = null!;
    }
}
