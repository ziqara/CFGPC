using System.Collections.Generic;

namespace DDMLib
{
    public class BuildValidator
    {
        public List<string> Validate(BuildDraft draft)
        {
            var errors = new List<string>();

            if (draft == null)
            {
                errors.Add("Пустая сборка");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(draft.ConfigName))
                errors.Add("Название сборки обязательно");

            if (draft.MotherboardId <= 0) errors.Add("Выберите материнскую плату");
            if (draft.CpuId <= 0) errors.Add("Выберите процессор");
            if (draft.RamId <= 0) errors.Add("Выберите оперативную память");
            if (draft.GpuId <= 0) errors.Add("Выберите видеокарту");
            if (draft.StorageId <= 0) errors.Add("Выберите накопитель");
            if (draft.PsuId <= 0) errors.Add("Выберите блок питания");
            if (draft.CaseId <= 0) errors.Add("Выберите корпус");
            if (draft.CoolingId <= 0) errors.Add("Выберите охлаждение");

            return errors;
        }
    }
}