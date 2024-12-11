using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibreriaDAIR.Models
{
    public class InformacionViewModel
    {
        public string _ClienteID { get; set; }
        public string _Nombre { get; set; }
        public string _Apellidos { get; set; }
        public string _Telefono { get; set; }

        // 
        public string _Direccion { get; set; }
        public string _Provincia { get; set; }
        public string _Canton { get; set; }
        public string _Distrito { get; set; }
        public string _CodPostal { get; set; }
    }
}