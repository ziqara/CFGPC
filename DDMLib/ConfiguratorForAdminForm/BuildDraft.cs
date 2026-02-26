namespace DDMLib
{
    public class BuildDraft
    {
        public string ConfigName { get; set; }
        public string Description { get; set; }

        public int MotherboardId { get; set; }
        public int CpuId { get; set; }
        public int RamId { get; set; }
        public int GpuId { get; set; }
        public int StorageId { get; set; }
        public int PsuId { get; set; }
        public int CaseId { get; set; }
        public int CoolingId { get; set; }
    }
}