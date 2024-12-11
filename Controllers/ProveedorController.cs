using LibreriaDAIR.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LibreriaDAIR.Controllers
{
    public class ProveedorController : Controller
    {
        private static readonly HttpClient cliente = new HttpClient();
        public async Task<ActionResult> BuscarProveedor(string titulolibro)
        {
            if (string.IsNullOrEmpty(titulolibro))
            {
                ViewBag.ErrorMessage = "Debe ingresar el título del libro para buscar las reseñas";
                return View();
            }

            string ApiUrl = $"http://localhost:3303/consultaproveedor/{titulolibro}";

            try
            {
                HttpResponseMessage response = await cliente.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                string respuesta = await response.Content.ReadAsStringAsync();

                // Verifica la respuesta
                if (string.IsNullOrEmpty(respuesta))
                {
                    ViewBag.ErrorMessage = "No se encontraron datos en la API.";
                    return View();
                }

                var proveedores = JsonConvert.DeserializeObject<RespuestaProveedor>(respuesta);

                // Verifica que haya datos en la respuesta
                if (proveedores?.Data == null || proveedores.Data.Count == 0)
                {
                    ViewBag.ErrorMessage = "No se han encontrado proveedores para este libro.";
                    return View();
                }

                return View(proveedores);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error al conectarse con el servicio: {ex.Message}";
                return View();
            }
        }
    }
}