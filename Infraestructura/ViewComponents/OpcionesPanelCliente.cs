using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace marketplace.Infraestructura.ViewComponents
{
    public class OpcionesPanelCliente : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<String> opciones = new List<String>() {
                "Inicio:PanelInicio",
                "Mi Perfil:MiPerfil",
                "Mis Direcciones:MisDirecciones",
                "Mis Pedidos:MisPedidos",
                "Mi Lista Deseos:MiListaDeseos",
                "Desconectarse:Logout"
            };

            return View(opciones);
        }
    }
}
