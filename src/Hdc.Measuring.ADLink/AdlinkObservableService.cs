using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Hdc.Reactive;

namespace Hdc.Measuring
{
    public class AdlinkObservableService
    {
        private readonly List<IRetainedSubject<bool>> _subjects = new List<IRetainedSubject<bool>>();

        public void Initialize()
        {
            var thread = new Thread(() =>
            {
                Dispatcher.Run();

                for (int i = 0; i < 16; i++)
                {
                    var subject = new RetainedSubject<bool>();
                    _subjects.Add(subject);
                }

                short ret;
                ret = DASK.Register_Card(DASK.PCI_7230, 0);

                if (ret != 0)
                    throw new InvalidOperationException("AdlinkObservableService: DASK.Register_Card: Error. at " +
                                                        DateTime.Now);

                Debug.WriteLine("AdlinkObservableService: DASK.Register_Card: OK " + DateTime.Now);

                while (true)
                {
                    for (ushort i = 0; i < 16; i++)
                    {
                        ushort value;
                        ret = DASK.DI_ReadLine(0, 0, i, out value);

                        if (ret != 0)
                            throw new InvalidOperationException(
                                "AdlinkObservableService: DASK.DI_ReadLine: Error. at " + DateTime.Now);

                        if(_subjects[i].Value != (value == 1))
                            _subjects[i].Value = (value == 1);
                    }
                    
                    Thread.Sleep(100);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        public IValueObservable<bool> GetObservable(int lineIndex)
        {
            return _subjects[lineIndex];
        }
    }
}