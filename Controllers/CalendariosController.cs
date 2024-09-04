using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_PEDIDOS.ModelsDBP;
using Newtonsoft.Json;
using API_PEDIDOS.ModelsDB2;
using Microsoft.CodeAnalysis.Operations;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendariosController : ControllerBase
    {
        private readonly DBPContext _context;

        public CalendariosController(DBPContext context)
        {
            _context = context;
        }

        // GET: api/Calendarios
        [HttpGet("getCalendarios/{idprov}")]
        public async Task<ActionResult<IEnumerable<Calendario>>> GetCalendarios(int idprov)
        {
            try
            {
                var dbcalendarios = _context.Calendarios.Where(c => c.Codproveedor == idprov).ToList();

                return StatusCode(StatusCodes.Status200OK, dbcalendarios);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }

        // GET: api/Calendarios/5
        [HttpGet("getCalendario/{idprov}/{idsucursal}")]
        public async Task<ActionResult<Calendario>> GetCalendario(int idprov, int idsucursal)
        {
            if (_context.Calendarios == null)
            {
                return NotFound();
            }
            var calendario = await _context.Calendarios.Where(c=>c.Codproveedor==idprov && c.Codsucursal==idsucursal).FirstOrDefaultAsync();

            if (calendario == null)
            {
                return NotFound();
            }

            return calendario;
        }

        // PUT: api/Calendarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("updateCalendar")]
        public async Task<IActionResult> PutCalendario([FromBody] calendarioModel calendario)
        {
            var dbcalendario = _context.Calendarios.Where(c => c.Id == calendario.Id).FirstOrDefault();

            if (dbcalendario == null)
            {
                return StatusCode(200); 
            }

            List<ItemModel> itemList = JsonConvert.DeserializeObject<List<ItemModel>>(calendario.articulos);
            var articulos = _context.ArticulosProveedors.Where(x => x.Idcalendario == calendario.Id).ToList();

            foreach (var articulo in articulos)
            {
                _context.ArticulosProveedors.Remove(articulo);
                await _context.SaveChangesAsync();
            }

            dbcalendario.Jdata = calendario.Jdata;
            dbcalendario.Especial = calendario.especial; 
            _context.Calendarios.Update(dbcalendario); 

            try
            {
                await _context.SaveChangesAsync();

                foreach (var item in itemList)
                {
                    _context.ArticulosProveedors.Add(new ArticulosProveedor()
                    {
                        Codarticulo = item.cod,
                        Codprov = calendario.Codproveedor,
                        Codsucursal = calendario.Codsucursal,
                        Idcalendario = dbcalendario.Id
                    });
                    await _context.SaveChangesAsync();
                }

                return StatusCode(200); 
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500); 
            }
        }

        // POST: api/Calendarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("saveCalendario")]
        public async Task<ActionResult<Calendario>> saveCalendario([FromBody] calendarioModel calendario)
        {
            try
            {
                List<ItemModel> itemList = JsonConvert.DeserializeObject<List<ItemModel>>(calendario.articulos);

                    var dbcalendario = new Calendario()
                    {
                        Codproveedor = calendario.Codproveedor,
                        Codsucursal = calendario.Codsucursal,
                        Jdata = calendario.Jdata,
                        Especial = calendario.especial
                    }; 
                    _context.Calendarios.Add(dbcalendario);
                    await _context.SaveChangesAsync();
               

                foreach (var item in itemList) 
                {
                    _context.ArticulosProveedors.Add(new ArticulosProveedor() 
                    {
                        Codarticulo = item.cod,
                        Codprov = calendario.Codproveedor,
                        Codsucursal = calendario.Codsucursal,
                        Idcalendario = dbcalendario.Id
                    });
                    await _context.SaveChangesAsync();
                }

                return StatusCode(StatusCodes.Status200OK);

            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

        // DELETE: api/Calendarios/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCalendario(int id)
        {

            try
            {
                if (_context.Calendarios == null)
                {
                    return NotFound();
                }
                var calendario = await _context.Calendarios.FindAsync(id);
                if (calendario == null)
                {
                    return NotFound();
                }

                _context.Calendarios.Remove(calendario);
                await _context.SaveChangesAsync();

                var articulos = _context.ArticulosProveedors.Where(x => x.Idcalendario == calendario.Id).ToList();

                foreach (var articulo in articulos)
                {
                    _context.ArticulosProveedors.Remove(articulo);
                    await _context.SaveChangesAsync();
                }


                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }

        }

        private bool CalendarioExists(int id)
        {
            return (_context.Calendarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        [HttpGet("getDiasEspeciales")]
        public async Task<ActionResult<IEnumerable<DiasEspeciale>>> GetDiasEspeciales()
        {
            try
            {
                var diasespeciales = await _context.DiasEspeciales.OrderBy(x => x.Fecha).ToListAsync();
                return StatusCode(StatusCodes.Status200OK, diasespeciales);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }

        [HttpPost]
        [Route("CreateDiaEspecial")]
        public async Task<ActionResult<IEnumerable<DiasEspeciale>>> CreateDiasEspeciales(DiasEspeciale dia)
        {
            try
            {
                 _context.DiasEspeciales.Add(dia);
                 await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }

        [HttpPut]
        [Route("UpdateDiaEspecial")]
        public async Task<ActionResult<IEnumerable<DiasEspeciale>>> UpdateDiaEspecial(DiasEspeciale dia)
        {
            try
            {
                _context.Entry(dia).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }


        [HttpDelete("deleteDiaEspecial/{id}")]
        public async Task<IActionResult> DeleteDiasEspecial(int id)
        {

            try
            {
                if (_context.DiasEspeciales == null)
                {
                    return NotFound();
                }
                var diaespecial = await _context.DiasEspeciales.FindAsync(id);
                if (diaespecial == null)
                {
                    return NotFound();
                }
                else { _context.DiasEspeciales.Remove(diaespecial); }
               
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }

        }



        [HttpPost]
        [Route("esDiaEspecial")]
        public async Task<ActionResult<Object>> esDiaEspecial([FromBody] EsdiaEspecialM model)
        {
            try
            {
                var diaespecialsuc = await _context.DiasEspecialesSucursals.Where(d => d.Fecha == model.fecha && d.Sucursal == model.idsucursal).FirstOrDefaultAsync(); 
                var diasespeciales = await _context.DiasEspeciales.Where(d => d.Fecha == model.fecha).FirstOrDefaultAsync();
                if (diasespeciales == null && diaespecialsuc == null)
                {
                    return StatusCode(StatusCodes.Status200OK, new { consumo = model.consumo, factor = 0, prioridadsuc = 0});
                }
                else 
                {
                    if (diaespecialsuc == null) 
                    {
                        return StatusCode(StatusCodes.Status200OK, new { consumo = model.consumo * diasespeciales.FactorConsumo, factor = diasespeciales.FactorConsumo, descripcion = diasespeciales.Descripcion, prioridadsuc = 0});
                    }
                    else 
                    {
                        int[] articulosdiesp = JsonConvert.DeserializeObject<int[]>(diaespecialsuc.Articulos);
                        if (articulosdiesp.Contains(model.idart))
                        {
                            return StatusCode(StatusCodes.Status200OK, new { consumo = model.consumo * diaespecialsuc.FactorConsumo, factor = diaespecialsuc.FactorConsumo, descripcion = diaespecialsuc.Descripcion, prioridadsuc = 1 });
                        }
                        else 
                        {
                            if (diasespeciales == null)
                            {
                                return StatusCode(StatusCodes.Status200OK, new { consumo = model.consumo, factor = 0, prioridadsuc = 0 });
                            }
                            else 
                            {
                                return StatusCode(StatusCodes.Status200OK, new { consumo = model.consumo * diasespeciales.FactorConsumo, factor = diasespeciales.FactorConsumo, descripcion = diasespeciales.Descripcion, prioridadsuc = 0 });
                            }
                        }
                        
                    }
                    
                }
               

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }



        [HttpGet("validararticulos")]
        public async Task<ActionResult> validararticulos()
        {
            try
            {
                var calendarios = _context.Calendarios.ToList(); 

                foreach (var calendar in calendarios) 
                {
                    var articulos = _context.ArticulosProveedors.Where(x => x.Codsucursal == calendar.Codsucursal && x.Codprov == calendar.Codproveedor).ToList();

                    foreach (var articulo in articulos) 
                    {
                        articulo.Idcalendario = calendar.Id;
                        _context.ArticulosProveedors.Update(articulo); 
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


    public class EsdiaEspecialM 
    {
        public DateTime fecha { get; set; }
        public double consumo { get; set;}
        public int idsucursal { get; set;}
        public int idart {  get; set;}
    }

    public class calendarioModel 
    {
        public int Id { get; set; }
        public int Codsucursal { get; set; }
        public int Codproveedor { get; set; }
        public string Jdata { get; set; } = null!;
        public string articulos { get; set; }   
        public Boolean especial { get; set; }
    }

    public class ItemModel 
    {
        public int cod { get; set; }
        public string descripcion { get; set; } 
        public int marca { get; set; }  
    }
}
