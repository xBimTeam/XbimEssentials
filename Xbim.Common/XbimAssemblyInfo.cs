using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Xbim.Common
{
    /// <summary>
    /// This class can be useful to understand exact version of the xbim assemblies used.
    /// </summary>
    public class XbimAssemblyInfo
    {
        /// <summary>
        /// Returns information suitable to identify the exact version of an xbim assembly
        /// </summary>
        /// <param name="theAssembly">The assembly to analyse</param>
        /// <returns></returns>
        public static string AssemblyInformation(Assembly theAssembly)
        {
            var xa = new XbimAssemblyInfo(theAssembly);
            //if (string.IsNullOrEmpty(theAssembly.Location))
            //    xa.OverrideLocation = MainWindow.GetAssemblyLocation(theAssembly);
            var assemblyDescription = string.Format("{0}\t{1}\t{2}\t{3}\r\n", theAssembly.GetName().Name, xa.AssemblyVersion,
                xa.FileVersion, xa.CompilationTime);
            return assemblyDescription;
        }

        private readonly Assembly _assembly;

        /// <summary>
        /// This property needs to be set when loading the assembly from a stream instead of a specific folder.
        /// </summary>
        public string OverrideLocation;

        public XbimAssemblyInfo(Assembly assembly)
        {
            _assembly = assembly;
        }

        public string AssemblyLocation
        {
            get
            {
                if (!string.IsNullOrEmpty(OverrideLocation))
                    return OverrideLocation;
                return _assembly.Location;
            }
        }

        public XbimAssemblyInfo(Type type)
        {
            _assembly = type.GetTypeInfo().Assembly;
        }

        public Version AssemblyVersion => _assembly.GetName().Version;

        public string FileVersion
        {
            get
            {
                if (string.IsNullOrEmpty(AssemblyLocation))
                    return "";
                var fvi = FileVersionInfo.GetVersionInfo(AssemblyLocation);
                return fvi.FileVersion;
            }
        }

        public FileInfo FileInfo
        {
            get => _assembly.IsDynamic ?
                new FileInfo("") :
                new FileInfo(new Uri(_assembly.CodeBase).LocalPath);
        }

        /// <summary>
        /// Returns the date and time of compilation, extracted from the fileversioninfo according to compilation policy.
        /// It returns DateTime.MinValue when the file properties can not be loaded. In this scenarios OverrideLocation needs to be set.
        /// </summary>
        public DateTime CompilationTime
        {
            get
            {
                var ret = DateTime.MinValue;
                if (FileVersion == null)
                    return ret;
                var versArray = FileVersion.Split(new[] { "." }, StringSplitOptions.None);

                if (versArray.Length != 4)
                    return DateTime.MinValue;
                try
                {
                    // This is a legacy build convention, where we encode the build time into the FileVersion. E.g. 4.1.1802.12124 
                    // Build is YYMM, while patch is DD + 30-second intervals into the day.
                    // In order to get coherent build numbers across projects we've stopped doing this
                    var dateYear = 2000 + Convert.ToInt32(versArray[2].Substring(0, 2));
                    var dateMonth = Convert.ToInt32(versArray[2].Substring(2, 2));
                    var dateday = Convert.ToInt32(versArray[3].Substring(0, 2));
                    var dateMinuteOfDay = Convert.ToInt32(versArray[3].Substring(2)) * 2;
                    var dateMinute = dateMinuteOfDay % 60;
                    var dateHour = dateMinuteOfDay / 60;

                    return new DateTime(dateYear, dateMonth, dateday, dateHour, dateMinute, 0);
                }
                catch (Exception)
                {
                    return FileInfo.CreationTimeUtc;    // Should be accurate if the build outputs are cleaned
                }
            }
        }
    }
}
