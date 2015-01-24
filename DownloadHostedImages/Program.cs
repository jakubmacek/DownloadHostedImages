using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WatiN.Core;

namespace DownloadHostedImages
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var wc = new WebClient();

            using (var browser = new IE())
            {
                browser.GoTo(args[1]);

                var links = browser.Links.Where(x => x.ClassName == "external");
                var urls = links.Where(link => link.ChildrenWithTag("img").Any()).Select(link => link.Url).Where(url => url.Contains(".jpg")).ToList();
                foreach (var url in urls)
                {
                    var pageUri = new Uri(url);
                    var fileName = pageUri.Segments.Last();
                    if (!File.Exists(fileName))
                    {
                        browser.GoTo(url);
                        var imageSrc = browser.Image("show_image").Src;
                        var imageUri = new Uri(imageSrc);
                        wc.DownloadFile(imageUri, fileName);
                    }
                }
            }
        }
    }
}
