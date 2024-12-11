using LibreriaDAIR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibreriaDAIR.Controllers
{
    public class OfertasController : Controller
    {
        // GET: Ofertas
        public ActionResult Index()
        {
            List<OfertaModelo> Ofertas = new List<OfertaModelo>();

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string procedimiento_almacenado = "ObtenerLibroxOferta";

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand(procedimiento_almacenado, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader(); 

                while (reader.Read())
                {
                    OfertaModelo oferta = new OfertaModelo
                    {
                        _IdLibro = (int)reader["IdLibro"],
                        _Titulo = reader["Titulo"].ToString(),
                        _Imagen = reader["Imagen"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["Imagen"]) : null,
                        _Precio = reader["PrecioNormal"] != DBNull.Value ? Convert.ToDecimal(reader["PrecioNormal"]) : 0m,
                        _PrecioDescuento = reader["PrecioConDescuento"] != DBNull.Value ? Convert.ToDecimal(reader["PrecioConDescuento"]) : 0m
                    };
                    Ofertas.Add(oferta);
                }
                reader.Close();

            }
            return View(Ofertas);
        }

        public ActionResult Ofertas()
        {
            List<OfertaModelo> Ofertas = new List<OfertaModelo>();

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string procedimiento_almacenado = "ObtenerLibroxOferta";

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand(procedimiento_almacenado, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OfertaModelo oferta = new OfertaModelo
                    {
                        _IdLibro = (int)reader["IdLibro"],
                        _Titulo = reader["Titulo"].ToString(),
                        _Imagen = reader["Imagen"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["Imagen"]) : null,
                        _Precio = reader["PrecioNormal"] != DBNull.Value ? Convert.ToDecimal(reader["PrecioNormal"]) : 0m,
                        _PrecioDescuento = reader["PrecioConDescuento"] != DBNull.Value ? Convert.ToDecimal(reader["PrecioConDescuento"]) : 0m
                    };
                    Ofertas.Add(oferta);
                }
                reader.Close();

            }
            return View(Ofertas);
        }
    }
}