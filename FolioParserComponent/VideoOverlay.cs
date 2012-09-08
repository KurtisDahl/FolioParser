﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class VideoOverlay
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "videoUrl", IsRequired = true)]
        public string VideoUrl { get; set; }

        [DataMember(Name = "playInContext", IsRequired = true)]
        public bool PlayInContext { get; set; }

        [DataMember(Name = "showControlsByDefault", IsRequired = true)]
        public bool ShowControlsByDefault { get; set; }

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
    }
}
