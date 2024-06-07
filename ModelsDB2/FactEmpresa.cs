﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class FactEmpresa
    {
        public string Id { get; set; } = null!;
        public string? NombreFiscal { get; set; }
        public string? Rfc { get; set; }
        public string? Calle { get; set; }
        public string? Municipio { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? CodigoPostal { get; set; }
        public string? NoExterior { get; set; }
        public string? NoInterior { get; set; }
        public string? Colonia { get; set; }
        public string? Localidad { get; set; }
        public string? Referencia { get; set; }
        public string? CalleExpedicion { get; set; }
        public string? NoExteriorExpedicion { get; set; }
        public string? NoInteriorExpedicion { get; set; }
        public string? ColoniaExpedicion { get; set; }
        public string? LocalidadExpedicion { get; set; }
        public string? ReferenciaExpedicion { get; set; }
        public string? MunicipioExpedicion { get; set; }
        public string? EstadoExpedicion { get; set; }
        public string? PaisExpedicion { get; set; }
        public string? CodigoPostalExpedicion { get; set; }
        public string? RutaCertificado { get; set; }
        public string? RutaLlave { get; set; }
        public string? ClaveLlave { get; set; }
        public string? SerieTickets { get; set; }
    }
}
