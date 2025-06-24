using SME_API_RISK.Entities;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public interface IApiInformationRepository
    {
        Task<IEnumerable<MApiInformation>> GetAllAsync(MapiInformationModels param);
        Task<MApiInformation> GetByIdAsync(int id);
        Task AddAsync(MApiInformation service);
        Task UpdateAsync(MApiInformation service);
        Task DeleteAsync(int id);
        Task UpdateAllBearerTokensAsync(string newBearerToken);
    }
}
