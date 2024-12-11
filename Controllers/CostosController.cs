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
    public class CostosController : Controller
    {
        private static readonly HttpClient cliente = new HttpClient();
        public async Task<ActionResult> ConsultarCostosEnvio(string origen, string destino, string tamanio, string tipoenvio)
        {
            if (string.IsNullOrEmpty(origen) && string.IsNullOrEmpty(destino)
            && string.IsNullOrEmpty(tamanio) && string.IsNullOrEmpty(tipoenvio))
            {
                return View(new RespuestaCostosEnvio { Data = new List<CostosEnvioModelo>() });
            }

            string ApiUrl = $"http://localhost:3304/costos";
            string parametros = "";

            if (!string.IsNullOrEmpty(origen)) parametros += $"origen={Uri.EscapeDataString(origen)}&";
            if (!string.IsNullOrEmpty(destino)) parametros += $"destino={Uri.EscapeDataString(destino)}&";
            if (!string.IsNullOrEmpty(tamanio)) parametros += $"tamanio={Uri.EscapeDataString(tamanio)}&";
            if (!string.IsNullOrEmpty(tipoenvio)) parametros += $"tipoenvio={Uri.EscapeDataString(tipoenvio)}&";


            if (!string.IsNullOrEmpty(parametros)) ApiUrl += "?" + parametros;

            try
            {
                HttpResponseMessage response = await cliente.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                string respuesta = await response.Content.ReadAsStringAsync();

                // Verifica la respuesta
                if (string.IsNullOrEmpty(respuesta))
                {
                    ViewBag.ErrorMessage = "No se encontraron datos en la API.";
                    return View(new RespuestaCostosEnvio { Data = new List<CostosEnvioModelo>() });
                }

                var costosenvios = JsonConvert.DeserializeObject<RespuestaCostosEnvio>(respuesta);

                // Verifica que haya datos en la respuesta
                if (costosenvios?.Data == null || costosenvios.Data.Count == 0)
                {
                    ViewBag.ErrorMessage = "No se han encontrado proveedores para este libro.";
                    return View(new RespuestaCostosEnvio { Data = new List<CostosEnvioModelo>() });
                }

                return View(costosenvios);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error al conectarse con el servicio: {ex.Message}";
                return View();
            }
        }
    }
}