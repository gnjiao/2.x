using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using Hdc.Mv.Inspection;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("Items")]
    public class ComposeCommandController : Collection<ICommandController>, ICommandController
    {
        public void Initialize()
        {
            foreach (var commandController in Items)
            {
                commandController.Initialize();
            }
        }

        public void Command(MeasureEvent measureEvent)
        {
            foreach (var commandController in Items)
            {
                commandController.Command(measureEvent);
            }
        }
    }
}