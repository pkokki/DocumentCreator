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

        public FileContentItem(string path, Stream contents)
        {
            contents.Position = 0;
            using (FileStream output = File.OpenWrite(path))
            {
                contents.CopyTo(output);
            }
            Initialize(path, contents.ToMemoryStream());
        }

        private FileContentItem(string path)
        {
            var contents = new MemoryStream();
            using (FileStream input = File.OpenRead(path))
            {
                input.CopyTo(contents);
            }
            contents.Position = 0;
            Initialize(path, contents);
        }

        protected FileContentItem()
        {
        }

        private void Initialize(string path, Stream contents)
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            FileName = System.IO.Path.GetFileName(path);
            Path = path;
            Size = (int)contents.Length;
            Timestamp = new FileInfo(path).CreationTime;
            Buffer = contents;
        }
    }
}
