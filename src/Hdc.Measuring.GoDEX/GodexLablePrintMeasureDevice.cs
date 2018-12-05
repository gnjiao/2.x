using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using EasyNetQ;
using EzioDll;
using Vins.ML.Domain;
using Hdc;
using Hdc.Serialization;
using Hdc.Toolkit;

namespace Hdc.Measuring.GoDEX
{
    [Serializable]
    [ContentProperty(nameof(PrintMeasureResultEntries))]
    public class GodexLablePrintMeasureDevice : IMeasureDevice
    {
        private readonly GodexPrinter _printer = new GodexPrinter();
        private readonly Guid _clientGuid = Guid.NewGuid();

        public static GodexPrinter GodexPrinter { get; set; }

        public void Initialize()
        {
            Console.WriteLine($"{nameof(GodexLablePrintMeasureDevice)}.Initialize(), begin");

            if (StationsCount == 0)
            {
                var msg = $"{nameof(GodexLablePrintMeasureDevice)}.StationsCount cannot be 0, app will quit.";
                Console.WriteLine(msg);
                throw new InvalidOperationException(msg);
            }

            InitPrinter();

            if (GodexPrinter != null)
            {
                
            }

            IsInitialized = true;
            Console.WriteLine($"{nameof(GodexLablePrintMeasureDevice)}.Initialize(), end");
        }

        public bool IsInitialized { get; private set; }

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            WorkpieceResult workpieceResult = null;

            if (PrinterDebug_Enabled)
            {
                workpieceResult = PrinterDebug();
            }
            else if (SimulationDebug_Enabled)
            {
                workpieceResult = SimulationFilePath.DeserializeFromXamlFile<WorkpieceResult>();
            }
            else
            {
                QueryWorkpieceResultMqResponse response = null;

                try
                {
                    response = MqInitializer.Bus.Request<QueryWorkpieceResultMqRequest, QueryWorkpieceResultMqResponse>(
                        new QueryWorkpieceResultMqRequest()
                        {
                            ClientGuid = _clientGuid,
                            WorkpieceTag = measureEvent.WorkpieceTag,
                            ExportXamlEnabled = DataServiceExportXamlEnabled,
                        });
                }
                catch (Exception e)
                {
                    MqInitializer.Bus.PublishAsync(new MeasureError()
                    {
                        Exception = e,
                        Message = nameof(GodexLablePrintMeasureDevice) + ".Measure() Error, at " + DateTime.Now,
                        MeasureEvent = measureEvent.DeepClone(),
                    }, p => p.WithExpires(5000));
                    return new MeasureResult() {HasWarning = true};
                }

                if ((StationsCount > 0) && (response.WorkpieceResult.StationResults.Count == StationsCount))
                {
                    workpieceResult = response.WorkpieceResult.DeepClone();
                }
                else
                {
                    workpieceResult = PrinterStationsCountNotEquals(response.WorkpieceResult.Tag, StationsCount,
                        response.WorkpieceResult.StationResults.Count);
                }
            }

            Debug.WriteLine("TotalCountChangedMqEvent QueryWorkpieceResultMqRequest end. at " + DateTime.Now);

            if ((!Printer_Disabled) && (workpieceResult.IsNG2 || Printer_PrintAll))
            {
                try
                {
                    PrintWorkpieceResult(workpieceResult);
                    PrintCompletedCommandController?.Command(measureEvent);
                    Console.WriteLine("GodexLablePrintMeasureDevice.Measure()::PrintCompletedCommandController, at " +
                                      DateTime.Now);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Printer Error! " + e.Message);
                    Console.WriteLine("Printer Error! " + e.Message);
                    //                    throw new InvalidOperationException("PrintWorkpieceResult error", e);
                    PrintErrorCommandController?.Command(measureEvent);
                    Console.WriteLine("GodexLablePrintMeasureDevice.Measure()::PrintErrorCommandController, at " +
                                      DateTime.Now);
                }
            }
            else
            {
                try
                {
                    if (PrintGOsEnabled)
                    {
                        PrintGOsWorkpieceResult(workpieceResult);
                    }

                    PrintPassedCommandController?.Command(measureEvent);
                    Console.WriteLine("GodexLablePrintMeasureDevice.Measure()::PrintPassedCommandController, at " +
                                      DateTime.Now);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Printer Error! " + e.Message);
                    Console.WriteLine("Printer Error! " + e.Message);

                    PrintErrorCommandController?.Command(measureEvent);
                    Console.WriteLine("GodexLablePrintMeasureDevice.Measure()::PrintErrorCommandController, at " +
                                      DateTime.Now);
                }
            }

