#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    CodeGenerator.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;

#endregion

namespace Xbim.Ifc2x3.XbimExtensions.ClassMaker
{
    public class CodeGenerator
    {
        public void Generate(string schemaName, string className, string baseType, List<IfcEntityAttribute> attributes)
        {
            // Create a new CodeCompileUnit to contain 
            // the program graph.
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // Declare a new namespace.
            CodeNamespace dpNamespace = new CodeNamespace(schemaName);
            // Add the new namespace to the compile unit.
            compileUnit.Namespaces.Add(dpNamespace);

            // Add the new namespace import for the System namespace.
            dpNamespace.Imports.Add(new CodeNamespaceImport("System"));
            dpNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            dpNamespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
            dpNamespace.Imports.Add(new CodeNamespaceImport("System.Text"));
            dpNamespace.Imports.Add(new CodeNamespaceImport("Xbim.XbimExtensions"));

            //dpNamespace.Imports.Add(new CodeNamespaceImport(ent.Type.Namespace));


            // Declare a new type called Class1.
            CodeTypeDeclaration class1 = new CodeTypeDeclaration(className);
            class1.BaseTypes.Add(baseType);
            // Add the new type to the namespace type collection.
            dpNamespace.Types.Add(class1);

            //class1.BaseTypes.Add("IEnumerable<" + ent.Type.FullName + ">");

            //add fields
            foreach (IfcEntityAttribute attribute in attributes)
            {
                CodeMemberField f = new CodeMemberField();
                f.Name = attribute.FieldName;
                f.Type = new CodeTypeReference(attribute.TypeName);
                f.Attributes = MemberAttributes.Private;
                class1.Members.Add(f);
            }

            foreach (IfcEntityAttribute attribute in attributes)
            {
                CodeMemberProperty p = new CodeMemberProperty();
                p.Name = attribute.PropertyName;
                p.Type = new CodeTypeReference(attribute.TypeName);
                p.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                class1.Members.Add(p);
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();

            // Build the output file name.
            String sourceFile;
            if (provider.FileExtension[0] == '.')
            {
                sourceFile = className + provider.FileExtension;
            }
            else
            {
                sourceFile = className + "." + provider.FileExtension;
            }

            // Create a TextWriter to a StreamWriter to the output file.
            IndentedTextWriter tw = new IndentedTextWriter(new StreamWriter(sourceFile, false), "    ");

            // Generate source code using the code provider.
            provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());

            // Close the output file.
            tw.Close();
            //CodeConstructor cstr = new CodeConstructor();
            //cstr.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("IModel"), "model"));
            //cstr.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_model"), new CodeArgumentReferenceExpression("model")));
            //cstr.Attributes = MemberAttributes.Final | MemberAttributes.Public;
            //class1.Members.Add(cstr);

            ////add the enumerator support
            ////CodeMemberMethod getEnumt = new CodeMemberMethod();
            ////getEnumt.Name = "IEnumerable<" + ent.Type.FullName + ">.GetEnumerator";
            ////getEnumt.ReturnType = new CodeTypeReference("IEnumerator<" + ent.Type.FullName + ">");
            ////getEnumt.Attributes = MemberAttributes.ScopeMask;
            ////getEnumt.Statements.Add(new CodeMethodReturnStatement(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_model"), "Instances.OfType<" + ent.Type.FullName + ">().GetEnumerator()")));
            ////class1.Members.Add(getEnumt);

            ////CodeMemberMethod getEnum = new CodeMemberMethod();
            ////getEnum.Name = "IEnumerable.GetEnumerator";
            ////getEnum.ReturnType = new CodeTypeReference("IEnumerator");
            ////getEnum.Attributes = MemberAttributes.ScopeMask;
            ////getEnum.Statements.Add(new CodeMethodReturnStatement(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_model"), "Instances.OfType<" + ent.Type.FullName + ">().GetEnumerator()")));
            ////class1.Members.Add(getEnum); 


            //CodeMemberProperty items = new CodeMemberProperty();
            //items.Name = "Items";
            //items.Type = new CodeTypeReference("IEnumerable<" + ent.Type.FullName + ">");
            //items.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            //items.GetStatements.Add(new CodeMethodReturnStatement(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_model"), "Instances.OfType<" + ent.Type.FullName + ">()")));

            //class1.Members.Add(items);

            //foreach (var subType in ent.IfcSubTypes.Where(s => !s.Type.IsValueType))
            //{
            //    dpNamespace.Imports.Add(new CodeNamespaceImport(subType.Type.Namespace));
            //    CodeMemberProperty p1 = new CodeMemberProperty();
            //    p1.Name = subType.Type.Name + "s";
            //    p1.Type = new CodeTypeReference(subType.Type.Name + "s");
            //    p1.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            //    p1.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(p1.Type, new CodeArgumentReferenceExpression("_model"))));

            //    class1.Members.Add(p1);
            //}
        }
    }
}