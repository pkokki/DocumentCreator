﻿namespace DocumentCreator.Repository
{
    public class ContentItem
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public byte[] Buffer { get; set; }
    }
}