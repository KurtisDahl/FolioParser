using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class AudioOverlay
    {
        public AudioOverlay()
        {
            this.OverlayAssets = new List<OverlayAsset>();
        }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "audioUrl", IsRequired = true)]
        public string AudioUrl { get; set; }

        [DataMember(Name = "autoStart", IsRequired = true)]
        public bool AutoStart { get; set; }

        [DataMember(Name = "autoStartDelay", IsRequired = true)]
        public double AutoStartDelay { get; set; }

        [DataMember(Name = "orientation", IsRequired = true)]
        public string Orientation { get; set; }

        [DataMember(Name = "y", IsRequired = true)]
        public double Y { get; set; }

        [DataMember(Name = "x", IsRequired = true)]
        public double X { get; set; }

        [DataMember(Name = "height", IsRequired = true)]
        public double Height { get; set; }

        [DataMember(Name = "width", IsRequired = true)]
        public double Width { get; set; }

        [DataMember(Name = "overlayAssets", IsRequired = true)]
        public IList<OverlayAsset> OverlayAssets { get; private set; }

    }
}
