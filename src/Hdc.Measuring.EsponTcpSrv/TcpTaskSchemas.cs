using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("TcpTaskSchema")]
    public class TcpTaskSchemas
    {
        public int StationIndex { get; set; }
        public Collection<TcpTaskSchema> TcpTaskSchema { get; set; } = new Collection<TcpTaskSchema>();
    }
}
