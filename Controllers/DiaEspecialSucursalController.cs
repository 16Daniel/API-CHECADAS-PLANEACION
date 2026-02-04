using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Security.Cryptography;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaEspecialSucursalController : ControllerBase
    {
        private readonly DBPContext _context;

        public DiaEspecialSucursalController(DBPContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("CreateDiaEspecialSucursal")]
        public async Task<ActionResult> CreateDiasEspeciales([FromBody] DiaEspecialSucursalModel model)
        {
            try
            {
                int[] sucursales = JsonConvert.DeserializeObject<int[]>(model.sucursales);

                foreach (var ids in sucursales) 
                {
                    //var reg = _context.DiasEspecialesSucursals.Where(x => x.Fecha.Value.Date == model.Fecha.Date && x.Descripcion == model.Descripcion && x.Sucursal == ids).FirstOrDefault();

                    //if (reg == null)
                    //{
                        _context.DiasEspecialesSucursals.Add(new DiasEspecialesSucursal()
                        {
                            Dia = model.Dia,
                            Semana = model.Semana,
                            Fecha = model.Fecha,
                            Descripcion = model.Descripcion,
                            FactorConsumo = model.FactorConsumo,
                            Sucursal = ids,
                            Articulos = model.articulos
                        });
                    //}
                    //else 
                    //{
                    //    reg.Dia = model.Dia;
                    //    reg.Semana = model.Semana;
                    //    reg.Fecha = model.Fecha;
                    //    reg.Descripcion = model.Descripcion;
                    //    reg.FactorConsumo = model.FactorConsumo;
                    //    reg.Articulos = model.articulos;

                    //    _context.DiasEspecialesSucursals.Update(reg);
                    //}
                    
                    
                }

                ;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }

        [HttpPost]
        [Route("CreateDiasEspecialesSucursal")]
        public async Task<ActionResult> CreateDiasEspecialesM([FromBody] List<DiaEspecialSucursalModel> lista)
        {
            try
            {
                foreach (DiaEspecialSucursalModel model in lista) 
                {
                    int[] sucursales = JsonConvert.DeserializeObject<int[]>(model.sucursales);

                    foreach (var ids in sucursales)
                    {
                        //var reg = _context.DiasEspecialesSucursals.Where(x => x.Fecha.Value.Date == model.Fecha.Date && x.Descripcion == model.Descripcion && x.Sucursal == ids).FirstOrDefault();

                        //if (true)
                        //{
                            _context.DiasEspecialesSucursals.Add(new DiasEspecialesSucursal()
                            {
                                Dia = model.Dia,
                                Semana = model.Semana,
                                Fecha = model.Fecha,
                                Descripcion = model.Descripcion,
                                FactorConsumo = model.FactorConsumo,
                                Sucursal = ids,
                                Articulos = model.articulos
                            });
                        //}
                        //else
                        //{
                        //    reg.Dia = model.Dia;
                        //    reg.Semana = model.Semana;
                        //    reg.Fecha = model.Fecha;
                        //    reg.Descripcion = model.Descripcion;
                        //    reg.FactorConsumo = model.FactorConsumo;
                        //    reg.Articulos = model.articulos;

                        //    _context.DiasEspecialesSucursals.Update(reg);
                        //}


                    };
                }
 
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }



        [HttpPost]
        [Route("UpdateDiaEspecialSucursal")]
        public async Task<ActionResult> UpdateDiasEspeciales([FromBody] UpdateDiaEspecialSucursalModel model)
        {
            try
            {
  
                int[] dias = JsonConvert.DeserializeObject<int[]>(model.ids);


                foreach (var id in dias)
                {
                    var reg = _context.DiasEspecialesSucursals.Where(x => x.Id == id).FirstOrDefault();

                    if (reg != null)
                    {
                        reg.Dia = model.Dia;
                        reg.Semana = model.Semana;
                        reg.Fecha = model.Fecha;    
                        reg.Descripcion = model.Descripcion;
                        reg.FactorConsumo = model.FactorConsumo;
                        reg.Articulos = model.articulos;

                        _context.DiasEspecialesSucursals.Update(reg);   
                        await _context.SaveChangesAsync();
                    }
                }

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }


        [HttpGet("getDiasEspecialesSucursales")]
        public async Task<ActionResult<IEnumerable<DiasEspecialesSucursal>>> GetDiasEspeciales()
        {
            try
            {
                var diasespeciales = await _context.DiasEspecialesSucursals.Where(x=> x.Fecha.Value.Date >= DateTime.Now.Date.AddDays(-30).Date).OrderBy(x => x.Fecha).ToListAsync();
                return StatusCode(StatusCodes.Status200OK, diasespeciales);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }


        // DELETE: api/Calendarios/5
        [HttpPost("deleteC")]
        public async Task<IActionResult> DeleteCalendarioSucursal([FromForm] string sucursales)
        {

            try
            {
                int[] arr_sucursales = JsonConvert.DeserializeObject<int[]>(sucursales);

                foreach (var id in arr_sucursales)
                {
                    var diaesp = await _context.DiasEspecialesSucursals.FindAsync(id);

                    if (diaesp != null) 
                    {
                        
                        _context.DiasEspecialesSucursals.Remove(diaesp);
                        await _context.SaveChangesAsync();
                    }
                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }

        }


    }

   

    public class DiaEspecialSucursalModel 
    {
        public int Id { get; set; }
        public int Dia { get; set; }
        public int Semana { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = null!;
        public double FactorConsumo { get; set; }
        public string sucursales { get; set; }
        public string articulos { get; set; }
    }

    public class UpdateDiaEspecialSucursalModel
    {
        public int Dia { get; set; }
        public int Semana { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = null!;
        public double FactorConsumo { get; set; }
        public string articulos { get; set; }
        public string ids { get; set; }
    }
}
