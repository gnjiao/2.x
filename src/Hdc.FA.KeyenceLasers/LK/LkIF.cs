using System;
using System.Runtime.InteropServices;

namespace Hdc.FA.KeyenceLasers.LK
{
    public  static class LkIF
    {

        #region enum
        
        // Measurement value structures
        public  enum LKIF_FLOATRESULT
        {
            LKIF_FLOATRESULT_VALID, // valid data
            LKIF_FLOATRESULT_RANGEOVER_P, // over range at positive (+) side
            LKIF_FLOATRESULT_RANGEOVER_N, // over range at negative (-) side
            LKIF_FLOATRESULT_WAITING // comparator result
        }

        // Set ABLE
        public enum LKIF_ABLEMODE
        {
            LKIF_ABLEMODE_AUTO , // automatic
            LKIF_ABLEMODE_MANUAL // manual
        }

        // Set Measurement Mode
        public enum LKIF_MEASUREMODE
        {
            LKIF_MEASUREMODE_NORMAL , // normal
            LKIF_MEASUREMODE_HALF_T , // translucent object
            LKIF_MEASUREMDOE_TRAN_1 , // transparent object
            LKIF_MEASUREMODE_TRAN_2 , // transparent object 2
            LKIF_MEASUREMODE_MRS // multireflective object
        }

        // Set Mounting Mode
        public enum LKIF_REFLECTIONMODE
        {
            LKIF_REFLECTIONMODE_DIFFUSION , // diffuse reflection
            LKIF_REFLECTIONMODE_MIRROR // specular reflection
        }

        // Set Calculation Method
        public enum LKIF_CALCMETHOD
        {
            LKIF_CALCMETHOD_HEADA , // head A
            LKIF_CALCMETHOD_HEADB , // head B
            LKIF_CALCMETHOD_HEAD_HEADA_PLUS_HEADB , // head A+head B
            LKIF_CALCMETHOD_HEAD_HEADA_MINUS_HEADB , // head A-head B
            LKIF_CALCMETHOD_HEAD_HEADA_TRANSPARENT , // head A transparent object
            LKIF_CALCMETHOD_HEAD_HEADB_TRANSPARENT // head B transparent object
        }

        // Set Filter Mode
        public enum LKIF_FILTERMODE
        {
            LKIF_FILTERMODE_MOVING_AVERAGE , // moving average
            LKIF_FILTERMODE_LOWPASS , // low pass filter
            LKIF_FILTERMODE_HIGHPASS // high pass filter
        }
        // Set Cutoff Frequency
        public enum LKIF_CUTOFFFREQUENCY
        {
            LKIF_CUTOFFFREQUENCY_1000 , // 1000Hz
            LKIF_CUTOFFFREQUENCY_300 , // 300Hz
            LKIF_CUTOFFFREQUENCY_100 , // 100Hz
            LKIF_CUTOFFFREQUENCY_30 , // 30Hz
            LKIF_CUTOFFFREQUENCY_10 , // 10Hz
            LKIF_CUTOFFFREQUENCY_3 , // 3Hz
            LKIF_CUTOFFFREQUENCY_1 , // 1Hz
            LKIF_CUTOFFFREQUENCY_0_3 , // 0.3Hz
            LKIF_CUTOFFFREQUENCY_0_1 // 0.1Hz
        }

        // Set Trigger Mode
        public enum LKIF_TRIGGERMODE
        {
            LKIF_TRIGGERMODE_EXT1 , // external trigger 1
            LKIF_TRIGGERMODE_EXT2 // external trigger 2
        }

        // Set Calculation Mode
        public enum LKIF_CALCMODE
        {
            LKIF_CALCMODE_NORMAL , // normal
            LKIF_CALCMODE_PEAKHOLD , // peak hold
            LKIF_CALCMODE_BOTTOMHOLD , // bottom hold
            LKIF_CALCMODE_PEAKTOPEAKHOLD , // peak-to-peak hold
            LKIF_CALCMODE_SAMPLEHOLD , // sample hold
            LKIF_CALCMODE_AVERAGEHOLD // average hold
        }
        // Measurement target
        public enum LKIF_CALCTARGET
        {
            LKIF_CALCTARGET_PEAK_1 , // peak 1
            LKIF_CALCTARGET_PEAK_2 , // peak 2
            LKIF_CALCTARGET_PEAK_3 , // peak 3
            LKIF_CALCTARGET_PEAK_4 , // peak 4
            LKIF_CALCTARGET_PEAK_1_2 , // peak 1-peak 2
            LKIF_CALCTARGET_PEAK_1_3 , // peak 1-peak 3
            LKIF_CALCTARGET_PEAK_1_4 , // peak 1-peak 4
            LKIF_CALCTARGET_PEAK_2_3 , // peak 2-peak 3
            LKIF_CALCTARGET_PEAK_2_4 , // peak 2-peak 4
            LKIF_CALCTARGET_PEAK_3_4 // peak 3-peak 4
        }

