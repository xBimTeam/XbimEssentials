# Roadmap notes

these are just notes that might be helpful in supporting the future development of the library, they are not listed in any particular order.

## Migration to .Net Standard 2.0

One interesting step for the future would be to migrate the solution to .Net standard; with the release of v. 2.0 this has become more feasible.

### Our code

At the moment the migration analysis lists issue with 3 projects:

1. **Utility.ExtractCobieData** - the application will have to be upgraded to a higher .net framework version.
2. **Test projects** - no need to port immediately, they can be upgraded to a higher .net framework version, until new projects are needed for specific OSs.
3. **XBim.Ifc2x3.Extensions** - A single class ```ClassMaker\CodeGenerator.cs``` uses c# code generation, otherwise the project is portable.
4. **Xbim.Io** - Only a small change is needed as the configuration is used to search for the default temporary folder. This is easily solved in ```Xbim.IO\Esent\PersistedEntityInstanceCache.cs```.

### Libraries

1. **Log4Net** - Is available on NetCore 2.0.
2. **ManagedEsent** - I haven't even looked at this, I don't think it will ever happen, so the solution would be to move Esent to a Windows extension on net framework, and use Microsoft.Data.Sqlite for local databases in the standard version.
3. **NPOI** - [This package on Github](https://github.com/dotnetcore/NPOI) looks promising.
4. **SharpZipLib** - [.Net Standard seems supported]( https://github.com/icsharpcode/SharpZipLib/issues/106)

## Ifc4x1 (and future)

Any relevant notes on this?
