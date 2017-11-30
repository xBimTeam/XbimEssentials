# XbimEssentials
XbimEssentials is part of Xbim, the eXtensible [Building Information Modelling Toolkit](https://xbimteam.github.io/)

## Build Status

Master: [ ![Build Status](http://xbimbuilds.cloudapp.net/app/rest/builds/buildType:(id:XbimEssentials_XbimEssentials),branch:(name:master)/statusIcon "Build Status") ](http://xbimbuilds.cloudapp.net/project.html?projectId=XbimEssentials&tab=projectOverview "Build Status")

Develop branch: [ ![Build Status](http://xbimbuilds.cloudapp.net/app/rest/builds/buildType:(id:XbimEssentials_XbimEssentials),branch:(name:develop)/statusIcon "Build Status") ](http://xbimbuilds.cloudapp.net/project.html?projectId=XbimEssentials&tab=projectOverview "Build Status")

The XbimTeam is committed to posting code that always compiles, if you see a build failure here the error is generally due to omitted increases in the Nuget version number.

## What is it?

The xBIM Tookit (eXtensible Building Information Modelling) is an open-source, software development BIM toolkit that
supports the BuildingSmart Data Model (aka the [Industry Foundation Classes IFC](http://en.wikipedia.org/wiki/Industry_Foundation_Classes)).

xBIM allows developers to read, create and view [Building Information (BIM)](http://en.wikipedia.org/wiki/Building_information_modeling) Models in the IFC format.
There is full support for geometric, topological operations and visualisation. In addition xBIM supports
bi-directional translation between IFC and COBie formats

## Getting Started

### Using the library

If you with to use Xbim.Essential in your code you just have to add the package to your Solution in Visual Studio using Nuget's Package Manager Console and issuing the following command:

```
PM> Install-Package Xbim.Essentials
```

### Compilation

You will need Visual Studio 2013 or newer to compile the Solution. All solutions target .NET 4.0. The 4.0 Client profile
may be supported for some projects. The roadmap expects to move to 4.5 versions of the .NET framework soon.

### Examples

XbimEssentials is a software library to be used for the creation of complex applications, other repositories under the [XbimTeam](https://github.com/xBimTeam) page include a number of example applications to demonstrate its capabilities.

If you wish to move your first steps these are the first resources to lookup:

* [The example list page](http://docs.xbim.net/examples/examples-list.html) can act as a short tutorial to familiarise with the library.

* [Small examples](https://github.com/xBimTeam/XbimSamples) - a list of small console application demonstrating how to undertake simple IFC activities with Xbim that compiles and runs in visual studio.

* [XbimXplorer](http://docs.xbim.net/downloads/xbimxplorer.html) - is a fairly complex WPF sample application that can open and render 3D IFC models as well as displaying semantic data, [its source code is available in the Xbim.WindowsUI solution](https://github.com/xBimTeam/XbimWindowsUI).

## Licence

The XBIM library is made available under [the CDDL Open Source licence](LICENCE.md).

All licences should support the commercial usage of the XBIM system within a 'Larger Work', as long as you honour
the licence agreements.

## Third Party Licences

The core XBIM library makes use of the following 3rd party software packages, under their associated licences:

* 'OpenCASCADE' Geometry Engine : http://www.opencascade.org/ - OPEN CASCADE Public License
* 'Gardens Point Parser Generator' http://gppg.codeplex.com/ - New BSD Licence
* Elements of '3D Tools' WPF library http://3dtools.codeplex.com/ - MS Permissive Licence
* Log4net : http://logging.apache.org/log4net/ - Apache 2.0 Licence
* NPOI : http://npoi.codeplex.com - Apache 2.0 Licence
* NewtonSoft JSON : http://json.codeplex.com/ - MIT Licence

Additionally the Samples also make use of the following libraries

* SceneJS: https://github.com/xeolabs/scenejs - joint MIT and GPL Licence
* SignalR : https://github.com/SignalR/SignalR - Apache 2.0 Licence

All licences are included in full under the Licences\3rd Party solution folder.

## Support & Help

Please use the community features of GitHub to ask any questions and raise issues.

## Acknowledgements
While we do not qualify anymore for open source licenses of JetBrains, we would like to acknowledge the good work and thank [JetBrains](https://www.jetbrains.com/) for supporting the XbimToolkit project with free open source [Resharper](https://www.jetbrains.com/resharper/) licenses in the past.

[![ReSharper Logo](https://raw.githubusercontent.com/xBimTeam/XbimWindowsUI/master/ReadmeResources/icon_ReSharper.png)](https://www.jetbrains.com/resharper/)

## Getting Involved

If you'd like to get involved and contribute to this project, please read the [CONTRIBUTING ](https://github.com/xBimTeam/XbimEssentials/blob/master/CONTRIBUTING.md) page or contact the Project Coordinators @CBenghi and @martin1cerny.
