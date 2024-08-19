using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;


        public UsuariosController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }

        [HttpGet]
        [Route("getusUarios")]
        public async Task<ActionResult> Getusuarios()
        {
            try
            {
                List<Usuario> usuarios = new List<Usuario>();
                usuarios = _dbpContext.Usuarios.ToList(); 

                return StatusCode(200, usuarios);
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
        [Route("createUser")]
        public async Task<ActionResult> createuser(Usuario model)
        {
            try
            {
                _dbpContext.Usuarios.Add
                    (
                        new Usuario() 
                        {
                            Nombre = model.Nombre,
                            ApellidoP = model.ApellidoP,
                            ApellidoM = model.ApellidoM,
                            IdRol = model.IdRol,
                            Email = model.Email,
                            Pass = model.Pass,  
                        }
                    );    
                await _dbpContext.SaveChangesAsync();
                return StatusCode(200);
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
        [Route("updateUser")]
        public async Task<ActionResult> updateuser(Usuario model)
        {
            try
            {
                _dbpContext.Usuarios.Update(model);
                await _dbpContext.SaveChangesAsync();
                return StatusCode(200);
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

        [HttpGet]
        [Route("deleteUser/{id}")]
        public async Task<ActionResult> deleteuser(int id)
        {
            try
            {
                var user = _dbpContext.Usuarios.Find(id);
                if (user != null)
                {
                    _dbpContext.Usuarios.Remove(user);
                    await _dbpContext.SaveChangesAsync();
                }
                return StatusCode(200);
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
        [Route("Login")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            try
            {
                var usuario = _dbpContext.Usuarios.Where(x=> x.Email == model.email && x.Pass == model.pass).FirstOrDefault();
                if (usuario != null)
                {
                    var sesion = _dbpContext.Sesiones.Where(x => x.Idu == usuario.Id).FirstOrDefault();
                    if (sesion == null)
                    {
                        _dbpContext.Sesiones.Add(new Sesione() { Idu = usuario.Id, Activo = true });
                        await _dbpContext.SaveChangesAsync();
                        return StatusCode(200, usuario);
                    }
                    else 
                    {
                        if (sesion.Activo == true)
                        {
                            return StatusCode(StatusCodes.Status423Locked);
                        }
                        else 
                        {
                            sesion.Activo = true;
                            _dbpContext.Sesiones.Update(sesion); 
                            await _dbpContext.SaveChangesAsync();
                            return StatusCode(200, usuario);
                        }
                    }
                   
                }
                else { return StatusCode(StatusCodes.Status404NotFound);  }
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


        [HttpGet]
        [Route("logout/{idu}")]
        public async Task<ActionResult> logout(int idu)
        {
            try
            {
                var sesion = _dbpContext.Sesiones.Where(x=> x.Idu == idu).FirstOrDefault();
                if (sesion == null)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                else 
                {
                    sesion.Activo = false;

                    _dbpContext.Sesiones.Update(sesion); 
                    await _dbpContext.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK); 
                }
               
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


        [HttpGet]
        [Route("TestCon")]
        public async Task<ActionResult> testCon()
        {
            var usuario = _dbpContext.Usuarios.FirstOrDefault();
            return StatusCode(200);
        }

    }

    public class LoginModel 
    {
        public string email { get; set; }
        public string pass { get; set; }
    }
}
