$versionNumber = "1.2.0";

$root = Split-Path -parent $MyInvocation.MyCommand.Definition

$currentPath = [System.IO.Directory]::GetParent($root).FullName;

$assemblyFiles = [System.IO.Directory]::GetFiles($currentPath, "AssemblyInfo.cs", [System.IO.SearchOption]::AllDirectories);

echo "AssemblyInfo.cs Count: " $assemblyFiles.Length;

foreach($file in $assemblyFiles)
{
	$text = [System.IO.File]::ReadAllText($file);
	$text = $text -replace "(?<=\[assembly: Assembly(File)?Version\(`")([\d\.]+)(?=`"\)\])", $versionNumber;
	[System.IO.File]::WriteAllText($file, $text);
}

echo "done!";