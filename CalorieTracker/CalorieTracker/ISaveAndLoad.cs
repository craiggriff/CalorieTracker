using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CalorieTracker
{
    public interface ISaveAndLoad
    {
        void DeleteFile(string filename);
        bool FileExists(string filename);
        byte[] LoadBinary(string filename);
        void CreateDirectory(string dir);
        string AddPathToFilename(string filename);
        void SaveStream(Stream stream, string filename);
        string GetLocalFilePath();
    }
}

