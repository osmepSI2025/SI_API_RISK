using SME_API_RISK.Models;

namespace SME_API_RISK.Services
{
    public interface ICallAPIService
    {
        Task<string> GetDataApiAsync(MapiInformationModels apiModels, object xdata);

    }
}
