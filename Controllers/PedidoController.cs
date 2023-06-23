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

        [HttpGet]
        public async Task<IActionResult> AddCestaProducto([FromBody] ItemPedido item) 
        {
            try
            {
                // recupero session del cliente
                Cliente cliente = this._servicioSession.RecuperaItemSession<Cliente>("datoscliente");
                List<ItemPedido> articulos = cliente.PedidoActual.ElementosPedido;

                int cantidad = item.CantidadPedido;
                string ean = item.ProductoPedido.EAN;

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
                return Ok(new { mensaje = "error en metodo AddCestaProducto" });
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

            cliente
                .PedidoActual
                .ElementosPedido
                .Where<ItemPedido>(item => item.ProductoPedido.EAN == ean)
                .Single<ItemPedido>()
                .CantidadPedido += 1;

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
            cliente.PedidoActual.IdCliente = cliente.IdCliente;

            bool pedidoAlmacenadoOk = await this._accesoBD.GuardarPedido(cliente.PedidoActual);
            if (pedidoAlmacenadoOk)
            {
                return View(cliente);
            }

            TempData["ErrorServer"] = "Error interno del server al procesar pedido, intentelo de nuevo mas tarde";
            return RedirectToAction("Productos", "Tienda");
        }

        [HttpGet]
        public IActionResult MostrarCesta()
        {
            try
            {
                Cliente cliente = this._servicioSession.RecuperaItemSession<Cliente>("datoscliente");

                TempData["ErrorServer"] = "";
                return View(cliente.PedidoActual);
            }
            catch (Exception)
            {
                //si no tiene sesion lo redireccionamos a la tienda
                return RedirectToAction("Productos", "Tienda");
            }
        }
    }
}
