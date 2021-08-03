using System.Threading.Tasks;

namespace Fance.TurnoFixer.Storage.Interfaces
{
    public interface IObjectStorage
    {
        Task PutObjectAsync(byte[] objectToStore, string fileName);
        Task<byte[]> GetObjectAsync(string fileName);
        Task<string> GetObjectLocationAsync(string fileName);
    }
}