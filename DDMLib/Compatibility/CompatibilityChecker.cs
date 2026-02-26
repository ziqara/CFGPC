// DDMLib/Compatibility/CompatibilityChecker.cs
using System;
using System.Collections.Generic;
using System.Linq;
using DDMLib.Component;

namespace DDMLib.Compatibility
{
    public class CompatibilityChecker
    {
        private readonly ComponentService _componentService;

        public CompatibilityChecker(ComponentService componentService)
        {
            _componentService = componentService ?? throw new ArgumentNullException(nameof(componentService));
        }

        /// <summary>
        /// Проверяет совместимость процессора с материнской платой
        /// </summary>
        public bool IsCpuCompatibleWithMotherboard(int cpuId, int motherboardId)
        {
            var cpu = _componentService.GetComponentSpec<CpuSpec>(cpuId);
            var motherboard = _componentService.GetComponentSpec<MotherboardSpec>(motherboardId);

            if (cpu == null || motherboard == null)
                return false;

            return cpu.Socket == motherboard.Socket;
        }

        /// <summary>
        /// Проверяет совместимость оперативной памяти с материнской платой
        /// </summary>
        public bool IsRamCompatibleWithMotherboard(int ramId, int motherboardId)
        {
            var ram = _componentService.GetComponentSpec<RamSpec>(ramId);
            var motherboard = _componentService.GetComponentSpec<MotherboardSpec>(motherboardId);

            if (ram == null || motherboard == null)
                return false;

            return ram.RamType == motherboard.RamType;
        }

        /// <summary>
        /// Проверяет совместимость видеокарты с материнской платой (версия PCIe)
        /// </summary>
        public bool IsGpuCompatibleWithMotherboard(int gpuId, int motherboardId)
        {
            var gpu = _componentService.GetComponentSpec<GpuSpec>(gpuId);
            var motherboard = _componentService.GetComponentSpec<MotherboardSpec>(motherboardId);

            if (gpu == null || motherboard == null)
                return true;

            return gpu.PcieVersion == motherboard.PcieVersion;
        }

        /// <summary>
        /// Проверяет совместимость материнской платы с корпусом (форм-фактор)
        /// </summary>
        public bool IsMotherboardCompatibleWithCase(int motherboardId, int caseId)
        {
            var motherboard = _componentService.GetComponentSpec<MotherboardSpec>(motherboardId);
            var caseSpec = _componentService.GetComponentSpec<CaseSpec>(caseId);

            if (motherboard == null || caseSpec == null)
                return false;

            return motherboard.FormFactor == caseSpec.FormFactor;
        }

        /// <summary>
        /// Проверяет, хватит ли мощности блока питания (с запасом 20%)
        /// </summary>
        public bool IsPsuPowerSufficient(int psuId, List<int> componentIds)
        {
            var psu = _componentService.GetComponentSpec<PsuSpec>(psuId);
            if (psu == null)
                return false;

            int totalPower = 0;

            foreach (int componentId in componentIds)
            {
                var component = _componentService.GetComponentById(componentId);
                if (component == null) continue;

                switch (component.Type.ToLower())
                {
                    case "cpu":
                        var cpu = _componentService.GetComponentSpec<CpuSpec>(componentId);
                        if (cpu != null) totalPower += cpu.Tdp;
                        break;
                    case "gpu":
                        var gpu = _componentService.GetComponentSpec<GpuSpec>(componentId);
                        if (gpu != null) totalPower += gpu.Tdp;
                        break;
                }
            }

            return psu.Wattage >= totalPower * 1.2;
        }

        /// <summary>
        /// Проверяет, подходит ли охлаждение к процессору по TDP
        /// </summary>
        public bool IsCoolingCompatibleWithCpu(int coolingId, int cpuId)
        {
            var cooling = _componentService.GetComponentSpec<CoolingSpec>(coolingId);
            var cpu = _componentService.GetComponentSpec<CpuSpec>(cpuId);

            if (cooling == null || cpu == null)
                return false;

            return cooling.TdpSupport >= cpu.Tdp;
        }

