$SolutionDir = "$PSScriptRoot\.."
$SourceDir = "$SolutionDir\Nostradamus"
$TargetDir = "$SolutionDir\NotradamusUnity\Assets\Notradamus\Scripts"

$SourceSubDirs = ("Client", "Core", "Examples", "GeneratedCode", "Networking", "Physics", "Server", "Utils")

foreach ($SubDir in $SourceSubDirs) {
    cp -Force -Recurse $SourceDir\$SubDir $TargetDir
}