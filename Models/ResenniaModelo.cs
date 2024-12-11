using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibreriaDAIR.Models
{
    public class ResenniaModelo
    {
        public string Titulo {  get; set; }
        public string NombreUsuario { get; set; }
        public int Calificacion {  get; set; }
        public string Comentario { get; set; }
        public DateTime FechaResennia { get; set; }
    }
}