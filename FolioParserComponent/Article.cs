using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class Article
    {
        public Article()
        {
            this.Pages = new List<Page>();
        }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "title", IsRequired = true)]
        public string Title { get; set; }

        [DataMember(Name = "description", IsRequired = true)]
        public string Description { get; set; }

        [DataMember(Name = "author", IsRequired = true)]
        public string Author { get; set; }

        [DataMember(Name = "kicker", IsRequired = true)]
        public string Kicker { get; set; }

        [DataMember(Name = "pages", IsRequired = true)]
        public IList<Page> Pages { get; set; }
    }
}
