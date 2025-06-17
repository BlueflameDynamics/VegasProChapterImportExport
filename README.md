# VegasProChapterImportExport
Vegas Pro Scripts for Chapter Import\Export

# What does it do?
These scripts work with VEGAS Pro Video Editor to provide a means of Importing & Exporting Markers
used for mp4\m4v chapters.

# What problem does this solve?
VEGAS Pro needs support from third-party apps like _MediaInfo_ in order to import chapter markers from third-party apps and the process of importing chapters can be laborious.
With '[Custom] Import-Mp4Chapters.cs' and the associated config file, you can import embedded chapters directly from within VEGAS Pro. While the standard Vegas Pro script 'Export Chapters.cs' will export Markers as chapters its content varies depending upon the output file type. The '[Custom] Export Chapters with Names.cs' exports Markers with uniform content.  

#Import Script History
- The `[Custom] Import-Mp4Chapters.cs` script is based upon the original script at:
 https://github.com/HimmDawg/VegasChapterImportFromObs

This version has been extended to support both mp4\m4v files, includes time_base in the marker position
calculation for generalized use overcoming the origials assumed 1/1000 time_base limitation. Additionally this version includes chapter titles as marker titles.

# Setup of Import Script
- install `ffprobe` and add it to your PATH (e.g. use winget like: https://winstall.app/apps/Gyan.FFmpeg , it'll automatically add the executable to PATH)
- download the `[Custom] Import-Mp4Chapters.cs` script and the `[Custom] Import-Mp4Chapters.cs.config`
    - you can rename the script, but make sure that the config file has the same name, followed by `.config`
- place both files in `C:\Program Files\VEGAS\VEGAS Pro x.0\Script Menu`
    - if not available, choose either of the locations listed here https://help.magix-hub.com/video/vegas/21/en/content/topics/external/vegasscriptfaq.html#1.10


# How to use Import Script
- select exactly one video file with chapters in the media explorer (Project Media Tab)
- in VEGAS Pro, navigate to `Tools > Scripting`
    - if the script is not visible here, hit `Rescan Script Menu Folder`
    - else you can use it from here now

**The markers wont take the track position into consideration.**

# Setup of Export Script
- download the `[Custom] Export Chapters with Names.cs` script
    - you can rename the script.
- place the file in the same location as the Import script.
    - you may need to `Rescan Script Menu Folder` as with the import script. 

# Export Script Features
- you may export to an XML, TXT, or CSV file. 
- When exporting to TXT or CSV you will be prompted about including optional fields.

# Remarks
The Import script was tested with VEGAS Pro 21.0 & 22.0, no guarantee that this will work with other versions.
For more infotmation about 'Timebases' see the Timebase PDF file.


