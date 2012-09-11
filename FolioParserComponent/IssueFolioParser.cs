using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using System.Xml;
using System.IO;

namespace FolioParserComponent
{
    internal class IssueFolioParser : IIssueFolioParser
    {
        public async Task<Issue> ParseAsync(IStorageFile issueFolioFile, string pathPrefix)
        { 
            Issue issue = new Issue(pathPrefix);
            
            Stream stream = await issueFolioFile.OpenStreamForReadAsync();

            XmlReader reader = XmlReader.Create(stream);
            if (reader != null)
            {
                // setting up the cover images assumed to be in the root folio folder
                issue.LandscapeImageUrl = "/cover_h.png";
                issue.PortraitImageUrl = "/cover_v.png";

                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.EndElement)
                    {
                        switch (reader.Name)
                        {
                            case "folio":
                                issue.Id = reader.GetAttribute("id");
                                issue.Date = reader.GetAttribute("date");
                                break;
                            case "magazineTitle":
                                reader.Read();
                                issue.DisplayName = reader.Value;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return issue;
        }
    }
}
