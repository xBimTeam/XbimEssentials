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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Esent;

#endregion

namespace Xbim.IO.Step21
{
    public class Part21FileWriter 
    {
        public void Write(IModel model, TextWriter output, ExpressMetaData metadata, IDictionary<int, int> map = null)
        {
            output.Write(HeaderAsString(model.Header ?? new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults)));
            foreach (var entity in model.Instances)
                entity.WriteEntity(output, metadata, map);

            output.WriteLine("ENDSEC;");
            output.WriteLine("END-ISO-10303-21;");
        }

        public void Write(EsentModel model, TextWriter output, ExpressMetaData metadata, IDictionary<int, int> map = null)
        {
            output.Write(HeaderAsString(
                model.Header ?? 
                new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults)
                {
                    FileSchema = new StepFileSchema(model.Factory.SchemasIds.FirstOrDefault())
                }));
            foreach (var item in model.InstanceHandles /*.Types.OrderBy(t=>t.Name)*/)
            {
                var entity = model.GetInstanceVolatile(item);
                entity.WriteEntity(output, metadata, map);
            }

            output.WriteLine("ENDSEC;");
            output.WriteLine("END-ISO-10303-21;");
        }


        private string HeaderAsString(IStepFileHeader header)
        {
            var headerStr = new StringBuilder();
            headerStr.AppendLine("ISO-10303-21;");
            headerStr.AppendLine("HEADER;");
            //FILE_DESCRIPTION
            headerStr.Append("FILE_DESCRIPTION ((");
            var i = 0;

            if (header.FileDescription.Description.Count == 0)
            {
                headerStr.Append(@"''");
            }
            else
            {
                foreach (var item in header.FileDescription.Description)
                {
                    headerStr.AppendFormat(@"{0}'{1}'", i == 0 ? "" : ",", item.ToPart21());
                    i++;
                }
            }
            headerStr.AppendFormat(@"), '{0}');", header.FileDescription.ImplementationLevel);
            headerStr.AppendLine();
            //FileName
            headerStr.Append("FILE_NAME (");
            headerStr.AppendFormat(@"'{0}'", (header.FileName !=null && header.FileName.Name!=null)? header.FileName.Name.ToPart21():"");
            headerStr.AppendFormat(@", '{0}'", header.FileName !=null? header.FileName.TimeStamp:"");
            headerStr.Append(", (");
            i = 0;
            if (header.FileName != null && header.FileName.AuthorName.Count == 0)
                headerStr.Append(@"''");
            else
            {
                if (header.FileName != null)
                    foreach (var item in header.FileName.AuthorName)
                    {
                        headerStr.AppendFormat(@"{0}'{1}'", i == 0 ? "" : ",", item.ToPart21());
                        i++;
                    }
            }
            headerStr.Append("), (");
            i = 0;
            if (header.FileName != null && header.FileName.Organization.Count == 0)
                headerStr.Append(@"''");
            else
            {
                if (header.FileName != null)
                    foreach (var item in header.FileName.Organization)
                    {
                        headerStr.AppendFormat(@"{0}'{1}'", i == 0 ? "" : ",", item.ToPart21());
                        i++;
                    }
            }
            if (header.FileName != null)
                headerStr.AppendFormat(@"), '{0}', '{1}', '{2}');", header.FileName.PreprocessorVersion.ToPart21(), header.FileName.OriginatingSystem.ToPart21(),
                    header.FileName.AuthorizationName.ToPart21());
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