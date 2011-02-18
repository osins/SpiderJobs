installutil /u SpiderService.exe
copy /y H:\Soft\OpenSources\Jobberbase\Spider\Spider\SpiderService\bin\Release\*.* D:\Tools\SpiderJobs\*.*
installutil SpiderService.exe 
net start SpiderJobs