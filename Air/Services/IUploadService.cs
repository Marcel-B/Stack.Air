using com.b_velop.stack.Classes;
using System;
using System.Threading.Tasks;

namespace com.b_velop.stack.Air.Services
{
    public interface IUploadService
    {
        Task<bool> UploadAsync(Airdata data, DateTimeOffset timestamp);
    }
}