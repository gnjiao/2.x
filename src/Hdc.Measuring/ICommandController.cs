using System.Collections.Generic;

namespace Hdc.Measuring
{
    public interface ICommandController: IInitializer
    {
        void Command(MeasureEvent measureEvent);
    }
}