        // Set Minimum Display Unit
        public enum LKIF_DISPLAYUNIT
        {
            LKIF_DISPLAYUNIT_0000_01MM , // 0.01mm
            LKIF_DISPLAYUNIT_000_001MM , // 0.001mm
            LKIF_DISPLAYUNIT_00_0001MM , // 0.0001mm
            LKIF_DISPLAYUNIT_0_00001MM , // 0.00001mm
            LKIF_DISPLAYUNIT_00000_1UM , // 0.1um
            LKIF_DISPLAYUNIT_0000_01UM // 0.01um
        }

        // Set Data Storage
        public enum LKIF_TARGETOUT
        {
            LKIF_TARGETOUT_NONE , // no target OUT
            LKIF_TARGETOUT_OUT1 , // OUT1
            LKIF_TARGETOUT_OUT2 , // OUT2
            LKIF_TARGETOUT_BOTH // OUT1 and OUT2
        }

        public enum LKIF_STORAGECYCLE
        {
            LKIF_STORAGECYCLE_1 , // sampling rate x 1
            LKIF_STORAGECYCLE_2 , // sampling rate x 2
            LKIF_STORAGECYCLE_5 , // sampling rate x 5
            LKIF_STORAGECYCLE_10 , // sampling rate x 10
            LKIF_STORAGECYCLE_20 , // sampling rate x 20
            LKIF_STORAGECYCLE_50 , // sampling rate x 50
            LKIF_STORAGECYCLE_100 , // sampling rate x 100
            LKIF_STORAGECYCLE_200 , // sampling rate x 200
            LKIF_STORAGECYCLE_500 , // sampling rate x 500
            LKIF_STORAGECYCLE_1000 // sampling rate x 1000
        }

        // Set Sampling Rate
        public enum LKIF_SAMPLINGCYCLE
        {
            LKIF_SAMPLINGCYCLE_20USEC , // 20us
            LKIF_SAMPLINGCYCLE_50USEC , // 50us
            LKIF_SAMPLINGCYCLE_100USEC , // 100us
            LKIF_SAMPLINGCYCLE_200USEC , // 200us
            LKIF_SAMPLINGCYCLE_500USEC , // 500us
            LKIF_SAMPLINGCYCLE_1MSEC // 1ms
        }

        // Set Timing Synchronization
        public enum LKIF_SYNCHRONIZATION
        {
            LKIF_SYNCHRONIZATION_ASYNCHRONOUS , // asynchronous
            LKIF_SYNCHRONIZATION_SYNCHRONIZED // synchronous
        }

        // Set Comparator Output Format
        public enum LKIF_TOLERANCE_COMPARATOR_OUTPUT_FORMAT
        {
            LKIF_TOLERANCE_COMPARATOR_OUTPUT_FORMAT_NORMAL , // normal
            LKIF_TOLERANCE_COMPARATOR_OUTPUT_FORMAT_HOLD , // hold
            LKIF_TOLERANCE_COMPARATOR_OUTPUT_FORMAT_OFF_DELAY // off-delay
        }

        // Set Strobe Time
        public enum LKIF_STOROBETIME
        {
            LKIF_STOROBETIME_2MS , // 2ms
            LKIF_STOROBETIME_5MS , // 5ms
            LKIF_STOROBETIME_10MS , // 10ms
            LKIF_STOROBETIME_20MS // 20ms
        }

        // Set Number of Times for Averaging
        public enum LKIF_AVERAGE
        {
            LKIF_AVERAGE_1 , // 1 time
            LKIF_AVERAGE_4 , //
            LKIF_AVERAGE_16 , //
            LKIF_AVERAGE_64 , //
            LKIF_AVERAGE_256 , //
            LKIF_AVERAGE_1024 , //
            LKIF_AVERAGE_4096 , //
            LKIF_AVERAGE_16384 , //
            LKIF_AVERAGE_65536 , //
            LKIF_AVERAGE_262144 // 262144 times
        }

        // Mode Switch
        public enum LKIF_MODE
        {
            LKIF_MODE_NORMAL , // normal mode
            LKIF_MODE_COMMUNICATION // setting mode
        }

        #endregion enum

        #region structure

