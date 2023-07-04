using marketplace.Models;
using marketplace.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IDBAccess _accesoBD;
        private readonly IControlSession _servicioSession;

        public PedidoController(IDBAccess servicioBDInyect, IControlSession servicioSessionInyect)
        {
            this._accesoBD = servicioBDInyect;
            this._servicioSession = servicioSessionInyect;
        }

        [HttpPost]
        public async Task<IActionResult> AddCestaProducto([FromBody] ItemPedido item) 
        {
            try
            {
                // recupero session del cliente
                Cliente cliente = this._servicioSession.RecuperaItemSession<Cliente>("datoscliente");
                if (cliente.IdCliente == null)
                {
                    return RedirectToAction("Login", "Cliente");
                }


                int cantidad = item.CantidadPedido;
                string ean = item.ProductoPedido.EAN;
                
                List<ItemPedido> articulos = cliente.PedidoActual.ElementosPedido;
                int index = articulos.FindIndex(item => item.ProductoPedido.EAN == ean);
                bool existeProductoEnPedido = index != -1;

                if (existeProductoEnPedido)
                {
                    articulos[index].CantidadPedido += cantidad;
                }
                else
                {
                    String filtro = "EAN";
                    String valor = ean;
                    Producto productoAAñadir = (await this._accesoBD.DevolverProductos(filtro, valor)).Single<Producto>();

                    articulos.Add(new ItemPedido
                    {
                        CantidadPedido = cantidad,
                        ProductoPedido = productoAAñadir
                    });
                }
                // actualizamos estado session
                this._servicioSession.AddItemSession<Cliente>("datoscliente", cliente);

                return RedirectToAction("Productos", "Tienda");
            }
            catch (Exception ex)
            {
                return Ok(new { mensaje = "Error en método AddCestaProducto" });
            }
        }

        [HttpGet]
        public IActionResult RestarCantidad(String ean)
        {
            Cliente cliente = this._servicioSession.RecuperaItemSession<Cliente>("datoscliente");
            List<ItemPedido> articulos = cliente.PedidoActual.ElementosPedido;

            int pos = articulos.FindIndex(item => item.ProductoPedido.EAN == ean);
            bool existeProductoEnPedido = articulos[pos].CantidadPedido > 1;

            if (existeProductoEnPedido)
            {
                articulos[pos].CantidadPedido -= 1;
                this._servicioSession.AddItemSession<Cliente>("datoscliente", cliente);

                return RedirectToAction("Productos", "Tienda");
            }

            articulos.RemoveAt(pos);
            this._servicioSession.AddItemSession<Cliente>("datoscliente", cliente);

            return RedirectToAction("Productos", "Tienda");
        }

        [HttpGet]
        public IActionResult SumarCantidad(String ean)
        {
            Cliente cliente = this._servicioSession.RecuperaItemSession<Cliente>("datoscliente");

            ItemPedido item = cliente
                .PedidoActual
                .ElementosPedido
                .Where<ItemPedido>(item => item.ProductoPedido.EAN == ean)
                .Single<ItemPedido>();

            item.CantidadPedido += 1;

            this._servicioSession.AddItemSession<Cliente>("datoscliente", cliente);

            return RedirectToAction("Productos", "Tienda");
        }

        [HttpGet]
        public IActionResult EliminarProductoCesta(String ean)
        {
            Cliente cliente = this._servicioSession.RecuperaItemSession<Cliente>("datoscliente");
            List<ItemPedido> articulos = cliente.PedidoActual.ElementosPedido;

            int pos = articulos.FindIndex(item => item.ProductoPedido.EAN == ean);
            articulos.RemoveAt(pos);
            this._servicioSession.AddItemSession<Cliente>("datoscliente", cliente);

            return RedirectToAction("Productos", "Tienda");
        }

        [HttpGet]
        public async Task<IActionResult> FinalizarPedido()
        {
            Cliente cliente = this._servicioSession.RecuperaItemSession<Cliente>("datoscliente");
            if (cliente.IdCliente == null)
            {
                return RedirectToAction("Login", "Cliente");
            }

            if (cliente.PedidoActual.ElementosPedido.Count == 0) {
                return RedirectToAction("Productos", "Tienda");
            }

            cliente.PedidoActual.IdCliente = cliente.IdCliente;
            bool pedidoAlmacenadoOk = await this._accesoBD.GuardarPedido(cliente.PedidoActual);

            if (pedidoAlmacenadoOk)
            {
                List<ItemPedido> articulos = cliente.PedidoActual.ElementosPedido;
                return View(articulos);
            }

            TempData["ErrorServer"] = "Error interno del servidor al procesar pedido";
            return RedirectToAction("Productos", "Tienda");
        }

        [HttpGet]
        public IActionResult MostrarCesta()
        {
            try
            {
                Cliente cliente = this._servicioSession.RecuperaItemSession<Cliente>("datoscliente");
                if (cliente.IdCliente == null)
                {
                    return RedirectToAction("Login", "Cliente");
                }

                TempData["ErrorServer"] = "";
                return View(cliente.PedidoActual);
            }
            catch (Exception)
            {
                return RedirectToAction("Productos", "Tienda");
            }
        }
    }
}
