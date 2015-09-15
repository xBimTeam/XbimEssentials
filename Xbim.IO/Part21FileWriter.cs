#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    Part21FileWriter.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.Interfaces;

using Xbim.XbimExtensions;
using Xbim.Common.Exceptions;

#endregion

namespace Xbim.IO
{
    public class Part21FileWriter 
    {
        private HashSet<long> _written;

        public void Write(XbimModel model, TextWriter output, IDictionary<int, int> map = null)
        {
            _written = new HashSet<long>();
            output.Write(HeaderAsString(model.Header ?? new IfcFileHeader(IfcFileHeader.HeaderCreationMode.InitWithXbimDefaults)));
            foreach (XbimInstanceHandle item in model.InstanceHandles /*.Types.OrderBy(t=>t.Name)*/)
            {
                try
                {
                    IPersistIfcEntity entity = model.GetInstanceVolatile(item);
                entity.WriteEntity(output, map);
                }
                catch (Exception e)
                {
                    
                    throw;
                }
               
            }

            output.WriteLine("ENDSEC;");
            output.WriteLine("END-ISO-10303-21;");
        }


        private string HeaderAsString(IIfcFileHeader header)
        {
            StringBuilder headerStr = new StringBuilder();
            headerStr.AppendLine("ISO-10303-21;");
            headerStr.AppendLine("HEADER;");
            //FILE_DESCRIPTION
            headerStr.Append("FILE_DESCRIPTION ((");
            int i = 0;

            if (header.FileDescription.Description.Count == 0)
            {
                headerStr.Append(@"''");
            }
            else
            {
                foreach (string item in header.FileDescription.Description)
                {
                    headerStr.AppendFormat(@"{0}'{1}'", i == 0 ? "" : ",", IfcText.Escape(item));
                    i++;
                }
            }
            headerStr.AppendFormat(@"), '{0}');", header.FileDescription.ImplementationLevel);
            headerStr.AppendLine();
            //FileName
            headerStr.Append("FILE_NAME (");
            headerStr.AppendFormat(@"'{0}'", (header.FileName !=null && header.FileName.Name!=null)? IfcText.Escape(header.FileName.Name):"");
            headerStr.AppendFormat(@", '{0}'", header.FileName !=null? header.FileName.TimeStamp:"");
            headerStr.Append(", (");
            i = 0;
            if (header.FileName.AuthorName.Count == 0)
                headerStr.Append(@"''");
            else
            {
                foreach (string item in header.FileName.AuthorName)
                {
                    headerStr.AppendFormat(@"{0}'{1}'", i == 0 ? "" : ",", IfcText.Escape(item));
                    i++;
                }
            }
            headerStr.Append("), (");
            i = 0;
            if (header.FileName.Organization.Count == 0)
                headerStr.Append(@"''");
            else
            {
                foreach (string item in header.FileName.Organization)
                {
                    headerStr.AppendFormat(@"{0}'{1}'", i == 0 ? "" : ",", IfcText.Escape(item));
                    i++;
                }
            }
            headerStr.AppendFormat(@"), '{0}', '{1}', '{2}');", IfcText.Escape(header.FileName.PreprocessorVersion), IfcText.Escape(header.FileName.OriginatingSystem),
                                IfcText.Escape(header.FileName.AuthorizationName));
            headerStr.AppendLine();
            //FileSchema
            headerStr.AppendFormat("FILE_SCHEMA (('{0}'));", header.FileSchema.Schemas.FirstOrDefault());
            headerStr.AppendLine();
            headerStr.AppendLine("ENDSEC;");
            headerStr.AppendLine("DATA;");
            return headerStr.ToString();
        }
        
    }
}