$sln = '.\Xamarin.Forms.sln'
$csproj = '.\Xamarin.Forms.ControlGallery.Android\Xamarin.Forms.ControlGallery.Android.csproj'
$dependency = '.\Xamarin.Forms.Build.Tasks\Xamarin.Forms.Build.Tasks.csproj'
$xaml = '.\Xamarin.Forms.Controls\GalleryPages\TitleView.xaml'
$adb = 'C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe'
$packageName = 'AndroidControlGallery.AndroidControlGallery'
$verbosity = 'quiet'
$java = '/p:JavaSdkDirectory=C:\Program Files (x86)\Java\jdk1.8.0_161'
$suffix = ''

$nuget = '.\nuget.exe'
if (!(Test-Path $nuget)) {
    Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nuget
}

function Touch {
    param ([string] $path)
    $date = (Get-Date)
    $date = $date.ToUniversalTime()
    $file = Get-Item $path
    $file.LastAccessTimeUtc = $date
    $file.LastWriteTimeUtc = $date
}

function MSBuild {
    param ([string] $msbuild, [string] $target, [string] $binlog)

    & $msbuild $csproj /t:$target /v:$verbosity /bl:$binlog $java
    if (!$?) {
        exit
    }

    # So git clean call doesn't delete
    & git add $binlog
}

function Profile {
    param ([string] $msbuild, [string] $version)
    
    # Reset working copy & device
    & $adb uninstall $packageName
    & git clean -dxf
    & $nuget restore $sln
    & $msbuild $dependency /v:$verbosity /bl:"./build-tasks-$version.binlog"
    if (!$?) {
        exit
    }
     # So git clean call doesn't delete
     & git add "./build-tasks-$version.binlog"

    # First
    MSBuild -msbuild $msbuild -target 'Build' -binlog "./first-build-$version$suffix.binlog"
    MSBuild -msbuild $msbuild -target 'SignAndroidPackage' -binlog "./first-package-$version$suffix.binlog"
    MSBuild -msbuild $msbuild -target 'Install' -binlog "./first-install-$version$suffix.binlog"

    # Second
    MSBuild -msbuild $msbuild -target 'Build' -binlog "./second-build-$version$suffix.binlog" 
    MSBuild -msbuild $msbuild -target 'SignAndroidPackage' -binlog "./second-package-$version$suffix.binlog"
    MSBuild -msbuild $msbuild -target 'Install' -binlog "./second-install-$version$suffix.binlog"

    # Third (Touch XAML)
    Touch $xaml
    MSBuild -msbuild $msbuild -target 'Build' -binlog "./third-build-$version$suffix.binlog"
    MSBuild -msbuild $msbuild -target 'SignAndroidPackage' -binlog "./third-package-$version$suffix.binlog"
    MSBuild -msbuild $msbuild -target 'Install' -binlog "./third-install-$version$suffix.binlog"
}

# 15.8.2
#$msbuild = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe'
#Profile -msbuild $msbuild -version '15.8'

# 16.0 P2
$msbuild = 'C:\Program Files (x86)\Microsoft Visual Studio\Preview\Enterprise\MSBuild\15.0\Bin\MSBuild.exe'
Profile -msbuild $msbuild -version '16.0'

# Print summary of results
$logs = Get-ChildItem .\*.binlog
foreach ($log in $logs) {
    $time = & $msbuild $log | Select-Object -Last 1
    Write-Host "$log $time"
}