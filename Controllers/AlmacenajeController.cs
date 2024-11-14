using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenajeController : ControllerBase
    {
        private readonly DBPContext _context;
        protected BD2Context _contextdb2;

        public AlmacenajeController(DBPContext context, BD2Context bD2)
        {
            _context = context;
            _contextdb2 = bD2;
        }

        // GET: api/Calendarios
        [HttpGet("getdata")]
        public async Task<ActionResult<IEnumerable<Almacenaje>>> Getdatat()
        {
            try
            {
                var datadb = _context.Almacenajes.ToList();

                List<Object> data = new List<Object>(); 

                foreach (var item in datadb) 
                {
                    var sucursal = _contextdb2.RemFronts.Where(x => x.Idfront == item.Idsucursal).FirstOrDefault(); 
                    var articulo = _contextdb2.Articulos1.Where(x=>x.Codarticulo == item.Codarticulo).FirstOrDefault();

                    data.Add(new 
                    {
                        id = item.Id,
                        codsucursal=item.Idsucursal,
                        codart=item.Codarticulo,
                        capacidad=item.Capacidad,
                        nomsucursal = sucursal.Titulo,
                        nomart = articulo.Descripcion
                    });
                }

                return StatusCode(StatusCodes.Status200OK, data);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }


        // GET: api/Calendarios
        [HttpPost("Savedata")]
        public async Task<ActionResult> Savesata(Almacenaje model)
        {
            try
            {
               
                    var regdb = _context.Almacenajes.Where(x => x.Idsucursal == model.Idsucursal && x.Codarticulo == model.Codarticulo).FirstOrDefault();
                if (regdb == null)
                {
                    _context.Almacenajes.Add(new Almacenaje
                    {
                        Idsucursal = model.Idsucursal,
                        Codarticulo = model.Codarticulo,
                        Capacidad = model.Capacidad,
                    });

                   await _context.SaveChangesAsync();
                }
                else 
                {
                    regdb.Capacidad = model.Capacidad;
                    _context.Almacenajes.Update(regdb); 
                    await _context.SaveChangesAsync();  
                }

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }

        [HttpDelete]
        [Route("deleteData/{jdata}")]
        public async Task<ActionResult> deleteAsignacion(string jdata)
        {
            try
            {
                int[] almacenajes = JsonConvert.DeserializeObject<int[]>(jdata);

                foreach (int id in almacenajes) 
                {
                    var reg = _context.Almacenajes.Where(x => x.Id == id).FirstOrDefault();

                    if (reg != null)
                    {
                        _context.Almacenajes.Remove(reg);
                        await _context.SaveChangesAsync();
                    }
                }
               

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.ToString(),
                });
            }
        }

    }
}
