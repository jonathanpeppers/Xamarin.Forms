param (
    [Boolean] $profile = $true,
    [string] $app = 'xappy'
)
function Invoke-MSBuild ([string] $project) {
    if ($profile) {
        $args = ''
    } else {
        $args = '/p:DontTime=True'
    }

    & msbuild /nologo /v:quiet $project /p:Configuration=Release $args
    if (!$?) {
        exit
    }
}

if ($app -eq 'xappy') {
    $package = 'com.microsoft.Xappy'
    $activity = 'md5dc8e1b02975c3158365aa81cf255d5af.MainActivity'
} elseif ($app -eq 'mdp') {
    $package = 'com.xamarin.masterdetail'
    $activity = 'md552a80a231c24d4b0901a6b4cb581ffb2.MainActivity'
} elseif ($app -eq 'shell') {
    $package = 'com.xamarin.shell'
    $activity = 'md50f820dafd10fa49e8fe186ddb2627970.MainActivity'
} else {
    Write-Host "Invalid app specified!"
    exit
}

Invoke-MSBuild .\Xamarin.Forms.Core\Xamarin.Forms.Core.csproj
Invoke-MSBuild .\Xamarin.Forms.Platform.Android\Xamarin.Forms.Platform.Android.csproj
Invoke-MSBuild .\Xamarin.Forms.Xaml\Xamarin.Forms.Xaml.csproj

$dir = "/storage/emulated/0/Android/data/$package/files/.__override__/"
& adb shell am force-stop $package
& adb push .\Xamarin.Forms.Platform.Android\bin\Release\monoandroid90\Xamarin.Forms.Platform.Android.dll $dir
& adb push .\Xamarin.Forms.Core\bin\Release\netstandard2.0\Xamarin.Forms.Core.dll $dir
& adb push .\Xamarin.Forms.Xaml\bin\Release\netstandard2.0\Xamarin.Forms.Xaml.dll $dir
& adb logcat -c
& adb shell am start -n "$package/$activity"

Start-Sleep -Seconds 10
if ($profile) {
    & adb logcat -d | Select-String _TM_ > .\ParseTimingLog\ParseTimingLog\log.txt
    Invoke-MSBuild .\ParseTimingLog\ParseTimingLog.sln
    .\ParseTimingLog\ParseTimingLog\bin\Release\ParseTimingLog.exe | clip
    Write-Host 'Data is copied to your clipboard, paste into Excel or your favorite editor, buddy!'
} else {
    & adb logcat -d | Select-String Displayed
}