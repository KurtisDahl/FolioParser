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
        public async Task<Issue> ParseAsync(IStorageFile issueFolioFile)
        { 
            string prevName = "";
            Issue issue = new Issue();
            
            Stream stream = await issueFolioFile.OpenStreamForReadAsync();

            XmlReader reader = XmlReader.Create(stream);
            if (reader != null)
            {
                // setting up the cover images assumed to be in the root folio folder
                issue.LandscapeImageUrl = "/cover_h.png";
                issue.PortraitImageUrl = "/cover_v.png";

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.
                            if(reader.Name == "folio")
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    switch (reader.Name)
                                    {
                                        case "id":
                                            issue.Id = reader.Value;
                                            break;
                                        case "date":
                                            issue.Date = reader.Value;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                prevName = reader.Name;
                            } 
                            break;
                        case XmlNodeType.Text: //Display the text in each element.
                            if (prevName == "magazineTitle")
                            {
                                issue.DisplayName = reader.Value;
                                prevName = "";
                            }
                            break;
                        case XmlNodeType.EndElement: //Display the end of the element.
                            break;
                        default:
                            break;
                    }
                }
                
            }
            return issue;

        }
    }
}
