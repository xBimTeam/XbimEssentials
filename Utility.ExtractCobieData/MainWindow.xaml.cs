using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace Utility.ExtractCobieData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public override void EndInit()
        {
            base.EndInit();

            //set last paths
            TxtInputFile.Text = Properties.Settings.Default.LastInput;
            TxtOutputFile.Text = Properties.Settings.Default.LastOutput;
            TxtGuidFilter.Text = Properties.Settings.Default.GuidFilter;
            CheckBoxIncludeGeometry.IsChecked = Properties.Settings.Default.IncludeGeometry;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //save settings
            Properties.Settings.Default.LastInput = TxtInputFile.Text;
            Properties.Settings.Default.LastOutput = TxtOutputFile.Text;
            Properties.Settings.Default.GuidFilter = TxtGuidFilter.Text;
            Properties.Settings.Default.IncludeGeometry = CheckBoxIncludeGeometry.IsChecked == true;
            Properties.Settings.Default.Save();

            base.OnClosing(e);
        }

        private void TxtOutputFile_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Select output IFC file",
                AddExtension = true,
                DefaultExt = ".ifc",
                Filter = "IFC file (*.ifc)|*.ifc|IFC file (*.ifcxml)|*.ifcxml"
            };
            if (dlg.ShowDialog(this) == true)
                TxtOutputFile.Text = dlg.FileName;
        }

        private void TxtInputFile_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "IFC file (*.ifc)|*.ifc",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Select input IFC file"
            };
            if (dlg.ShowDialog(this) == true)
                TxtInputFile.Text = dlg.FileName;
        }


        

        private void BtnRun_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtInputFile.Text) ||
                string.IsNullOrWhiteSpace(TxtOutputFile.Text))
            {
                MessageBox.Show("You have to specify input and output file.");
                return;
            }

            Cursor = Cursors.Wait;
            var w = Stopwatch.StartNew();
            long originalCount;
            long resultCount;

            using (var source = IfcStore.Open(TxtInputFile.Text))
            {
                using (var target = IfcStore.Create(source.IfcSchemaVersion , XbimStoreType.InMemoryModel))
                {
                    //insertion itself will be configured to happen out of transaction but other operations might need to be transactional
                    using (var txn = target.BeginTransaction("COBie data extraction"))
                    {
                        var toInsert = GetProductsToInsert(source);
                        var cache = new XbimInstanceHandleMap(source, target);
                        target.InsertProductsWithContext(toInsert, CheckBoxIncludeGeometry.IsChecked ?? false, true, cache);
                        txn.Commit();
                    }

                    target.SaveAs(TxtOutputFile.Text);
                    originalCount = source.Instances.Count;
                    resultCount = target.Instances.Count;
                }
            }
            Cursor = Cursors.Arrow;
            w.Stop();

            var originalSize = new FileInfo(TxtInputFile.Text).Length;
            var cobieSize = new FileInfo(TxtOutputFile.Text).Length;
            var processingTime = w.ElapsedMilliseconds;
            var msg = string.Format("COBie content extracted in {0}s \n" +
                                    "Original size: {1:n0}B \n" +
                                    "Resulting size: {2:n0}B \n" +
                                    "Number of objects in original: {3:n0} \n" +
                                    "Number of objects in result: {4:n0}", 
                processingTime/1000, 
                originalSize, 
                cobieSize,
                originalCount,
                resultCount);

            MessageBox.Show(this, msg);
            Close();
        }

        private IEnumerable<IIfcProduct> GetProductsToInsert(IModel model)
        {
            var guids = TxtGuidFilter.Text;
            if (string.IsNullOrWhiteSpace(guids))
                return model.Instances.OfType<IIfcProduct>();

            var ids =
                guids
                .Split(new[] {',', ' ', ';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(g => g.Trim()).ToList();

            if (GetLabel(ids.First()) < 0)
                //root elements specified by GUID
                return model.Instances.Where<IIfcProduct>(r => ids.Contains(r.GlobalId.ToString()));

            var labels = ids.Select(GetLabel).ToList();
            //root elements specified by GUID
            return model.Instances.Where<IIfcProduct>(r => labels.Contains(r.EntityLabel));
        }

        private static int GetLabel(string value)
        {
            if (value.StartsWith("#"))
            {
                var trim = value.Trim('#');
                int i;
                if (int.TryParse(trim, out i))
                    return i;
                return -1;
            }

            int j;
            if (int.TryParse(value, out j))
                return j;
            return -1;
        }
    }
}
