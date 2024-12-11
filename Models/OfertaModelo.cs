using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDAIR.Models
{
    public class OfertaModelo
    {
        public int _IdLibro { get; set; }
        public string _Imagen { get; set; }
        public string _Titulo { get; set; }
        public decimal _Precio { get; set; }
        public decimal _PrecioDescuento { get; set; }

    }
}
