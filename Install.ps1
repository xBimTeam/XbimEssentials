param($installPath, $toolsPath, $package, $project)

$values = New-Object System.Collections.Specialized.NameValueCollection
$values.Add("Project", $project.Name)
$values.Add("PackageId", $package.Id)
$values.Add("PackageVersion", $package.Version)
$values.Add("IdeName", $project.DTE.Name)
$values.Add("IdeVersion", $project.DTE.Version)
$webClient = New-Object System.Net.WebClient
$webClient.UploadValuesAsync("http://xbimtracking.azurewebsites.net/api/log", $values)	

Start-Process "https://goo.gl/forms/S2u2YvPBrwkIUgXm1"