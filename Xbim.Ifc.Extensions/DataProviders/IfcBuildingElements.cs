#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBuildingElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.ProductExtension;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcBuildingElements
    {
        private readonly IModel _model;

        public IfcBuildingElements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcBuildingElement> Items
        {
            get { return this._model.Instances.OfType<IfcBuildingElement>(); }
        }

        public IfcRoofs IfcRoofs
        {
            get { return new IfcRoofs(_model); }
        }

        public IfcRampFlights IfcRampFlights
        {
            get { return new IfcRampFlights(_model); }
        }

        public IfcRamps IfcRamps
        {
            get { return new IfcRamps(_model); }
        }

        public IfcRailings IfcRailings
        {
            get { return new IfcRailings(_model); }
        }

        public IfcMembers IfcMembers
        {
            get { return new IfcMembers(_model); }
        }

        public IfcWalls IfcWalls
        {
            get { return new IfcWalls(_model); }
        }

        public IfcBuildingElementProxys IfcBuildingElementProxys
        {
            get { return new IfcBuildingElementProxys(_model); }
        }

        public IfcCurtainWalls IfcCurtainWalls
        {
            get { return new IfcCurtainWalls(_model); }
        }

        public IfcCoverings IfcCoverings
        {
            get { return new IfcCoverings(_model); }
        }

        public IfcColumns IfcColumns
        {
            get { return new IfcColumns(_model); }
        }

        public IfcFootings IfcFootings
        {
            get { return new IfcFootings(_model); }
        }

        public IfcBuildingElementComponents IfcBuildingElementComponents
        {
            get { return new IfcBuildingElementComponents(_model); }
        }

        public IfcStairs IfcStairs
        {
            get { return new IfcStairs(_model); }
        }

        public IfcPiles IfcPiles
        {
            get { return new IfcPiles(_model); }
        }

        public IfcPlates IfcPlates
        {
            get { return new IfcPlates(_model); }
        }

        public IfcSlabs IfcSlabs
        {
            get { return new IfcSlabs(_model); }
        }

        public IfcBeams IfcBeams
        {
            get { return new IfcBeams(_model); }
        }

        public IfcStairFlights IfcStairFlights
        {
            get { return new IfcStairFlights(_model); }
        }

        public IfcWindows IfcWindows
        {
            get { return new IfcWindows(_model); }
        }

        public IfcDoors IfcDoors
        {
            get { return new IfcDoors(_model); }
        }
    }
}