using System;
using System.Collections.Generic;

namespace DDMLib
{
    public class BuildService
    {
        private readonly IBuildRepository repo_;
        private readonly BuildValidator validator_;

        public const string AdminEmail = "admin";

        public BuildService(IBuildRepository repo, BuildValidator validator = null)
        {
            repo_ = repo ?? throw new ArgumentNullException(nameof(repo));
            validator_ = validator ?? new BuildValidator();
        }

        public List<BuildCard> GetBuildCards(bool onlyPresets)
        {
            return repo_.ReadAllBuildCards(onlyPresets);
        }

        public string DeleteBuild(int configId)
        {
            try
            {
                bool ok = repo_.DeleteBuildIfNoOrders(configId);
                if (ok) return string.Empty;

                if (repo_.HasOrders(configId))
                    return "Нельзя удалить: сборка уже заказана";

                return "Сборка не найдена или не удалена";
            }
            catch (Exception ex)
            {
                return "Вероятно, проблемы в соединении с БД: " + ex.Message;
            }
        }

        public string CreatePreset(BuildDraft draft, out int createdId)
        {
            createdId = 0;

            var errors = validator_.Validate(draft);
            if (errors.Count > 0)
                return string.Join("\n", errors);

            int[] ids = new[]
            {
                draft.MotherboardId, draft.CpuId, draft.RamId, draft.GpuId,
                draft.StorageId, draft.PsuId, draft.CaseId, draft.CoolingId
            };

            int badCount;
            bool allOk = repo_.AreAllComponentsAvailable(ids, out badCount);
            if (!allOk)
                return $"Нельзя сохранить: есть недоступные/отсутствующие компоненты: {badCount}";

            try
            {
                createdId = repo_.CreatePreset(draft, AdminEmail);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Не удалось сохранить сборку: " + ex.Message;
            }
        }
    }
}