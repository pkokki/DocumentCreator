using DocumentCreator.Core.Repository;
using System;
using System.IO;

namespace DocumentCreator.Repository
{
    public class FileContentItem : ContentItem
    {
        public FileContentItem(string folderPath, string fileName)
            : this(System.IO.Path.Combine(folderPath, fileName))
        {
        }

        public FileContentItem(string path, byte[] contents)
        {
            File.WriteAllBytes(path, contents);
            Initialize(path, contents);
        }

        public FileContentItem(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException();
            var contents = File.ReadAllBytes(path);
            Initialize(path, contents);
        }

        protected FileContentItem()
        {
        }

        private void Initialize(string path, byte[] contents)
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            FileName = System.IO.Path.GetFileName(path);
            Path = path;
            Size = contents.Length;
            Timestamp = new FileInfo(path).CreationTime;
            Buffer = contents;
        }
    }
}