            return new MeasureResult();
        }

        private WorkpieceResult PrinterDebug()
        {
            WorkpieceResult workpieceResult;
            workpieceResult = SampleGenerator.CreateWorkpieceResult(99, 5);

            var bitArray = new BitArray(new int[] {PrinterDebug_OkNgDecOfByte});
            var reminder = PrinterDebug_OkNgDecOfByte_Counter%8;
            var isOk = bitArray.Get(reminder);

            if (!isOk)
            {
                var calculateResult = workpieceResult.StationResults.First().CalculateResults.First();
                calculateResult.Definition.Name = $"DecOfByte: {PrinterDebug_OkNgDecOfByte}; " +
                                                  $"Counter: {PrinterDebug_OkNgDecOfByte_Counter}; " +
                                                  $"Reminder: {reminder};";
                calculateResult.Definition.StandardValue = PrinterDebug_OkNgDecOfByte_Counter;
                calculateResult.Definition.ToleranceUpper = PrinterDebug_OkNgDecOfByte_Counter*0.1;
                calculateResult.Output.Value = calculateResult.Definition.StandardValue +
                                               calculateResult.Definition.ToleranceUpper + 1;
            }
            else
            {
            }

            PrinterDebug_OkNgDecOfByte_Counter++;
            return workpieceResult;
        }

        private WorkpieceResult PrinterStationsCountNotEquals(int wpcTag, int expectStationsCount,
                        int actualStationsCount)
        {
            var workpieceResult = SampleGenerator.CreateWorkpieceResult(wpcTag, 5);

            var firstStationResult = workpieceResult.StationResults.First();
            firstStationResult.CalculateResults[0].Definition.Name = $"Error, StationsCountNotEquals !!!";
            firstStationResult.CalculateResults[0].Definition.StandardValue = 0;
            firstStationResult.CalculateResults[0].Definition.ToleranceUpper = 0.1;
            firstStationResult.CalculateResults[0].Output.Value = 1;

            firstStationResult.CalculateResults[1].Definition.Name = $"WorkpieceTag: {wpcTag};";
            firstStationResult.CalculateResults[1].Definition.StandardValue = 0;
            firstStationResult.CalculateResults[1].Definition.ToleranceUpper = 1;
            firstStationResult.CalculateResults[1].Output.Value = wpcTag + 1;

            firstStationResult.CalculateResults[2].Definition.Name = $"ExpectStationsCount: {expectStationsCount};";
            firstStationResult.CalculateResults[2].Definition.StandardValue = 0;
            firstStationResult.CalculateResults[2].Definition.ToleranceUpper = 1;
            firstStationResult.CalculateResults[2].Output.Value = expectStationsCount + 1;

            firstStationResult.CalculateResults[3].Definition.Name = $"ActualStationsCount: {actualStationsCount};";
            firstStationResult.CalculateResults[3].Definition.StandardValue = 0;
            firstStationResult.CalculateResults[3].Definition.ToleranceUpper = 1;
            firstStationResult.CalculateResults[3].Output.Value = actualStationsCount + 1;

            return workpieceResult;
        }

        private void InitPrinter()
        {
            while (true)
            {
                try
                {
                    switch (Printer_OpenType)
                    {
                        case OpenType.USB:
                            _printer.Open(PortType.USB);
                            break;
                        case OpenType.Ethernet:
                            _printer.Open(Printer_Ip, Printer_Port);
                            break;
                    }
                    Debug.WriteLine("GodexLablePrintMeasureDevice.InitPrinter(): End. at " + DateTime.Now);
                    break;
                }
                catch (Exception e)
                {
                    MqInitializer.Bus.PublishAsync(new MeasureError()
                    {
                        Exception = e,
                        Message = "Printer Open Error. Type: " + Printer_OpenType
                    }, p => p.WithExpires(5000));
                    Debug.WriteLine("GodexLablePrintMeasureDevice.InitPrinter(): Error. at " + DateTime.Now);
                }

                Thread.Sleep(5000);
            }

            _printer.Config.LabelMode((EzioDll.PaperMode)Printer_PaperMode, Printer_LabelHeight, Printer_LabelGap);
            _printer.Config.LabelWidth(Printer_LabelWidth);
            _printer.Config.Dark(Printer_Dark);
            _printer.Config.Speed(Printer_Speed);
            _printer.Config.PageNo(Printer_PageNo);
            _printer.Config.CopyNo(Printer_CopyNo);
            _printer.Config.LabelsPerCut(Printer_LabelsPerCut);
            _printer.Config.StopPositionSetting(Printer_StopPositionSetting);
            _printer.Config.PrintingMode((EzioDll.PrintingMode)Printer_PrintingMode);
        }

