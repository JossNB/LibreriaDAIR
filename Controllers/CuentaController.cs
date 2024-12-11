using System;
using LibreriaDAIR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;


namespace LibreriaDAIR.Controllers
{
    public class CuentaController : Controller
    {
        // GET: Cuenta
        public ActionResult Index()
        {
            string email = Session["Email"]?.ToString();
            if (email == null)
            {
                return RedirectToAction("Index", "Principal"); 
            }

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerCliente", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);

                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    InformacionViewModel cliente = new InformacionViewModel
                    {
                        _ClienteID = reader["Identificacion"].ToString(),
                        _Nombre = reader["Nombre"].ToString(),
                        _Apellidos = reader["Apellidos"].ToString(),
                        _Telefono = reader["Telefono"].ToString()
                    };
                    return View(cliente);
                }

            }
            ViewBag.ErrorMessage = "No se encontró información";
            return View();
        }

        private readonly string _stringConexion;
        public CuentaController()
        {
            _stringConexion = System.Configuration.ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
        }            

        [HttpGet]

        public JsonResult ObtenerUbicaciones(int? relacion)
        {
            var Ubicaciones = new List<object>();

            using(SqlConnection connection = new SqlConnection(_stringConexion))
            {
                connection.Open();

                string query = relacion == null
                    ? "SELECT IdUbicacion, Descripcion FROM InfoUbicaciones WHERE Relacion IS NULL"
                    : "SELECT IdUbicacion, Descripcion FROM InfoUbicaciones WHERE Relacion = @Relacion";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if(relacion != null)
                    {
                        command.Parameters.AddWithValue("@Relacion", relacion.Value);
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Ubicaciones.Add(new
                            {
                                IdUbicacion = reader["IdUbicacion"],
                                Descripcion = reader["Descripcion"]

                            });
                        }
                    }   
                }
            }
            return Json(Ubicaciones, JsonRequestBehavior.AllowGet);
        }

       





















        [HttpPost]
        public ActionResult GuardarDireccion(DireccionModelo model)
        {
            if (ModelState.IsValid)
            {
                string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(string_conexion))
                {
                    SqlCommand cmd = new SqlCommand("GuardarDireccion", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", model._ClienteID);
                    cmd.Parameters.AddWithValue("@Pais", model._IdPais);
                    cmd.Parameters.AddWithValue("@Provincia", model._Provincia);
                    cmd.Parameters.AddWithValue("@Canton", model._Canton);
                    cmd.Parameters.AddWithValue("@Distrito", model._Distrito);
                    cmd.Parameters.AddWithValue("@Direccion", model._Direccion);
                    cmd.Parameters.AddWithValue("@CodPostal", model._CodPostal);

                    connection.Open();
                    cmd.ExecuteNonQuery();

                    TempData["Message"] = "La dirección se ha agregado exitosamente";
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}