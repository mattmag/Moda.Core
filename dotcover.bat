dotnet dotcover test --dcReportType=HTML --dcFilters="-:module=*Tests;-:class=*Exception"
start "" "dotCover.Output.html"