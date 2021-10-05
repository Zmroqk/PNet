using System;

namespace PNetClient.Updater
{
    public class Asset
    {
        public string? url;
        public int id;
        public string node_id;
        public string? name;
        public string? label;
        public Author uploader;
        public string content_type;
        public string? state;
        public long size;
        public int download_count;
        public DateTime created_at;
        public DateTime updated_at;
        public string? browser_download_url;
    }
}