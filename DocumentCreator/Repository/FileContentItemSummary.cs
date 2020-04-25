using DocumentCreator.Core.Repository;
using System;
using System.IO;

namespace DocumentCreator.Repository
{
    public class FileContentItemSummary : ContentItemSummary
    {
        public FileContentItemSummary(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException();
            Initialize(path);
        }

        private void Initialize(string path)
        {
            var info = new FileInfo(path);
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            FileName = System.IO.Path.GetFileName(path);
            Path = path;
            Size = (int)info.Length;
            Timestamp = info.CreationTime;
        }
    }
}
