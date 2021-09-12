using System;
using System.IO;
using System.IO.Compression;

namespace ZipFolderWhenChanged
{
    class Program
    {
        static System.Threading.Timer _myTimer;
        static DateTime _lastChangedDateTime = DateTime.Now;
        static bool _watchedFolderDirty = false;
        static int _numMaxBackups;
        static string _pathToBackup;
        static string _pathForBackups;

        static void Main(string[] args)
        {            
            if( args.Length != 2 )
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("ZipFolderWhenChanged.exe MaxNumBackups PathToFolderToBackup");
                Console.WriteLine("");
                Console.WriteLine("Example:");
                Console.WriteLine("ZipFolderWhenChanged.exe 10 \"C:\\Users\\KarateSnoopy\\AppData\\LocalLow\\Freehold Games\\CavesOfQud\\Saves\\c5271ec1-2259-44a3-95b3-82f77be04b7d\"");
                return;
            }

            _numMaxBackups = int.Parse(args[0]);
            _pathToBackup = args[1];
            _pathForBackups = Path.Combine(_pathToBackup, "..");

            using var watcher = new FileSystemWatcher(_pathToBackup);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += OnFolderChanged;
            watcher.Filter = "*";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            _myTimer = new System.Threading.Timer(OnTimerElapsed, null, 0, 500);

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void OnFolderChanged(object sender, FileSystemEventArgs e)
        {
            // This event trigger a number of times when the game is saving
            _lastChangedDateTime = DateTime.Now;
            _watchedFolderDirty = true;
            Console.WriteLine("Folder changed. " + DateTime.Now);
        }

        static void OnTimerElapsed(object state)
        {
            // This gets called every 0.5s.
            // Check if its we need to backup and its been at least 5 seconds since the last write
            // If so, then backup
            if( _watchedFolderDirty )
            {
                var delaySinceLastWrite = _lastChangedDateTime + new TimeSpan(0, 0, 0, 5);
                if( DateTime.Now > delaySinceLastWrite)
                {
                    Console.WriteLine("No writes in last 5 seconds. Backing up now. " + DateTime.Now);
                    _watchedFolderDirty = false;
                    BackupFolder(_numMaxBackups, _pathToBackup, _pathForBackups);
                }
            }
        }

        private static void BackupFolder(int numMaxSaves, string pathToBackup, string pathForBackups)
        {
            string zipFullPath = string.Empty;
            bool foundEmptySlot = false;
            FileInfo oldestFound = null;
            SearchForSlot(numMaxSaves, pathToBackup, pathForBackups, ref zipFullPath, ref foundEmptySlot, ref oldestFound);
            if (!foundEmptySlot)
            {
                Console.WriteLine($"Deleting oldest backup. {oldestFound.Name} created at {oldestFound.LastWriteTime}");
                File.Delete(oldestFound.FullName);
                SearchForSlot(numMaxSaves, pathToBackup, pathForBackups, ref zipFullPath, ref foundEmptySlot, ref oldestFound);
                if (!foundEmptySlot)
                {
                    Console.WriteLine("ERROR: still couldn't find empty slot");
                    return;
                }
            }

            ZipFile.CreateFromDirectory(pathToBackup, zipFullPath);
            FileInfo newFi = new FileInfo(zipFullPath);
            Console.WriteLine($"Backed folder up to {newFi.Name} created at {newFi.LastWriteTime}");
        }

        private static void SearchForSlot(
            int numMaxSaves,
            string monitorPath,
            string destPath,
            ref string zipFullPath,
            ref bool foundEmptySlot,
            ref FileInfo oldestFound)
        {
            DirectoryInfo diMonitor = new DirectoryInfo(monitorPath);
            for (int i = 0; i < numMaxSaves; i++)
            {
                string zipName = $"{diMonitor.Name}-backup{i}.zip";
                zipFullPath = Path.Combine(destPath, zipName);
                FileInfo fi = new FileInfo(zipFullPath);
                if (!fi.Exists)
                {
                    foundEmptySlot = true;
                    break;
                }
                else
                {
                    if (oldestFound != null)
                    {
                        if (fi.LastWriteTime < oldestFound.LastWriteTime)
                        {
                            oldestFound = fi;
                        }
                    }
                    else
                    {
                        oldestFound = fi;
                    }
                }
            }
        }
    }
}
