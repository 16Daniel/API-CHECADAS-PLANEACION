using API_PEDIDOS.ModelsBD2P;
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DescuentosController : ControllerBase
    {
        private readonly DBPContext _context;
        protected BD2Context _contextdb2;

        public DescuentosController(DBPContext context, BD2Context bD2)
        {
            _context = context;
            _contextdb2 = bD2;
        }

        // GET: api/Calendarios/5
        [HttpGet("getProveedoresDescuentos")]
        public async Task<ActionResult> GetProvedorescondescuentos()
        {
            try 
            {
                List<Object> list = new List<Object>(); 
                var data = _context.Descuentos.ToList();

                foreach (var item in data) 
                {
                    var proveedor = _contextdb2.Proveedores.Where(x => x.Codproveedor == item.Codprov).FirstOrDefault();
                    if (proveedor != null) 
                    {
                        list.Add(new { id= item.Id, codprov = item.Codprov, nomprov = proveedor.Nomproveedor });
                    }
                }

                return StatusCode(StatusCodes.Status200OK,list);
            }
            catch(Exception ex)  
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message.ToString() });
            }
        }

        // GET: api/Calendarios/5
        [HttpPost("addDescuento/{codprov}")]
        public async Task<ActionResult> addDescuento(int codprov)
        {
            try
            {
                var descuento = _context.Descuentos.Where(x => x.Codprov == codprov).FirstOrDefault();
                if (descuento == null)
                {
                    _context.Descuentos.Add(new Descuento() { Codprov = codprov }); 
                    await _context.SaveChangesAsync();
                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message.ToString() });
            }
        }


        // DELETE: api/Calendarios/5
        [HttpDelete("deleteDescuento/{id}")]
        public async Task<IActionResult> DeleteDescuento(int id)
        {

            try
            {
                var descuento = await _context.Descuentos.FindAsync(id);
                if (descuento != null)
                {
                    _context.Descuentos.Remove(descuento);
                   await _context.SaveChangesAsync();
                }


                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }

        }


        // GET: api/Calendarios/5
        [HttpPost("UpdateDescuentoPedido")]
        public async Task<ActionResult> updatedescped([FromForm] int idp, [FromForm] double descuento)
        {
            try
            {
                var pedidodb = _context.Pedidos.Find(idp);
                if (pedidodb != null)
                {
                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                    p.cantidaddescuento = descuento;

                    pedidodb.Jdata = JsonConvert.SerializeObject(p);

                    _context.Pedidos.Update(pedidodb);
                    await _context.SaveChangesAsync();

                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message.ToString() });
            }
        }


        [HttpPost("UpdateDescuentoPedidoSuc")]
        public async Task<ActionResult> updatedescpedSuc([FromForm] int idp, [FromForm] double descuento)
        {
            try
            {
                var pedidodb = _context.PedidosSucursales.Find(idp);
                if (pedidodb != null)
                {
                    PedidoSuc p = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);
                    p.cantidaddescuento = descuento;

                    pedidodb.Jdata = JsonConvert.SerializeObject(p);

                    _context.PedidosSucursales.Update(pedidodb);
                    await _context.SaveChangesAsync();

                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message.ToString() });
            }
        }


    }
}
