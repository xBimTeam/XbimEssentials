#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    StepP21Entity.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;

#endregion

namespace Xbim.IO.Step21.Parser
{
    public class StepP21Entity
    {
        private int _id;
        private string _name;
        private string _parameters;
        private object _instance;
        private int _parseIndex;
        private readonly List<object> _propValues = new List<object>();

        public StepP21Entity()
        {
        }

        public StepP21Entity(int id, string name, string parameters)
        {
            _id = id;
            _name = name;
            _parameters = parameters;
        }

        /// <summary>
        ///   The identity number in the STEP P21 file
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        ///   The name of the Entity Class
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        ///   The unparsed string of parameters
        /// </summary>
        public string Params
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        /// <summary>
        ///   The Ifc Model Instance
        /// </summary>
        public object Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return (this.ID == ((StepP21Entity) obj).ID);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return _id;
        }

        /// <summary>
        ///   Sets the property at curent parseindex of the curent entity
        /// </summary>
        public void SetProperty(object val)
        {
            _propValues.Add(val);
            _parseIndex++;
        }
    }
}