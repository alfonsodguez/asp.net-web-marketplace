using marketplace.Models;
using marketplace.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace marketplace.Controllers
{
    public class TiendaController : Controller
    {
        private readonly IDBAccess _accesoBD;

        public TiendaController(IDBAccess servicioBDInyect)
        {
            this._accesoBD = servicioBDInyect;
        }


        [HttpGet]
        public IActionResult Principal()
        {
            return View();
        }

#nullable enable
        [HttpGet]
        public async Task<IActionResult> Productos(String? id)
        {
#nullable disable
            String filtro = "categoria";
            String defaultPath = "15-8-5";
            String pathcategoria = id;
            String valor = String.IsNullOrEmpty(pathcategoria) ? defaultPath : pathcategoria;

            List<Producto> productos = await this._accesoBD.DevolverProductos(filtro, valor);

            try
            {
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datoscliente"));
                if (cliente == null)
                {
                    throw new Exception();
                }

                return View(productos);
            }
            catch (Exception ex)
            {
                Cliente nuevoCliente = new Cliente();
                //añado cliente a la session 
                HttpContext.Session.SetString("datoscliente", JsonSerializer.Serialize(nuevoCliente));
                return View(productos);
            }
        }
    }
}
