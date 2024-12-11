using LibreriaDAIR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;
using System.Web.Helpers;


namespace LibreriaDAIR.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string password) //Función principal para el inicio de sesión
        {
            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand("sp_IniciarSesion", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();

                bool temporal = false;
                string nombre = null;
                DateTime fechaContrasenna;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        temporal = reader.GetBoolean(reader.GetOrdinal("Temporal"));
                        nombre = reader.GetString(reader.GetOrdinal("Nombre"));
                        fechaContrasenna = reader.GetDateTime(reader.GetOrdinal("FechaActualizacion"));
                    }
                    else
                    {
                        if (Request.IsAjaxRequest())
                        {
                            return Json(new { success = false, message = "Error en las credenciales" });
                        }
                        return View();
                    }
                }

                if(DateTime.Now > fechaContrasenna.AddMonths(3))
                {
                    TempData["Email"] = email;
                    TempData["Vencimiento"] = "Su contraseña venció. Cree una nueva contraseña";

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = false, redirectUrl = Url.Action("CambiarPassword") });
                    }
                    return RedirectToAction("CambiarPassword");
                }

                RegistroDeAuditoria(email);

                if (temporal)
                {
                    TempData["Email"] = email;

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = false, redirectUrl = Url.Action("CambiarPassword") });
                    }
                    return RedirectToAction("CambiarPassword");
                }
                else
                {
                    string token = CrearToken();

                    SqlCommand commandtoken = new SqlCommand("CrearToken", connection);
                    commandtoken.CommandType = System.Data.CommandType.StoredProcedure;
                    commandtoken.Parameters.AddWithValue("@Email", email);
                    commandtoken.Parameters.AddWithValue("@Token", token);
                    commandtoken.ExecuteNonQuery();

                    EnviarToken(email, token);

                    TempData["Nombre"] = nombre;
                    TempData["Email"] = email;

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("VerificarToken") });
                    }
                    return RedirectToAction("VerificarToken");
                }
            }
        }

        [HttpGet]
        public ActionResult CambiarPassword() //Vista para cambiar contraseña
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarPassword(string NuevaContrasenna) //Función para cambiar la contraseña
        {
        
            if (string.IsNullOrEmpty(NuevaContrasenna))
            {
                ViewBag.Error = "La nueva contraseña no puede estar vacía";
                return View("CambiarPassword");
            }

            string validacionPassword = @"^(?=.*\d)(?=.*[\u0021-\u002b\u003c-\u0040])(?=.*[A-Z])(?=.*[a-z])\S{8,10}$";

            if (!Regex.IsMatch(NuevaContrasenna, validacionPassword))
            {
                ViewBag.Error = "La contraseña debe tener 8-10 caracteres con mayúsculas, minúsculas, números y un símbolo";
                return View("CambiarPasswor");
            }

            string email = TempData["Email"]?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index");
            }

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand("CambiarPassword", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@NuevaPassword", NuevaContrasenna);
                command.Parameters.AddWithValue("@Temporal", false);

                connection.Open();

                int filas = command.ExecuteNonQuery();

                if(filas == 0)
                {
                    ViewBag.Error = "No se pudo actualizar la contraseña. Inténtalo de nuevo";
                    return View("CambiarPasswor");
                }
               
            }
            ViewBag.Message = "Contraseña actualizada exitosamente. Vuelva a iniciar sesión";
            return View("CambiarPassword");
        }

        [HttpGet]
        public ActionResult PasswordOlvidada()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordOlvidada(string email) 
        {
            string passwordtemporal = CrearContraTemporal();

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand("PasswordTemporal", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PasswordTemporal", passwordtemporal);
                command.Parameters.AddWithValue("@Temporal", true);

                connection.Open();
                command.ExecuteNonQuery();
            }
            RegistroDeAuditoriaContrasennaOlvidada(email);

            EnviarContraTemporal(email, passwordtemporal);

            ViewBag.Message = "Se envió una contraseña temporal a su correo electrónico";
            return View("PasswordOlvidada");
        }

        private string CrearContraTemporal(int longitud = 8) //Función para crear una contraseña temporan random
        {
            const string digitosvalidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            var randomcontra = new Random();
            return new string(Enumerable.Repeat(digitosvalidos, longitud).Select(i => i[randomcontra.Next(i.Length)]).ToArray());
        }

        public void EnviarContraTemporal(string email, string passwordtemporal) //Función para enviar el correo con la contraseña temporal
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("libreriadair@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Recuperar contraseña";
            mail.Body = $"Su contraseña temporal: {passwordtemporal}, al iniciar sesión será redirigido a la pagina de restablecimiento de contraseña";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("libreriadair@gmail.com", "ztcsdfunflghbkyg"),
                EnableSsl = true
            };

            smtpClient.Send(mail);

        }

        public ActionResult VerificarToken()
        {
            return View();
        }

        [HttpPost]

        public ActionResult VerificarToken(string token) //Función para verificar que el token ingresado por el usuario sea valido
        {
            string email = TempData["Email"]?.ToString();
            string nombre = TempData["Nombre"]?.ToString();

            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nombre))
            {
                return RedirectToAction("Index");
            }

            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand("ValidarToken", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Token", token);

                connection.Open();
                int tokenvalido = (int)command.ExecuteScalar();

                if(tokenvalido == 1)
                {
                    Session["Email"] = email;
                    Session["Nombre"] = nombre;

                    return RedirectToAction("Index", "Principal");
                }
                else
                {
                    ViewBag.ErrorMessage = "El token es inválido o ya expiró";
                    return View();
                }
            }

        }
        
        public ActionResult RegistroCliente()
        {
            return View();
        }

        public ActionResult InsertarCliente() //Función para registrar a un nuevo cliente
        {
            string identificacion = Request.Form["identificacion"].ToString();

            string nombre = Request.Form["nombre"].ToString();

            string apellidos = Request.Form["apellidos"].ToString();

            string telefono = Request.Form["telefono"].ToString();

            string email = Request.Form["email"].ToString();

            string password = Request.Form["password"].ToString();

            string respuesta1 = Request.Form["respuesta1"].ToString();

            string respuesta2 = Request.Form["respuesta2"].ToString();

            string respuesta3 = Request.Form["respuesta3"].ToString();

            string validacionCorreo = @"^(([^<>()\[\]\\.,;:\s@”]+(\.[^<>()\[\]\\.,;:\s@”]+)*)|(“.+”))@((\[[0–9]{1,3}\.[0–9]{1,3}\.[0–9]{1,3}\.[0–9]{1,3}])|(([a-zA-Z\-0–9]+\.)+[a-zA-Z]{2,}))$";

            string validacionPassword = @"^(?=.*\d)(?=.*[\u0021-\u002b\u003c-\u0040])(?=.*[A-Z])(?=.*[a-z])\S{8,10}$";

            if (!Regex.IsMatch(email, validacionCorreo))
            {
                ViewBag.Error = "El formato del correo no es valido";
                ViewBag.FormData = Request.Form;
                return View("RegistroCliente");
            }

            if (!Regex.IsMatch(password, validacionPassword))
            {
                ViewBag.Error = "La contraseña debe tener 8-10 caracteres con mayúsculas, minúsculas, números y un símbolo";
                ViewBag.FormData = Request.Form;
                return View("RegistroCliente");
            }

            try
            {
                ClienteModelo cliente = new ClienteModelo();

                string idCliente = cliente.EscribirCliente(identificacion, nombre, apellidos, telefono, email, password);

                cliente.GuardarRespuestasdeSeguridad(idCliente, respuesta1, respuesta2, respuesta3);

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al guardar el cliente: " + ex.Message;
                ViewBag.FormData = Request.Form;
                return View("RegistroCliente"); // Retorna la vista con el mensaje de error
            }
            return View();
            
        }

        private string CrearToken() //Se genera un token aleatorio a cada cliente
        {
            Random tokenrandom = new Random();
            return tokenrandom.Next(100000, 999999).ToString();
        }

        public void EnviarToken(string email, string token) //Función para enviar el correo con el token
        {
            var CorreoOrigen = new MailAddress("libreriadair@gmail.com", "Libreria DAIR");
            var CorreoDestino = new MailAddress(email);
            const string contra = "ztcsdfunflghbkyg";
            const string asunto = "Token de verificación para su cuenta";
            string cuerpo = $"Su token de verificación es: {token}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(CorreoOrigen.Address, contra)
            };

            using (var mensaje = new MailMessage(CorreoOrigen, CorreoDestino)
            {
                Subject = asunto,
                Body = cuerpo
            })
            {
                smtp.Send(mensaje);
            }
            
        }

        public void RegistroDeAuditoria(string correo)
        {
            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand("SP_AuditoriaInicioSesion", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", correo);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void RegistroDeAuditoriaContrasennaOlvidada(string correo)
        {
            string string_conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(string_conexion))
            {
                SqlCommand command = new SqlCommand("SP_AuditoriaOlvidoPassword", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", correo);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}