namespace MasterFileProcessing.Models
{
    public class MasterFileModel
    {
        private List<string> duplicateEquipments { get; set; } = new List<string>();
        private List<string> empCodes { get; set; } = new List<string>();
        private List<int> empCodeCount { get; set; } = new List<int>();
        private List<string> ownerCodes { get; set; } = new List<string>();
        private List<int> ownerCodeCount { get; set; } = new List<int>();

        public List<string> DuplicateEquipments
        {
            get => duplicateEquipments;
            set
            {
                if (duplicateEquipments != value)
                {
                    duplicateEquipments = value;
                }
            }
        }
        public List<string> EmpCodes
        {
            get => empCodes;
            set
            {
                if (empCodes != value)
                {
                    empCodes = value;
                }
            }
        }
        public List<string> OwnerCodes
        {
            get => ownerCodes;
            set
            {
                if (ownerCodes != value)
                {
                    empCodes = value;
                }
            }
        }

        public List<int> EmpCodeCount
        {
            get => empCodeCount;
            set
            {
                if (empCodeCount != value)
                {
                    empCodeCount = value;
                    ////OnPropertyChanged();
                }
            }
        }

        public List<int> OwnerCodeCount
        {
            get => ownerCodeCount;
            set
            {
                if (ownerCodeCount != value)
                {
                    ownerCodeCount = value;
                    ////OnPropertyChanged();
                }
            }
        }
    }
}
