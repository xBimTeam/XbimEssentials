# Changelog

All notable changes to this project should be documented in this file

## [v5.1.527] 2019-05-30

This is the 5.1 release of XBIM

### Changed
- `IXbimGeometryEngine.CreateSurfaceModel` interface changed to use abstract Face Set
- Allow IModelProviders to overide EntityFactory

### Added
- Re-introduced method to parse header without loading whole file [#229](https://github.com/xBimTeam/XbimEssentials/issues/229) 
- `EsentModelProvider` allows filename to be specified
- Support for IfcPolygonalFaceSet added to tessellation
- Added separate .netcore unit test project
- Enumerator support added to XbimCartesianPointList3D

### Removed

### Fixed
- `IfcStore` preserves Filename from model
- Esent Model not removed when filename specified
- Ifc4 'CorrectObjectAssignment' validation function fixed 
- Fix for .netcore XML deserialisation [#247](https://github.com/xBimTeam/XbimEssentials/issues/247)
- Fix for infinite loop in Scanner with bad EOF [#249](https://github.com/xBimTeam/XbimEssentials/issues/249)
- Spelling corrections in IfcStore Xml Docs [#250](https://github.com/xBimTeam/XbimEssentials/issues/250)
- Better handling / reporting of duplicate entity labels
- Improved handling of invalid Enums in parser
- Handle ambiguous specification of PnIndex in IfcTriangulatedFaceSet [XbimGeometry#167](https://github.com/xBimTeam/XbimGeometry/issues/167)
- `IfcStore` supports STP and STPZIP files.
- Opening ifcXml file in ifcZip failed when using progress delegate [#258](https://github.com/xBimTeam/XbimEssentials/issues/258)


## [v5.0.213] 2018-12-17 Nuget Release

The first official suite of Xbim v5 packages!

### Changed
- `IfcValidator` moved from `Xbim.Ifc` to `Xbim.Common` and renamed `Validator`

## [v5.0.204] 2018-12-11

### Changed
- The base .NET Framework target has been raised from `net45` to `net47`. This is to enable .NET Standard [#213](https://github.com/xBimTeam/XbimEssentials/issues/213)
- XBIMEssentials is now a 'meta package' which brings in the individual packages. **Important**: To avoid a [failed upgrade (#233)](https://github.com/xBimTeam/XbimEssentials/issues/233) 
make sure you update your `net4x` projects to at least `net47` **before** you upgrade the Essentials package. 
- All project major projects now use the new 'SDK' style *csproj* format [#216](https://github.com/xBimTeam/XbimEssentials/issues/216)
- All tests consolidated into *Xbim.Essentials.Tests*
- `XbimDBAccess` has moved from `Xbim.IO.Esent` namespace to `Xbim.IO` (in *Xbim.Common*)
- `XbimStoreType` has moved from `Xbim.IO.Esent` namespace to `Xbim.IO` (in *Xbim.Common*)
- `IfcSchemaVersion` has been renamed `XbimSchemaVersion`
- `IfcStorageType` has been renamed `StorageType`
- *Xbim.Ifc* no longer references *Xbim.IO.Esent* in order to support netstandard, which required decoupling the Store from specific IModel implementations. **See [#228](https://github.com/xBimTeam/XbimEssentials/issues/227)**
  - **BREAKING CHANGE**: Windows forms and Console apps using *IfcStore* must now call `IfcStore.ModelProviderFactory.UseHeuristicModelProvider();` at application startup (or at least before using *IfcStore.Open()*)
  - If you don't configure the `IfcStore` to do this, you will likely result in use of the very basic `MemoryModel` implementation which does not support *.xbim* files
  - ASP.NET [Core] consumers should not need to do this, but no harm in configuring explicitly.
- IfcStore methods factored out to Extension methods: *InsertCopy*, *SaveAsIfc/IfcXml/IfcZip*, *SaveAsWexbim*
- Assemblies code signed again - early versions of v5 removed this. [#225](https://github.com/xBimTeam/XbimEssentials/issues/225)

### Added
- Support for IFC4x1 and IFC4 Addendum 2 [#177](https://github.com/xBimTeam/XbimEssentials/issues/177)
- Support for building against `netstandard2.0` for all Essentials components (except for `Xbim.IO.Esent`). 
This should enable the use of XBIM in .NET Core apps and other targets, as well as providing cross-platform support. [#213](https://github.com/xBimTeam/XbimEssentials/issues/213)
- Support for *Microsoft.Extensions.Logging* and 3rd party logging providers. (See *log4net* note below) [#214](https://github.com/xBimTeam/XbimEssentials/issues/214)
  - `Xbim.Common.XbimLogging` added as a host for *ILoggerFactory*
  - [Usage Example 1](https://github.com/xBimTeam/XbimExchange/blob/60f4d0489042fe46f7cccef515d633b861223bb2/Xbim.Exchange/Program.cs#L252): Using the standard [Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console/)
  - [Usage Example 2](https://github.com/xBimTeam/XbimWindowsUI/blob/5557cf841670aee7d4f8d902ca25e0a43004b491/XbimXplorer/XplorerMainWindow.xaml.cs#L116): Using [Serilog](https://serilog.net/) with XBim
- Support for Visual Studio 2017
- Support for partial files - selective scanning of part of the model
- Inverse relation caching and entity caching
- Ability to skip certain entity types in the Step21 scanner/ MemoryModel
- Support for IFC files with badly specified CodePages [#120](https://github.com/xBimTeam/XbimEssentials/issues/120)

### Removed

- `XBim.Ifc2x3.IO` assembly has been removed, including `XbimModel`. Please use `Xbim.Ifc.IfcStore` instead
- `Xbim.CobieExpress.*` and `XBim.IO.TableStore` have been moved to a [new dedicated repo](https://github.com/xBimTeam/XbimCobieExpress)
- `Xbim.Ifc.Extensions` has been dropped due to the logic only targeting IFC2x3 interfaces and the subjective and potentially ambiguous implementation logic.
We recommend you build your own or reference it from our [old source](https://github.com/xBimTeam/XbimEssentials/tree/a3787e2c5da268543e480c6f5fe16279787c7449/Xbim.Ifc.Extensions).
PRs for a new repo are welcome!
- *log4net* logging has been removed. We now use `Microsoft.Extensions.Logging` abstraction interfaces, meaning you can use the 
[logging framework of your choice](https://github.com/aspnet/Extensions/blob/master/src/Logging/README.md) through an 
[ILoggerProvider](https://blog.stephencleary.com/2018/06/microsoft-extensions-logging-part-2-types.html#iloggerprovider)
  - log4net dependencies and configuration in any of upstream app.config or web.config can be removed

### Fixed

- Clean Esent JFM flushmap files when deleting XBIM databases [#206](https://github.com/xBimTeam/XbimEssentials/issues/206)
- SaveAs doesn't set "FileName" in IfcStore [#223](https://github.com/xBimTeam/XbimEssentials/issues/223)
- Indexing and Parsing performance + stability improvements [#215](https://github.com/xBimTeam/XbimEssentials/issues/215)
- Schema recognition without Exception throwing
- Metadata created only when needed
- Increased error tolerance in the Step21 parser (types of errors)
- Centralised ModelFactor code
- Fixed issue with IfcTriangulatedFaceSet [#211](https://github.com/xBimTeam/XbimEssentials/issues/211)
- Fixed threading issue in IO.Esent [#208](https://github.com/xBimTeam/XbimEssentials/issues/208)
- Fixed IFC4 Tesselator issue with MeshPolyhedronBinary [#196](https://github.com/xBimTeam/XbimEssentials/issues/196)
- Fix for empty RelatedObjects in Ifcxml [#188](https://github.com/xBimTeam/XbimEssentials/issues/188)
- Support IfcZip containing multiple files [#185](https://github.com/xBimTeam/XbimEssentials/issues/185)
- Duplicate IfcOrganization created [#173](https://github.com/xBimTeam/XbimEssentials/issues/173)
- IfcCalendar.ToString provides Date [#171](https://github.com/xBimTeam/XbimEssentials/issues/171)
- Fix BeginTransaction erasing FileHeader [#168](https://github.com/xBimTeam/XbimEssentials/issues/168)
- Added missing SI Units for IFC4/2x3 [#162](https://github.com/xBimTeam/XbimEssentials/issues/162)
- Support parsing of IfcDuration in other cultures [#153](https://github.com/xBimTeam/XbimEssentials/issues/153)
- More tolerant schema checking in MemoryModel [#126](https://github.com/xBimTeam/XbimEssentials/issues/126)
- Better tolerance for loading 'bad' files [#107](https://github.com/xBimTeam/XbimEssentials/issues/107)
- Tessellation is done in floats, resulting in loss of precision [#113](https://github.com/xBimTeam/XbimEssentials/issues/113)



## [4.0.29] - 2017-12-06

### Added
- Support for [**Ifc4-Add1**](http://www.buildingsmart-tech.org/specifications/ifc-releases/ifc4-add1-release) and
[**Ifc2x3-TC1**](http://www.buildingsmart-tech.org/specifications/ifc-releases/ifc2x3-tc1-release/summary).

## [4.0.28] - 2017-09-09
### Added
- Preliminary support for Ifc4-Add1 and Ifc2x3-TC1
- Where clause validation re-added

## [4.0.4] - 2017-10-24

Major new release. See http://docs.xbim.net/xbim-4/xbim-4-release-notes.html
### Added
- Support for IFC4 and side-by-side using code-generated schemas
- Ifc4 XML support
- Introduced `IModel` interface with `MemoryModel` and `EsentModel` implementations
- IfcStore added - wrapping `MemoryModel` and `EsentModel`

### Changed

- XbimModel moved to Xbim.IO.Ifc2x3

### Removed

- IFC `WHERE` rules have not been brought forward with the new code generated IFC schemas. To be addressed in future
