using System.Threading.Tasks;

namespace Fance.TurnoFixer.ImageHandler.Interfaces
{
    public interface IImageHandler
    {
        Task<string> ProcessImagesAsync(string originalFile, string overlayFile);
    }
}