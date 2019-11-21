using com.b_velop.stack.Air.Models;
using System;
using System.Threading.Tasks;

namespace com.b_velop.stack.Air.Services
{
    public interface IUploadService
    {
        Task<bool> UploadAsync(AirDto data, DateTimeOffset timestamp);
    }
}