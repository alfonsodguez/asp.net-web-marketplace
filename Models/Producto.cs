using System;
using System.Data;
using System.Reflection;

namespace marketplace.Models
{
    public class Producto
    {
        public String EAN { get; set; } // numero identificativo unico de cada producto
        public String IdCategoria { get; set; }
        public String Nombre { get; set; }
        public String MarcaProveedor { get; set; }
        public Decimal Precio { get; set; }
        public Decimal PrecioUnidad { get; set; }
        public String InfoIngredientes { get; set; }
        public String InfoNutricional { get; set; }
        public String Imagen { get; set; }

        public Producto() { }
        // constructor de objetos a partir de filas de tabla Productos
        public Producto(IDataRecord filaProducto)
        {
            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                String nombreprop = prop.Name;
                prop.SetValue(this, filaProducto[nombreprop]);
            }
        }
    }
}

