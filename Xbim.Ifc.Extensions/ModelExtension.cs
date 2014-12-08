using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.UtilityResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ModelExtension
    {
        //public static void GenerateNoChangeOwnerHistoryForAll(this IModel model)
        //{
        //    IEnumerable<IfcRoot> instances = model.Instances.OfType<IfcRoot>();
        //    foreach (var instance in instances)
        //    {
        //        //create new object of owner history
        //        instance.OwnerHistory = GetNewOwnerHistory(model, IfcChangeActionEnum.NOCHANGE);
        //    }
        //}

        //private static IfcOwnerHistory GetNewOwnerHistory(IModel model, IfcChangeActionEnum changeAction)
        //{
        //    //existing default owner history
        //    IfcOwnerHistory defOwner = model.OwnerHistoryAddObject;
        //    IfcTimeStamp stamp = IfcTimeStamp.ToTimeStamp(DateTime.Now);

        //    //return new object
        //    return model.Instances.New<IfcOwnerHistory>(h => { h.OwningUser = defOwner.OwningUser; h.OwningApplication = defOwner.OwningApplication; h.CreationDate = stamp; h.ChangeAction = changeAction; });
        //}

        //public static IEnumerable<IfcRoot> GetNewOrChangedObjects(this IModel model)
        //{
        //    return model.Instances.Where<IfcRoot>(r => r.OwnerHistory.ChangeAction == IfcChangeActionEnum.MODIFIED || r.OwnerHistory.ChangeAction == IfcChangeActionEnum.MODIFIEDADDED || r.OwnerHistory.ChangeAction == IfcChangeActionEnum.ADDED);
        //}

        //public static void TryToCreateUndefinedElementTypes(this IModel model)
        //{
 
        //}

        //public static void GenerateMaterialVolumeTables(this IModel model)
        //{
 
        //}


    }
}
