using marketplace.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace marketplace.Models
{
    public class ServicioGestionSession : IControlSession
    {
        private readonly IHttpContextAccessor _httpContextService;

        public ServicioGestionSession(IHttpContextAccessor servicioHttpContextInyect)
        {
            this._httpContextService = servicioHttpContextInyect;
        }


        public void AddItemSession<T>(string clave, T valor)
        {
            this._httpContextService.HttpContext.Session.SetString(clave, JsonSerializer.Serialize<T>(valor));
        }

        public T RecuperaItemSession<T>(string clave)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(this._httpContextService.HttpContext.Session.GetString(clave));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
