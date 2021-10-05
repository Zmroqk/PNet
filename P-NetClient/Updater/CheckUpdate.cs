using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace PNetClient.Updater
{
    public class Updater
    {
        public const string updateUrl = "https://api.github.com/repos/Zmroqk/PNet/releases";
        const string assetType = "application/x-zip-compressed";
        public Release? UpdateData { get; set; }

        public Updater() {}

        public async Task<bool> CheckForUpdate() {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Other");
                string json = await Task.Run(() => client.DownloadString(updateUrl));
                Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                UpdateData = JsonConvert.DeserializeObject<Release[]>(json)?.Where(r => !r.prerelease).FirstOrDefault();
                Regex regex = new Regex(@"v(\d.\d.\d(?:.\d)?)");
                if (regex.IsMatch(UpdateData.tag_name)){
                    string version = regex.Match(UpdateData.tag_name).Groups[1].Value;
                    if (UpdateData != null && !new Version(version).Equals(assemblyVersion))
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }

        public async Task DownloadNewVersion()
        {
            if(UpdateData == null)
            {
                return;
            }
            using(WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Other");
                if(UpdateData.assets.Length > 0)
                {
                    try
                    {
                        string path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PNet", "versions");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string filePath = Path.Join(path, $"{UpdateData.tag_name}.zip");
                        if (File.Exists(filePath))
                        {
                            return;
                        }
                        await Task.Run(() => {
                            try
                            {
                                client.DownloadFile(UpdateData.assets.Where(asset => asset.content_type == assetType).First().browser_download_url, filePath);
                            }
                            catch (Exception e) { Console.Error.WriteLine(e.Message); }
                        });
                    }
                    catch(Exception e) { Console.Error.WriteLine(e.Message); }                    
                }
            }
                
        }
    }
}
