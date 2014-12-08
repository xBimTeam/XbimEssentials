#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPostalAddress.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ActorResource
{
    [IfcPersistedEntityAttribute]
    public class PostalAddressCollection : ReadOnlyObservableCollection<IfcPostalAddress>
    {
        internal PostalAddressCollection(ObservableCollection<IfcPostalAddress> paAddresses)
            : base(paAddresses)
        {
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            bool first = true;
            foreach (IfcPostalAddress item in this)
            {
                if (!first) str.AppendLine();
                first = false;
                str.Append(item.ToString());
            }
            return str.ToString();
        }

        [Browsable(true)]
        public string Summary
        {
            get { return this.ToString(); }
        }
    }


    [IfcPersistedEntityAttribute]
    public class IfcPostalAddress : IfcAddress
    {
        #region Fields

        private IfcLabel? _internalLocation;
        private LabelCollection _addressLines;
        private IfcLabel? _postalBox;
        private IfcLabel? _town;
        private IfcLabel? _region;
        private IfcLabel? _postalCode;
        private IfcLabel? _country;

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   Optional. An organization defined address for internal mail delivery.
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLabel? InternalLocation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _internalLocation;
            }
            set
            {
                this.SetModelValue(this, ref _internalLocation, value, v => InternalLocation = v,
                                           "InternalLocation");
            }
        }

        /// <summary>
        ///   Optional. The postal address.
        /// </summary>
        /// <remarks>
        ///   NOTE: A postal address may occupy several lines (or elements) when recorded. It is expected that normal usage will incorporate relevant
        ///   elements of the following address concepts: A location within a building (e.g. 3rd Floor) Building name (e.g. Interoperability House) Street              /// number (e.g. 6400) Street name (e.g. Alliance Boulevard). Typical content of address lines may vary in different countries.
        /// </remarks>
        /// <value>it should be restricted to max. 255 characters.</value>
        [IfcAttribute(5, IfcAttributeState.Optional, IfcAttributeType.List, 1)]
        public LabelCollection AddressLines
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _addressLines;
            }
            set { this.SetModelValue(this, ref _addressLines, value, v => _addressLines = v, "AddressLines"); }
        }


        /// <summary>
        ///   Optional. An address that is implied by an identifiable mail drop.
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcLabel? PostalBox
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _postalBox;
            }
            set
            {
                if (_postalBox != value)
                {
                    this.SetModelValue(this, ref _postalBox, value, v => PostalBox = v, "PostalBox");
                }
            }
        }


        /// <summary>
        ///   Optional. The name of the town
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcLabel? Town
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _town;
            }
            set { this.SetModelValue(this, ref _town, value, v => Town = v, "Town"); }
        }


        /// <summary>
        ///   Optional. The name of a region.
        /// </summary>
        /// <remarks>
        ///   NOTE: The counties of the United Kingdom and the states of North America are examples of regions.
        /// </remarks>
        /// <value>it should be restricted to max. 255 characters.</value>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcLabel? Region
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _region;
            }
            set { this.SetModelValue(this, ref _region, value, v => Region = v, "Region"); }
        }


        /// <summary>
        ///   Optional. The code that is used by the country's postal service.
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcLabel? PostalCode
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _postalCode;
            }
            set { this.SetModelValue(this, ref _postalCode, value, v => PostalCode = v, "PostalCode"); }
        }

        /// <summary>
        ///   Optional. The name of a country.
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcLabel? Country
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _country;
            }
            set { this.SetModelValue(this, ref _country, value, v => Country = v, "Country"); }
        }

        #endregion

        #region Ifc Inverse Relationships

        #endregion

        #region Part 21 Step file Parse routines

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _internalLocation = value.StringVal;
                    break;
                case 4:
                    if (_addressLines == null) _addressLines = new LabelCollection(this);
                    _addressLines.Add(value.StringVal);
                    break;
                case 5:
                    _postalBox = value.StringVal;
                    break;
                case 6:
                    _town = value.StringVal;
                    break;
                case 7:
                    _region = value.StringVal;
                    break;
                case 8:
                    _postalCode = value.StringVal;
                    break;
                case 9:
                    _country = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (string.IsNullOrEmpty(InternalLocation.GetValueOrDefault()) &&
                (AddressLines == null || AddressLines.Count == 0) &&
                string.IsNullOrEmpty(Town.GetValueOrDefault()) &&
                string.IsNullOrEmpty(Region.GetValueOrDefault()) &&
                string.IsNullOrEmpty(Country.GetValueOrDefault()))
                baseErr +=
                    "PostalAddress: At least one attribute of internal location, address lines, town, region or country must be asserted.";
            return baseErr;
        }

        #endregion

        /// <summary>
        ///   Initialise the addresslines list of address lines. Clears any existing lines, further lines can be added through the AddressLine property
        /// </summary>
        public LabelCollection SetAddressLines(params string[] lines)
        {
            if (AddressLines == null) AddressLines = new LabelCollection(this);
            else AddressLines.Clear();
            foreach (string line in lines)
            {
                AddressLines.Add(line.Trim());
            }

            return AddressLines;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            String addrStr = base.ToString();
            if (!string.IsNullOrEmpty(addrStr)) str.AppendLine(addrStr);
            if (!string.IsNullOrEmpty(InternalLocation.GetValueOrDefault()))
                str.AppendLine(InternalLocation.GetValueOrDefault());
            if (!string.IsNullOrEmpty(PostalBox.GetValueOrDefault())) str.AppendLine(PostalBox.GetValueOrDefault());
            if (!string.IsNullOrEmpty(Town.GetValueOrDefault())) str.AppendLine(Town.GetValueOrDefault());
            if (!string.IsNullOrEmpty(Region.GetValueOrDefault())) str.AppendLine(Region.GetValueOrDefault());
            if (!string.IsNullOrEmpty(PostalCode.GetValueOrDefault())) str.AppendLine(PostalCode.GetValueOrDefault());
            if (!string.IsNullOrEmpty(Country.GetValueOrDefault())) str.AppendLine(Country.GetValueOrDefault());
            return str.ToString().Trim();
        }
    }
}
