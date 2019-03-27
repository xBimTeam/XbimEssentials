param($installPath, $toolsPath, $package, $project)

$values = New-Object System.Collections.Specialized.NameValueCollection
[xml]$doc = Get-Content $project.FullName
$values.Add("ProjectGuid", $doc.GetElementsByTagName("ProjectGuid")[0].InnerText)
$values.Add("TargetFrameworkVersion", $doc.GetElementsByTagName("TargetFrameworkVersion")[0].InnerText)
$values.Add("PackageId", $package.Id)
$values.Add("PackageVersion", $package.Version)
$values.Add("IdeName", $project.DTE.Name)
$values.Add("IdeVersion", $project.DTE.Version)
$webClient = New-Object System.Net.WebClient
$webClient.UploadValuesAsync("http://xbimtracking.azurewebsites.net/api/log", $values) 