        [StructLayout(LayoutKind.Sequential)]
        public struct LKIF_FLOATVALUE
        {
            public LKIF_FLOATRESULT FloatResult; // valid or invalid data
            public float Value; // measurement value during LKIF_FLOATRESULT_VALID.
            //Any other times will return an invalid value
        };

        // Statistical Results Output
        [StructLayout(LayoutKind.Sequential)]
        public struct LKIF_FIGUREDATA
        {
            public LKIF_FLOATVALUE ToleUpper; // tolerance upper limit
            public LKIF_FLOATVALUE ToleLower; // tolerance lower limit
            public LKIF_FLOATVALUE AverageValue; // average value
            public LKIF_FLOATVALUE MaxValue; // maximum value
            public LKIF_FLOATVALUE MinValue; // minimum value
            public LKIF_FLOATVALUE DifValue; // maximum value - minimum value
            public LKIF_FLOATVALUE SdValue; // standard deviation
            public int DataCnt; // number of all data
            public int HighDataCnt; // number of tolerance High data
            public int GoDataCnt; // number of tolerance Go data
            public int LowDataCnt; // number of tolerance Low data
        };

        #endregion

        #region dll method

        //''''''''''''''''''''''''''''''''''''''''''''''
        // Measurement Control Command
        //
        // Measurement Value Output
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetCalcData(ref LKIF_FLOATVALUE CalcData1, ref LKIF_FLOATVALUE CalcData2);
        // Timing ON/OFF
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetTiming(int OutNo, int IsOn);
        // Auto-zero ON/OFF
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetZero(int OutNo, int IsOn);
        // Reset
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetReset(int OutNo);
        // Panel Lock
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetPanelLock(int IsLock);
        // Program Change
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetProgramNo(int ProgramNo);
        // Program Check
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetProgramNo(ref int ProgramNo);
        // Statistical Results Output
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetFigureData(int OutNo, ref LKIF_FIGUREDATA FigureData);
        // Clearing Statistics
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_ClearFigureData();
        // Starting the Data Storage
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_DataStorageStart();
        // Stopping the Data Storage
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_DataStorageStop();
        // Initializing the Data Storage
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_DataStorageInit();
        // Outputting the Data Storage
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_DataStorageGetData(int OutNo, int NumOutBuffer, ref LKIF_FLOATVALUE OutBuffer, ref int NumReceived);
        // Data Storage Accumulation Status Output
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_DataStorageGetStatus(int OutNo, ref int IsStorage, ref int NumStorageData);
        // Receive Light Waveform
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetLight(int HeadNo, int PeekNo, ref int MeasurePosition, ref int NumReceived, ref byte Value);

