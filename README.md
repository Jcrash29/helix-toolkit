Helix Toolkit
====

Helix Toolkit is a collection of 3D components for .NET. Currently it contains one component that adds functionality to the WPF 3D model (Media3D namespace), and one WPF component that creates a similar scene graph for DirectX (based on SharpDX).

[![Build status](https://ci.appveyor.com/api/projects/status/tmqafdk9p7o98gw7)](https://ci.appveyor.com/project/objorke/helix-toolkit)

Description         | Value
--------------------|-----------------------
License             | The MIT License (MIT)
Web page            | http://helix-toolkit.org/
Documentation       | http://docs.helix-toolkit.org/
Forum               | http://forum.helix-toolkit.org/
Chat                | https://gitter.im/helix-toolkit/helix-toolkit
Source repository   | http://github.com/helix-toolkit/helix-toolkit
Latest build        | http://ci.appveyor.com/project/objorke/helix-toolkit
Issue tracker       | http://github.com/helix-toolkit/helix-toolkit/issues
NuGet packages      | http://www.nuget.org/packages?q=HelixToolkit
MyGet feed          | https://www.myget.org/F/helix-toolkit
StackOverflow       | http://stackoverflow.com/questions/tagged/helix-3d-toolkit
Twitter             | https://twitter.com/hashtag/Helix3DToolkit

News
====
We are currently working on HelixToolkit 2.0 under develop branch. Mainly focus on HelixToolkit.SharpDX.

All 1.x.x related pull requests, please use [1.1.0/Release](https://github.com/helix-toolkit/helix-toolkit/tree/release/1.1.0) branch.

#### Note: 2.0 Breaking changes from version 1.x.x. (HelixToolkit.SharpDX only)
1. New architecture for backend rendering and shader management. No more dependency from obsoleted Effects framework. EffectsManager is mandatory to be provided from ViewModel for resource live cycle management by user to avoid memory leak.
2. Many performance improvements. Viewports binding with same EffectsManager will share common resources. Models binding with same geometry3D will share same geometry buffers. Materials binding with same texture will share same resources.
3. Support basic direct2d rendering and layouts arrangement. (Still needs a lot of implementations)
4. No more HelixToolkit.WPF project dependency.
5. Unify dependency property types. All WPF.SharpDx model's dependency properties are using class under System.Windows.Media. Such as Vector3D and Color. More Xaml friendly.
6. Other on going changes.

### 2018-02-06

V1.1.0 release is available.

https://www.nuget.org/packages/HelixToolkit.Wpf/1.1.0

https://www.nuget.org/packages/HelixToolkit.Wpf.SharpDX/1.1.0

V1.1.0 Relase source code is under : https://github.com/helix-toolkit/helix-toolkit/tree/release/1.1.0
