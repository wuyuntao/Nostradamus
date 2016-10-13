$SolutionDir = "$PSScriptRoot\.."
$ProjectDir = "$SolutionDir\Nostradamus"

& $SolutionDir\Lib\FlatBuffers\flatc.exe -n -o $ProjectDir\GeneratedCode $ProjectDir\Nostradamus.fbs
