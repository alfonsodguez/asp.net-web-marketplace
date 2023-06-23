using System;
using System.Collections.Generic;

namespace marketplace.Models
{
    public class Cliente
    {
        //[Required(ErrorMessage = "* Nombre obligatorio")]
        //[MaxLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public String Nombre { get; set; }

        //[Required(ErrorMessage = "* Primer Apellido obligatorio")]
        //[MaxLength(100, ErrorMessage = "El primer apellido no puede exceder los 100 caracteres")]
        public String PrimerApellido { get; set; }

        //[Required(ErrorMessage = "* Segundo Apellido obligatorio")]
        //[MaxLength(100, ErrorMessage = "El segundo apellido no puede exceder los 100 caracteres")]
        public String SegundoApellido { get; set; }

        //[Required(ErrorMessage ="* Fecha de nacimiento requerida")]
        public DateTime FechaNacimiento { get; set; }

        public String IdCliente { get; set; }
        public TipoIdentificacion TipoIdentificacionCliente { get; set; }
        public Dictionary<String, Telefono> Telefonos { get; set; }     // TODO: validacion tlfnos
        public Dictionary<String, Direccion> Direcciones { get; set; }  // TODO: validacion direcciones
        public Credenciales CredencialesCliente { get; set; }
        public Pedido PedidoActual { get; set; }
        public Dictionary<String, Pedido> HistoricoPedidos { get; set; }


        public Cliente()
        {
            this.CredencialesCliente = new Credenciales();
            this.TipoIdentificacionCliente = new TipoIdentificacion();
            this.Direcciones = new Dictionary<String, Direccion>();
            this.Telefonos = new Dictionary<String, Telefono>();
            this.PedidoActual = new Pedido
            {
                Fecha = DateTime.Now,
                Estado = "en curso"
            };
            this.HistoricoPedidos = new Dictionary<String, Pedido>();
        }

        public class Credenciales
        {
            //[Required(ErrorMessage = "* Email obligatorio")]
            //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage ="Formato de Email invalido")]
            public String Email { get; set; }

            //[Required(ErrorMessage = "* Password obligatoria")]
            //[MinLength(4, ErrorMessage = "Se requieren al menos 4 caracteres MIN")]
            //[MaxLength(20, ErrorMessage = "la Password no debe tener mas de 20 caracteres")]
            //[RegularExpression("^(?=.*[A-Z].*[A-Z])(?=.*[!@#$&*])(?=.*[0-9].*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{4,}$", ErrorMessage = "la password debe tener al menos una letra min, letra MAYS, numero y simbolo")]
            public String Password { get; set; }
#nullable enable
            public String? HashPasword { get; set; }
#nullable disable
        }

        public class TipoIdentificacion
        {
            //[Required(ErrorMessage ="* Tipo Identificacion requerida")]
            public String TipoId { get; set; } = "NIF";

            // TODO: validacion del numero de identif en funcion del tipo
            //[Required(ErrorMessage ="*Numero de Identificacion requerido")]
            public String NumeroId { get; set; }
        }
    }

}

