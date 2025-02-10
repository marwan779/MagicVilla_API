using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int Id);
        Task<T> CreateAsync<T>(CreateVillaDTO DTO);
        Task<T> UpdateAsync<T>(CreateVillaDTO DTO);

        Task<T> DeleteAsync<T>(int Id);
    }
}
