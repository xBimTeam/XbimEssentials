# XbimEssentials
Part of Xbim; the eXtensible Building Information Modelling (BIM) Toolkit

Build Status (master branch): [ ![Build Status](http://xbimbuilds.cloudapp.net/app/rest/builds/buildType:(id:XbimEssentials_XbimEssentials),branch:(name:master)/statusIcon "Build Status") ](http://xbimbuilds.cloudapp.net/project.html?projectId=XbimEssentials&tab=projectOverview "Build Status")

Build Status (develop branch): [ ![Build Status](http://xbimbuilds.cloudapp.net/app/rest/builds/buildType:(id:XbimEssentials_XbimEssentials),branch:(name:develop)/statusIcon "Build Status") ](http://xbimbuilds.cloudapp.net/project.html?projectId=XbimEssentials&tab=projectOverview "Build Status")

## What is it?

The xBIM Tookit (eXtensible Building Information Modelling) is an open-source, software development BIM toolkit that 
supports the BuildingSmart Data Model (aka the [Industry Foundation Classes IFC](http://en.wikipedia.org/wiki/Industry_Foundation_Classes)).

xBIM allows developers to read, create and view [Building Information (BIM)](http://en.wikipedia.org/wiki/Building_information_modeling) Models in the IFC format. 
There is full support for geometric, topological operations and visualisation. In addition xBIM supports 
bi-directional translation between IFC and COBie formats

## Getting Started

You will need Visual Studio 2010 SP1 or newer to compile the Solution. To compile the SceneJSWebViewer sample
project you will require ASP.NET MVC 3.0 to be installed. All solutions target .NET 4.0. The 4.0 Client profile
may be supported for some projects.

XbimEssentials is a software library to be used for the creation of complex applications, other repositories under the [XbimTeam](https://github.com/xBimTeam) page include a number of sample applications to demonstrate its capabilities

* [XbimXplorer](https://github.com/xBimTeam/XbimWindowsUI) - a Windows WPF sample application that can open and render 3D IFC models (and native XBIM models ) as well as displaying semantic data.
* [XbimWebUI](https://github.com/xBimTeam/XbimWebUI) - a web application that can open and render 3D IFC models. 
* [XbimUtilities](https://github.com/xBimTeam/XbimUtilities) - a set of sample console applications to perform bulk functions on IFC files.
* [Xbim.COBie.Client]() - A sample windows application demonstrating how a COBie spreadsheet can be generated from an IFC model.
* [HelloWall](https://github.com/xBimTeam/XbimSamples) - a sample console application demonstrating how to undertake simple IFC creation with Xbim.

Please note: all the applications are provided as examples of how to use the Xbim library, they are not intended for use in uncontrolled 
production environments.

## Licence

The XBIM library is made available under the CDDL Open Source licence.  See the licences folder for a full text.

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

## Compilation
The toolkit uses the Nuget technology for the management of several required packages as well as for distributing the libraries.
If you wish to use the development versions of xbim make sure to add our nuget feeds for the master and develop branches of the solution.
Nuget can download all the required dependencies for you if you have the correct package source configuration.

if you use Visual Studio 2015+ add the following package sources:
* https://www.myget.org/F/xbim-develop/api/v3/index.json
* https://www.myget.org/F/xbim-master/api/v3/index.json

if you use Visual Studio 2012+ add the following package sources:
* https://www.myget.org/F/xbim-develop/api/v2
* https://www.myget.org/F/xbim-master/api/v2

the resulting configuration pages looks like this in VS2015:
![example of VS2015 configuration](https://raw.githubusercontent.com/xBimTeam/XbimWindowsUI/master/ReadmeResources/NugetCongfigurationVS2015.png)

## Support & Help

Please use the community features of GitHub to ask any questions and raise issues.

## Acknowledgements
The XbimTeam wishes to thank [JetBrains](https://www.jetbrains.com/) for supporting the XbimToolkit project with free open source [Resharper](https://www.jetbrains.com/resharper/) licenses.

[![ReSharper Logo](https://raw.githubusercontent.com/xBimTeam/XbimWindowsUI/master/ReadmeResources/icon_ReSharper.png)](https://www.jetbrains.com/resharper/)

## Getting Involved

If you'd like to get involved and contribute to this project, please contact the Project Coordinator @SteveLockley.