        //''''''''''''''''''''''''''''''''''''''''''''''
        // Change Parameter Command
        //
        // Display Panel Switch
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetPanel(int OutNo);
        // Set Tolerance
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetTolerance(int OutNo, int UpperLimit, int LowerLimit, int Hysteresis);
        // Set ABLE
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetAbleMode(int HeadNo, LKIF_ABLEMODE AbleMode);
        // Set ABLE Control Range
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetAbleMinMax(int HeadNo, int Min, int Max);
        // Set Measurement Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetMeasureMode(int HeadNo, LKIF_MEASUREMODE MeasureMode);
        // Set Number of Times of Alarm Processing
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetNumAlarm(int HeadNo, int NumAlarm);
        // Set Alarm Level
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetAlarmLevel(int HeadNo, int AlarmLevel);
        // Starting the ABLE Calibration
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_AbleStart(int HeadNo);
        // Finishing the ABLE Calibration
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_AbleStop();
        // Stopping the ABLE Calibration
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_AbleCancel();
        // Set Mounting Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetReflectionMode(int HeadNo, LKIF_REFLECTIONMODE ReflectionMode);
        // Set Calculation Method
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetCalcMethod(int OutNo, LKIF_CALCMETHOD CalcMethod, LKIF_CALCTARGET CalcTarget);
        // Set Scaling
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetScaling(int OutNo, int HeadNo, int InputValue1, int OutputValue1, int InputValue2, int OutputValue2);
        // Set Filter Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetFilterMode(int OutNo, LKIF_FILTERMODE FilterMode);
        // Set Number of Times for Averaging
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetAverage(int OutNo, LKIF_AVERAGE Average);
        // Set Cutoff Frequency
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetCutOffFrequency(int OutNo, LKIF_CUTOFFFREQUENCY CutOffFrequency);
        // Set Trigger Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetTriggerMode(int OutNo, LKIF_TRIGGERMODE TriggerMode);
        // Set Offset
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetOffset(int OutNo, int Offset);
        // Set Analog Output Scaling
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetAnalogScaling(int OutNo, int InputValue1, int OutputVoltage1, int InputValue2, int OutputVoltage2);
        // Set Calculation Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetCalcMode(int OutNo, LKIF_CALCMODE CalcMode);
        // Set Minimum Display Unit
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetDisplayUnit(int OutNo, LKIF_DISPLAYUNIT DisplayUnit);
        //Set Analog-Through
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetAnalogThrough(int OutNo, int IsOn);
        //Set Data Storage
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetDataStorage(LKIF_TARGETOUT TargetOut, int NumStorage, LKIF_STORAGECYCLE StorageCycle);
        //Set Sampling Rate
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetSamplingCycle(LKIF_SAMPLINGCYCLE SamplingCycle);
        //Set Mutual Interference Prevention
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetMutualInterferencePrevention(int IsOn);
        //Set Timing Scynchronization
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetTimingSynchronization(LKIF_SYNCHRONIZATION Synchronization);
        //Set Comparator Output Format
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetToleranceComparatorOutputFormat(LKIF_TOLERANCE_COMPARATOR_OUTPUT_FORMAT ToleranceComparatorOutputFormat);
        //Set Strobe Time
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetStorobeTime(LKIF_STOROBETIME StorobeTime);
        //''''''''''''''''''''''''''''''''''''''''''''''
        // Check Parameter Command
        //
        // Display Panel Check
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetPanel(ref int OutNo);
        //Get Tolerance
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetTolerance(int OutNo, ref int UpperLimit, ref int LowerLimit, ref int Hysteresis);
        //Get ABLE
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetAbleMode(int HeadNo, ref LKIF_ABLEMODE AbleMode);
        //ABLE Control Range
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetAbleMinMax(int HeadNo, ref int Min, ref int Max);
        //Get Measurement Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetMeasureMode(int HeadNo, ref LKIF_MEASUREMODE MeasureMode);
        //Get Number of Times of Alarm Processing
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetNumAlarm(int HeadNo, ref int NumAlarm);
        //Get Alarm Level
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetAlarmLevel(int HeadNo, ref int AlarmLevel);
        //Get Mounting Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetReflectionMode(int HeadNo, ref LKIF_REFLECTIONMODE ReflectionMode);
        //Get Calculation Method
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetCalcMethod(int OutNo, ref LKIF_CALCMETHOD CalcMethod, ref LKIF_CALCTARGET CalcTarget);
        //Get Scaling
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetScaling(int OutNo, int HeadNo, ref int InputValue1, ref int OutputValue1, ref int InputValue2, ref int OutputValue2);
        //Get Filter Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetFilterMode(int OutNo, ref LKIF_FILTERMODE FilterMode);
        //Get Number of Times for Averaging
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetAverage(int OutNo, ref LKIF_AVERAGE FilterMoving);
        //Get Cutoff Frequency
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetCutOffFrequency(int OutNo, ref LKIF_CUTOFFFREQUENCY CutOffFrequency);
        //Get Trigger Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetTriggerMode(int OutNo, ref LKIF_TRIGGERMODE TriggerMode);
        //Get Offset
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetOffset(int OutNo, ref int Offset);
        //Get Analog Output Scaling
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetAnalogScaling(int OutNo, ref int InputValue1, ref int OutputVoltage1, ref int InputValue2, ref int OutputVoltage2);
        //Get Calculation Mode
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetCalcMode(int OutNo, ref LKIF_CALCMODE CalcMode);
        //Get Minimum Display Unit
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetDisplayUnit(int OutNo, ref LKIF_DISPLAYUNIT DisplayUnit);
        //Analog-Through
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetAnalogThrough(int OutNo, ref int IsOn);
        //Get Data Storage
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetDataStorage(ref LKIF_TARGETOUT TargetOut, ref int NumStorage, ref LKIF_STORAGECYCLE StorageCycle);
        //Get Sampling Rate
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetSamplingCycle(ref LKIF_SAMPLINGCYCLE SamplingCycle);
        //Get Mutual Interference Prevention
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetMutualInterferencePrevention(ref int IsOn);
        //Get Timing Synchronization
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetTimingSynchronization(ref LKIF_SYNCHRONIZATION Synchronization);
        //Get Comparator Output Format
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetToleranceComparatorOutputFormat(ref LKIF_TOLERANCE_COMPARATOR_OUTPUT_FORMAT ToleranceComparatorOutputFormat);
        //Get Strobe Time
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_GetStorobeTime(ref LKIF_STOROBETIME StorobeTime);
        //''''''''''''''''''''''''''''''''''''''''''''''
        // Mode Change Command
        //
        [DllImport("LkIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LKIF_SetMode(LKIF_MODE Mode);

        #endregion dll method
    }
}