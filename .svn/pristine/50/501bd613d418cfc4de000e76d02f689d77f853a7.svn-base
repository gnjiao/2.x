using System;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Contexts;
using System.Windows.Markup;

namespace ReportsMergeTool
{
    [Serializable]
    public class Config
    {
         public Collection<ReportDef>  SourceReportDefs { get; set; } = new Collection<ReportDef>();
         public Collection<ReportDef>  TargetReportDefs { get; set; } = new Collection<ReportDef>();
    }

    [Serializable]
    public class ReportDef
    {
        public string SourceFileName { get; set; }
        public string TargetFileName { get; set; }
    }

    [Serializable]
    [ContentProperty("ValueClips")]
    public class ValueGroupDef
    {
        public ValueClipDef SipNoClip { get; set; }
        public ValueClipDef NameClip { get; set; }
        public ValueClipDef ToleranceUpperClip { get; set; }
        public ValueClipDef ToleranceLowerClip { get; set; }
        public ValueClipDef ToolClip { get; set; }
        public ValueClipDef Comment1Clip { get; set; }
        public ValueClipDef Comment2Clip { get; set; }
        public Collection<ValueClipDef> ValueClips { get; set; }
    }

    [Serializable]
    public class ValueClipDef
    {
        public int StartRow { get; set; }
        public int StartColumn { get; set; }
        public Direction Direction { get; set; }
        public int Count { get; set; }
    }

    public enum Direction
    {
        LeftToRight,
        TopToBottom,
    }
}