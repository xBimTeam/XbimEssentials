#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    Log4NetProvider.cs
// (See accompanying copyright.rtf)

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Xbim.Common.Helpers;

namespace Xbim.Common.Logging.Providers
{

	/// <summary>
	/// Provides advanced logging capabilities using log4net through the <see cref="Log4NetLogger"/> Logger.
	/// </summary>
	/// <remarks>See http://logging.apache.org/log4net/release/manual/introduction.html for more on log4net logging.</remarks>
	internal class Log4NetProvider : ILoggingProvider
	{

		#region ILoggingProvider Members

		/// <summary>
		/// Configures the log4Net environment for first use.
		/// </summary>
		public void Configure()
		{
			Initialise();
            // Set up some default properties we can use to provide consistent log
            // file naming conventions. 
            // Skip if already set - GlobalContext is global and Properties may be set by another part of the application. Don't over-write
            if (log4net.GlobalContext.Properties["LogName"] == null)
            {
                log4net.GlobalContext.Properties["LogName"] = Path.Combine(LogPath, LogFileName);
                log4net.GlobalContext.Properties["ApplicationName"] = ApplicationName;
            }
			XmlConfigurator.Configure();

		}

		/// <summary>
		/// Gets the <see cref="ILogger"/> applicable for this <see cref="T:System.Type"/>.
		/// </summary>
		/// <param name="callingType">The type.</param>
		/// <remarks>Logging consumers provider a Type to this call so that the Logging Provider
		/// can customise the logger dynamically for the Type. More advanced logging systems, such 
		/// as log4Net can use this to provide different logging levels and outputs for different
		/// parts of the application.</remarks>
		/// <returns>An <see cref="ILogger"/> for this Type.</returns>
		public ILogger GetLogger(Type callingType)
		{
			return new Log4NetLogger(callingType);
		}
		#endregion

		/// <summary>
		/// Gets the log path.
		/// </summary>
		/// <value>The log path.</value>
		public string LogPath
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the name of the log file.
		/// </summary>
		/// <value>The name of the log file.</value>
		public string LogFileName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		/// <value>The name of the application.</value>
		public string ApplicationName
		{
			get;
			private set;
		}

		private void Initialise()
		{
			Assembly mainAssembly = GetAssembly();

			String company = GetCompany(mainAssembly);
			String product = GetProductName(mainAssembly);
			String path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

			path = Path.Combine(path, company);
			path = Path.Combine(path, product);

			if(!Directory.Exists(path))
			{
				try
				{
					Directory.CreateDirectory(path);
				}
				catch (SystemException) { }
			}

			LogPath = path;

			ApplicationName = mainAssembly.GetName().Name;
			LogFileName = ApplicationName;
		}

		private Assembly GetAssembly()
		{
			// This will work for classic .NET Executables...
			Assembly mainAssembly = Assembly.GetEntryAssembly();

			// When we are hosted under an un-managed process we have to work harder to get a path. 
			// e.g. MMC.exe or Excel.exe
			// This also works for apps hosted under IIS etc, but results can be unexpected,
			// since the entry method is often in the App_Code.dll which has limited meta data.
			if (mainAssembly == null)
			{
				try
				{
					System.Diagnostics.StackTrace stack =  new System.Diagnostics.StackTrace();
					// HACK: 6 stackframes up is typically the caller into the ILogger.
					mainAssembly = stack.GetFrame(6).GetMethod().DeclaringType.Assembly;
				}
				catch
				{ }
			}
			if (mainAssembly == null)
			{
				// Default to this assembly.
				mainAssembly = Assembly.GetExecutingAssembly();
			}

			return mainAssembly;
		}

		private string GetCompany(Assembly assembly)
		{
			String companyName = String.Empty;
			AssemblyCompanyAttribute companyAttr = AttributeHelper.GetAttribute<AssemblyCompanyAttribute>(assembly, true);

			if (companyAttr != null)
			{
				companyName = companyAttr.Company;
			}
			return companyName;
		}

		private string GetProductName(Assembly assembly)
		{
			String productName = String.Empty;
            AssemblyProductAttribute productAttr = AttributeHelper.GetAttribute<AssemblyProductAttribute>(assembly, true);

			if (productAttr != null)
			{
				productName = productAttr.Product;
			}
			return productName;
		}




        public string AttachMemoryLogger()
        {
            // Set up a memory logger against the root, to intercept at All Levels

            String name = String.Format("MA{0}", DateTime.UtcNow);
            MemoryAppender memoryAppender = new MemoryAppender();
            memoryAppender.Name = name;
            memoryAppender.Layout = new PatternLayout("%date{dd-MM-yyyy HH:mm:ss,fff} %5level [%2thread] %message (%logger{1}:%line)%n");
            memoryAppender.Threshold = Level.All;
            memoryAppender.ActivateOptions();

            Logger root = LoggingHierarchy.Root;
            root.AddAppender(memoryAppender);
            root.Repository.Configured = true;

            return name;
        }

        public void DetatchMemoryLogger(string name)
        {
            try
            {
                MemoryAppender appender = GetMemoryAppender(name);
                LoggingHierarchy.Root.RemoveAppender(appender);
                appender.Clear();
            }
            catch(Exception)
            {
                // Ignore
            }
        }

        private MemoryAppender GetMemoryAppender(String name)
        {
            MemoryAppender appender = LoggingHierarchy.Root.GetAppender(name) as MemoryAppender;
            if (appender == null)
            {
                throw new InvalidOperationException(String.Format("Could not locate memory appender named '{0}'", name));
            }
            return appender;
        }

        private Hierarchy LoggingHierarchy
        {
            get
            {
                if (_hierarchy == null)
                {
                    _hierarchy = LogManager.GetRepository() as Hierarchy;
                }
                return _hierarchy;
            }

        }

        private Hierarchy _hierarchy;


        public List<Event> GetEvents(string name)
        {

            var results = from e in GetMemoryAppender(name).GetEvents()
                          select new Event
                          {
                              EventTime = e.TimeStamp,
                              EventLevel = (EventLevel)Enum.Parse(typeof(EventLevel), e.Level.ToString()),
                              Message = e.RenderedMessage,
                              User = e.Identity,
                              Logger = e.LoggerName,
                              Method = e.LocationInformation.MethodName
                          };

            return results.ToList();
        }
    }
}
