using CalorieTracker;
using GreenGlassX;
using GreenGlassX.UWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(SaveAndLoad_UWP))]

namespace GreenGlassX.UWP
{
    public class SaveAndLoad_UWP : ISaveAndLoad
    {
        #region ISaveAndLoad implementation
        public void DeleteFile(string filename)
        {
            if (FileExists(filename))
                File.Delete(AddPathToFilename(filename));
        }
        public bool FileExists(string filename)
        {
            return File.Exists(AddPathToFilename(filename));
        }
        public byte[] LoadBinary(string filename)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(AddPathToFilename(filename), FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }
        public void CreateDirectory(string dir)
        {
            Directory.CreateDirectory(ApplicationData.Current.LocalFolder.Path + "\\" + dir);
        }
        public void SaveStream(Stream stream, string filename)
        {
            var fileStream = new FileStream(AddPathToFilename(filename), FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);
            fileStream.Dispose();
        }
        public string AddPathToFilename(string filename)
        {
            return ApplicationData.Current.LocalFolder.Path + "\\" + filename;
        }
        public string GetLocalFilePath()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }

        #endregion

    }
}
