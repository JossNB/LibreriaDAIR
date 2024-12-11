using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibreriaDAIR.Models
{
    public class LibroModelo
    {
        //Propiedades de cada libro
        public int _idLibro { get; set; }
        public string _titulo { get; set; }
        public string _autor {  get; set; }
        public string _editorial { get; set; }
        public string _idioma { get; set; }
        public int _numPaginas { get; set; }
        public string _imagen { get; set; }
        public decimal _precio { get; set; }
        public int _categoria { get; set; }
        public int _stock { get; set; }
        public int Cantidad { get; set; }
        public CategoriaModelo Categoria { get; set; } //Relación con el modelo Categoria ya que cada libro está asociado a una categoria
    }
}