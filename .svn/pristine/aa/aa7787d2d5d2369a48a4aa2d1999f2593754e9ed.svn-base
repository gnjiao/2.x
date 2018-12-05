using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;
using Hdc.FA.KeyenceLasers.LJG;
using Hdc.Measuring.Keyence;
using Hdc.Reactive.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(EventController))]
    public class LJGGetDataStorageMeasureDevice : IMeasureDevice
    {
        private int _tagCounter;
        private static bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
                return;

            Console.WriteLine("LJGGetDataStorageMeasureDevice.Initialize(), begin");

            int ret = -1;
            switch (OpenType)
            {
                case OpenType.USB:
                    ret = LJIF.LJIF_OpenDeviceUSB();
                    if (ret != 0)
                        throw new InvalidOperationException("LJGGetDataStorageMeasureDevice.LJIF_OpenDeviceUSB error");
                    Console.WriteLine("LJGGetDataStorageMeasureDevice.LJIF_OpenDeviceUSB() OK");
                    break;
                case OpenType.Ethernet:
                    var para = new LJIF.LJIF_OPENPARAM_ETHERNET()
                    {
                        IPAddress = new LJIF.IN_ADDR()
                        {
                            IPAddress1 = (byte) IPAddress1,
                            IPAddress2 = (byte) IPAddress2,
                            IPAddress3 = (byte) IPAddress3,
                            IPAddress4 = (byte) IPAddress4,
                        }
                    };
                    ret = LJIF.LJIF_OpenDeviceETHER(ref para);
                    if (ret != 0)
                        throw new InvalidOperationException("LJGGetDataStorageMeasureDevice.LJIF_OpenDeviceETHER error");
                    Console.WriteLine("LJGGetDataStorageMeasureDevice.LJIF_OpenDeviceETHER() OK");
                    break;
            }

            EventController.Initialize();
            EventController.Event.ObserveOnTaskPool()
                .Subscribe(measureEvent => { _measureEvents.Add(measureEvent); });

            ret = LJIF.LJIF_StopStorage();
//            if (ret != 0)
//                throw new InvalidOperationException("LJGGetDataStorageMeasureDevice.LJIF_StopStorage error");

            ret = LJIF.LJIF_ClearStorageData();
//            if (ret != 0)
//                throw new InvalidOperationException("LJGGetDataStorageMeasureDevice.LJIF_ClearStorageData error");
//            Console.WriteLine("LJGGetDataStorageMeasureDevice.LJIF_ClearStorageData() OK");

            _isInitialized = true;
            Console.WriteLine("LJGGetDataStorageMeasureDevice.Initialize(), end");
        }

        public bool IsInitialized => _isInitialized;


        public OpenType OpenType { get; set; } = OpenType.USB; //
        public int IPAddress1 { get; set; }
        public int IPAddress2 { get; set; }
        public int IPAddress3 { get; set; }
        public int IPAddress4 { get; set; }

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            LJIF.LJIF_StopStorage();

            int nCount = PointCount; //data number
            var zValues = GetZValuesFromLJG(nCount);
            var xValues = _measureEvents.Select(x => x.X).ToArray();
            var yValues = _measureEvents.Select(x => x.Y).ToArray();

            var measureResult = new MeasureResult()
            {
                Tag = _tagCounter,
                MeasureDateTime = DateTime.Now,
            };

            AppendFlatnessErrorOutputs(measureResult, xValues, yValues, zValues, 0);

            if (AlternativePointsStrings != null)
            {
                for (int i = 0; i < AlternativePointsStrings.Count; i++)
                {
                    var alternativePointsString = AlternativePointsStrings[i];
                    var indexs = alternativePointsString.Split(',').Select(int.Parse).ToList();

                    var subXValues = xValues.Where((t, j) => indexs.Contains(j)).ToArray();
                    var subYValues = yValues.Where((t, j) => indexs.Contains(j)).ToArray();
                    var subZValues = zValues.Where((t, j) => indexs.Contains(j)).ToArray();

                    AppendFlatnessErrorOutputs(measureResult, subXValues, subYValues, subZValues, (i + 1)*100);
                }
            }

            _measureEvents.Clear();
            return measureResult;
        }

        private void AppendFlatnessErrorOutputs(MeasureResult measureResult, double[] xValues, double[] yValues,
            double[] zValues, int startIndex)
        {
            double[] distances;
            double f;
            FlatnessErrorOfA.FlatnessErrorOfACalculate(xValues, yValues, zValues, out distances, out f);


            if (startIndex < measureResult.Outputs.Count)
                throw new InvalidOperationException();

            // reserve blank
            while (startIndex > measureResult.Outputs.Count)
            {
                var index = measureResult.Outputs.Count;
                measureResult.Outputs.Add(new MeasureOutput()
                {
                    Index = index,
                    Validity = MeasureValidity.Valid,
                    Judge = MeasureOutputJudge.Go,
                    Value = -999.999,
                });
            }

            measureResult.Outputs.Add(new MeasureOutput()
            {
                Index = startIndex,
                Validity = MeasureValidity.Valid,
                Judge = MeasureOutputJudge.Go,
                Value = f,
            });

            for (int i = 0; i < PointCount; i++)
            {
                measureResult.Outputs.Add(new MeasureOutput()
                {
                    Index = startIndex + i + 1,
                    Validity = MeasureValidity.Valid,
                    Judge = MeasureOutputJudge.Go,
                    Value = distances[i],
                });
            }
        }

        private static double[] GetZValuesFromLJG(int nCount)
        {
            double[] zValues = new double[nCount];

            LJIF.LJIF_DATA_STORAGE[] OutBuffer = new LJIF.LJIF_DATA_STORAGE[65536];
            int nDataOutCount = 0;
            LJIF.LJIF_STIME BaseTime = new LJIF.LJIF_STIME();

            int outNo = 1;
            int rc = LJIF.LJIF_GetDataStorage(outNo, ref nDataOutCount, ref BaseTime, ref OutBuffer[0], nCount);

            if (rc != 0)
            {
                throw new InvalidOperationException("LJGGetDataStorageMeasureDevice.LJIF_GetDataStorage error");
            }

            for (int i = 0; i < nCount; i++)
            {
                var ljifDataStorage = OutBuffer[i];
                Console.WriteLine($"Point{i:D2}={ljifDataStorage.MeasureData.fValue}");
                zValues[i] = ljifDataStorage.MeasureData.fValue;
            }
            return zValues;
        }

        readonly List<MeasureEvent> _measureEvents = new List<MeasureEvent>();

        public IEventController EventController { get; set; }

        public int PointCount { get; set; }

        public Collection<string> AlternativePointsStrings { get; set; } = new Collection<string>();
    }
}