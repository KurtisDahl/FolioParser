using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class Page
    {
         public Page()
        {
            this.AudioOverlays = new List<AudioOverlay>();
            this.HyperlinkOverlays = new List<HyperlinkOverlay>();
            this.ImagepanOverlays = new List<ImagepanOverlay>();
            this.SlideshowOverlays = new List<SlideshowOverlay>();
            this.WebviewOverlay = new List<WebviewOverlay>();
            this.VideoOverlays = new List<VideoOverlay>();
        }

        /// <summary>
        /// adds landscape properties to an existing Page
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        //public static Page operator +(Page p1, Page p2)
        //{
        //    p1.LandscapeContentUrl = p2.LandscapeContentUrl;
        //    p1.LandscapeContentWidth = p2.LandscapeContentWidth;
        //    p1.LandscapeContentHeight = p2.LandscapeContentHeight;

        //    p1.LandscapeThumbUrl = p2.LandscapeThumbUrl;
        //    p1.LandscapeThumbWidth = p2.LandscapeThumbWidth;
        //    p1.LandscapeThumbHeight = p2.LandscapeThumbHeight;

        //    p1.LandscapeScrubberUrl = p2.LandscapeScrubberUrl;
        //    p1.LandscapeScrubberWidth = p2.LandscapeScrubberWidth;
        //    p1.LandscapeScrubberHeight = p2.LandscapeScrubberHeight;

        //    return p1;
        //}

        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "subId", IsRequired = true)]
        public string SubId { get; set; }

        ///
        /// Portrait
        /// 
        // content
        [DataMember(Name = "portraitContentUrl", IsRequired = true)]
        public string PortraitContentUrl { get; set; }

        [DataMember(Name = "portraitContentHeight", IsRequired = true)]
        public double PortraitContentHeight { get; set; }

        [DataMember(Name = "portraitContentWidth", IsRequired = true)]
        public double PortraitContentWidth { get; set; }

        //thumb
        [DataMember(Name = "portraitThumbUrl", IsRequired = true)]
        public string PortraitThumbUrl { get; set; }

        [DataMember(Name = "portraitThumbHeight", IsRequired = true)]
        public double PortraitThumbHeight { get; set; }

        [DataMember(Name = "portraitThumbWidth", IsRequired = true)]
        public double PortraitThumbWidth { get; set; }

        //scrubber
        [DataMember(Name = "portraitScrubberUrl", IsRequired = true)]
        public string PortraitScrubberUrl { get; set; }

        [DataMember(Name = "portraitScrubberHeight", IsRequired = true)]
        public double PortraitScrubberHeight { get; set; }

        [DataMember(Name = "portraitScrubberWidth", IsRequired = true)]
        public double PortraitScrubberWidth { get; set; }

        ///
        /// Landscape
        /// 
        // content
        [DataMember(Name = "landscapeContentUrl", IsRequired = true)]
        public string LandscapeContentUrl { get; set; }

        [DataMember(Name = "landscapeContentHeight", IsRequired = true)]
        public double LandscapeContentHeight { get; set; }

        [DataMember(Name = "landscapeContentWidth", IsRequired = true)]
        public double LandscapeContentWidth { get; set; }

        //thumb
        [DataMember(Name = "landscapeThumbUrl", IsRequired = true)]
        public string LandscapeThumbUrl { get; set; }

        [DataMember(Name = "landscapeThumbHeight", IsRequired = true)]
        public double LandscapeThumbHeight { get; set; }

        [DataMember(Name = "landscapeThumbWidth", IsRequired = true)]
        public double LandscapeThumbWidth { get; set; }

        //scrubber
        [DataMember(Name = "landscapeScrubberUrl", IsRequired = true)]
        public string LandscapeScrubberUrl { get; set; }

        [DataMember(Name = "landscapeScrubberHeight", IsRequired = true)]
        public double LandscapeScrubberHeight { get; set; }

        [DataMember(Name = "landscapeScrubberWidth", IsRequired = true)]
        public double LandscapeScrubberWidth { get; set; }


        /// <summary>
        /// Overlays
        /// </summary>
        [DataMember(Name = "audioOverlays", IsRequired = true)]
        public IList<AudioOverlay> AudioOverlays { get; private set; }

        [DataMember(Name = "hyperlinkOverlays", IsRequired = true)]
        public IList<HyperlinkOverlay> HyperlinkOverlays { get; private set; }

        [DataMember(Name = "imagepanOverlays", IsRequired = true)]
        public IList<ImagepanOverlay> ImagepanOverlays { get; private set; }

        [DataMember(Name = "slideshowOverlays", IsRequired = true)]
        public IList<SlideshowOverlay> SlideshowOverlays { get; private set; }

        [DataMember(Name = "webviewOverlays", IsRequired = true)]
        public IList<WebviewOverlay> WebviewOverlay { get; private set; }

        [DataMember(Name = "videoOverlays", IsRequired = true)]
        public IList<VideoOverlay> VideoOverlays { get; private set; }

        
    }
}
