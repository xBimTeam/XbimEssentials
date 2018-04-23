// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Xbim.Common.Step21;
using Xbim.Common;

namespace Xbim.CobieExpress
{
	public sealed class EntityFactory : IEntityFactory
	{
		private readonly System.Reflection.Assembly _assembly;
		
		public EntityFactory()
		{
			_assembly = GetType().Assembly;
		}

		public T New<T>(IModel model, int entityLabel, bool activated) where T: IInstantiableEntity
		{
			return (T)New(model, typeof(T), entityLabel, activated);
		}

		public T New<T>(IModel model, Action<T> init, int entityLabel, bool activated) where T: IInstantiableEntity
		{
			var o = New<T>(model, entityLabel, activated);
			init(o);
			return o;
		}

		public IInstantiableEntity New(IModel model, Type t, int entityLabel, bool activated)
		{
			//check that the type is from this assembly
			if(t.Assembly != _assembly)
				throw new Exception("This factory only creates types from its assembly");

			return New(model, t.Name, entityLabel, activated);
		}

		public IInstantiableEntity New(IModel model, string typeName, int entityLabel, bool activated)
		{
			if (model == null || typeName == null)
				throw new ArgumentNullException();

			var name = typeName.ToUpper();
			switch(name)
			{
				case "COBIEPHASE": return new CobiePhase ( model, entityLabel, activated );
				case "PHASE": return new CobiePhase ( model, entityLabel, activated);
				case "COBIEEXTERNALSYSTEM": return new CobieExternalSystem ( model, entityLabel, activated );
				case "EXTERNALSYSTEM": return new CobieExternalSystem ( model, entityLabel, activated);
				case "COBIEEXTERNALOBJECT": return new CobieExternalObject ( model, entityLabel, activated );
				case "EXTERNALOBJECT": return new CobieExternalObject ( model, entityLabel, activated);
				case "COBIECREATEDINFO": return new CobieCreatedInfo ( model, entityLabel, activated );
				case "CREATEDINFO": return new CobieCreatedInfo ( model, entityLabel, activated);
				case "COBIECONTACT": return new CobieContact ( model, entityLabel, activated );
				case "CONTACT": return new CobieContact ( model, entityLabel, activated);
				case "COBIEFACILITY": return new CobieFacility ( model, entityLabel, activated );
				case "FACILITY": return new CobieFacility ( model, entityLabel, activated);
				case "COBIEPROJECT": return new CobieProject ( model, entityLabel, activated );
				case "PROJECT": return new CobieProject ( model, entityLabel, activated);
				case "COBIESITE": return new CobieSite ( model, entityLabel, activated );
				case "SITE": return new CobieSite ( model, entityLabel, activated);
				case "COBIEFLOOR": return new CobieFloor ( model, entityLabel, activated );
				case "FLOOR": return new CobieFloor ( model, entityLabel, activated);
				case "COBIESPACE": return new CobieSpace ( model, entityLabel, activated );
				case "SPACE": return new CobieSpace ( model, entityLabel, activated);
				case "COBIEZONE": return new CobieZone ( model, entityLabel, activated );
				case "ZONE": return new CobieZone ( model, entityLabel, activated);
				case "COBIETYPE": return new CobieType ( model, entityLabel, activated );
				case "TYPE": return new CobieType ( model, entityLabel, activated);
				case "COBIECOMPONENT": return new CobieComponent ( model, entityLabel, activated );
				case "COMPONENT": return new CobieComponent ( model, entityLabel, activated);
				case "COBIESYSTEM": return new CobieSystem ( model, entityLabel, activated );
				case "SYSTEM": return new CobieSystem ( model, entityLabel, activated);
				case "COBIECONNECTION": return new CobieConnection ( model, entityLabel, activated );
				case "CONNECTION": return new CobieConnection ( model, entityLabel, activated);
				case "COBIESPARE": return new CobieSpare ( model, entityLabel, activated );
				case "SPARE": return new CobieSpare ( model, entityLabel, activated);
				case "COBIERESOURCE": return new CobieResource ( model, entityLabel, activated );
				case "RESOURCE": return new CobieResource ( model, entityLabel, activated);
				case "COBIEJOB": return new CobieJob ( model, entityLabel, activated );
				case "JOB": return new CobieJob ( model, entityLabel, activated);
				case "COBIEIMPACT": return new CobieImpact ( model, entityLabel, activated );
				case "IMPACT": return new CobieImpact ( model, entityLabel, activated);
				case "COBIEDOCUMENT": return new CobieDocument ( model, entityLabel, activated );
				case "DOCUMENT": return new CobieDocument ( model, entityLabel, activated);
				case "COBIEATTRIBUTE": return new CobieAttribute ( model, entityLabel, activated );
				case "ATTRIBUTE": return new CobieAttribute ( model, entityLabel, activated);
				case "COBIEISSUE": return new CobieIssue ( model, entityLabel, activated );
				case "ISSUE": return new CobieIssue ( model, entityLabel, activated);
				case "COBIECOORDINATE": return new CobieCoordinate ( model, entityLabel, activated );
				case "COORDINATE": return new CobieCoordinate ( model, entityLabel, activated);
				case "COBIECATEGORY": return new CobieCategory ( model, entityLabel, activated );
				case "CATEGORY": return new CobieCategory ( model, entityLabel, activated);
				case "COBIECLASSIFICATION": return new CobieClassification ( model, entityLabel, activated );
				case "CLASSIFICATION": return new CobieClassification ( model, entityLabel, activated);
				case "COBIEROLE": return new CobieRole ( model, entityLabel, activated );
				case "ROLE": return new CobieRole ( model, entityLabel, activated);
				case "COBIELINEARUNIT": return new CobieLinearUnit ( model, entityLabel, activated );
				case "LINEARUNIT": return new CobieLinearUnit ( model, entityLabel, activated);
				case "COBIEAREAUNIT": return new CobieAreaUnit ( model, entityLabel, activated );
				case "AREAUNIT": return new CobieAreaUnit ( model, entityLabel, activated);
				case "COBIEVOLUMEUNIT": return new CobieVolumeUnit ( model, entityLabel, activated );
				case "VOLUMEUNIT": return new CobieVolumeUnit ( model, entityLabel, activated);
				case "COBIECURRENCYUNIT": return new CobieCurrencyUnit ( model, entityLabel, activated );
				case "CURRENCYUNIT": return new CobieCurrencyUnit ( model, entityLabel, activated);
				case "COBIEDURATIONUNIT": return new CobieDurationUnit ( model, entityLabel, activated );
				case "DURATIONUNIT": return new CobieDurationUnit ( model, entityLabel, activated);
				case "COBIEASSETTYPE": return new CobieAssetType ( model, entityLabel, activated );
				case "ASSETTYPE": return new CobieAssetType ( model, entityLabel, activated);
				case "COBIECONNECTIONTYPE": return new CobieConnectionType ( model, entityLabel, activated );
				case "CONNECTIONTYPE": return new CobieConnectionType ( model, entityLabel, activated);
				case "COBIESPARETYPE": return new CobieSpareType ( model, entityLabel, activated );
				case "SPARETYPE": return new CobieSpareType ( model, entityLabel, activated);
				case "COBIERESOURCETYPE": return new CobieResourceType ( model, entityLabel, activated );
				case "RESOURCETYPE": return new CobieResourceType ( model, entityLabel, activated);
				case "COBIEJOBTYPE": return new CobieJobType ( model, entityLabel, activated );
				case "JOBTYPE": return new CobieJobType ( model, entityLabel, activated);
				case "COBIEJOBSTATUSTYPE": return new CobieJobStatusType ( model, entityLabel, activated );
				case "JOBSTATUSTYPE": return new CobieJobStatusType ( model, entityLabel, activated);
				case "COBIEIMPACTTYPE": return new CobieImpactType ( model, entityLabel, activated );
				case "IMPACTTYPE": return new CobieImpactType ( model, entityLabel, activated);
				case "COBIEIMPACTSTAGE": return new CobieImpactStage ( model, entityLabel, activated );
				case "IMPACTSTAGE": return new CobieImpactStage ( model, entityLabel, activated);
				case "COBIEIMPACTUNIT": return new CobieImpactUnit ( model, entityLabel, activated );
				case "IMPACTUNIT": return new CobieImpactUnit ( model, entityLabel, activated);
				case "COBIEDOCUMENTTYPE": return new CobieDocumentType ( model, entityLabel, activated );
				case "DOCUMENTTYPE": return new CobieDocumentType ( model, entityLabel, activated);
				case "COBIESTAGETYPE": return new CobieStageType ( model, entityLabel, activated );
				case "STAGETYPE": return new CobieStageType ( model, entityLabel, activated);
				case "COBIEAPPROVALTYPE": return new CobieApprovalType ( model, entityLabel, activated );
				case "APPROVALTYPE": return new CobieApprovalType ( model, entityLabel, activated);
				case "COBIEISSUETYPE": return new CobieIssueType ( model, entityLabel, activated );
				case "ISSUETYPE": return new CobieIssueType ( model, entityLabel, activated);
				case "COBIEISSUECHANCE": return new CobieIssueChance ( model, entityLabel, activated );
				case "ISSUECHANCE": return new CobieIssueChance ( model, entityLabel, activated);
				case "COBIEISSUERISK": return new CobieIssueRisk ( model, entityLabel, activated );
				case "ISSUERISK": return new CobieIssueRisk ( model, entityLabel, activated);
				case "COBIEISSUEIMPACT": return new CobieIssueImpact ( model, entityLabel, activated );
				case "ISSUEIMPACT": return new CobieIssueImpact ( model, entityLabel, activated);
				default:
					return null;
			}
		}
		public IInstantiableEntity New(IModel model, int typeId, int entityLabel, bool activated)
		{
			if (model == null)
				throw new ArgumentNullException();

			switch(typeId)
			{
				case 7: return new CobiePhase ( model, entityLabel, activated );
				case 8: return new CobieExternalSystem ( model, entityLabel, activated );
				case 9: return new CobieExternalObject ( model, entityLabel, activated );
				case 10: return new CobieCreatedInfo ( model, entityLabel, activated );
				case 12: return new CobieContact ( model, entityLabel, activated );
				case 14: return new CobieFacility ( model, entityLabel, activated );
				case 15: return new CobieProject ( model, entityLabel, activated );
				case 16: return new CobieSite ( model, entityLabel, activated );
				case 17: return new CobieFloor ( model, entityLabel, activated );
				case 18: return new CobieSpace ( model, entityLabel, activated );
				case 19: return new CobieZone ( model, entityLabel, activated );
				case 21: return new CobieType ( model, entityLabel, activated );
				case 23: return new CobieComponent ( model, entityLabel, activated );
				case 24: return new CobieSystem ( model, entityLabel, activated );
				case 25: return new CobieConnection ( model, entityLabel, activated );
				case 26: return new CobieSpare ( model, entityLabel, activated );
				case 27: return new CobieResource ( model, entityLabel, activated );
				case 28: return new CobieJob ( model, entityLabel, activated );
				case 29: return new CobieImpact ( model, entityLabel, activated );
				case 30: return new CobieDocument ( model, entityLabel, activated );
				case 31: return new CobieAttribute ( model, entityLabel, activated );
				case 32: return new CobieIssue ( model, entityLabel, activated );
				case 33: return new CobieCoordinate ( model, entityLabel, activated );
				case 36: return new CobieCategory ( model, entityLabel, activated );
				case 37: return new CobieClassification ( model, entityLabel, activated );
				case 38: return new CobieRole ( model, entityLabel, activated );
				case 39: return new CobieLinearUnit ( model, entityLabel, activated );
				case 40: return new CobieAreaUnit ( model, entityLabel, activated );
				case 41: return new CobieVolumeUnit ( model, entityLabel, activated );
				case 42: return new CobieCurrencyUnit ( model, entityLabel, activated );
				case 43: return new CobieDurationUnit ( model, entityLabel, activated );
				case 44: return new CobieAssetType ( model, entityLabel, activated );
				case 45: return new CobieConnectionType ( model, entityLabel, activated );
				case 46: return new CobieSpareType ( model, entityLabel, activated );
				case 47: return new CobieResourceType ( model, entityLabel, activated );
				case 48: return new CobieJobType ( model, entityLabel, activated );
				case 49: return new CobieJobStatusType ( model, entityLabel, activated );
				case 50: return new CobieImpactType ( model, entityLabel, activated );
				case 51: return new CobieImpactStage ( model, entityLabel, activated );
				case 52: return new CobieImpactUnit ( model, entityLabel, activated );
				case 53: return new CobieDocumentType ( model, entityLabel, activated );
				case 54: return new CobieStageType ( model, entityLabel, activated );
				case 55: return new CobieApprovalType ( model, entityLabel, activated );
				case 56: return new CobieIssueType ( model, entityLabel, activated );
				case 57: return new CobieIssueChance ( model, entityLabel, activated );
				case 58: return new CobieIssueRisk ( model, entityLabel, activated );
				case 59: return new CobieIssueImpact ( model, entityLabel, activated );
				default:
					return null;
			}
		}

		public IExpressValueType New(string typeName)
		{
		if (typeName == null)
				throw new ArgumentNullException();

			var name = typeName.ToUpper();
			switch(name)
			{
				case "STRINGVALUE": return new StringValue ();
				case "INTEGERVALUE": return new IntegerValue ();
				case "FLOATVALUE": return new FloatValue ();
				case "BOOLEANVALUE": return new BooleanValue ();
				case "DATETIMEVALUE": return new DateTimeValue ();
				default:
					return null;
			}
		}

		private static readonly List<string> _schemasIds = new List<string> { "COBIE_EXPRESS" };
		public IEnumerable<string> SchemasIds { get { return _schemasIds; } }

		/// <summary>
        /// Gets the Ifc Schema version of the model if this is IFC schema
        /// </summary>
		public IfcSchemaVersion SchemaVersion { 
			get
			{
				return IfcSchemaVersion.Cobie2X4;
			}
		}

	}
}
