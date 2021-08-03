using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Fance.TurnoFixer.ImageHandler.Interfaces;
using Fance.TurnoFixer.Storage.Interfaces;
using Rectangle = System.Drawing.Rectangle;

namespace Fance.TurnoFixer.ImageHandler
{
    public class ImageHandler : IImageHandler
    {
        private readonly IObjectStorage _objectStorage;
        
        public ImageHandler(IObjectStorage objectStorage)
        {
            _objectStorage = objectStorage;
        }
        
        public async Task<string> ProcessImagesAsync(string originalFile, string overlayFile)
        {
            try
            {
                var originalByteData = await _objectStorage.GetObjectAsync(originalFile);
                await using var msOriginal = new MemoryStream(originalByteData);
                var horarioByteData = await _objectStorage.GetObjectAsync(overlayFile);
                await using var msHorario = new MemoryStream(horarioByteData);
                
                var original = new Bitmap(msOriginal);
                var horario = new Bitmap(msHorario);
                
                var cropRect = new Rectangle(350, 160, 250, 65);
                var cutImg = horario.Clone(cropRect, horario.PixelFormat); 
                
                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(original))
                {
                    g.DrawImage(cutImg, cropRect);
                }
                
                await using MemoryStream ms = new();
                original.Save(ms, ImageFormat.Png);
                var newFileName = $"processed___{originalFile}___{overlayFile}.png";
                await _objectStorage.PutObjectAsync(ms.ToArray(), newFileName);
                return newFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("falló: " + ex.Message);
                throw;
            }
            finally
            {
                // Dispose everything
            }

            return "";
        }
    }
}