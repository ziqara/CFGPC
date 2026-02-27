using System.Collections.Generic;
using DDMLib.Configuration;

namespace DDMLib
{
    public interface IBuildRepository
    {
        List<BuildCard> ReadAllBuildCards(bool onlyPresets);

        bool DeleteBuildIfNoOrders(int configId);

        bool HasOrders(int configId);

        int CreatePreset(BuildDraft draft, string adminEmail);

        bool AreAllComponentsAvailable(int[] componentIds, out int badCount);

        decimal SumComponentsPrice(int[] componentIds);

        void EnsureAdminExists(string adminEmail);

        ConfigurationDto GetConfigurationDto(int configId);
    }
}