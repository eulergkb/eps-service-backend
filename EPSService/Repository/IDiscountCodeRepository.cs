using ESPService.Data.Models;

namespace ESPService.Repository
{
    public interface IDiscountCodeRepository
    {
        Task<DiscountCode> Insert(string code, bool checkIfExists);

        Task<string[]> GetAll();

        Task<bool> UseCode(string code);
    }
}
