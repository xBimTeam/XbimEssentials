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
using Xbim.Common.Metadata;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.IO.Memory;

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
                Title = "Select output IFC2x3 file",
                AddExtension = true,
                DefaultExt = ".ifc", 
                Filter = "IFC2x3 files (*.ifc)|*.ifc"
            };
            if (dlg.ShowDialog(this) == true)
                TxtOutputFile.Text = dlg.FileName;
        }

        private void TxtInputFile_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "IFC2x3 files (*.ifc)|*.ifc",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Select input IFC2x3 file"
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
            using (var source = new MemoryModel<EntityFactory>())
            {
                source.Open(TxtInputFile.Text);
                using (var target = new MemoryModel<EntityFactory>())
                {
                    //insertion itself will be configured to happen out of transaction but other operations might need to be transactional
                    using (var txn = target.BeginTransaction("COBie data extraction"))
                    {
                        var toInsert = GetEntitiesToInsert(source);
                        var cache = new Dictionary<int, IPersistEntity>();

                        //set to happen out of transaction. This should save some memory used by transaction log
                        foreach (var entity in toInsert)
                            target.InsertCopy(entity, cache, false, true, Filter, true);

                        txn.Commit();
                    }

                    target.Save(TxtOutputFile.Text);
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

            //clear primary elements
            _primaryElements = null;

            MessageBox.Show(this, msg);
            Close();
        }

        private IEnumerable<IPersistEntity> GetEntitiesToInsert(IModel model)
        {
            var guids = TxtGuidFilter.Text;
            if (string.IsNullOrWhiteSpace(guids))
                return model.Instances.OfType<IfcRoot>();

            var ids =
                guids
                .Split(new[] {',', ' ', ';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(g => g.Trim()).ToList();

            List<IPersistEntity> roots;
            if (GetLabel(ids.First()) < 0)
                //root elements specified by GUID
                roots = model.Instances.Where<IfcRoot>(r => ids.Contains(r.GlobalId.ToString())).Cast<IPersistEntity>().ToList();
            else
            {
                var labels = ids.Select(GetLabel).ToList();
                //root elements specified by GUID
                roots = model.Instances.Where<IfcRoot>(r => labels.Contains(r.EntityLabel)).Cast<IPersistEntity>().ToList();
            }
            
            _primaryElements = roots.OfType<IfcProduct>().ToList();

            //add any aggregated elements. For example IfcRoof is typically aggregation of one or more slabs so we need to bring
            //them along to have all the information both for geometry and for properties and materials.
            //This has to happen before we add spatial hierarchy or it would bring in full hierarchy which is not an intention
            var decompositionRels = GetAggregations(_primaryElements.ToList(), model).ToList();
            var decomposition = decompositionRels.SelectMany(r => r.RelatedObjects).OfType<IfcProduct>();
            _primaryElements.AddRange(decomposition);
            roots.AddRange(decompositionRels);
            
            //we should add spatial hierarchy right here so it brings its attributes as well
            var spatialRels = model.Instances.Where<IfcRelContainedInSpatialStructure>(
                r => _primaryElements.Any(e => r.RelatedElements.Contains(e))).ToList();
            var spatialRefs =
                model.Instances.Where<IfcRelReferencedInSpatialStructure>(
                    r => _primaryElements.Any(e => r.RelatedElements.Contains(e))).ToList();
            var bottomSpatialHierarchy =
                spatialRels.Select(r => r.RelatingStructure).Union(spatialRefs.Select(r => r.RelatingStructure)).ToList();
            var spatialAggregations = GetUpstreamHierarchy(bottomSpatialHierarchy, model).ToList();

            //add all spatial elements from bottom and from upstream hierarchy
            _primaryElements.AddRange(bottomSpatialHierarchy);
            _primaryElements.AddRange(spatialAggregations.Select(r => r.RelatingObject).OfType<IfcProduct>());
            roots.AddRange(spatialAggregations);
            roots.AddRange(spatialRels);
            roots.AddRange(spatialRefs);

            //we should add any feature elements used to subtract mass from a product
            var featureRels = GetFeatureRelations(_primaryElements).ToList();
            var openings = featureRels.Select(r => r.RelatedOpeningElement);
            _primaryElements.AddRange(openings);
            roots.AddRange(featureRels);

            //object types and properties for all primary products (elements and spatial elements)
            roots.AddRange(model.Instances.Where<IfcRelDefines>( r => _primaryElements.Any(e => r.RelatedObjects.Contains(e))));
            

            
            //assignmnet to groups will bring in all system aggregarions if defined in the file
            roots.AddRange(model.Instances.Where<IfcRelAssigns>(r => _primaryElements.Any(e => r.RelatedObjects.Contains(e))));
            
            //associations with classification, material and documents
            roots.AddRange(model.Instances.Where<IfcRelAssociates>(r => _primaryElements.Any(e => r.RelatedObjects.Contains(e))));

            return roots;
        }

        private int GetLabel(string value)
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

        private IEnumerable<IfcRelVoidsElement> GetFeatureRelations(IEnumerable<IfcProduct> products)
        {
            var elements = products.OfType<IfcElement>().ToList();
            if(!elements.Any()) yield break;
            var model = elements.First().Model;
            var rels = model.Instances.Where<IfcRelVoidsElement>(r => elements.Any(e => e == r.RelatingBuildingElement));
            foreach (var rel in rels)
                yield return rel;
        }

        private IEnumerable<IfcRelDecomposes> GetAggregations(List<IfcProduct> products, IModel model)
        {
            while (true)
            {
                if (!products.Any())
                    yield break;

                var products1 = products;
                var rels = model.Instances.Where<IfcRelDecomposes>(r => products1.Any(p => r.RelatingObject == p)).ToList();
                var relatedProducts = rels.SelectMany(r => r.RelatedObjects).OfType<IfcProduct>().ToList();
                foreach (var rel in rels)
                    yield return rel;

                products = relatedProducts;
            }
        }

        private IEnumerable<IfcRelAggregates> GetUpstreamHierarchy(IEnumerable<IfcSpatialStructureElement> spatialStructureElements, IModel model)
        {
            while (true)
            {
                var elements = spatialStructureElements.ToList();
                if (!elements.Any())
                    yield break;

                var rels = model.Instances.Where<IfcRelAggregates>(r => elements.Any(s => r.RelatedObjects.Contains(s))).ToList();
                var decomposing = rels.Select(r => r.RelatingObject).OfType<IfcSpatialStructureElement>();

                foreach (var rel in rels)
                    yield return rel;

                spatialStructureElements = decomposing;
            }
        }

        private List<IfcProduct> _primaryElements; 

        private object Filter(ExpressMetaProperty property, object parentObject)
        {
            if (_primaryElements != null && _primaryElements.Any())
            {
                if (typeof(IfcProduct).IsAssignableFrom(property.PropertyInfo.PropertyType))
                {
                    var element = property.PropertyInfo.GetValue(parentObject, null) as IfcProduct;
                    if (element != null && _primaryElements.Contains(element))
                        return element;
                    return null;
                }
                if (typeof(IEnumerable<IPersistEntity>).IsAssignableFrom(property.PropertyInfo.PropertyType))
                {
                    var entities = property.PropertyInfo.GetValue(parentObject, null) as IEnumerable<IPersistEntity>;
                    if (entities == null)
                        return null;
                    var elementsToRemove = entities.OfType<IfcProduct>().Where(e => !_primaryElements.Contains(e)).ToList();
                    //if there are no IfcElements return what is in there with no care
                    if (elementsToRemove.Any())
                        //return original values excluding elements not included in the primary set
                        return entities.Except(elementsToRemove).ToList();
                }
            }
                


            //if geometry is to be included don't filter it out
            if (CheckBoxIncludeGeometry.IsChecked == true)
                return property.PropertyInfo.GetValue(parentObject, null);

            //leave out geometry and placement of products
            if (parentObject is IfcProduct && 
                (property.PropertyInfo.Name == "Representation" || property.PropertyInfo.Name == "ObjectPlacement")
                )
                return null;

            //leave out representation maps
            if (parentObject is IfcTypeProduct && property.PropertyInfo.Name == "RepresentationMaps")
                return null;

            //leave out eventual connection geometry
            if (parentObject is IfcRelSpaceBoundary && property.PropertyInfo.Name == "ConnectionGeometry")
                return null;

            //return the value for anything else
            return property.PropertyInfo.GetValue(parentObject, null);
        }
    }
}