        private void PrintWorkpieceResult(WorkpieceResult workpieceResult)
        {
            var lineIndex = 0;
            var allCalculateResults = workpieceResult.StationResults.SelectMany(x => x.CalculateResults).ToList();

            _printer.Command.Start();

            // Print Head
            PrintHead(workpieceResult.Tag, workpieceResult.IsLevelB);

            // Print Levels
            foreach (var entry in PrintMeasureResultEntries)
            {
                var levelName = string.Empty;
                var cr = allCalculateResults.SingleOrDefault(y => y.Definition.SipNo == entry.SipNo);

                if (cr == null || entry.PrintClass) continue;

                if (entry.Level0Enabled && cr.FinalValue >= entry.Level0Min && cr.FinalValue <= entry.Level0Max)
                    levelName = entry.Level0Name;

                if (entry.Level1Enabled && cr.FinalValue >= entry.Level1Min && cr.FinalValue <= entry.Level1Max)
                    levelName = entry.Level0Name;  

                if (entry.Level2Enabled && cr.FinalValue >= entry.Level2Min && cr.FinalValue <= entry.Level2Max)
                    levelName = entry.Level0Name;
                

                PrintSipEntry(lineIndex, cr.Definition.SipNo, cr.Definition.Name, cr.DeviationOverTolerance, levelName);
                lineIndex++;
            }

            // Print NGs
            var ngCalculateResults = allCalculateResults.Where(x => x.IsNG2).ToList();

            foreach (var result in ngCalculateResults)
            {
                var measureResultEntry = PrintMeasureResultEntries.FirstOrDefault(p => p.SipNo == result.Definition.SipNo);

                if (measureResultEntry?.PrintClass == true)
                {
                    if (result.IsNG2Upper)
                        PrintSipEntry(lineIndex, result.Definition.SipNo, result.Definition.Name, result.DeviationOverTolerance,
                        "超上限");

                    if (result.IsNG2Lower)
                        PrintSipEntry(lineIndex, result.Definition.SipNo, result.Definition.Name, result.DeviationOverTolerance,
                        "超下限");
                }
                else
                {
                    PrintSipEntry(lineIndex, result.Definition.SipNo, result.Definition.Name,
                        result.DeviationOverTolerance,
                        string.Empty);
                }           
                lineIndex++;
            }

            _printer.Command.End();
        }
         
        void PrintClassHead(int wpcTag, string levelName)
        {
            var b = levelName;
            if (!string.IsNullOrEmpty(LineName))
                b += " | " + LineName;

            _printer.Command.PrintText(PosXBegin, PosYBegin, FontHeightTitle, "MS Gothic", ("工件号 " + wpcTag), 0,
                EzioDll.FontWeight.FW_700_BOLD, RotateMode.Angle_0, Italic_State.OFF, Underline_State.OFF,
                Strikeout_State.OFF,
                Inverse_State.ON);
            _printer.Command.PrintText(PosXForDevBegin, PosYBegin, FontHeightTitle, "MS Gothic", $"分类" + b);
        }

        private void PrintGOsWorkpieceResult(WorkpieceResult workpieceResult)
        {
            var lineIndex = 0;
            var allCalculateResults = workpieceResult.StationResults.SelectMany(x => x.CalculateResults).ToList();

            _printer.Command.Start();


            var levelName = string.Empty;



            // Print Levels
            foreach (var entry in PrintMeasureResultEntries)
            {
                            
                var cr = allCalculateResults.SingleOrDefault(y => y.Definition.SipNo == entry.SipNo);
                if (cr == null || entry.PrintClass) continue;

                if (entry.Level0Enabled && cr.FinalValue >= entry.Level0Min && cr.FinalValue <= entry.Level0Max)
                    levelName = entry.Level0Name;

                if (entry.Level1Enabled && cr.FinalValue >= entry.Level1Min && cr.FinalValue <= entry.Level1Max)
                    levelName = entry.Level1Name;

                if (entry.Level2Enabled && cr.FinalValue >= entry.Level2Min && cr.FinalValue <= entry.Level2Max)
                    levelName = entry.Level2Name;

                if (entry.Level3Enabled && cr.FinalValue >= entry.Level3Min && cr.FinalValue <= entry.Level3Max)
                    levelName = entry.Level3Name;

                PrintSipEntry(lineIndex, cr.Definition.SipNo, cr.Definition.Name, cr.DeviationOverTolerance, levelName);
                lineIndex++;
            }


            // Print ClassHead
            //PrintClassHead(workpieceResult.Tag, levelName);
        }

