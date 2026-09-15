using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;

namespace API_PEDIDOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MediaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("image-to-base64")]
        public async Task<IActionResult> ConvertImageUrlToBase64([FromBody] ImageConvertRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.ImageUrl))
            {
                return BadRequest(new { message = "La URL de la imagen es requerida." });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                // 1. Descargar los bytes desde la URL
                byte[] originalBytes = await client.GetByteArrayAsync(request.ImageUrl);

                if (originalBytes == null || originalBytes.Length == 0)
                {
                    return BadRequest(new { message = "La imagen descargada está vacía." });
                }

                // 2. Cargar en MemoryStream y procesar la imagen con la sintaxis de ImageSharp v3.x
                using (var inputStream = new MemoryStream(originalBytes))
                {
                    // Se pasa directamente el stream; la detección del formato es automática
                    using (var image = Image.Load(inputStream))
                    using (var outputStream = new MemoryStream())
                    {
                        // Guardar como JPEG compatible con pdfmake
                        image.Save(outputStream, new JpegEncoder { Quality = 75 });

                        byte[] jpegBytes = outputStream.ToArray();
                        string base64String = Convert.ToBase64String(jpegBytes);

                        return Ok(new ImageConvertResponse
                        {
                            Base64 = $"data:image/jpeg;base64,{base64String}"
                        });
                    }
                }
            }
            catch (UnknownImageFormatException)
            {
                // Si la URL no apunta a una imagen (ej. devolvió un error HTML 403/404 de Firebase)
                return BadRequest(new { message = "El recurso en la URL no es una imagen válida o no tiene un formato soportado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al procesar la imagen", details = ex.Message });
            }
        }
    }

    public class ImageConvertRequest { public string ImageUrl { get; set; } = string.Empty; }
    public class ImageConvertResponse { public string Base64 { get; set; } = string.Empty; }
}
