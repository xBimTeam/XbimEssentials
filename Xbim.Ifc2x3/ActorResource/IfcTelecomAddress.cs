#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTelecomAddress.cs
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
    public class TelecomAddressCollection : ReadOnlyObservableCollection<IfcTelecomAddress>
    {
        internal TelecomAddressCollection(ObservableCollection<IfcTelecomAddress> tcAddresses)
            : base(tcAddresses)
        {
        }


        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            bool first = true;
            foreach (IfcAddress item in this)
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
    public class IfcTelecomAddress : IfcAddress
    {
        #region Fields

        private IfcLabel? _wwwHomePageUrl;
        private LabelCollection _telephoneNumbers;
        private LabelCollection _facsimileNumbers;
        private LabelCollection _electronicMailAddresses;
        private IfcLabel? _pagerNumber;

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   Optional. The list of telephone numbers at which telephone messages may be received.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional, IfcAttributeType.List, 1)]
        public LabelCollection TelephoneNumbers
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _telephoneNumbers;
            }
            set
            {
                this.SetModelValue(this, ref _telephoneNumbers, value, v => _telephoneNumbers = v,
                                           "TelephoneNumbers");
            }
        }


        /// <summary>
        ///   Optional. The list of fax numbers at which fax messages may be received.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional, IfcAttributeType.List, 1)]
        public LabelCollection FacsimileNumbers
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _facsimileNumbers;
            }
            set
            {
                this.SetModelValue(this, ref _facsimileNumbers, value, v => _facsimileNumbers = v,
                                           "FacsimileNumbers");
            }
        }

        /// <summary>
        ///   Optional. The pager number at which paging messages may be received.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcLabel? PagerNumber
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _pagerNumber;
            }
            set { this.SetModelValue(this, ref _pagerNumber, value, v => PagerNumber = v, "PagerNumber"); }
        }


        /// <summary>
        ///   Optional. The list of Email addresses at which Email messages may be received.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional, IfcAttributeType.List, 1)]
        public LabelCollection ElectronicMailAddresses
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _electronicMailAddresses;
            }
            set
            {
                this.SetModelValue(this, ref _electronicMailAddresses, value, v => ElectronicMailAddresses = v,
                                           "ElectronicMailAddresses");
            }
        }

        /// <summary>
        ///   Optional. The world wide web address at which the preliminary page of information for the person or organization can be located.
        /// </summary>
        /// <remarks>
        ///   NOTE: Information on the world wide web for a person or organization may be separated into a number of pages and across a number of host sites, all of which may be linked together. It is assumed that all such information may be referenced from a single page that is termed the home page for that person or organization.
        /// </remarks>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcLabel? WWWHomePageURL
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _wwwHomePageUrl;
            }
            set { this.SetModelValue(this, ref _wwwHomePageUrl, value, v => WWWHomePageURL = v, "WWWHomePageURL"); }
        }

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
                    if (_telephoneNumbers == null) _telephoneNumbers = new LabelCollection(this);
                    _telephoneNumbers.Add(value.StringVal);
                    break;

                case 4:
                    if (_facsimileNumbers == null) _facsimileNumbers = new LabelCollection(this);
                    _facsimileNumbers.Add(value.StringVal);
                    break;
                case 5:
                    _pagerNumber = value.StringVal;
                    break;
                case 6:
                    if (_electronicMailAddresses == null) _electronicMailAddresses = new LabelCollection(this);
                    _electronicMailAddresses.Add(value.StringVal);
                    break;
                case 7:
                    _wwwHomePageUrl = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Inverse Relationships

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if ((_facsimileNumbers == null || _facsimileNumbers.Count == 0 ||
                 string.IsNullOrEmpty(_facsimileNumbers.ToString())) &&
                (_telephoneNumbers == null || _telephoneNumbers.Count == 0 ||
                 string.IsNullOrEmpty(_telephoneNumbers.ToString())) &&
                (_electronicMailAddresses == null || _electronicMailAddresses.Count == 0 ||
                 string.IsNullOrEmpty(_electronicMailAddresses.ToString())) &&
                string.IsNullOrEmpty(PagerNumber.GetValueOrDefault()) &&
                string.IsNullOrEmpty(WWWHomePageURL.GetValueOrDefault()))
                baseErr +=
                    "WR1 TelecomAddress: At least one attribute of facsimile numbers, telephone numbers, electronic mail addresses, pager number or world wide web home page URL must be asserted.";
            return baseErr;
        }

        #endregion

        #region Collection initialisation methods

        /// <summary>
        ///   Initialise the ElectronicMailAddress the list of email addresses. Clears any existing addresses, further lines can be added through the ElectronicMailAddress property.
        /// </summary>
        public LabelCollection SetElectronicMailAddress(params string[] emails)
        {
            if (_electronicMailAddresses == null) _electronicMailAddresses = new LabelCollection(this);
            else _electronicMailAddresses.Clear();
            foreach (string email in emails)
            {
                _electronicMailAddresses.Add(email);
            }
            return _electronicMailAddresses;
        }

        /// <summary>
        ///   Initialise the TelephoneNumbers to the list of numbers. Clears any existing numbers, further lines can be added through the TelephoneNumbers property.
        /// </summary>
        public LabelCollection SetTelephoneNumbers(params string[] numbers)
        {
            if (_telephoneNumbers == null) _telephoneNumbers = new LabelCollection(this);
            else _telephoneNumbers.Clear();
            foreach (string num in numbers)
            {
                _telephoneNumbers.Add(num);
            }
            return _telephoneNumbers;
        }

        /// <summary>
        ///   Initialise the FacsimileNumbers to the list of numbers. Clears any existing numbers, further lines can be added through the FacsimileNumbers property.
        /// </summary>
        public LabelCollection SetFacsimileNumbers(params string[] numbers)
        {
            if (_facsimileNumbers == null) _facsimileNumbers = new LabelCollection(this);
            else _facsimileNumbers.Clear();
            foreach (string num in numbers)
            {
                _facsimileNumbers.Add(num);
            }
            return _facsimileNumbers;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            String addrStr = base.ToString();
            if (_telephoneNumbers != null && _telephoneNumbers.Count > 0) str.AppendLine(_telephoneNumbers.ToString());
            if (_facsimileNumbers != null && _facsimileNumbers.Count > 0) str.AppendLine(_facsimileNumbers.ToString());
            if (_electronicMailAddresses != null && _electronicMailAddresses.Count > 0)
                str.AppendLine(_electronicMailAddresses.ToString());
            if (!string.IsNullOrEmpty(WWWHomePageURL.GetValueOrDefault()))
                str.AppendLine(WWWHomePageURL.GetValueOrDefault());
            if (!string.IsNullOrEmpty(PagerNumber.GetValueOrDefault())) str.AppendLine(PagerNumber.GetValueOrDefault());
            return str.ToString().Trim();
        }
    }
}
