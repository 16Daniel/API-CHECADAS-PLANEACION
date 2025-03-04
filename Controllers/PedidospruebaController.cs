using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace API_PEDIDOS.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PedidospruebaController : ControllerBase
  {
    private readonly ILogger<CatalogosController> _logger;
    protected BD2Context _contextdb2;
    protected DBPContext _dbpContext;

    public PedidospruebaController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc)
    {
      _logger = logger;
      _contextdb2 = db2c;
      _dbpContext = dbpc;
    }

    [HttpGet]
    [Route("getPedidos/{idu}")]
    public async Task<ActionResult> GetPedidosBD(int idu)
    {
      try
      {
        List<Pedidos> pedidos = new List<Pedidos>();
        var pedidosdb = _dbpContext.PedidosPruebas.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && (x.Estatus.Equals("POR ACEPTAR") || x.Estatus.Equals("INCOMPLETO")) && x.Temporal != true).ToList();

        foreach (var item in pedidosdb)
        {
            Pedidos p = JsonConvert.DeserializeObject<Pedidos>(item.Jdata);
            p.id = item.Id;
            pedidos.Add(p);

        }

        return StatusCode(200, pedidos);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);

        return StatusCode(500, new
        {
          Success = false,
          Message = ex.ToString(),
        });
      }
    }


    [HttpPost]
    [Route("recalcularpedidos")]
    public async Task<ActionResult> recalcularpedidos([FromForm] int idu, [FromForm] int filtroproveedor, [FromForm] int filtrosucursal)
    {
      try
      {
        //_dbpContext.ValidacionPedidos.Add(new ValidacionPedido() { Status = true, Idu = idu });
       // await _dbpContext.SaveChangesAsync();
        var asignaciones = _dbpContext.AsignacionProvs.Where(x => x.Idu == idu).ToList();

        var parametros = _dbpContext.Parametros.FirstOrDefault();
        dynamic obj = JsonConvert.DeserializeObject<dynamic>(parametros.Jdata);


        List<PedidosPrueba> rangopedidosdel = new List<PedidosPrueba>();
        var delpedidos = _dbpContext.PedidosPruebas.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && (x.Estatus == "POR ACEPTAR" || x.Estatus == "INCOMPLETO") && x.Temporal != true).ToList();
        foreach (var item in delpedidos)
        {
          if (asignaciones.Any(x => x.Idprov == item.Proveedor && x.Idsuc == int.Parse(item.Sucursal)))
          {
            rangopedidosdel.Add(item);
          }
        }


        if (filtroproveedor > -999)
        {
          rangopedidosdel = rangopedidosdel.Where(x => x.Proveedor == filtroproveedor).ToList();
        }

        if (filtrosucursal > -999)
        {
          rangopedidosdel = rangopedidosdel.Where(x => x.Sucursal == filtrosucursal.ToString()).ToList();
        }

        //foreach (var item in rangopedidosdel)
        //{
        //  var modificaciones = _dbpContext.Modificaciones.Where(x => x.IdPedido == item.Id).ToList();
        //  if (modificaciones.Count > 0) ;
        //  _dbpContext.Modificaciones.RemoveRange(modificaciones);
        //  await _dbpContext.SaveChangesAsync();
        //}

        _dbpContext.RemoveRange(rangopedidosdel);
        await _dbpContext.SaveChangesAsync();

        SqlConnection conn = (SqlConnection)_dbpContext.Database.GetDbConnection();
        conn.Open();
        List<Pedidos> pedidos = new List<Pedidos>();
        List<Calendario> calendarioshoy = new List<Calendario>();
        DateTime fechaHoy = DateTime.Now;
        DateTime fechaentrega = DateTime.Now;
        DayOfWeek diaSemana = fechaHoy.DayOfWeek;
        int numdia = 0;
        switch (diaSemana)
        {
          case DayOfWeek.Monday:
            numdia = 1;
            break;
          case DayOfWeek.Tuesday:
            numdia = 2;
            break;
          case DayOfWeek.Wednesday:
            numdia = 3;
            break;
          case DayOfWeek.Thursday:
            numdia = 4;
            break;
          case DayOfWeek.Friday:
            numdia = 5;
            break;
          case DayOfWeek.Saturday:
            numdia = 6;
            break;
          case DayOfWeek.Sunday:
            numdia = 0;
            break;
          default:
            break;
        }
        var calendarios = _dbpContext.Calendarios.Where(x => x.Temporal != true).ToList();
        foreach (var item in calendarios)
        {
          int[][] array = JsonConvert.DeserializeObject<int[][]>(item.Jdata);
          for (int i = 0; i < array.Length; i++)
          {
            if (array[i][numdia] == 1)
            {
              if (asignaciones.Any(x => x.Idprov == item.Codproveedor && x.Idsuc == item.Codsucursal))
              {
                calendarioshoy.Add(item);
              }
            }
          }
        }

        DateTime[] fechas = new DateTime[7];
        DateTime tempdt = fechaHoy.AddDays(-numdia);
        for (int i = 0; i < 7; i++)
        {
          fechas[i] = tempdt;
          tempdt = tempdt.AddDays(1);
        }

        if (filtroproveedor > -999)
        {
          calendarioshoy = calendarioshoy.Where(x => x.Codproveedor == filtroproveedor).ToList();
        }

        if (filtrosucursal > -999)
        {
          calendarioshoy = calendarioshoy.Where(x => x.Codsucursal == filtrosucursal).ToList();
        }

        foreach (var item in calendarioshoy)
        {
          Boolean articulosdiferentes = false;
          var haypedido = _dbpContext.PedidosPruebas.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString() && x.Temporal != true).ToList();

          Boolean generarpedido = true;

          foreach (var itemp in haypedido)
          {
            if (itemp.Idcal == item.Id)
            {
              generarpedido = false;
            }
          }

          if (haypedido.Count == 0 || generarpedido)
          {
            Boolean requierecartones = false;
            double totalpedido = 0;
            string nombresucursal = "";
            string nombreproveedor = "";
            int status = 1;
            string rfcprov = "";
            var prov = _contextdb2.Proveedores.Where(p => p.Codproveedor == item.Codproveedor).FirstOrDefault();
            rfcprov = prov.Nif20;
            var itemproveedor = _contextdb2.Proveedores.Where(p => p.Codproveedor == item.Codproveedor).FirstOrDefault();
            nombreproveedor = itemproveedor.Nomproveedor;

            var itemsucursal = _contextdb2.RemFronts.Where(s => s.Idfront == item.Codsucursal).FirstOrDefault();
            nombresucursal = itemsucursal.Titulo;

            Boolean inventarioteorico = false;
            var reginvt = _dbpContext.InventarioTeoricos.Where(x => x.Idfront == item.Codsucursal).FirstOrDefault();
            if (reginvt != null)
            {
              var regprovinvt = _dbpContext.InvTeoricoProveedores.Where(x => x.Codprov == item.Codproveedor).FirstOrDefault();
              if (regprovinvt != null)
              {
                inventarioteorico = true;
              }

            }

            int[][] array = JsonConvert.DeserializeObject<int[][]>(item.Jdata);
            double consumopromedio = 0;

            List<articuloModel> articulosl = new List<articuloModel>();
            var articulosdb = _dbpContext.ArticulosProveedors.Where(x => x.Idcalendario == item.Id).ToList();
            foreach (var artdb in articulosdb)
            {
              var tempq = from art in _contextdb2.Articulos1
                          join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                          into gj
                          from subartcl in gj.DefaultIfEmpty()
                          join prec in _contextdb2.Precioscompras on art.Codarticulo equals prec.Codarticulo
                          into gj2
                          from subprec in gj2.DefaultIfEmpty()
                          where subartcl != null && subartcl.Codarticulo == artdb.Codarticulo && subprec.Codproveedor == item.Codproveedor
                          select new
                          {
                            cod = art.Codarticulo,
                            descripcion = art.Descripcion,
                            precio = subprec.Pbruto,
                            referencia = art.Referenciasprovs,
                            tipoimpuesto = art.Impuestocompra
                          };
              var tempart = tempq.FirstOrDefault();
              articulosl.Add(new articuloModel()
              {
                cod = tempart.cod,
                descripcion = tempart.descripcion,
                precio = (double)tempart.precio,
                referencia = tempart.referencia,
                tipoimpuesto = (int)tempart.tipoimpuesto
              });

            }


            var query = from art in _contextdb2.Articulos1
                        join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                        into gj
                        from subartcl in gj.DefaultIfEmpty()
                        join prec in _contextdb2.Precioscompras on art.Codarticulo equals prec.Codarticulo
                        into gj2
                        from subprec in gj2.DefaultIfEmpty()
                        where subartcl != null && subartcl.Planeacion == "T" && subprec.Codproveedor == item.Codproveedor
                        select new articuloModel()
                        {
                          cod = art.Codarticulo,
                          descripcion = art.Descripcion,
                          precio = (double)subprec.Pbruto,
                          referencia = art.Referenciasprovs,
                          tipoimpuesto = (int)art.Impuestocompra
                        };


            int count = articulosl.Count;

            var articulos = articulosl.Count > 0 ? articulosl : query.ToList();

            List<ArticuloPedido> articulospedido = new List<ArticuloPedido>();
            int numlinea = 0;
            foreach (var art in articulos)
            {
              Boolean esretornable = false;
              if (_dbpContext.Retornables.Where(x => x.Codart == art.cod).ToList().Count() > 0)
              {
                esretornable = true;
                requierecartones = true;
              }

              fechaentrega = DateTime.Now;
              numlinea++;
              List<ConsumoModel> consumos = new List<ConsumoModel>();
              consumos.Clear();

              // Crear comando para ejecutar el procedimiento almacenado
              using (SqlCommand command = new SqlCommand("SP_Consumo_Promedio", conn))
              {
                command.CommandType = CommandType.StoredProcedure;
                string codalm = "";
                if (item.Codsucursal < 10)
                {
                  codalm = "0" + item.Codsucursal;
                }
                else { codalm = item.Codsucursal.ToString(); }
                // Agregar parámetros al procedimiento almacenado



                int parametrosemanas = (int)obj.pedido.diasconprom;

                command.Parameters.Add("@sucursal", SqlDbType.NVarChar).Value = codalm;
                command.Parameters.Add("@articulo", SqlDbType.Int).Value = art.cod;
                command.Parameters.Add("@semanas", SqlDbType.Int).Value = parametrosemanas;

                try
                {
                  // Ejecutar el procedimiento almacenado
                  SqlDataReader reader = command.ExecuteReader();

                  while (reader.Read())
                  {
                    consumos.Add(new ConsumoModel
                    {
                      dia = int.Parse(reader["DIA"].ToString()),
                      consumo = double.Parse(reader["CONSUMO"].ToString())
                    });
                  }

                  reader.Close();
                }
                catch (Exception ex)
                {
                  Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                }
              }

              if (consumos.Count < 7)
              {
                int iteraciones = 7 - consumos.Count;
                for (int z = 0; z < iteraciones; z++)
                {
                  consumos.Add(new ConsumoModel()
                  {
                    dia = 0,
                    consumo = 0
                  });
                }
              }

              consumopromedio = consumos[numdia].consumo;

              double mayorconsumo = consumos.OrderByDescending(c => c.consumo).First().consumo;

              double factorstock = (double)obj.pedido.factorstock;

              double stockSeguridad = mayorconsumo * factorstock;

              double consumopedido = 0;

              int[] arraycal = new int[array[0].Length];
              DiasEspecialesSucursal[] diasespeciales = { null, null, null, null, null, null, null };

              double consumoentrega = 0;
              for (int i = 0; i < array.Length; i++)
              {
                if (array[i][numdia] == 1)
                {
                  for (int j = 0; j < array[i].Length; j++)
                  {
                   
                    if (array[i][j] == 1 || array[i][j] == 2 || array[i][j] == 3)
                    {
                      if (array[i][j] == 3)
                      {
                        consumoentrega = consumos[j].consumo;
                      }
                      arraycal = array[i];
                      DiasEspecialesSucursal diaespecialsuc = null;

                      DateTime fechabusqueda = fechas[j];
                      if (j < numdia)
                      {
                        fechabusqueda = fechabusqueda.AddDays(7); 
                      }

                      var diaespecialessuc = _dbpContext.DiasEspecialesSucursals.ToList().Where(d => d.Fecha.Value.ToString("yyyy-MM-dd") == fechabusqueda.ToString("yyyy-MM-dd") && d.Sucursal == item.Codsucursal).ToList();

                      foreach(var ides in diaespecialessuc)
                      {
                        int[] articulosdiesp = JsonConvert.DeserializeObject<int[]>(ides.Articulos);
                        if (articulosdiesp.Contains(art.cod))
                        {
                          diaespecialsuc = ides; 
                        }
                      }

                      var diaespecial = _dbpContext.DiasEspeciales.ToList().Where(d => d.Fecha.ToString("yyyy-MM-dd") == fechabusqueda.ToString("yyyy-MM-dd")).FirstOrDefault();
                      if (diaespecial == null && diaespecialsuc == null)
                      {
                        consumopedido += consumos[j].consumo;
                      }
                      else
                      {
                        if (diaespecialsuc == null)
                        {
                          diasespeciales[j] = new DiasEspecialesSucursal()
                          {
                            Id = diaespecial.Id,
                            Dia = diaespecial.Dia,
                            Semana = diaespecial.Semana,
                            Fecha = diaespecial.Fecha,
                            Descripcion = diaespecial.Descripcion,
                            FactorConsumo = diaespecial.FactorConsumo,
                            Sucursal = 0
                          };
                          consumopedido += (consumos[j].consumo * diaespecial.FactorConsumo);
                        }
                        else
                        {
                          if (diaespecial != null)
                          {
                            if (diaespecial.FactorConsumo > diaespecialsuc.FactorConsumo)
                            {
                              diasespeciales[j] = new DiasEspecialesSucursal()
                              {
                                Id = diaespecial.Id,
                                Dia = diaespecial.Dia,
                                Semana = diaespecial.Semana,
                                Fecha = diaespecial.Fecha,
                                Descripcion = diaespecial.Descripcion,
                                FactorConsumo = diaespecial.FactorConsumo,
                                Sucursal = 0
                              };
                              consumopedido += (consumos[j].consumo * diaespecial.FactorConsumo);
                            }
                            else
                            {
                              diasespeciales[j] = diaespecialsuc;
                              double factor = (diaespecialsuc.FactorConsumo ?? 1.5);
                              consumopedido += (consumos[j].consumo * factor);
                            }
                          }
                          else
                          {
                            diasespeciales[j] = diaespecialsuc;
                            double factor = (diaespecialsuc.FactorConsumo ?? 1.5);
                            consumopedido += (consumos[j].consumo * factor);
                          }

                        }

                      }

                    }
                  }
                }

              }

              int countday = numdia;
              for (int i = 0; i < 7; i++)
              {
                countday++;
                fechaentrega = fechaentrega.AddDays(1);
                if (countday == 7)
                {
                  countday = 0;
                }

                if (arraycal[countday] == 3)
                {
                  break;
                }
              }


              List<PinventarioModel> inventarios = new List<PinventarioModel>();
              inventarios.Clear();

              if (inventarioteorico)
              {
                using (SqlCommand command = new SqlCommand("SPS_GET_DIFERENCIA_LIN", conn))
                {
                  command.CommandType = CommandType.StoredProcedure;
                  string codalm = "";
                  if (item.Codsucursal < 10)
                  {
                    codalm = "0" + item.Codsucursal;
                  }
                  else { codalm = item.Codsucursal.ToString(); }
                  // Añadir parámetros al comando
                  command.Parameters.Add("@FECHA", System.Data.SqlDbType.VarChar, 10).Value = DateTime.Now.ToString("dd/MM/yyyy");
                  command.Parameters.Add("@CODALM", System.Data.SqlDbType.NVarChar, 10).Value = codalm;
                  command.Parameters.Add("@CODART", System.Data.SqlDbType.Int).Value = art.cod;
                  command.CommandTimeout = 120;

                  // Ejecutar el comando y leer los resultados
                  using (SqlDataReader reader = command.ExecuteReader())
                  {
                    while (reader.Read())
                    {
                      DateTime fecha = DateTime.Now;
                      double unidades = (double)reader["INVFORMULA"];
                      inventarios.Add(new PinventarioModel()
                      {
                        fecha = fecha,
                        unidades = unidades,
                      });
                    }
                  }
                }
              }
              else
              {

                using (SqlCommand command = new SqlCommand("SP_GET_INVENTARIO", conn))
                {
                  command.CommandType = CommandType.StoredProcedure;
                  string codalm = "";
                  if (item.Codsucursal < 10)
                  {
                    codalm = "0" + item.Codsucursal;
                  }
                  else { codalm = item.Codsucursal.ToString(); }
                  // Añadir parámetros al comando
                  command.Parameters.Add("@sucursal", SqlDbType.NVarChar, 5).Value = codalm;
                  command.Parameters.Add("@articulo", SqlDbType.Int).Value = art.cod;
                  command.Parameters.Add("@FI", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");
                  command.Parameters.Add("@FF", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");

                  // Ejecutar el comando y leer los resultados
                  using (SqlDataReader reader = command.ExecuteReader())
                  {
                    while (reader.Read())
                    {
                      DateTime fecha = (DateTime)reader["FECHA"];
                      double unidades = reader.GetDouble(1);

                      inventarios.Add(new PinventarioModel()
                      {
                        fecha = fecha,
                        unidades = unidades,
                      });
                    }
                  }
                }

              }

              double inventario = 0;
              Boolean hayinventario = false;
              if (inventarios.Count > 0) { inventario = inventarios[0].unidades; hayinventario = true; } else { status = 2; }
              double proyeccion = 0;
              bool tieneudspendientes = false;
              double unidadespendientes = 0;
              if (item.Especial == true)
              {
                string fechastr = DateTime.Now.ToString("yyyy-MM-dd");
                var pedidosentrega = _dbpContext.PedidosPruebas.Where(x => x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString()
                && x.Jdata.Contains("\"fechaEntrega\":\"" + fechastr) && x.Estatus == "AUTORIZADO").ToList();

                foreach (var pe in pedidosentrega)
                {
                  Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pe.Jdata);
                  var temp = p.articulos.Where(x => x.codArticulo == art.cod).FirstOrDefault();
                  if (temp != null)
                  {
                    tieneudspendientes = true;
                    unidadespendientes = temp.unidadestotales;
                    proyeccion = (consumopedido + stockSeguridad - inventario) - temp.unidadestotales;
                  }
                  else { proyeccion = (consumopedido + stockSeguridad - inventario); }
                }

                if (pedidosentrega.Count == 0)
                {
                  proyeccion = (consumopedido + stockSeguridad - inventario);
                }

              }
              else
              {
                proyeccion = (consumopedido + stockSeguridad - inventario);
              }
              int iva = 0;
              var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == rfcprov && p.Codarticulo == art.cod).FirstOrDefault();
              Boolean tienemultiplo = itprod == null ? false : true;
              if (!tienemultiplo) { status = 2; }
              double unidadescaja = itprod == null ? 1 : (double)itprod.Uds;
              int cajas = 0;
              if (proyeccion % unidadescaja == 0)
              {
                cajas = (int)(proyeccion / unidadescaja);
              }
              else
              {
                double resultado = proyeccion / unidadescaja;
                cajas = (int)Math.Floor(resultado) + 1;
              }
              double unidades_totales = cajas * unidadescaja;
              double total_linea = (double)(unidades_totales * art.precio);
              totalpedido += total_linea;
              var itemimpuesto = _contextdb2.Impuestos.Where(p => p.Tipoiva == art.tipoimpuesto).FirstOrDefault();
              double ivaArt = (double)(itemimpuesto.Iva == null ? 16 : itemimpuesto.Iva);


              var regalmacenaje = _dbpContext.Almacenajes.Where(x => x.Idsucursal == item.Codsucursal && x.Codarticulo == art.cod).FirstOrDefault();
              Boolean tienelimitealmacen = false;
              double unidadesentrega = 0;
              double capacidadalm = 0;
              if (regalmacenaje != null)
              {
                capacidadalm = regalmacenaje.Capacidad;
                string fechastr = DateTime.Now.ToString("yyyy-MM-dd");
                var pedidosentrega = _dbpContext.PedidosPruebas.Where(x => x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString()
                && x.Jdata.Contains("\"fechaEntrega\":\"" + fechastr) && x.Estatus == "AUTORIZADO").ToList();

                foreach (var pe in pedidosentrega)
                {
                  Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pe.Jdata);
                  var temp = p.articulos.Where(x => x.codArticulo == art.cod).FirstOrDefault();
                  if (temp != null)
                  {
                    unidadesentrega = unidadesentrega + temp.unidadestotales;
                  }
                }

                tienelimitealmacen = true;

              }

              articulospedido.Add(new ArticuloPedido()
              {
                codArticulo = art.cod,
                nombre = art.descripcion,
                inventariohoy = inventario,
                precio = art.precio,
                numlinea = numlinea,
                cajas = cajas,
                unidadescaja = unidadescaja,
                unidadestotales = unidades_totales,
                tipoImpuesto = (int)art.tipoimpuesto,
                iva = ivaArt,
                total_linea = total_linea,
                codigoAlmacen = item.Codsucursal.ToString(),
                tienemultiplo = tienemultiplo,
                hayinventario = hayinventario,
                consumospromedios = consumos,
                consumomayor = mayorconsumo,
                factorseguridad = 1.5,
                arraycalendario = arraycal,
                diasespeciales = diasespeciales,
                calendarioespecial = tieneudspendientes,
                unidadesextra = unidadespendientes,
                esretornable = esretornable,
                tienelimitealmacen = tienelimitealmacen,
                capacidadalmfinal = (capacidadalm - unidadesentrega + (consumoentrega / 2)),
                invformulado = inventarioteorico
              });
            }

            // validar si requiere cartones 
            double cartones = 0;
            Boolean cartonescapturados = false;
            if (requierecartones)
            {
              List<PinventarioModel> invcartones = new List<PinventarioModel>();

              if (inventarioteorico)
              {
                using (SqlCommand command = new SqlCommand("SPS_GET_DIFERENCIA_LIN", conn))
                {
                  command.CommandType = CommandType.StoredProcedure;
                  string codalm = "";
                  if (item.Codsucursal < 10)
                  {
                    codalm = "0" + item.Codsucursal;
                  }
                  else { codalm = item.Codsucursal.ToString(); }
                  // Añadir parámetros al comando
                  command.Parameters.Add("@FECHA", System.Data.SqlDbType.VarChar, 10).Value = DateTime.Now.ToString("dd/MM/yyyy");
                  command.Parameters.Add("@CODALM", System.Data.SqlDbType.NVarChar, 10).Value = codalm;
                  command.Parameters.Add("@CODART", System.Data.SqlDbType.Int).Value = 10277;
                  command.CommandTimeout = 120;

                  // Ejecutar el comando y leer los resultados
                  using (SqlDataReader reader = command.ExecuteReader())
                  {
                    while (reader.Read())
                    {
                      DateTime fecha = DateTime.Now;
                      double unidades = (double)reader["INVFORMULA"];
                      invcartones.Add(new PinventarioModel()
                      {
                        fecha = fecha,
                        unidades = unidades,
                      });
                    }
                  }
                }
              }
              else
              {

                using (SqlCommand command = new SqlCommand("SP_GET_INVENTARIO", conn))
                {
                  command.CommandType = CommandType.StoredProcedure;
                  string codalm = "";
                  if (item.Codsucursal < 10)
                  {
                    codalm = "0" + item.Codsucursal;
                  }
                  else { codalm = item.Codsucursal.ToString(); }
                  // Añadir parámetros al comando
                  command.Parameters.Add("@sucursal", SqlDbType.NVarChar, 5).Value = codalm;
                  command.Parameters.Add("@articulo", SqlDbType.Int).Value = 10277;
                  command.Parameters.Add("@FI", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");
                  command.Parameters.Add("@FF", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");

                  // Ejecutar el comando y leer los resultados
                  using (SqlDataReader reader = command.ExecuteReader())
                  {
                    while (reader.Read())
                    {
                      DateTime fecha = (DateTime)reader["FECHA"];
                      double unidades = reader.GetDouble(1);

                      invcartones.Add(new PinventarioModel()
                      {
                        fecha = fecha,
                        unidades = unidades,
                      });
                    }
                  }
                }

              }

              if (invcartones.Count > 0)
              {
                cartonescapturados = true;
                cartones = invcartones[0].unidades;
              }
            }

            double totalimpuestos = 0;
            foreach (var itemart in articulospedido)
            {
              totalimpuestos += (itemart.total_linea * itemart.iva) / 100;
            }

            pedidos.Add(new Pedidos()
            {
              idSucursal = item.Codsucursal.ToString()
               ,
              codProveedor = item.Codproveedor,
              total = totalpedido,
              fechaEntrega = fechaentrega,
              articulos = articulospedido,
              nombreproveedor = nombreproveedor,
              nombresucursal = nombresucursal,
              status = status,
              rfc = rfcprov,
              cartones = cartones,
              tieneretornables = requierecartones,
              capturacartones = cartonescapturados,
              cantidaddescuento = 0,
              totiva = totalimpuestos
            });

            string tempjdata = JsonConvert.SerializeObject(pedidos.Last());
            var temppedido = _dbpContext.PedidosPruebas.Where(p => p.Sucursal == item.Codsucursal.ToString() && p.Proveedor == item.Codproveedor && p.Jdata == tempjdata && p.Fecha.Value.Date == DateTime.Now.Date).FirstOrDefault();
            if (temppedido == null)
            {
              await _dbpContext.PedidosPruebas.AddAsync(new PedidosPrueba()
              {
                Sucursal = item.Codsucursal.ToString(),
                Proveedor = item.Codproveedor,
                Jdata = tempjdata,
                Estatus = status == 1 ? "POR ACEPTAR" : "INCOMPLETO",
                Fecha = DateTime.Now,
                Numpedido = "",
                Idcal = item.Id,
                Temporal = false
              });
              await _dbpContext.SaveChangesAsync();
            }
            else
            {
              pedidos.RemoveAt(pedidos.Count - 1);
            }

          }

        }

        var estatuspedido = _dbpContext.ValidacionPedidos.Where(x => x.Idu == idu).FirstOrDefault();
        if (estatuspedido != null)
        {
          _dbpContext.ValidacionPedidos.Remove(estatuspedido);
          await _dbpContext.SaveChangesAsync();
        }

        return StatusCode(200, pedidos);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);

        return StatusCode(500, new
        {
          Success = false,
          Message = ex.ToString(),
        });
      }
    }
  }
}
