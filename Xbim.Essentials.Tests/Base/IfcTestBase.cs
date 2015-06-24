using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Xbim.Common.Logging;
using Xbim.IO;
using Xbim.XbimExtensions;

namespace Xbim.Essentials.Tests
{
    /// <summary>
    /// A base class for unit testing IFC files. Provides a standard means of parsing and opening models for
    /// Unit tests. 
    /// </summary>
    /// <remarks>Parsing is performed in ths constructor since ClassInitialize does not work on base classes under mstest</remarks>
    public abstract class IfcTestBase
    {
        protected readonly ILogger Logger = LoggerFactory.GetLogger();

        protected string XbimFile;

        /// <summary>
        /// The Model instance under test. Automatically opened fresh before each test method is run.
        /// </summary>
        protected XbimModel Model
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a Test Environment and initialises a model. The model is inferred from the Class name of the test fixture.
        /// </summary>
        public IfcTestBase()
        {
            var inferredModel = Path.ChangeExtension(this.GetType().Name, ".ifc");
            InitialiseModel(inferredModel);
        }

        /// <summary>
        /// Constructs a Test Environment and initialises a model with the given file. Use this overload if your
        /// model is not named after your test fixture.
        /// </summary>
        public IfcTestBase(string ifcFile)
        {
            InitialiseModel(ifcFile);
        }

        /// <summary>
        /// Parses the IFC file to xbim
        /// </summary>
        /// <remarks>Typically once per fixture</remarks>
        /// <param name="ifcFile"></param>
        public void InitialiseModel(String ifcFile)
        {
            if(!File.Exists(ifcFile))
            {
                var message = String.Format("Model '{0}' could not be located in '{1}'. \nCheck that you have applied a [DeploymentItem] attribute to the class and that the file is set to be copied on build.", 
                    ifcFile,
                    Environment.CurrentDirectory);
                throw new FileNotFoundException(message,
                    ifcFile);
            }
            Logger.DebugFormat(@"Opening model '{0}\{1}'", Environment.CurrentDirectory, ifcFile);
            XbimFile = Path.ChangeExtension(ifcFile, ".xbim");
            var model = new XbimModel();
            // Parse the file and create .xbim
            model.CreateFrom(ifcFile, XbimFile);
        }

        public static void CleanUpModel()
        {
            // TODO - delete xbim
        }

        /// <summary>
        /// Runs before each test to provide a clean context
        /// </summary>
        [TestInitialize]
        public void BeforeTest()
        {
            OpenModel(XbimFile);
        }

        /// <summary>
        /// Runs after each test to explicitly close the model
        /// </summary>
        [TestCleanup]
        public void AfterTest()
        {
            CloseModel();
            // TODO: consider restoring xbim from a fresh version in case changes are made
        }

        /// <summary>
        /// Loads Model with a fresh instance, in the desired access mode
        /// </summary>
        protected void OpenModel(string xBimFile, XbimDBAccess accessMode = XbimDBAccess.Read)
        {
            var model = new XbimModel();
            model.Open(XbimFile, XbimDBAccess.ReadWrite);

            if(Model != null)
            {
                CloseModel();
            }
            Model = model;

        }

        /// <summary>
        /// Closes and disposes of model resources
        /// </summary>
        protected void CloseModel()
        {
            if (Model != null)
            {
                Model.Close();
                Model.Dispose();
            }
        }

    }
}
