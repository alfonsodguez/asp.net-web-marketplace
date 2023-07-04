using marketplace.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.Models
{
    public class ServicioDBAccess : IDBAccess
    {
        private readonly IConfiguration _accessAppSettins;
        private readonly String _cadenaConexionDB;

        public ServicioDBAccess(IConfiguration accesoConfigInyect)
        {
            this._accessAppSettins = accesoConfigInyect;
            this._cadenaConexionDB = this._accessAppSettins.GetConnectionString("SqlServerMercadona");
        }


        public async Task<Cliente> ComprobarCredenciales(string email, string password)
        {
            try
            {
                SqlConnection conexionDB = new SqlConnection(this._cadenaConexionDB);
                await conexionDB.OpenAsync();

                SqlCommand selectCredenciales = new SqlCommand("SELECT * FROM dbo.Credenciales WHERE Email=@email;", conexionDB);
                selectCredenciales.Parameters.AddWithValue("@email", email);
                SqlDataReader cursorCredenciales = await selectCredenciales.ExecuteReaderAsync();

                if (cursorCredenciales.Read())
                {
                    string idCliente = cursorCredenciales["IdCliente"].ToString();
                    string hash = cursorCredenciales["HashPassword"].ToString();

                    bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, hash);
                    if (isValidPassword)
                    {
                        SqlCommand selectCliente = new SqlCommand("SELECT * FROM dbo.Clientes WHERE IdCliente=@idcli;", conexionDB);
                        selectCliente.Parameters.AddWithValue("@idcli", idCliente);
                        SqlDataReader cursorCliente = await selectCliente.ExecuteReaderAsync();

                        if (cursorCliente.Read())
                        {
                            SqlCommand selectDireccion = new SqlCommand("SELECT * FROM dbo.Direcciones WHERE IdCliente=@idcli;", conexionDB);
                            selectDireccion.Parameters.AddWithValue("@idcli", idCliente);
                            SqlDataReader cursorDireccion = await selectDireccion.ExecuteReaderAsync();

                            Dictionary<string, Direccion> direcciones = cursorDireccion
                                .Cast<IDataRecord>()
                                .Select((IDataRecord fila) => new Direccion
                                {
                                    IdDireccion = fila["IdDireccion"].ToString(),
                                    IdCliente = fila["IdCliente"].ToString(),
                                    TipoVia = fila["TipoVia"].ToString(),
                                    NombreVia = fila["NombreVia"].ToString(),
                                    NumeroVia = fila["NumeroVia"].ToString(),
                                    CP = System.Convert.ToInt32(fila["CP"]),
                                    Piso = fila["Piso"].ToString(),
                                    Bloque = fila["Bloque"].ToString(),
                                    Escalera = fila["Escalera"].ToString(),
                                    Urbanizacion = fila["Urbanizacion"].ToString(),
                                    Obseraciones = fila["Observaciones"].ToString(),
                                    Provincia = new Provincia { CodProvincia = System.Convert.ToInt32(fila["CodProvincia"]) },
                                    Municipio = new Municipio { CodProvincia = System.Convert.ToInt32(fila["CodProvincia"]), CodMunicipio = System.Convert.ToInt32(fila["CodMunicipio"]) },
                                    EsPrincipal = System.Convert.ToBoolean(cursorDireccion["EsPrincipal"])
                                })
                                .ToDictionary(key => key.IdDireccion);                            

                            SqlCommand selectTlfno = new SqlCommand("SELECT * FROM dbo.Telefonos WHERE IdCliente=@idcli;", conexionDB);
                            selectTlfno.Parameters.AddWithValue("@idcli", idCliente);
                            SqlDataReader cursorTlfno = await selectTlfno.ExecuteReaderAsync();

                            Dictionary<string, Telefono> telefonos = cursorTlfno
                                .Cast<IDataRecord>()
                                .Select((IDataRecord fila) => new Telefono
                                {
                                    IdCliente = fila["IdCliente"].ToString(),
                                    IdTelefono = fila["IdTelefono"].ToString(),
                                    Numero = fila["Numero"].ToString(),
                                    EsPrincipal = System.Convert.ToBoolean(fila["EsPrincipal"])
                                })
                                .ToDictionary(key => key.IdTelefono);

                            SqlCommand selectHistorico = new SqlCommand("SELECT * FROM dbo.Pedidos WHERE IdCliente=@idcli;", conexionDB);
                            selectHistorico.Parameters.AddWithValue("@idcli", idCliente);
                            SqlDataReader cursorHistorico = await selectHistorico.ExecuteReaderAsync();

                            Dictionary<string, Pedido> historicoPedidos = cursorHistorico
                                .Cast<IDataRecord>()
                                .Select((IDataRecord fila) => new Pedido
                                {
                                    IdCliente = fila["IdCliente"].ToString(),
                                    IdPedido = fila["IdPedido"].ToString(),
                                    Fecha = System.Convert.ToDateTime(fila["Fecha"]),
                                    Estado = fila["Estado"].ToString()
                                })
                                .ToDictionary(key => key.IdPedido);

                            Cliente cliente = new Cliente
                            {
                                IdCliente = idCliente,
                                Nombre = cursorCliente["Nombre"].ToString(),
                                PrimerApellido = cursorCliente["PrimerApellido"].ToString(),
                                SegundoApellido = cursorCliente["SegundoApellildo"].ToString(),
                                FechaNacimiento = System.Convert.ToDateTime(cursorCliente["FechaNacimiento"]),
                                TipoIdentificacionCliente = new Cliente.TipoIdentificacion
                                {
                                    TipoId = cursorCliente["TipoIdentificacion"].ToString(),
                                    NumeroId = cursorCliente["NumeroIdentificacion"].ToString()
                                },
                                Direcciones = direcciones,
                                Telefonos = telefonos,
                                HistoricoPedidos = historicoPedidos,
                                PedidoActual = new Pedido() { IdCliente = idCliente }
                            };

                            return cliente;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> RegistrarCliente(Cliente cliente)
        {
            int salt = 10;
            String password = cliente.CredencialesCliente.Password;
            String hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            int prefix = new Random().Next(0, 1000);
            String separador = ":";
            String DNI = cliente.TipoIdentificacionCliente.NumeroId;
            String idCliente = prefix + separador + DNI;

            try
            {
                SqlConnection conexionDB = new SqlConnection(this._cadenaConexionDB);
                await conexionDB.OpenAsync();

                SqlCommand insertarCliente = new SqlCommand(@"INSERT INTO dbo.Clientes VALUES(@idcli, @nom, @ape1, @ape2, @fnacim, @tipoident, @numident);", conexionDB);
                insertarCliente.Parameters.AddWithValue("@idcli", idCliente);
                insertarCliente.Parameters.AddWithValue("@nom", cliente.Nombre);
                insertarCliente.Parameters.AddWithValue("@ape1", cliente.PrimerApellido);
                insertarCliente.Parameters.AddWithValue("@ape2", cliente.SegundoApellido);
                insertarCliente.Parameters.AddWithValue("@fnacim", cliente.FechaNacimiento);
                insertarCliente.Parameters.AddWithValue("@tipoident", cliente.TipoIdentificacionCliente.TipoId);
                insertarCliente.Parameters.AddWithValue("@numident", cliente.TipoIdentificacionCliente.NumeroId);

                SqlCommand insertarCredenciales = new SqlCommand(@"INSERT INTO dbo.Credenciales VALUES(@idcli, @em, @hp);", conexionDB);
                insertarCredenciales.Parameters.AddWithValue("@idcli", idCliente);
                insertarCredenciales.Parameters.AddWithValue("@em", cliente.CredencialesCliente.Email);
                insertarCredenciales.Parameters.AddWithValue("@hp", hash);

                int clientesInsertados = await insertarCliente.ExecuteNonQueryAsync();
                int credencialesInsertados = await insertarCredenciales.ExecuteNonQueryAsync();

                if (clientesInsertados == 1 && credencialesInsertados == 1)
                {
                    int direccionesInsertadas = 0;
                    int telefonosInsertados = 0;
                    Dictionary<String, Direccion> direcciones = cliente.Direcciones;
                    Dictionary<String, Telefono> telefonos = cliente.Telefonos;

                    foreach (var direccion in direcciones)
                    {
                        DateTime now = DateTime.Now;
                        String idDireccion = now + separador + idCliente;

                        SqlCommand insertarDireccion = new SqlCommand("INSERT INTO dbo.Direcciones VALUES(@idDir, @idCli, @tvia, @nomVia, @numVia, @piso, @bloque, @escalera, @urban, @obs, @cp, @codpro, @codmun, @esppal, @puerta);", conexionDB);
                        insertarDireccion.Parameters.AddWithValue("@idCli", idCliente);
                        insertarDireccion.Parameters.AddWithValue("@idDir", idDireccion);
                        insertarDireccion.Parameters.AddWithValue("@tvia", direccion.Value.TipoVia);
                        insertarDireccion.Parameters.AddWithValue("@nomVia", direccion.Value.NombreVia);
                        insertarDireccion.Parameters.AddWithValue("@numVia", direccion.Value.NumeroVia);
                        insertarDireccion.Parameters.AddWithValue("@piso", direccion.Value.Piso);
                        insertarDireccion.Parameters.AddWithValue("@puerta", direccion.Value.Puerta);
                        insertarDireccion.Parameters.AddWithValue("@bloque", direccion.Value.Bloque);
                        insertarDireccion.Parameters.AddWithValue("@escalera", direccion.Value.Escalera);
                        insertarDireccion.Parameters.AddWithValue("@urban", direccion.Value.Urbanizacion);
                        insertarDireccion.Parameters.AddWithValue("@obs", direccion.Value.Obseraciones);
                        insertarDireccion.Parameters.AddWithValue("@codpro", direccion.Value.CodProvincia);
                        insertarDireccion.Parameters.AddWithValue("@codmun", direccion.Value.CodMunicipio);
                        insertarDireccion.Parameters.AddWithValue("@cp", direccion.Value.CP);
                        insertarDireccion.Parameters.AddWithValue("@esppal", direccion.Value.EsPrincipal);

                        direccionesInsertadas += await insertarDireccion.ExecuteNonQueryAsync();
                        insertarDireccion = null;
                    };

                    foreach (var telefono in telefonos)
                    {
                        DateTime now = DateTime.Now;
                        String idTelefono = now + separador + idCliente;

                        SqlCommand insertarTelefono = new SqlCommand("INSERT INTO dbo.Telefonos VALUES(@idCli, @num, @esppal, @idTlfn);", conexionDB);
                        insertarTelefono.Parameters.AddWithValue("@idCli", idCliente);
                        insertarTelefono.Parameters.AddWithValue("@idTlfn", idTelefono);
                        insertarTelefono.Parameters.AddWithValue("@num", telefono.Value.Numero);
                        insertarTelefono.Parameters.AddWithValue("@esppal", telefono.Value.EsPrincipal);

                        telefonosInsertados += await insertarTelefono.ExecuteNonQueryAsync();
                        insertarTelefono = null;
                    };

                    if (direccionesInsertadas >= 1 && telefonosInsertados >= 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Municipio>> DevolverMunicipios(int codProvincia)
        {
            try
            {
                SqlConnection conexionBD = new SqlConnection(this._cadenaConexionDB);
                await conexionBD.OpenAsync();

                SqlCommand selectMunicipios = new SqlCommand();
                selectMunicipios.Connection = conexionBD;
                selectMunicipios.CommandText = "SELECT * FROM dbo.Municipios WHERE CodProvincia=@codpro ORDER BY Nombre ASC;";
                selectMunicipios.Parameters.AddWithValue("@codpro", codProvincia);

                SqlDataReader cursorMunicipios = await selectMunicipios.ExecuteReaderAsync();

                return cursorMunicipios
                    .Cast<IDataRecord>()
                    .Select((IDataRecord fila) => new Municipio
                    {
                        CodProvincia = codProvincia,
                        CodMunicipio = System.Convert.ToInt16(fila["CodMunicipio"]),
                        Nombre = fila["Nombre"].ToString()
                    })
                    .ToList<Municipio>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Provincia>> DevolverProvincias()
        {
            try
            {
                SqlConnection conexionDb = new SqlConnection(this._cadenaConexionDB);
                await conexionDb.OpenAsync();

                SqlCommand selectProvincias = new SqlCommand();
                selectProvincias.Connection = conexionDb;
                selectProvincias.CommandText = "SELECT * FROM dbo.Provincias ORDER BY Nombre asc";

                SqlDataReader cursorProvincias = await selectProvincias.ExecuteReaderAsync();

                return cursorProvincias
                    .Cast<IDataRecord>()
                    .Select((IDataRecord fila) => new Provincia
                    {
                        CodProvincia = System.Convert.ToInt16(fila["CodProvincia"]),
                        Nombre = fila["Nombre"].ToString()
                    })
                    .ToList<Provincia>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Producto>> DevolverProductos(String filtro, String id)
        {
            try
            {
                SqlConnection conexionDB = new SqlConnection(this._cadenaConexionDB);
                await conexionDB.OpenAsync();

                SqlCommand selectProductos = new SqlCommand();
                selectProductos.Connection = conexionDB;
                selectProductos.CommandText = "SELECT * FROM dbo.Productos";

                switch (filtro)
                {
                    case "categoria":
                        selectProductos.CommandText += " WHERE Pathcategoria=@value;";
                        selectProductos.Parameters.AddWithValue("@value", id);
                        break;
                    case "EAN":
                        selectProductos.CommandText += " WHERE EAN=@value;";
                        selectProductos.Parameters.AddWithValue("@value", id);
                        break;
                    default:
                        selectProductos.CommandText += "";
                        break;
                };

                SqlDataReader cursorProductos = await selectProductos.ExecuteReaderAsync();

                return cursorProductos
                    .Cast<IDataRecord>()
                    .Select((IDataRecord fila) => new Producto
                    {
                        EAN = cursorProductos["EAN"].ToString(),
                        IdCategoria = cursorProductos["Pathcategoria"].ToString(),
                        Nombre = cursorProductos["NombreProducto"].ToString(),
                        Precio = System.Convert.ToDecimal(cursorProductos["PrecioProducto"]),
                        PrecioUnidad = System.Convert.ToDecimal(cursorProductos["PrecioKilo"]),
                        MarcaProveedor = cursorProductos["MarcaProveedor"].ToString(),
                        InfoIngredientes = cursorProductos["Ingredientes"].ToString(),
                        InfoNutricional = cursorProductos["Nutrientes"].ToString()
                    })
                    .ToList<Producto>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Categoria>> DevolverCategorias()
        {
            try
            {
                SqlConnection conexionDb = new SqlConnection(this._cadenaConexionDB);
                await conexionDb.OpenAsync();

                SqlCommand selectCategorias = new SqlCommand("SELECT * FROM dbo.Categorias", conexionDb);
                SqlDataReader cursorCategorias = await selectCategorias.ExecuteReaderAsync();

                return cursorCategorias
                    .Cast<IDataRecord>()
                    .Select((IDataRecord fila) => new Categoria
                    {
                        Nombre = fila["Nombre"].ToString(),
                        Path = fila["Path"].ToString()
                    })
                    .ToList<Categoria>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> GuardarPedido(Pedido pedido)
        {
            String idCliente = pedido.IdCliente;
            String estado = "en curso";
            SqlDateTime now = DateTime.Now;
            String separador = ":";
            String idPedido = now + separador + idCliente;

            try
            {
                SqlConnection conexionBD = new SqlConnection(this._cadenaConexionDB);
                await conexionBD.OpenAsync();

                SqlCommand insertPedido = new SqlCommand(@"INSERT INTO dbo.Pedidos VALUES(@idpedido, @idcli, @fecha, @estado, @subtotal, @gastosdeenvio, @total);", conexionBD);
                insertPedido.Parameters.AddWithValue("@idpedido", idPedido);
                insertPedido.Parameters.AddWithValue("@idcli", pedido.IdCliente);
                insertPedido.Parameters.AddWithValue("@fecha", now);
                insertPedido.Parameters.AddWithValue("@estado", estado);
                insertPedido.Parameters.AddWithValue("@gastosdeenvio", pedido.GastosEnvio);
                insertPedido.Parameters.AddWithValue("@subtotal", pedido.SubTotal);
                insertPedido.Parameters.AddWithValue("@total", pedido.Total);

                int resultInsertPedido = await insertPedido.ExecuteNonQueryAsync();
                if (resultInsertPedido == 1)
                {
                    List<ItemPedido> articulos = pedido.ElementosPedido;

                    articulos.ForEach(async (ItemPedido item) =>
                    {
                        SqlCommand insertItemPedido = new SqlCommand(@"INSERT INTO dbo.ItemsPedido VALUES(@idpedido, @ean, @cantidad);", conexionBD);
                        insertItemPedido.Parameters.AddWithValue("@idpedido", idPedido);
                        insertItemPedido.Parameters.AddWithValue("@ean", item.ProductoPedido.EAN);
                        insertItemPedido.Parameters.AddWithValue("@Cantidad", item.CantidadPedido);

                        int resultInsertPedido = await insertItemPedido.ExecuteNonQueryAsync();
                        if (resultInsertPedido != 1)
                        {
                            throw new Exception();
                        }

                        insertItemPedido = null;
                    });
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
