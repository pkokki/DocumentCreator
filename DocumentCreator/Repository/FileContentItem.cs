using DocumentCreator.Core.Repository;
using System;
using System.IO;

namespace DocumentCreator.Repository
{
    public class FileContentItem : ContentItem
    {
        public static FileContentItem Create(string folderPath, string fileName)
        {
            var path = System.IO.Path.Combine(folderPath, fileName);
            if (File.Exists(path))
                return new FileContentItem(path);
            return null;
        }
        public static FileContentItem Create(string path)
        {
            if (File.Exists(path))
                return new FileContentItem(path);
            return null;
        }

        public FileContentItem(string path, byte[] contents)
        {
            File.WriteAllBytes(path, contents);
            Initialize(path, contents);
        }

        private FileContentItem(string path)
        {
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
