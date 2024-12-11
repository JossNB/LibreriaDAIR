using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibreriaDAIR.Models;

namespace LibreriaDAIR.Controllers
{
    public class LibrosController : Controller
    {
        // GET: Libros
        public ActionResult Index() //
        {
            List<LibroModelo> Libros = new List<LibroModelo>(); //Utilizo una lista para guardar los resultados de los libros

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString; //Conección a la base de datos
            string procedimiento_almacenado = "ObtenerLibroxCategoria"; //Procedimiento almacenado

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand(procedimiento_almacenado, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure; 

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    LibroModelo libro = new LibroModelo //Crea un modelo de cada libro con los datos leidos
                    {
                        _idLibro = (int)reader["IdLibro"],
                        _titulo = reader["Titulo"].ToString(),
                        _imagen = reader["Imagen"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["Imagen"]) : null,
                        _precio = reader["Precio"] != DBNull.Value ? Convert.ToDecimal(reader["Precio"]) : 0m
,
                        Categoria = new CategoriaModelo //Crea un modelo para la categoria
                        {
                            _Id = (int)reader["CategoriaId"],
                            _Nombre = reader["CategoriaNombre"].ToString()
                        }
                    };
                    Libros.Add(libro); //Guardo cada modelo libro en la lista
                }
                reader.Close(); 
            }
            return View(Libros);
        }
        //Vista de cada libro con su información
        public ActionResult DetallesLibro(int id) //Es el metodo que mostrará los detalles de cada libro utilizando el id del libro
        {
           LibroModelo libro = ObtenerLibroId(id); //Se llama el metodo que contiene la información del libro
                return View(libro);
        }

        private LibroModelo ObtenerLibroId(int id)
        {
            LibroModelo libro = null;

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerDetalleLibro", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdLibro", id);

                    using (SqlDataReader reader = command.ExecuteReader())//Para leer los datos desde la base
                    {
                        if(reader.Read())//Si se encuentra el libro
                        {
                            libro = new LibroModelo // Se agregan los valores a el modelo libro
                            {
                                _titulo = reader["Titulo"].ToString(),
                                _precio = reader["Precio"] != DBNull.Value ? Convert.ToDecimal(reader["Precio"]) : 0m, 
                                _autor = reader["Autor"].ToString(),
                                _editorial = reader["Editorial"].ToString(),
                                _idioma = reader["Idioma"].ToString(),
                                _numPaginas = (int)reader["NumPaginas"],
                                _imagen = reader["Imagen"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["Imagen"]) : null
                            };
                        }
                    }
                }
            }
                return libro;//Devuelve el modelo
        }
    }
}