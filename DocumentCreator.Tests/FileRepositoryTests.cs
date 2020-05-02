using DocumentCreator.Core.Repository;
using DocumentCreator.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentCreator
{
    public class FileRepositoryTests : IRepositoryTests
    {
        private static readonly string TEST_DIR = $".{Path.DirectorySeparatorChar}test_root";

        static FileRepositoryTests()
        {
            if (Directory.Exists(TEST_DIR))
                Directory.Delete(TEST_DIR, true);
            Directory.CreateDirectory(TEST_DIR);
        }

        protected override IRepository CreateRepository()
        {
            return new FileRepository(TEST_DIR);
        }
    }
}
