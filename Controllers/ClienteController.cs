using marketplace.Models;
using marketplace.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace marketplace.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IDBAccess _accesoBD;

        public ClienteController(IDBAccess servicioDBInyect)
        {
            this._accesoBD = servicioDBInyect;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Cliente.Credenciales credenciales)
        {
            String email = credenciales.Email;
            String password = credenciales.Password;
            bool credencialesValidas = ModelState.GetValidationState("Email") == ModelValidationState.Valid && ModelState.GetValidationState("Password") == ModelValidationState.Valid;

            if (credencialesValidas)
            {
                Cliente cliente = await this._accesoBD.ComprobarCredenciales(email, password);
                if (cliente != null)
                {
                    HttpContext.Session.SetString("datoscliente", JsonSerializer.Serialize(cliente));
                    return RedirectToAction("Productos", "Tienda");
                }
            }
            ModelState.AddModelError("", "Email o password invalidos");
            return View(credenciales);
        }

        [HttpGet]
        public async Task<IActionResult> Registro()
        {
            ViewData["ListaProvincias"] = await this._accesoBD.DevolverProvincias();
            return View(new Cliente());
        }

        [HttpPost]
        public async Task<IActionResult> Registro([FromBody] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                bool clienteRegistradoOk = await this._accesoBD.RegistrarCliente(cliente);
                if (clienteRegistradoOk)
                {
                    return RedirectToAction("Login", "Cliente");
                }
            }
            ModelState.AddModelError("", "Error de procesado de datos en servidor...");
            ViewData["mensajeError"] = "Error de procesado de datos en servidor, intentelo de nuevo mas tarde...";
            ViewData["ListaProvincias"] = await this._accesoBD.DevolverProvincias();
            return View(cliente);
        }

        [HttpGet]
        public async Task<IActionResult> MisDirecciones()
        {
            // TODO
            Dictionary<String, Direccion> direcciones = new Dictionary<string, Direccion>();
            return View(direcciones);
        }
    }
}
