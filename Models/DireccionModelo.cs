using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibreriaDAIR.Models
{
    public class DireccionModelo
    {
        public string _ClienteID { get; set; }  
        public string _Direccion { get; set; }
        public int _IdPais { get; set; }
        public int _Provincia { get; set; }
        public int _Canton {  get; set; }
        public int _Distrito { get; set; }
        public string _CodPostal { get; set; }
    }
    
}