using System;
using System.Collections.Generic;

namespace marketplace.Models
{
    public class Pedido
    {
        public String IdPedido { get; set; }
        public String IdCliente { get; set; }
        public DateTime Fecha { get; set; }
        public String Estado { get; set; }
        public Decimal SubTotal { get => this.CalculoSubTotalPedido(); }
        public Decimal GastosEnvio { get; set; }
        public Decimal Total { get => this.SubTotal + this.GastosEnvio; }
        public List<ItemPedido> ElementosPedido { get; set; }

        public Pedido()
        {
            this.IdPedido = System.Guid.NewGuid().ToString();
            this.ElementosPedido = new List<ItemPedido>();
        }

        private Decimal CalculoSubTotalPedido()
        {
            Decimal subtotal = 0;
            foreach (ItemPedido item in this.ElementosPedido)
            {
                subtotal += item.CantidadPedido * item.ProductoPedido.Precio;
            }

            return subtotal;
        }
    }
}
