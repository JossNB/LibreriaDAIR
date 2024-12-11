using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Text;

namespace LibreriaDAIR.Models
{
    public class ClienteModelo
    {
        public string _IdCliente { get; set; }
        public string _Nombre { get; set; }
        public string _Apellidos { get; set; }
        public string _Telefono { get; set; }
        public string _Email { get; set; }
        public string _Password { get; set; }
        public string _Respuesta1 { get; set; }
        public string _Respuesta2 { get; set; }
        public string _Respuesta3 { get; set; }
        public string _Error { get; set; }



        //Registro una nueva cuenta del cliente
        public string EscribirCliente(string identificacion, string nombre, string apellidos, string telefono, string email, string password)
        {
            string idCliente = identificacion;

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string procedimiento_almacenado = "SP_AgregarCliente"; //Utilizo un procedimiento almacenado

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(procedimiento_almacenado, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Identificacion", identificacion);
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@Apellidos", apellidos);
                    command.Parameters.AddWithValue("@Telefono", telefono);
                    command.Parameters.AddWithValue("@CorreoElectronico", email);
                    command.Parameters.AddWithValue("@Contrasenna", password);

                    command.ExecuteNonQuery();
                }
            }
            return idCliente;
        } 
        
        public void GuardarRespuestasdeSeguridad(string idCliente, string respuesta1, string respuesta2, string respuesta3)
        {
            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_RespuestasdeSeguridad", connection))
                {
                    command.CommandType= CommandType.StoredProcedure;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@IdCliente", idCliente);
                    command.Parameters.AddWithValue("@IdPregunta", 1);
                    command.Parameters.AddWithValue("@Respuesta", respuesta1);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@IdCliente", idCliente);
                    command.Parameters.AddWithValue("@IdPregunta", 2);
                    command.Parameters.AddWithValue("@Respuesta", respuesta2);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@IdCliente", idCliente);
                    command.Parameters.AddWithValue("@IdPregunta", 3);
                    command.Parameters.AddWithValue("@Respuesta", respuesta3);
                    command.ExecuteNonQuery();

                }
            }
        }
    }
}