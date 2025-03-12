using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int Id);
        Task<T> CreateAsync<T>(CreateVillaNumberDTO DTO);
        Task<T> UpdateAsync<T>(UpdateVillaNumberDTO DTO);
        Task<T> DeleteAsync<T>(int Id);
    }
}
