using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibreriaDAIR.Models
{
    public class ProveedorModelo
    {
        public string TituloLibro { get; set; }
        public string Autor {  get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public string NombreProveedor { get; set; }
        public string ContactoProveedor { get; set; }
        public string CorreoProveedor { get; set; }
        public string UbicacionProveedor { get; set; }
    }
}