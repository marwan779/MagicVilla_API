using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T> GetAllAsync<T>(string Token);
        Task<T> GetAsync<T>(int Id, string Token);
        Task<T> CreateAsync<T>(CreateVillaDTO DTO, string Token);
        Task<T> UpdateAsync<T>(UpdateVillaDTO DTO, string Token);
        Task<T> DeleteAsync<T>(int Id, string Token);
    }
}
