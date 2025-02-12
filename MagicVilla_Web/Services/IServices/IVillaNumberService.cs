using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllAsync<T>(string Token);
        Task<T> GetAsync<T>(int Id, string Token);
        Task<T> CreateAsync<T>(CreateVillaNumberDTO DTO, string Token);
        Task<T> UpdateAsync<T>(UpdateVillaNumberDTO DTO, string Token);
        Task<T> DeleteAsync<T>(int Id, string Token);
    }
}
