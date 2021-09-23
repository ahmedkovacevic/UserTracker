using System.IO;

namespace UserTrackerVS
{
    class FileHandler
    {
        public bool IsFileLocked()
        {
            try
            {
                FileStream stream = File.Open(@"C:\LOG\log-app.txt", FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        public void AddText(string filename, string value)
        {
            string fullPath = @"C:\LOG\" + filename;
            using (FileStream fs = File.Open(fullPath, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(value);
                }
            }
        }
    }
}
