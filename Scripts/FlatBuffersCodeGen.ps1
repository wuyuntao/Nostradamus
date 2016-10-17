$SolutionDir = "$PSScriptRoot\.."
$ProjectDir = "$SolutionDir\Nostradamus"

& $SolutionDir\Lib\FlatBuffers\flatc.exe -n --gen-onefile -o $ProjectDir\Networking $ProjectDir\Networking\Schema.fbs