        /// <summary>
        /// Проверяет все совместимости для выбранного компонента
        /// </summary>
        public List<string> CheckCompatibilityForComponent(int componentId, Dictionary<string, int> selectedComponents)
        {
            var issues = new List<string>();
            var component = _componentService.GetComponentById(componentId);
            if (component == null) return issues;

            switch (component.Type.ToLower())
            {
                case "cpu":
                    if (selectedComponents.ContainsKey("motherboard") &&
                        !IsCpuCompatibleWithMotherboard(componentId, selectedComponents["motherboard"]))
                    {
                        issues.Add("Процессор не совместим с выбранной материнской платой (разные сокеты)");
                    }
                    break;

                case "motherboard":
                    if (selectedComponents.ContainsKey("cpu") &&
                        !IsCpuCompatibleWithMotherboard(selectedComponents["cpu"], componentId))
                    {
                        issues.Add("Материнская плата не совместима с выбранным процессором (разные сокеты)");
                    }
                    if (selectedComponents.ContainsKey("ram") &&
                        !IsRamCompatibleWithMotherboard(selectedComponents["ram"], componentId))
                    {
                        issues.Add("Материнская плата не совместима с выбранной оперативной памятью (разные типы)");
                    }
                    if (selectedComponents.ContainsKey("gpu") &&
                        !IsGpuCompatibleWithMotherboard(selectedComponents["gpu"], componentId))
                    {
                        issues.Add("Материнская плата может не поддерживать версию PCIe выбранной видеокарты");
                    }
                    if (selectedComponents.ContainsKey("case") &&
                        !IsMotherboardCompatibleWithCase(componentId, selectedComponents["case"]))
                    {
                        issues.Add("Материнская плата не подходит к выбранному корпусу (разные форм-факторы)");
                    }
                    break;

                case "ram":
                    if (selectedComponents.ContainsKey("motherboard") &&
                        !IsRamCompatibleWithMotherboard(componentId, selectedComponents["motherboard"]))
                    {
                        issues.Add("Оперативная память не совместима с выбранной материнской платой (разные типы)");
                    }
                    break;

                case "gpu":
                    if (selectedComponents.ContainsKey("motherboard") &&
                        !IsGpuCompatibleWithMotherboard(componentId, selectedComponents["motherboard"]))
                    {
                        issues.Add("Видеокарта может не полностью использовать возможности материнской платы (разные версии PCIe)");
                    }
                    break;

                case "case":
                    if (selectedComponents.ContainsKey("motherboard") &&
                        !IsMotherboardCompatibleWithCase(selectedComponents["motherboard"], componentId))
                    {
                        issues.Add("Корпус не подходит для выбранной материнской платы (разные форм-факторы)");
                    }
                    break;

                case "psu":
                    var allComponentIds = selectedComponents.Values.ToList();
                    if (selectedComponents.ContainsKey("psu"))
                    {
                        allComponentIds.Remove(selectedComponents["psu"]);
                    }
                    allComponentIds.Add(componentId);

                    if (!IsPsuPowerSufficient(componentId, allComponentIds))
                    {
                        issues.Add("Мощность блока питания недостаточна для текущей сборки");
                    }
                    break;

                case "cooling":
                    if (selectedComponents.ContainsKey("cpu") &&
                        !IsCoolingCompatibleWithCpu(componentId, selectedComponents["cpu"]))
                    {
                        issues.Add("Охлаждение не справится с тепловыделением выбранного процессора");
                    }
                    break;
            }

            return issues;
        }

        /// <summary>
        /// Фильтрует компоненты по совместимости с уже выбранными
        /// </summary>
        public List<ComponentDto> FilterCompatibleComponents(string category, List<ComponentDto> components, Dictionary<string, int> selectedComponents)
        {
            if (selectedComponents == null || !selectedComponents.Any())
                return components;

            var filtered = new List<ComponentDto>();

            foreach (var component in components)
            {
                bool isCompatible = true;

                switch (category)
                {
                    case "motherboard":
                        // Если выбран процессор, фильтруем по сокету
                        if (selectedComponents.ContainsKey("cpu"))
                        {
                            var cpuSpec = _componentService.GetComponentSpec<CpuSpec>(selectedComponents["cpu"]);
                            var moboSpec = component.Specs as MotherboardSpec;
                            if (cpuSpec != null && moboSpec != null && cpuSpec.Socket != moboSpec.Socket)
                                isCompatible = false;
                        }
                        break;

                    case "cpu":
                        // Если выбрана материнская плата, фильтруем по сокету
                        if (selectedComponents.ContainsKey("motherboard"))
                        {
                            var moboSpec = _componentService.GetComponentSpec<MotherboardSpec>(selectedComponents["motherboard"]);
                            var cpuSpec = component.Specs as CpuSpec;
                            if (moboSpec != null && cpuSpec != null && cpuSpec.Socket != moboSpec.Socket)
                                isCompatible = false;
                        }
                        break;

                    case "ram":
                        // Если выбрана материнская плата, фильтруем по типу памяти
                        if (selectedComponents.ContainsKey("motherboard"))
                        {
                            var moboSpec = _componentService.GetComponentSpec<MotherboardSpec>(selectedComponents["motherboard"]);
                            var ramSpec = component.Specs as RamSpec;
                            if (moboSpec != null && ramSpec != null && ramSpec.RamType != moboSpec.RamType)
                                isCompatible = false;
                        }
                        break;

                    case "cooling":
                        // Если выбран процессор, фильтруем по TDP
                        if (selectedComponents.ContainsKey("cpu"))
                        {
                            var cpuSpec = _componentService.GetComponentSpec<CpuSpec>(selectedComponents["cpu"]);
                            var coolingSpec = component.Specs as CoolingSpec;
                            if (cpuSpec != null && coolingSpec != null && coolingSpec.TdpSupport < cpuSpec.Tdp)
                                isCompatible = false;
                        }
                        break;

                    case "case":
                        // Если выбрана материнская плата, фильтруем по форм-фактору
                        if (selectedComponents.ContainsKey("motherboard"))
                        {
                            var moboSpec = _componentService.GetComponentSpec<MotherboardSpec>(selectedComponents["motherboard"]);
                            var caseSpec = component.Specs as CaseSpec;
                            if (moboSpec != null && caseSpec != null && moboSpec.FormFactor != caseSpec.FormFactor)
                                isCompatible = false;
                        }
                        break;
                }

                if (isCompatible)
                    filtered.Add(component);
            }

            return filtered;
        }
    }
}