$xunitPath = "$workingDir\xunit.runner.console.2.2.0\tools\xunit.console.exe"
	foreach($test in $tests)
	{
		$projectTestPath = "$workingSourceDir\$test\bin\Release\$test.dll"
		Write-Host -ForegroundColor Green "Running xunit tests: $projectTestPath"
		exec{ & $xunitPath $projectTestPath }
	}