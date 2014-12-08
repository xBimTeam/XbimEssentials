using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Xbim.IO
{
    /// <summary>
    /// Class to retrieve the file stream of an IFC file within a ZIP file
    /// </summary>
    public class IfcZipInputStream : IDisposable
    {
        #region Fields

        private Stream _inputFile = null;
        private ZipFile _zipFile = null;
        private ZipInputStream _zipStream = null;
        private FileStream _fileStream = null;
        private String _fileExt;

        #endregion

        #region Properties

        /// <summary>
        /// The file stream for the Ifc file within the zip file
        /// </summary>
        public Stream InputFile
        {
            get { return _inputFile; }
           
        }

        /// <summary>
        /// File extension of the Ifc file held within the zip file
        /// </summary>
        public String FileExt 
        {
            get { return _fileExt; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Create a file stream to the Ifc file held withing a zip file
        /// </summary>
        /// <param name="fileName"> Zip file name to get Ifc file stream from</param>
        public IfcZipInputStream(String fileName)
        {
            string ZipExt = Path.GetExtension(fileName).ToLower();
            if (ZipExt == ".zip" || ZipExt == ".ifczip")
            {
                _fileExt = "";
                _fileStream = File.OpenRead(fileName);
                // used because - The ZipInputStream has one major advantage over using ZipFile to read a zip: 
                // it can read from an unseekable input stream - such as a WebClient download
                _zipStream = new ZipInputStream(_fileStream);      
                ZipEntry entry = _zipStream.GetNextEntry();
                while (entry != null)
                {
                    if ( entry.IsFile && 
                        (entry.Name.ToLower().EndsWith(".ifc") || entry.Name.ToLower().EndsWith(".ifcxml"))
                        )
                    {
                        _zipFile = new ZipFile(fileName);
                        _fileExt = Path.GetExtension(entry.Name).ToLower();
                        _inputFile = _zipFile.GetInputStream(entry);
                        break; // we only want the first file
                    }

                    entry = _zipStream.GetNextEntry(); //get next entry
                }

            }
            else
            {
                throw new Exception("IfcZipInputStream : Not a supported zip file extension");
            }
            // check we found a stream to use, if not file not found exception
            if (_inputFile == null)
            {
                throw new FileNotFoundException("IfcZipInputStream: No Ifc or IfcXml file found in zip file");
            }

        }

        /// <summary>
        /// Close resources
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose by closing any open resources
        /// </summary>
        public void Dispose()
        {
            if (_inputFile != null) _inputFile.Close();
            if (_zipFile != null) 
            {
                _zipFile.IsStreamOwner = true;               // Makes close also shut the underlying stream
                _zipFile.Close();
            }
            //just to make sure
            if (_zipStream != null) _zipStream.Close();             
            if (_fileStream != null) _fileStream.Close();

        }
        #endregion
    }
}
