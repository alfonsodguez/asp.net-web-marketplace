using System;

namespace marketplace.Models.Interfaces
{
    public interface IControlSession
    {
        void AddItemSession<T>(String clave, T valor);
        T RecuperaItemSession<T>(String clave);
    }
}
