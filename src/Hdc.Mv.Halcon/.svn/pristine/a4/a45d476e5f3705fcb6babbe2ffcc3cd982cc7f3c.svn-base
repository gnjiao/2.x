using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Mv.Halcon
{
    /// <summary>
    /// Available Attributes: 
    /// - BrowsableAttribute
    /// - CategoryAttribute
    /// - DisplayNameAttribute
    /// - ReadOnlyAttribute
    /// - DescriptionAttribute
    ///  
    /// 
    /// Alternative Attributes:
    /// - TypeDescriptionProviderAttribute
    /// - DesignOnlyAttribute
    /// - EditorAttribute
    /// - EditorBrowsableAttribute 
    /// - DefaultPropertyAttribute
    /// - DefaultValueAttribute
    /// - AttributeProviderAttribute 
    /// - BindableAttribute
    /// - LocalizableAttribute 
    /// - TypeConverterAttribute 
    ///  
    /// </summary>
    [Serializable]
    [ContentProperty("PortReferences")]
    public abstract class Block : IBlock
    {
        public virtual void Initialize()
        {
            Exception = null;
            Message = null;
            Status = BlockStatus.Initilaized;
        }

        public virtual void Uninitialize()
        {
            Exception = null;
            Message = null;
            Status = BlockStatus.Uninitialized;
        }

        public abstract void Process();

        [Category(BlockPropertyCategories.Runtime)]
        [Browsable(true)]
        [ReadOnly(true)]
        public BlockStatus Status { get; set; } = BlockStatus.Uninitialized;

        [Category(BlockPropertyCategories.Common)]
        [Browsable(true)]
        [ReadOnly(true)]
        public string Name { get; set; }

        [Category(BlockPropertyCategories.Runtime)]
        [Browsable(true)]
        [ReadOnly(true)]
        public string Message { get; set; }

        [Category(BlockPropertyCategories.Runtime)]
        [Browsable(true)]
        [ReadOnly(true)]
        public Exception Exception { get; set; }

        [Browsable(false)]
        [ReadOnly(true)]
        public Collection<PortReference> PortReferences { get; set; } = new Collection<PortReference>();

        public bool AddPortReference(string targetPortName, string sourceFunctionBlockName = null, string sourcePortName = null)
        {
            var exist = PortReferences.Any(x => x.TargetPortName == targetPortName);
            if (exist)
                return false;

            PortReferences.Add(new PortReference(targetPortName, sourceFunctionBlockName, sourcePortName));
            return true;
        }
    }
}