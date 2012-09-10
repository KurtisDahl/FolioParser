using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class ImagepanOverlay
    {
        public ImagepanOverlay()
        {
            this.OverlayAssets = new List<OverlayAsset>();
        }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "y", IsRequired = true)]
        public double Y { get; set; }

        [DataMember(Name = "x", IsRequired = true)]
        public double X { get; set; }

        [DataMember(Name = "height", IsRequired = true)]
        public double Height { get; set; }

        [DataMember(Name = "width", IsRequired = true)]
        public double Width { get; set; }

        [DataMember(Name = "orientation", IsRequired = true)]
        public string Orientation { get; set; }

        [DataMember(Name = "overlayAssets", IsRequired = true)]
        public IList<OverlayAsset> OverlayAssets { get; private set; }

        [DataMember(Name = "anchorY", IsRequired = true)]
        public double AnchorY { get; set; }

        [DataMember(Name = "anchorX", IsRequired = true)]
        public double AnchorX { get; set; }

        [DataMember(Name = "viewPortBoundsX", IsRequired = true)]
        public double ViewPortBoundsX { get; set; }

        [DataMember(Name = "viewPortBoundsY", IsRequired = true)]
        public double ViewPortBoundsY { get; set; }

        [DataMember(Name = "viewPortBoundsHeight", IsRequired = true)]
        public double ViewPortBoundsHeight { get; set; }

        [DataMember(Name = "viewPortyBoundsWidth", IsRequired = true)]
        public double ViewPortBoundsWidth { get; set; }
    }
}
