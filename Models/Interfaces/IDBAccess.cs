using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace marketplace.Models.Interfaces
{
    public interface IDBAccess
    {
        Task<List<Provincia>> DevolverProvincias();
        Task<List<Municipio>> DevolverMunicipios(int codpro);
        Task<List<Categoria>> DevolverCategorias();
        Task<List<Producto>> DevolverProductos(String filtro, String id);
        Task<Cliente> ComprobarCredenciales(String email, String password);
        Task<bool> RegistrarCliente(Cliente cliente);
        Task<bool> GuardarPedido(Pedido pedido);
    }
}
