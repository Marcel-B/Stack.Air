using System;
using System.Threading.Tasks;
using com.b_velop.stack.Classes.Dtos;

namespace com.b_velop.stack.Air.Services
{
    public interface IUploadService
    {
        Task<bool> UploadAsync(AirdataDto data, DateTimeOffset timestamp);
    }
}