#!/bin/bash

# Clean
git clean -dxf

# NuGet restore
nuget restore Xamarin.Forms.sln
msbuild Xamarin.Forms.sln /t:Restore

# Build XF build tasks
msbuild Xamarin.Forms.Build.Tasks/Xamarin.Forms.Build.Tasks.csproj

# Build Android stuff
msbuild .Xamarin.Forms.Android.sln
