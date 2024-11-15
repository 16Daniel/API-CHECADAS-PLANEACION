﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class SmsTexto
    {
        public SmsTexto()
        {
            SmsEnviados = new HashSet<SmsEnviado>();
        }

        public int Idsms { get; set; }
        public string? Sms { get; set; }

        public virtual ICollection<SmsEnviado> SmsEnviados { get; set; }
    }
}
