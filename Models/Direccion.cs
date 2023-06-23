using System;

namespace marketplace.Models
{
    public class Direccion
    {
        public String IdCliente { get; set; }
        public String IdDireccion { get; set; }
        public String TipoVia { get; set; }
        public String NombreVia { get; set; }
        public String NumeroVia { get; set; } = "";
        public String Piso { get; set; } = "";
        public String Puerta { get; set; } = "";
        public String Bloque { get; set; } = "";
        public String Escalera { get; set; } = "";
        public String Urbanizacion { get; set; } = "";
        public String Obseraciones { get; set; } = "";
        public int CP { get; set; }
        public int? CodProvincia { get; set; }
        public int? CodMunicipio { get; set; }
        public Provincia Provincia { get; set; }
        public Municipio Municipio { get; set; }
        public bool EsPrincipal { get; set; }
    }
}
