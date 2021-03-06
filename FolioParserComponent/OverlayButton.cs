﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioParserComponent
{
    [DataContract]
    public sealed class OverlayButton
    {
        [DataMember(Name = "defaultId", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "targetId", IsRequired = true)]
        public string TargetId { get; set; }

        [DataMember(Name = "selectedId", IsRequired = true)]
        public string SelectedId { get; set; }

        [DataMember(Name = "stopEnabled", IsRequired = true)]
        public bool StopEnabled { get; set; }

        [DataMember(Name = "x", IsRequired = true)]
        public double X { get; set; }

        [DataMember(Name = "y", IsRequired = true)]
        public double Y { get; set; }

        [DataMember(Name = "width", IsRequired = true)]
        public double Width { get; set; }

        [DataMember(Name = "height", IsRequired = true)]
        public double Height { get; set; }
    }
}
