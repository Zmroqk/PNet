using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetClient.Updater
{
    public class Release
    {
        public string? url;
        public string? assets_url;
        public string? upload_url; 
        public string? html_url;
        public int id;
        public Author author;
        public string? node_id;
        public string? tag_name;
        public string? target_commitish;
        public string? name;
        public bool draft;
        public bool prerelease;
        public DateTime created_at;
        public DateTime published_at;
        public Asset[] assets;
        public string? tarball_url;
        public string? zipball_url;
        public string? body;
    }
}
