using System;
using System.Threading.Tasks;
using com.b_velop.Home.Classes;

namespace com.b_velop.stack.Air.Services
{
    public interface IUploadService
    {
        Task<bool> UploadAsync(Airdata data, DateTimeOffset timestamp);
    }
}