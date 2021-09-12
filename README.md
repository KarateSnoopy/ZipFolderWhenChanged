# ZipFolderWhenChanged
Tool that zips a folder when the folder is written to.  
It was created for Caves of QUD save scumming but useful for other games, etc.
It keeps a max number of backup zips that's controlled by first arg.

Usage:
ZipFolderWhenChanged.exe MaxNumBackups PathToFolderToBackup

Example:
ZipFolderWhenChanged.exe 10 "C:\Users\KarateSnoopy\AppData\LocalLow\Freehold Games\CavesOfQud\Saves\c5271ec1-2259-44a3-95b3-82f77be04b7d"

Here's an example:

ZipFolderWhenChanged.exe 10 "C:\Users\KarateSnoopy\AppData\LocalLow\Freehold Games\CavesOfQud\Saves\c5271ec1-2259-44a3-95b3-82f77be04b7d"
Press enter to exit.
Folder changed. 9/12/2021 1:39:16 PM
No writes in last 5 seconds. Backing up now. 9/12/2021 1:39:23 PM
Backed folder up to c5271ec1-2259-44a3-95b3-82f77be04b7d-backup3.zip created at 9/12/2021 1:39:26 PM

