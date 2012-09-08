using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class OverlayAsset
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "height", IsRequired = true)]
        public double Height { get; set; }

        [DataMember(Name = "width", IsRequired = true)]
        public double Width { get; set; }

        [DataMember(Name = "url", IsRequired = true)]
        public string Url { get; set; }
    }
}