        //private int HeadHeight = 20;
        private int HeadHeight = 18;
        //private int LineGap = 4;
        private int LineGap = 4;
        //int FontHeight = 24;
        int FontHeight = 20;
        //int FontHeightTitle = 28;
        int FontHeightTitle = 18;
        //private int PosXBegin = 2;
        private int PosXBegin = 2;
        //private int PosYBegin = 2;
        private int PosYBegin = 15;
        //private int PosXForDevBegin = 250;
        private int PosXForDevBegin = 220;


        void PrintHead(int wpcTag, bool isLevelB)
        {
            var b = isLevelB ? " (B)" : "";
            if (!string.IsNullOrEmpty(LineName))
                b += " | " + LineName;

            _printer.Command.PrintText(PosXBegin, PosYBegin, FontHeightTitle, "MS Gothic", ("工件号 " + wpcTag), 0,
                EzioDll.FontWeight.FW_700_BOLD, RotateMode.Angle_0, Italic_State.OFF, Underline_State.OFF,
                Strikeout_State.OFF,
                Inverse_State.ON);
            _printer.Command.PrintText(PosXForDevBegin, PosYBegin, FontHeightTitle, "MS Gothic", $"超差" + b);
        }

        void PrintSipEntry(int lineIndex, string sipNo, string deviceName, double deviationOverTolerance,
            string levelName)
        {
            int posY = PosYBegin + HeadHeight + LineGap + (FontHeight + LineGap)*lineIndex;

            _printer.Command.PrintText(PosXBegin, posY, FontHeight, "bold",
                sipNo.PadLeft(4, '0') + " " + deviceName);
            _printer.Command.PrintText(PosXForDevBegin, posY, FontHeight, "MS Gothic",
                deviationOverTolerance.ToString("0.000").PadLeft(8) + " mm|" + levelName);
        }

        /// <summary>
        /// GoDEX EZ2040
        /// </summary>
        public bool Printer_Disabled { get; set; } = false;
        public bool Printer_PrintAll { get; set; } = false;

        //        public string Printer_LabelMode { get; set; } = "GapLabel";
        public int Printer_LabelHeight { get; set; } = 25;//
        public int Printer_LabelWidth { get; set; } = 50;//
        public int Printer_LabelGap { get; set; } = 2;//
        public PaperMode Printer_PaperMode { get; set; } = PaperMode.PlainPaperLabel;//
        //        public double Printer_LabelMarginHorizontal { get; set; } = 1;
        //        public double Printer_LabelMarginVertical { get; set; } = 0;
        public int Printer_Dark { get; set; } = 10;//
        public int Printer_Speed { get; set; } = 3;//
        public int Printer_PageNo { get; set; } = 1;//
        public int Printer_CopyNo { get; set; } = 1;//
        public int Printer_LabelsPerCut { get; set; } = 0;//
        public int Printer_StopPositionSetting { get; set; } = 12;//
        public PrintingMode Printer_PrintingMode { get; set; } = PrintingMode.DirectThermal;//

        public OpenType Printer_OpenType { get; set; } = OpenType.USB;//
        public string Printer_Ip { get; set; } = "127.0.0.1";//
        public int Printer_Port { get; set; } = 0;//

        public bool PrintNGsEnabled { get; set; }
        public bool PrintGOsEnabled { get; set; }

        public bool PrinterDebug_Enabled { get; set; }
        public byte PrinterDebug_OkNgDecOfByte { get; set; }
         int PrinterDebug_OkNgDecOfByte_Counter;

        public int StationsCount { get; set; }
        public string LineName { get; set; }
        public bool DataServiceExportXamlEnabled { get; set; }

        public Collection<PrintMeasureResultEntry> PrintMeasureResultEntries { get; set; } =
            new Collection<PrintMeasureResultEntry>();

        public ICommandController PrintPassedCommandController { get; set; }
        public ICommandController PrintCompletedCommandController { get; set; }
        public ICommandController PrintErrorCommandController { get; set; }

        public bool SimulationDebug_Enabled { get; set; } = false;
        public string SimulationFilePath { get; set; }
    }
}