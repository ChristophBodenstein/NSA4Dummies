﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NSA4Dummies
{
    /// <summary>
    /// This class handles the data of the charts
    /// </summary>
    public class GUIViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// The chart type struct
        /// </summary>
        public struct ChartType
        {
            public ChartType(string title, string key)
            {
                this.title = title;
                this.key = key;
            }

            private string title;
            private string key;

            public string Title
            {
                get
                {
                    return title;
                }
                set
                {
                    title = value;
                }
            }
            public string Key
            {
                get
                {
                    return key;
                }
                set
                {
                    key = value;
                }
            }

            public override string ToString()
            {
                return Title;
            }
        }

        /// <summary>
        /// This member holds the font size of the charts
        /// </summary>
        public List<double> FontSizes { get; set; }
        /// <summary>
        /// This member holds the inner radius of the doughnut chart
        /// </summary>
        public List<double> DoughnutInnerRadiusRatios { get; set; }
        public List<string> SelectionBrushes { get; set; }
        public ObservableCollection<ChartType> ViewTypes { get; set; }
        public Dictionary<string, De.TorstenMandelkow.MetroChart.ResourceDictionaryCollection> Palettes { get; set; }


        /*
         *  Chart-Data dictionaries
         * */
        private Dictionary<string, float> SizePerCountry = new Dictionary<string, float>();
        private Dictionary<string, uint> Countries = new Dictionary<string, uint>();
        private Dictionary<string, int> Size = new Dictionary<string, int>();
        private Dictionary<string, int> Protocols = new Dictionary<string, int>();


        /// <summary>
        /// Adds a package to the charts
        /// </summary>
        /// <param name="packageSize">The size (in bytes) of the package</param>
        /// <param name="protocol">The used protocol of the package (see: DataPacket.DataTransferProtocol)</param>
        /// <param name="countryShort">The two-letter code of the country</param>
        public void addPackage(int packageSize, DataPacket.DataTransferProtocol protocol, string countryShort)
        {
            countryShort = countryShort.ToUpper();

            string sizeNormalized;
            string protocolString = "";
            
            if (packageSize <= 100)
            {
                sizeNormalized = "< 100B";
            }
            else if(packageSize < 500)
            {
                sizeNormalized = "100B - 500B";
            }
            else if(packageSize < 1000)
            {
                sizeNormalized = "500B - 1kB";
            }
            else if(packageSize < 1500)
            {
                sizeNormalized = "1kB - 1.5kB";
            }
            else if (packageSize < 2000)
            {
                sizeNormalized = "1.5kB - 2kB";
            }
            else
            {
                sizeNormalized = "> 2kB";
            }

            if (Size.ContainsKey(sizeNormalized))
            {
                Size[sizeNormalized] += 1;
            }
            else
            {
                Size.Add(sizeNormalized, 1);
            }

            switch (protocol)
            {
                case DataPacket.DataTransferProtocol.DTP_TCP:
                    protocolString = "TCP";
                    break;
                case DataPacket.DataTransferProtocol.DTP_UDP:
                    protocolString = "UDP";
                    break;
                case DataPacket.DataTransferProtocol.DTP_ICMP:
                    protocolString = "ICMP";
                    break;
                default:
                    protocolString = "OTHER";
                    break;
            }

            if (Protocols.ContainsKey(protocolString))
            {
                Protocols[protocolString] += 1;
            }
            else
            {
                Protocols.Add(protocolString, 1);
            }

            if (Countries.ContainsKey(countryShort))
            {
                Countries[countryShort] += 1;
            }
            else
            {
                Countries.Add(countryShort, 1);
            }

            if (SizePerCountry.ContainsKey(countryShort))
            {
                SizePerCountry[countryShort] += (float)packageSize / (float)1000000.0;
            }
            else
            {
                SizePerCountry.Add(countryShort, (float)packageSize / (float)1000000.0);
            }
        }


        /// <summary>
        /// This function is called whenever graphs shall be updated
        /// </summary>
        public void updateDataGraphs()
        {
            PackagesPerCountry.Clear();
            FileSizePerCountry.Clear();
            UsedProtocols.Clear();
            PackageSize.Clear();

            foreach(var c in Countries){
                PackagesPerCountry.Add(new DataClass() { Category = c.Key, Number = c.Value });
            }
            
            foreach (var p in Protocols)
            {
                UsedProtocols.Add(new DataClass() { Category = p.Key, Number = p.Value });
            }

            foreach (var s in SizePerCountry)
            {
                FileSizePerCountry.Add(new DataClass() { Category = s.Key, Number = (float)Math.Round(s.Value, 2) });
            }

            foreach (var s in Size)
            {
                PackageSize.Add(new DataClass() { Category = s.Key, Number = s.Value });
            }
        }


        /// <summary>
        /// The dictionary that holds the translations for the whole program
        /// </summary>
        public static Dictionary<string, string> T
        {
            get
            {
                return App.translation;
            }
        }
       

        private ChartType selectedChartType;
        /// <summary>
        /// Returns the selected chrt type
        /// </summary>
        public ChartType SelectedChartType
        {
            get
            {
                return selectedChartType;
            }
            set
            {
                selectedChartType = value;
                NotifyPropertyChanged("SelectedChartType");
            }
        }


        private object selectedPalette = null;
        /// <summary>
        /// Returns the selected palette
        /// </summary>
        public object SelectedPalette
        {
            get
            {
                return selectedPalette;
            }
            set
            {
                selectedPalette = value;
                NotifyPropertyChanged("SelectedPalette");
            }
        }


        private string selectedBrush = null;
        /// <summary>
        /// Returns the selected brush
        /// </summary>
        public string SelectedBrush
        {
            get
            {
                return selectedBrush;
            }
            set
            {
                selectedBrush = value;
                NotifyPropertyChanged("SelectedBrush");
            }
        }


        private double selectedDoughnutInnerRadiusRatio = 0.75;
        /// <summary>
        /// Returns the selected inner radius of the doughnut chart
        /// </summary>
        public double SelectedDoughnutInnerRadiusRatio
        {
            get
            {
                return selectedDoughnutInnerRadiusRatio;
            }
            set
            {
                selectedDoughnutInnerRadiusRatio = value;
                NotifyPropertyChanged("SelectedDoughnutInnerRadiusRatio");
                NotifyPropertyChanged("SelectedDoughnutInnerRadiusRatioString");
            }
        }

        
        /// <summary>
        /// Returns the selected font size as string
        /// </summary>
        public string SelectedFontSizeString
        {
            get
            {
                return SelectedFontSize.ToString() + "px";
            }
        }


        private object selectedItem = null;
        /// <summary>
        /// Returns the selected item
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }


        private double fontSize = 11.0;
        /// <summary>
        /// Returns the selected font size
        /// </summary>
        public double SelectedFontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                NotifyPropertyChanged("SelectedFontSize");
                NotifyPropertyChanged("SelectedFontSizeString");
            }
        }


        private bool isRowColumnSwitched = false;
        /// <summary>
        /// Returns whether the row column is switched or not
        /// </summary>
        public bool IsRowColumnSwitched
        {
            get
            {
                return isRowColumnSwitched;
            }
            set
            {
                isRowColumnSwitched = value;
                NotifyPropertyChanged("IsRowColumnSwitched");
            }
        }


        private bool isLegendVisible = true;
        /// <summary>
        /// Returns whether the legend is visible or not
        /// </summary>
        public bool IsLegendVisible
        {
            get
            {
                return isLegendVisible;
            }
            set
            {
                isLegendVisible = value;
                NotifyPropertyChanged("IsLegendVisible");
            }
        }


        private bool isTitleVisible = true;
        /// <summary>
        /// Returns whether the title column is switched or not
        /// </summary>
        public bool IsTitleVisible
        {
            get
            {
                return isTitleVisible;
            }
            set
            {
                isTitleVisible = value;
                NotifyPropertyChanged("IsTitleVisible");
            }
        }


        /// <summary>
        /// Returns the selected inner radius of the doughnut chart as string
        /// </summary>
        public string SelectedDoughnutInnerRadiusRatioString
        {
            get
            {
                return String.Format("{0:P1}.", SelectedDoughnutInnerRadiusRatio);
            }
        }


        
        /*----------
         * 
         *  Layout
         * 
         * --------*/

        /*
         *  Get/Set of layout elements
         * */
        /// <summary>
        /// Color of Charts-Text
        /// </summary>
        public string Foreground
        {
            get
            {
                return (string)Application.Current.FindResource("chartsLabelColor");
            }
        }


        /// <summary>
        /// Color of Header texts
        /// </summary>
        public string MainForeground
        {
            get
            {
                return (string)Application.Current.FindResource("chartsLabelColor");
            }
        }


        /// <summary>
        /// The background color
        /// </summary>
        public string Background
        {
            get
            {
                return (string)Application.Current.FindResource("chartsBackgroundColor");
            }
        }


        /// <summary>
        /// The main background color of the program
        /// </summary>
        public string MainBackground
        {
            get
            {
                return (string)Application.Current.FindResource("programMainBackgroundColor");
            }
        }


        /// <summary>
        /// Default color of the countries
        /// </summary>
        public string MapDefaultColor
        {
            get
            {
                return (string)Application.Current.FindResource("mapDefaultShadingColor");
            }
        }



        /*
         *  Events
         * */
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns the tooltip string
        /// </summary>
        public string ToolTipFormat
        {
            get
            {
                return "{0} in series '{2}' has value '{1}' ({3:P2})";
            }
        }

        /*
         *  Private methods
         * */
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }


        private void LoadPalettes()
        {
            Palettes = new Dictionary<string, De.TorstenMandelkow.MetroChart.ResourceDictionaryCollection>();
            Palettes.Add("Default", null);

            var resources = Application.Current.Resources.MergedDictionaries.ToList();
            foreach (var dict in resources)
            {
                foreach (var objkey in dict.Keys)
                {
                    if (dict[objkey] is De.TorstenMandelkow.MetroChart.ResourceDictionaryCollection)
                    {
                        Palettes.Add(objkey.ToString(), dict[objkey] as De.TorstenMandelkow.MetroChart.ResourceDictionaryCollection);
                    }
                }
            }

            SelectedPalette = Palettes.FirstOrDefault();
        }


        int newSeriesCounter = 1;
        private void AddSeries()
        {
            ObservableCollection<DataClass> data = new ObservableCollection<DataClass>();

            data.Add(new DataClass() { Category = "Globalization", Number = 5 });
            data.Add(new DataClass() { Category = "Features", Number = 10 });
            data.Add(new DataClass() { Category = "ContentTypes", Number = 15 });
            data.Add(new DataClass() { Category = "Correctness", Number = 20 });
            data.Add(new DataClass() { Category = "Naming", Number = 15 });
            data.Add(new DataClass() { Category = "Best Practices", Number = 10 });

            newSeriesCounter++;
        }


        /// <summary>
        /// The constructor of GUIViewModel
        /// </summary>
        public GUIViewModel()
        {
            LoadPalettes();

            ViewTypes = new ObservableCollection<ChartType>();
            ViewTypes.Add(new ChartType(App.translation["mainWindow.map"], "Map"));
            ViewTypes.Add(new ChartType(App.translation["mainWindow.stats"], "Statistics"));
            ViewTypes.Add(new ChartType(App.translation["mainWindow.settings"], "Settings"));
            ViewTypes.Add(new ChartType(App.translation["mainWindow.credits"], "Credits"));
            SelectedChartType = ViewTypes.FirstOrDefault();
			

            FontSizes = new List<double>();
            FontSizes.Add(8.0);
            FontSizes.Add(9.0);
            FontSizes.Add(10.0);
            FontSizes.Add(11.0);
            FontSizes.Add(12.0);
            FontSizes.Add(13.0);
            FontSizes.Add(18.0);
            SelectedFontSize = 12.0;

            DoughnutInnerRadiusRatios = new List<double>();
            DoughnutInnerRadiusRatios.Add(0.90);
            DoughnutInnerRadiusRatios.Add(0.75);
            DoughnutInnerRadiusRatios.Add(0.5);
            DoughnutInnerRadiusRatios.Add(0.25);
            DoughnutInnerRadiusRatios.Add(0.1);
            SelectedDoughnutInnerRadiusRatio = 0.75;

            SelectionBrushes = new List<string>();
            SelectionBrushes.Add("Orange");
            SelectionBrushes.Add("Red");
            SelectionBrushes.Add("Yellow");
            SelectionBrushes.Add("Blue");
            SelectionBrushes.Add("[NoColor]");
            SelectedBrush = SelectionBrushes.FirstOrDefault();

            UsedProtocols = new ObservableCollection<DataClass>();
            PackageSize = new ObservableCollection<DataClass>();
            FileSizePerCountry = new ObservableCollection<DataClass>();
            PackagesPerCountry = new ObservableCollection<DataClass>();
        }


        /// <summary>
        /// The collection that holds the top websites
        /// </summary>
        public ObservableCollection<DataClass> UsedProtocols
        {
            get;
            set;
        }


        /// <summary>
        /// The collection that holds the (un-)encrypted packages
        /// </summary>
        public ObservableCollection<DataClass> PackageSize
        {
            get;
            set;
        }


        /// <summary>
        /// The collection that holds the filetypes of the packages
        /// </summary>
        public ObservableCollection<DataClass> FileSizePerCountry
        {
            get;
            set;
        }


        /// <summary>
        /// The collection that holds number of packages sent to a country
        /// </summary>
        public ObservableCollection<DataClass> PackagesPerCountry
        {
            get;
            set;
        }
    }


    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }


    /// <summary>
    /// The class that is used to store data to the charts
    /// </summary>
    public class DataClass
    {
        public string Category { get; set; }

        private float _number = 0;
        /// <summary>
        /// The numerical value of a chart element
        /// </summary>
        public float Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }
    }
}
