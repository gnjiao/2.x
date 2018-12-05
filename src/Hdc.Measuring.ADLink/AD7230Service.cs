using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Threading;

namespace Hdc.Measuring
{
    public class AD7230Service
    {
        private readonly Subject<Unit> _measureDataReadyRobotEvent = new Subject<Unit>();
        private readonly Subject<Unit> _robotResetCompleatedRobotEvent = new Subject<Unit>();

        public IObservable<Unit> MeasureDataReadyRobotEvent => _measureDataReadyRobotEvent;
        public IObservable<Unit> RobotResetCompleatedRobotEvent => _robotResetCompleatedRobotEvent;

        private static AD7230Service _singletone = new AD7230Service();

        public static AD7230Service Singletone => _singletone;

        public AD7230Service()
        {
            if (_singletone == null)
                _singletone = this;
            else
            {
                throw new Exception("Cannot create AD7230Service more than one.");
            }
        }

        public void Initialize()
        {
            var waitEvent = new AutoResetEvent(false);

            var thread = new Thread(new ThreadStart(() =>
            {
                short ret;
                ret = DASK.Register_Card(DASK.PCI_7230, 0);

                if (ret != 0)
                    throw new InvalidOperationException("DASK.Register_Card: Error. at " + DateTime.Now);

                Debug.WriteLine("DASK.Register_Card: OK " + DateTime.Now);

                ret = DASK.DIO_SetDualInterrupt(0, DASK.INT1_EXT_SIGNAL, DASK.INT2_EXT_SIGNAL, 0);
                if (ret != 0)
                    throw new InvalidOperationException("DASK.DIO_SetDualInterrupt: Error. at " + DateTime.Now);

                ret = DASK.DIO_INT1_EventMessage(0, DASK.INT1_EXT_SIGNAL, 0, 0, new Action(Interupt1Callback));
                if (ret != 0)
                    throw new InvalidOperationException("DASK.DIO_INT1_EventMessage: Error. at " + DateTime.Now);

                ret = DASK.DIO_INT2_EventMessage(0, DASK.INT2_EXT_SIGNAL, 0, 0, new Action(Interupt2Callback));
                if (ret != 0)
                    throw new InvalidOperationException("DASK.DIO_INT2_EventMessage: Error. at " + DateTime.Now);

                waitEvent.Set();
                Dispatcher.Run();
            }));

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            waitEvent.WaitOne();
        }

        private int _interupt1CallbackCounter;
        private int _interupt2CallbackCounter;

        private void Interupt1Callback()
        {
            Debug.WriteLine("Interupt1Callback: " + _interupt1CallbackCounter + ", at " + DateTime.Now);
            _measureDataReadyRobotEvent.OnNext(new Unit());
            _interupt1CallbackCounter++;
        }

        private void Interupt2Callback()
        {
            Debug.WriteLine("Interupt2Callback: " + _interupt2CallbackCounter + ", at " + DateTime.Now);
            _robotResetCompleatedRobotEvent.OnNext(new Unit());
            _interupt2CallbackCounter++;
        }

        public void MeasureDataCompletedForRobot()
        {
            Console.WriteLine("MeasureDataCompletedForRobot begin at " + DateTime.Now);
            DASK.DO_WriteLine(0, 0, 0, 1);
            Thread.Sleep(10);
            DASK.DO_WriteLine(0, 0, 0, 0);
            Console.WriteLine("MeasureDataCompletedForRobot end at " + DateTime.Now);
        }

        public void AllMeasureDataCompletedForRobot()
        {
            Console.WriteLine("AllMeasureDataCompletedForRobot begin at " + DateTime.Now);
            DASK.DO_WriteLine(0, 0, 1, 1);
            Thread.Sleep(10);
            DASK.DO_WriteLine(0, 0, 1, 0);
            Console.WriteLine("AllMeasureDataCompletedForRobot end at " + DateTime.Now);
        }

        public bool ReadLine0()
        {
            ushort value;
            DASK.DI_ReadLine(0, 0, 0, out value);
            var readLine0 = value != 0;

            Console.WriteLine("ReadLine0 == " + value + ", at" + DateTime.Now);
            return readLine0;
        }

        public void OpenLight(int index)
        {
            DASK.DO_WriteLine(0, 0, (ushort) (0 + index), 1);
        }

        public void CloseLight(int index)
        {
            DASK.DO_WriteLine(0, 0, (ushort) (0 + index), 0);
        }

        public void WriteLine(int index, bool value)
        {
            ushort intValue = (ushort) (value ? 1 : 0);
            DASK.DO_WriteLine(0, 0, (ushort) (index), intValue);
        }
    }
}