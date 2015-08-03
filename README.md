[![NuGet](https://img.shields.io/nuget/v/xbim.essentials.svg?label=Nuget version)](https://www.nuget.org/packages/xbim.essentials)

[![MyGet](https://img.shields.io/myget/xbim-master/vpre/Xbim.Essentials.svg?label=Myget Master)]()

[![MyGet](https://img.shields.io/myget/xbim-develop/vpre/Xbim.Essentials.svg?label=Myget Develop)](https://www.nuget.org/packages/xbim.essentials)

[![Issues](https://img.shields.io/github/issues/XbimTeam/XbimEssentials.svg)](https://github.com/xBimTeam/XbimEssentials/issues)

# XBIM - the eXtensible Building Information Modelling (BIM) Toolkit

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


Xbim is a software library, and is currently deployed with a number of sample applications to demonstrate its capabilities

* XbimXplorer - a Windows WPF sample application that can open and render 3D IFC models (and native XBIM models ) as well as displaying semantic data.
* Xbim.SceneJSWebViewer - a MVC web application that can open and render 3D IFC models (previously converted to XBIM) using a WebGL compatible browser. 
* XbimConvert - a sample console application to generate native XBIM model and geometry files from an IFC file.
* Xbim.COBie.Client - A sample windows application demonstrating how a COBie spreadsheet can be generated from an IFC model.
* CodeExamples - a sample console application demonstrating how to undertake a number of BIM processes using XBIM

Please note: all the samples in this solution are examples of how to use the Xbim library, and not intended to be used in a 
production environment without further development.

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

## Support & Help

Please use the Discussion forum at http://xbim.codeplex.com/discussions to ask any questions.
Alternatively use the Issue Tracker to raise any bugs relating to Xbim.

## Getting Involved

If you'd like to get involved and contribute to this project, please contact the Project Coordinator, [Steve Lockley](https://www.codeplex.com/site/users/view/SteveLockley).
