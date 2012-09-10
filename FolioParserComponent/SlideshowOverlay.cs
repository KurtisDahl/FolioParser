using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class SlideshowOverlay
    {
        public SlideshowOverlay()
        {
            this.OverlayAssets = new List<OverlayAsset>();
            this.OverlayButtons = new List<OverlayButton>();
        }

        public SlideshowOverlay(SlideshowOverlay so)
        {
            this.AutoStart = so.AutoStart;
            this.AutoStartDelay = so.AutoStartDelay;
            this.AutoTransitionDuration = so.AutoTransitionDuration;
            this.CrossFadeImages = so.CrossFadeImages;
            this.CrossFadeImagesDelay = so.CrossFadeImagesDelay;
            this.DefaultSelected = so.DefaultSelected;
            this.DisplayBoundsHeight = so.DisplayBoundsHeight;
            this.DisplayBoundsWidth = so.DisplayBoundsWidth;
            this.DisplayBoundsX = so.DisplayBoundsX;
            this.DisplayBoundsY = so.DisplayBoundsY;
            this.DrawHighlight = so.DrawHighlight;
            this.Height = so.Height;
            this.Id = so.Id;
            this.LoopCount = so.LoopCount;
            this.Orientation = so.Orientation;
            this.OverlayAssets = new List<OverlayAsset>();
            this.OverlayButtons = new List<OverlayButton>();
            this.ReverseImageOrder = so.ReverseImageOrder;
            this.SelectedHighlight = so.SelectedHighlight;
            this.SwipeEnabled = so.SwipeEnabled;
            this.SwipeStop = so.SwipeStop;
            this.TapEnabled = so.TapEnabled;
            this.Width = so.Width;
            this.X = so.X;
            this.Y = so.Y;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "drawHighlight", IsRequired = true)]
        public bool DrawHighlight { get; set; }

        [DataMember(Name = "selectedHighlight", IsRequired = true)]
        public double SelectedHighlight { get; set; }

        [DataMember(Name = "crossFadeImages", IsRequired = true)]
        public bool CrossFadeImages { get; set; }

        [DataMember(Name = "crossFadeImagesDelay", IsRequired = true)]
        public double CrossFadeImagesDelay { get; set; }

        [DataMember(Name = "defaultSelected", IsRequired = true)]
        public bool DefaultSelected { get; set; }

        [DataMember(Name = "autoStart", IsRequired = true)]
        public bool AutoStart { get; set; }

        [DataMember(Name = "autoStartDelay", IsRequired = true)]
        public double AutoStartDelay { get; set; }

        [DataMember(Name = "autoTransitionDuration", IsRequired = true)]
        public double AutoTransitionDuration { get; set; }

        [DataMember(Name = "loopCount", IsRequired = true)]
        public int LoopCount { get; set; }

        [DataMember(Name = "reverseImageOrder", IsRequired = true)]
        public bool ReverseImageOrder { get; set; }

        [DataMember(Name = "tapEnabled", IsRequired = true)]
        public bool TapEnabled { get; set; }

        [DataMember(Name = "swipeEnabled", IsRequired = true)]
        public bool SwipeEnabled { get; set; }

        [DataMember(Name = "swipeStop", IsRequired = true)]
        public bool SwipeStop { get; set; }

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

        [DataMember(Name = "displayBoundsX", IsRequired = true)]
        public double DisplayBoundsX { get; set; }

        [DataMember(Name = "displayBoundsY", IsRequired = true)]
        public double DisplayBoundsY { get; set; }

        [DataMember(Name = "displayBoundsHeight", IsRequired = true)]
        public double DisplayBoundsHeight { get; set; }

        [DataMember(Name = "displayBoundsWidth", IsRequired = true)]
        public double DisplayBoundsWidth { get; set; }

        [DataMember(Name = "buttons", IsRequired = true)]
        public IList<OverlayButton> OverlayButtons { get; set; }

    }
}
