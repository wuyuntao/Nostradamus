$SolutionDir = "$PSScriptRoot\.."
$ProjectDir = "$SolutionDir\Nostradamus"
$OutputDir = "$ProjectDir\Networking"
$SchemaPath = "$ProjectDir\Networking\Schema.fbs"

& $SolutionDir\Lib\FlatBuffers\flatc.exe -n --gen-onefile -o $OutputDir $SchemaPath
& node $SolutionDir\node_modules\fbagen\index.js -n -o $OutputDir $SchemaPath