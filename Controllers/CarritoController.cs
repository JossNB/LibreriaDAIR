using LibreriaDAIR.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace LibreriaDAIR.Controllers
{
    public class CarritoController : Controller
    {
        // GET: Carrito
        public ActionResult Index()
        {
            var Carrito = (List<CarritoModelo>)Session["Carrito"] ?? new List<CarritoModelo>();
            return View(Carrito);
        }

        [HttpPost]
        public ActionResult AgregarCarrito(string titulo, decimal precio, int cantidad)
        {
            var Carrito = (List<CarritoModelo>)Session["Carrito"] ?? new List<CarritoModelo>();

            var LibroExiste = Carrito.FirstOrDefault(l => l.Titulo == titulo);

            if (LibroExiste != null) 
            {
                LibroExiste.Cantidad += cantidad;
            }
            else
            {
                Carrito.Add(new CarritoModelo { Titulo = titulo, Precio = precio, Cantidad = cantidad });
            }

            Session["Carrito"] = Carrito;

            // Devolver un JSON con el mensaje para AJAX
            return Json(new { mensaje = $"{titulo} se ha agregado al carrito." });

        }
        
        public ActionResult EliminarLibro(string titulo)
        {

            var Carrito = (List<CarritoModelo>)Session["Carrito"] ?? new List<CarritoModelo>();

            var Eliminar = Carrito.FirstOrDefault(l => l.Titulo == titulo);

            if (Eliminar != null)
            {
                Carrito.Remove(Eliminar);
            }

            Session["Carrito"] = Carrito;
            return RedirectToAction("Index");
        }

        public ActionResult VaciarCarrito()
        {
            Session["Carrito"] = null;
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult ActualizarCantidad(string titulo, decimal precio, string accion)
        {
            var Carrito = (List<CarritoModelo>)Session["Carrito"] ?? new List<CarritoModelo>();

            // Buscar el libro en el carrito
            var libro = Carrito.FirstOrDefault(l => l.Titulo == titulo);

            if (libro != null)
            {
                if (accion == "aumentar")
                {
                    libro.Cantidad++; // Aumentar la cantidad
                }
                else if (accion == "disminuir" && libro.Cantidad > 1)
                {
                    libro.Cantidad--; // Disminuir la cantidad, sin que sea menor a 1
                }
            }

            // Guardar los cambios en la sesión
            Session["Carrito"] = Carrito;

            return RedirectToAction("Index"); // Redirigir de nuevo al carrito
        }


        public ActionResult PasarelaPago() { 
            var Carrito = (List<CarritoModelo>)Session["Carrito"] ?? new List<CarritoModelo>();
            var total = Carrito.Sum(item => item.Precio * item.Cantidad);

            ViewBag.Total = total;
            return View();
        }

        public async Task<ActionResult> RealizarPagoTarjeta(string Numtarjeta, string Fechavencimiento, string Cvv, decimal MontoPago, string IdTitular)
        {
            var Carrito = (List<CarritoModelo>)Session["Carrito"] ?? new List<CarritoModelo>();
            var total = Carrito.Sum(item => item.Precio * item.Cantidad);

            if (total <= 0) 
            {
                ViewBag.Error = "El total del carrito no puede ser cero.";
                return View("PasarelaPago");
            }

            var pagoDatos = new
            {
                Numtarjeta = Numtarjeta,
                Fechavencimiento = Fechavencimiento,
                Cvv = Cvv,
                MontoPago = MontoPago,
                IdTitular = IdTitular
            };

            var json = JsonConvert.SerializeObject(pagoDatos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var cliente = new HttpClient())
            {
                try
                {
                    var ApiUrl = "http://localhost:3301/pagotarjeta";
                    var response = await cliente.PostAsync(ApiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Vaciar el carrito tras el éxito
                        Session["Carrito"] = new List<CarritoModelo>();
                        return RedirectToAction("ConfirmacionCompra");
                    }
                    else
                    {
                        var mensajeError = await response.Content.ReadAsStringAsync();

                        // Deserializar el mensaje de error (JSON) para obtener el mensaje más claro
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(mensajeError);
                            if (errorObj != null && errorObj.ContainsKey("error"))
                            {
                                ViewBag.Error = "Error al procesar el pago: " + errorObj["error"];
                            }
                            else
                            {
                                ViewBag.Error = "Error al procesar el pago, por favor intente nuevamente.";
                            }
                        }
                        catch (JsonException)
                        {
                            // Si no se puede deserializar, mostrar el mensaje crudo
                            ViewBag.Error = "Error al procesar el pago: " + mensajeError;
                        }
                    }
                }catch (Exception ex)
                {
                    ViewBag.Error = "Error al conectar con el servicio: " + ex.Message;
                }
            }
            ViewBag.Total = total;
            return View("PasarelaPago");
        }

        public ActionResult ConfirmacionCompra()
        {
            return View();
        }
    }
}









