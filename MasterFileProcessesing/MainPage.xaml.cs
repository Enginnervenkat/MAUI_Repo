using MasterFileProcessing.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MasterFileProcessing
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<string> duplicateEquipments;
        private ObservableCollection<EmployeeCodes> employeeCodesDetails;
        private ObservableCollection<OwnerCodesDetails> ownerCodesDetails;

        private long empCodeTotalCount;
        private long ownerCodeTotalCount;
        private bool isDuplicateEquipments { get; set; } = false;
        private bool isLoading = false;


        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public ObservableCollection<string> DuplicateEquipments
        {
            get => duplicateEquipments;
            set
            {
                if (duplicateEquipments != value)
                {
                    duplicateEquipments = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<EmployeeCodes> EmployeeCodesDetails
        {
            get => employeeCodesDetails;
            set
            {
                if (employeeCodesDetails != value)
                {
                    employeeCodesDetails = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<OwnerCodesDetails> OwnerCodesDetails
        {
            get => ownerCodesDetails;
            set
            {
                if (ownerCodesDetails != value)
                {
                    ownerCodesDetails = value;
                    OnPropertyChanged();
                }
            }
        }

        public long EmpCodeTotalCount
        {
            get => empCodeTotalCount;
            set
            {
                if (empCodeTotalCount != value)
                {
                    empCodeTotalCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public long OwnerCodeTotalCount
        {
            get => ownerCodeTotalCount;
            set
            {
                if (ownerCodeTotalCount != value)
                {
                    ownerCodeTotalCount = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsDuplicateEquipments
        {
            get => isDuplicateEquipments;
            set
            {
                if (isDuplicateEquipments != value)
                {
                    isDuplicateEquipments = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                if (isLoading != value)
                {
                    isLoading = value; // Use the backing field here
                    OnPropertyChanged(); // Notify property change if needed
                }
            }
        }

        private async void OnRefreshFileExplorerClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a file"
            });

            if (result != null && result.FileName.Contains(".upd"))
            {
                this.IsLoading = true;
                try
                {
                    using var stream = await result.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var lines = new List<string>();
                    var eqNumbers = new List<string>();
                    var empCode = new List<string>();
                    var ownerCode = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (line != null)
                        {
                            var number = line.Split(',');
                            eqNumbers.Add(number[1]);
                            empCode.Add(number[2]);
                            ownerCode.Add(number[4]);
                            lines.Add(line);
                        }
                    }

                    // Group by trimmed empCode, count each group, and also get the total sum of all counts
                    var empCodeGroupCounts = empCode
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .GroupBy(x => x.Trim(), StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(g => g.Key, g => g.Count());
                    this.EmpCodeTotalCount = empCodeGroupCounts.Values.Sum();
                    var ownerCodeGroupCounts = ownerCode
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .GroupBy(x => x.Trim(), StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(g => g.Key, g => g.Count());
                     this.OwnerCodeTotalCount = ownerCodeGroupCounts.Values.Sum();

                    var dupEqIndex = eqNumbers
                        .Select((line, index) => new { Value = line?.Trim(), Index = index })
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                        .GroupBy(x => x.Value, StringComparer.OrdinalIgnoreCase)
                        .Where(g => g.Count() > 1)
                        .SelectMany(g => g.Select(x => x.Index));
                    var duplist = dupEqIndex.Select(index => lines[index]).ToList();

                    this.DuplicateEquipments = new ObservableCollection<string>();
                    foreach (var val in duplist)
                    {
                        this.DuplicateEquipments.Add(val);
                    }

                    this.EmployeeCodesDetails = new ObservableCollection<EmployeeCodes>();
                    foreach (var val in empCodeGroupCounts)
                    {
                        this.EmployeeCodesDetails.Add(
                        new EmployeeCodes
                        {
                            EmpCodes = val.Key,
                            EmpCodeCount = val.Value
                        });
                    }
                    this.OwnerCodesDetails = new ObservableCollection<OwnerCodesDetails>();
                    foreach (var val in ownerCodeGroupCounts)
                    {
                        this.OwnerCodesDetails.Add(
                        new OwnerCodesDetails
                        {
                            ownerCodes = val.Key,
                            ownerCodeCount = val.Value
                        });
                    }
                    this.IsDuplicateEquipments = true;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to read file: {ex.Message}", "OK");
                }
                finally
                {
                    this.IsLoading = false;
                }
            }
            else
            {
                await DisplayAlert("No File", "No, Master file was selected.", "OK");
            }
        }
    }

}
