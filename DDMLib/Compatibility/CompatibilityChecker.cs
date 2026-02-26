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
            {
                ErrorLogger.LogError("FilterCompatibleComponents", "No selected components, returning all");
                return components;
            }

            var filtered = new List<ComponentDto>();
            ErrorLogger.LogError("FilterCompatibleComponents", $"Category: {category}, selected components count: {selectedComponents.Count}");

            foreach (var component in components)
            {
                bool isCompatible = true;

                switch (category)
                {
                    case "motherboard":
                        // Проверка совместимости с процессором
                        if (selectedComponents.ContainsKey("cpu"))
                        {
                            var cpuSpec = _componentService.GetComponentSpec<CpuSpec>(selectedComponents["cpu"]);
                            var moboSpec = component.Specs as MotherboardSpec;
                            if (cpuSpec != null && moboSpec != null)
                            {
                                if (cpuSpec.Socket != moboSpec.Socket)
                                {
                                    isCompatible = false;
                                    ErrorLogger.LogError("FilterCompatibleComponents",
                                        $"Motherboard {component.Component.Name} (socket {moboSpec.Socket}) incompatible with CPU socket {cpuSpec.Socket}");
                                }
                            }
                            else
                            {
                                ErrorLogger.LogError("FilterCompatibleComponents",
                                    $"Missing specs: CPU spec null: {cpuSpec == null}, Motherboard spec null: {moboSpec == null}");
                            }
                        }
                        // Проверка совместимости с RAM
                        if (isCompatible && selectedComponents.ContainsKey("ram"))
                        {
                            var ramSpec = _componentService.GetComponentSpec<RamSpec>(selectedComponents["ram"]);
                            var moboSpec = component.Specs as MotherboardSpec;
                            if (ramSpec != null && moboSpec != null)
                            {
                                if (ramSpec.RamType != moboSpec.RamType)
                                {
                                    isCompatible = false;
                                    ErrorLogger.LogError("FilterCompatibleComponents",
                                        $"Motherboard {component.Component.Name} (RAM type {moboSpec.RamType}) incompatible with RAM type {ramSpec.RamType}");
                                }
                            }
                        }
                        break;

                    case "cpu":
                        // Проверка совместимости с материнской платой
                        if (selectedComponents.ContainsKey("motherboard"))
                        {
                            var moboSpec = _componentService.GetComponentSpec<MotherboardSpec>(selectedComponents["motherboard"]);
                            var cpuSpec = component.Specs as CpuSpec;
                            if (moboSpec != null && cpuSpec != null)
                            {
                                if (cpuSpec.Socket != moboSpec.Socket)
                                {
                                    isCompatible = false;
                                    ErrorLogger.LogError("FilterCompatibleComponents",
                                        $"CPU {component.Component.Name} (socket {cpuSpec.Socket}) incompatible with motherboard socket {moboSpec.Socket}");
                                }
                            }
                        }
                        break;

                    case "ram":
                        // Проверка совместимости с материнской платой
                        if (selectedComponents.ContainsKey("motherboard"))
                        {
                            var moboSpec = _componentService.GetComponentSpec<MotherboardSpec>(selectedComponents["motherboard"]);
                            var ramSpec = component.Specs as RamSpec;
                            if (moboSpec != null && ramSpec != null)
                            {
                                if (ramSpec.RamType != moboSpec.RamType)
                                {
                                    isCompatible = false;
                                    ErrorLogger.LogError("FilterCompatibleComponents",
                                        $"RAM {component.Component.Name} (type {ramSpec.RamType}) incompatible with motherboard RAM type {moboSpec.RamType}");
                                }
                            }
                        }
                        break;

                    case "cooling":
                        // Проверка совместимости с процессором по TDP
                        if (selectedComponents.ContainsKey("cpu"))
                        {
                            var cpuSpec = _componentService.GetComponentSpec<CpuSpec>(selectedComponents["cpu"]);
                            var coolingSpec = component.Specs as CoolingSpec;
                            if (cpuSpec != null && coolingSpec != null)
                            {
                                if (coolingSpec.TdpSupport < cpuSpec.Tdp)
                                {
                                    isCompatible = false;
                                    ErrorLogger.LogError("FilterCompatibleComponents",
                                        $"Cooling {component.Component.Name} (TDP support {coolingSpec.TdpSupport}) insufficient for CPU TDP {cpuSpec.Tdp}");
                                }
                            }
                        }
                        break;

                    case "case":
                        // Проверка совместимости с материнской платой по форм-фактору
                        if (selectedComponents.ContainsKey("motherboard"))
                        {
                            var moboSpec = _componentService.GetComponentSpec<MotherboardSpec>(selectedComponents["motherboard"]);
                            var caseSpec = component.Specs as CaseSpec;
                            if (moboSpec != null && caseSpec != null)
                            {
                                if (moboSpec.FormFactor != caseSpec.FormFactor)
                                {
                                    isCompatible = false;
                                    ErrorLogger.LogError("FilterCompatibleComponents",
                                        $"Case {component.Component.Name} (form factor {caseSpec.FormFactor}) incompatible with motherboard form factor {moboSpec.FormFactor}");
                                }
                            }
                        }
                        break;

                    // Для остальных категорий пока нет фильтрации
                    case "gpu":
                    case "storage":
                    case "psu":
                        // Пока не фильтруем, так как нет жестких зависимостей
                        break;
                }

                if (isCompatible)
                {
                    filtered.Add(component);
                    ErrorLogger.LogError("FilterCompatibleComponents", $"Component {component.Component.Name} is compatible, adding to filtered list");
                }
            }

            ErrorLogger.LogError("FilterCompatibleComponents", $"Filtered {filtered.Count} out of {components.Count} components");
            return filtered;
        }
    }
}