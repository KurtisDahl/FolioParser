﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class HyperlinkOverlay
    {
        public HyperlinkOverlay()
        {
        }

        public HyperlinkOverlay(HyperlinkOverlay ho)
        {
            this.Height = ho.Height;
            this.Id = ho.Id;
            this.OpenInApp = ho.OpenInApp;
            this.Orientation = ho.Orientation;
            this.ReqNavConfirm = ho.ReqNavConfirm;
            this.Url = ho.Url;
            this.Width = ho.Width;
            this.X = ho.X;
            this.Y = ho.Y;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "url", IsRequired = true)]
        public string Url { get; set; }

        [DataMember(Name = "reqNavConfirm", IsRequired = true)]
        public bool ReqNavConfirm { get; set; }

        [DataMember(Name = "openInApp", IsRequired = true)]
        public bool OpenInApp { get; set; }

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
    }
}
