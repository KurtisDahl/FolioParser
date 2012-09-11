using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class Issue
    {
        public Issue(string pathPrefix)
        {
            this.pathPrefix = pathPrefix;

            this.Articles = new List<Article>();
        }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "date", IsRequired = true)]
        public string Date { get; set; }

        [DataMember(Name = "displayName", IsRequired = true)]
        public string DisplayName { get; set; }

        [DataMember(Name = "packageUrl", IsRequired = true)]
        public string PackageUrl { get; set; }

        [DataMember(Name = "landscapeImageUrl", IsRequired = true)]
        public string LandscapeImageUrl { get; set; }

        [DataMember(Name = "portraitImageUrl", IsRequired = true)]
        public string PortraitImageUrl { get; set; }

        public IList<Article> Articles { get; set; }

        private string pathPrefix;
    }
}
