using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using LibreriaDAIR.Models;

namespace LibreriaDAIR.Controllers
{
    public class ResenniasController : Controller
    {
        private static readonly HttpClient cliente = new HttpClient();
        public async Task<ActionResult> BuscarResenias(string titulolibro)
        {
            if (string.IsNullOrEmpty(titulolibro))
            {
                ViewBag.ErrorMessage = "Debe ingresar el título del libro para buscar las reseñas";
                return View();
            }

            string ApiUrl = $"http://localhost:3302/resennias/{titulolibro}";

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

                var resenias = JsonConvert.DeserializeObject<RespuestaResenias>(respuesta);

                // Verifica que haya datos en la respuesta
                if (resenias?.Data == null || resenias.Data.Count == 0)
                {
                    ViewBag.ErrorMessage = "No se han encontrado reseñas para este libro.";
                    return View();
                }

                return View(resenias);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error al conectarse con el servicio: {ex.Message}";
                return View();
            }
        }
    }
}
