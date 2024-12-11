using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibreriaDAIR.Models
{
    public class CostosEnvioModelo
    {
        public string Origen {  get; set; }
        public string Destino { get; set; }
        public string Tamanio { get; set; }
        public string Tipo { get; set; }
        public decimal Costo { get; set; }
        public string TiempoEstimado { get; set; }

    }
}