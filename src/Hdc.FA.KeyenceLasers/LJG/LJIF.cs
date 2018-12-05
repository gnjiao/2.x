using System.Runtime.InteropServices;

namespace Hdc.FA.KeyenceLasers.LJG
{
    public static class LJIF
    {
        #region struct

        //IP address
        [StructLayout(LayoutKind.Sequential)]
        public struct IN_ADDR
        {
            public byte IPAddress1;
            public byte IPAddress2;
            public byte IPAddress3;
            public byte IPAddress4;
        };

        //Ethernet
        [StructLayout(LayoutKind.Sequential)]
        public struct LJIF_OPENPARAM_ETHERNET
        {
            public LJIF.IN_ADDR IPAddress;
        };

        //[StructLayout(LayoutKind.Sequential)]
        public struct LJIF_MEASUREDATA
        {
            public LJIF_FLOATRESULT FloatResult; // Indicates whether data is valid
            public float fValue; // Measurement value when the value is "LJIF_FLOATRESULT_VALID". Indicates an invalid value in other situations.
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct LJIF_MEASUREDATAEX
        {
            public LJIF_FLOATRESULT FloatResult; // Indicates whether data is valid
            public LJIF_MEASURERESULT MeasureResult; // Measurement result
            public float fValue; // Measurement value when the value is "LJIF_FLOATRESULT_VALID". Indicates an invalid value in other situations.
        };

        // Get Storage Status
        [StructLayout(LayoutKind.Sequential)]
        public struct LJIF_GET_STORAGE_STATUS
        {
            public int lSetting; // Storage setting
            public int lStorageNum; // Amount of storage data
            public int lStatus; // Storage status
        };

        //Profile infomation
        [StructLayout(LayoutKind.Sequential)]
        public struct LJIF_PROFILE_INFO
        {
            public float fXStart;
            public float fXPitch;
        };

        //Time structure
        [StructLayout(LayoutKind.Sequential)]
        public struct LJIF_STIME
        {
            public short wYear; // Year 2000-2099
            public short wMonth; // Month, 1/12 (Month/Day)
            public short wDayOfWeek; // Day of the week 0-6 (Sunday - Saturday)
            public short wDay; // Day 1/31 (Month/Day)
            public short wHour; // Hour 0-23
            public short wMinute; // Minute 0-59
            public short wSecond; // Second 0-59
            public short wMillisecond; // Millisecond
        };

        //Measurement Value Information
        [StructLayout(LayoutKind.Sequential)]
        public struct LJIF_DATA_STORAGE
        {
            public int dwDataTime; //Data and time for storing the measurement value
            public LJIF.LJIF_MEASUREDATA MeasureData; //Measurement value
        }

        //Data storage
        public struct LJIF_SETTING_DATA_STORAGE
        {
            public int lCount; //No of data points
            public byte bySkip; //Skipping
        }

        //Profile storage
        public struct LJIF_SETTING_PROFILE_STORAGE
        {
            public byte byTargetProfile; //The profile to acquire
            public byte byStorageCondition; //Storage mode
            public byte byReserve1; //Reserve
            public byte byReserve2; //Reserve
            public int lCount; //No of profiles
        }

        #endregion struct

        #region enum

        // Return Code List
        internal enum rc
        {
            RC_OK = 0x0 , // Completed without errors
            RC_CMD_FIRST = 0x0 ,
            RC_CMD_NO_DATA = 0x2 ,
            RC_CMD_STORAGE_STATE = 0x3 ,
            RC_CMD_NO_STORAGE = 0x4 ,
            RC_CMD_STATE = 0x5 ,
            RC_CMD_LAST = 0xFF ,
            // Communication error from controller notification
            RC_NAK_FIRST = 0x1000 ,
            RC_NAK_COMMAND , // Command error
            RC_NAK_COMMAND_LENGTH , // Command length error
            RC_NAK_TIMEOUT , // Time out
            RC_NAK_CHECKSUM , // Check sum error
            RC_NAK_INVALID_STATE , // Status error
            RC_NAK_OTHER , // Other errors
            RC_NAK_LAST = 0x1FFF ,
            // Communication DLL error notification
            RC_ERR_OPEN_DEVICE = 0x2000 , // Failed to open device
            RC_ERR_NO_DEVICE , // Device is not open
            RC_ERR_SEND , // Command sending error
            RC_ERR_RECEIVE , // Response receiving error
            RC_ERR_TIMEOUT , // Time out
            RC_ERR_CHECKSUM , // Check sum error
            RC_ERR_PARAMETER , // Parameter error
            RC_ERR_NODATA , // No data
            RC_ERR_NOMEMORY , // No free memory
            RC_ERR_OPEN_FILE , // Failed to open file
            RC_ERR_INVALID_FILE , // Target file does not exist
            RC_ERR_READ_FILE , // Failed to load file
            RC_ERR_WRITE_FILE , // Failed to write file
            RC_ERR_DISCONNECT , // Connection may be disconnected
            RC_ERR_UNKNOWN , // Undefined error
            RC_NO_GET_HEAD_TYPE = 0x100 //Unable to acquire head type information error
        }

        //Measurement value structure
        public enum LJIF_FLOATRESULT
        {
            LJIF_FLOATRESULT_VALID = 0 , // Valid data
            LJIF_FLOATRESULT_RANGEOVER_P = 1 , // Overrange at the plus side
            LJIF_FLOATRESULT_RANGEOVER_N = 2 , // Overrange at the minus side
            LJIF_FLOATRESULT_WAITING = 3 // Comparator standby
        }

        //Measurement value structure
        public enum LJIF_MEASURERESULT
        {
            LJIF_GO = 0 , // GO
            LJIF_HI = 1 , // HI
            LJIF_LO = 2 , // LO
            LJIF_WAITINGRESULT = 3 // Comparator standby
        }

        // Storage
        public enum LJIF_STORAGETARGET
        {
            LJIF_STORAGETARGET_OFF = 0 , //OFF
            LJIF_STORAGETARGET_DATA = 1 , //Data storage
            LJIF_STORAGETARGET_PROFILE = 2 //Profile storage
        }

        // Storage status
        public enum LJIF_COMMON_STORAGESTATUS
        {
            LJIF_STORAGESTATUS_IDLE = 0 , //Stop storage
            LJIF_STORAGESTATUS_BUSY = 1 , //Start storage
            LJIF_STORAGESTATUS_READ = 2 //Load storage
        }

        // Setting type
        public enum LJIF_DISPLAY_TARGET
        {
            LJIF_DISPLAY_MEASURE_ALL = 0 , // Number, Multiple screens
            LJIF_DISPLAY_MEASURE_OUTNO_1 = 1 , //Number, Single screen with OUT1
            LJIF_DISPLAY_MEASURE_OUTNO_2 = 2 , //Number, Single screen with OUT2
            LJIF_DISPLAY_MEASURE_OUTNO_3 = 3 , //Number, Single screen with OUT3
            LJIF_DISPLAY_MEASURE_OUTNO_4 = 4 , //Number, Single screen with OUT4
            LJIF_DISPLAY_MEASURE_OUTNO_5 = 5 , //Number, Single screen with OUT5
            LJIF_DISPLAY_MEASURE_OUTNO_6 = 6 , //Number, Single screen with OUT6
            LJIF_DISPLAY_MEASURE_OUTNO_7 = 7 , //Number, Single screen with OUT7
            LJIF_DISPLAY_MEASURE_OUTNO_8 = 8 , //Number, Single screen with OUT8
            LJIF_DISPLAY_PROFILE_HEAD_A = 10 , //Profile display - Head A
            LJIF_DISPLAY_PROFILE_HEAD_B = 11 , //Profile display - Head B
            LJIF_DISPLAY_PROFILE_CALC = 12 //Profile display - After calculation
        }

        //The file No. to use for the new program
        public enum LJIF_FILETARGET
        {
            LJIF_FILE_CONFIG_00 = 0 , // File No.00
            LJIF_FILE_CONFIG_01 = 1 , // File No.01
            LJIF_FILE_CONFIG_02 = 2 , // File No.02
            LJIF_FILE_CONFIG_03 = 3 , // File No.03
            LJIF_FILE_CONFIG_04 = 4 , // File No.04
            LJIF_FILE_CONFIG_05 = 5 , // File No.05
            LJIF_FILE_CONFIG_06 = 6 , // File No.06
            LJIF_FILE_CONFIG_07 = 7 , // File No.07
            LJIF_FILE_CONFIG_08 = 8 , // File No.08
            LJIF_FILE_CONFIG_09 = 9 // File No.09
        }

        //Type of setting
        public enum LJIF_SETTINGTARGET
        {
            LJIF_SETTINGTARGET_PROGRAM_00 = 0 , // Program No.0
            LJIF_SETTINGTARGET_PROGRAM_01 = 1 , // Program No.1
            LJIF_SETTINGTARGET_PROGRAM_02 = 2 , // Program No.2
            LJIF_SETTINGTARGET_PROGRAM_03 = 3 , // Program No.3
            LJIF_SETTINGTARGET_PROGRAM_04 = 4 , // Program No.4
            LJIF_SETTINGTARGET_PROGRAM_05 = 5 , // Program No.5
            LJIF_SETTINGTARGET_PROGRAM_06 = 6 , // Program No.6
            LJIF_SETTINGTARGET_PROGRAM_07 = 7 , // Program No.7
            LJIF_SETTINGTARGET_PROGRAM_08 = 8 , // Program No.8
            LJIF_SETTINGTARGET_PROGRAM_09 = 9 , // Program No.9
            LJIF_SETTINGTARGET_PROGRAM_10 = 10 , // Program No.10
            LJIF_SETTINGTARGET_PROGRAM_11 = 11 , // Program No.11
            LJIF_SETTINGTARGET_PROGRAM_12 = 12 , // Program No.12
            LJIF_SETTINGTARGET_PROGRAM_13 = 13 , // Program No.13
            LJIF_SETTINGTARGET_PROGRAM_14 = 14 , // Program No.14
            LJIF_SETTINGTARGET_PROGRAM_15 = 15 , // Program No.15
            LJIF_SETTINGTARGET_PROGRAM_ALL = 255 //All programs
        }

        // Type of initialization
        public enum LJIF_INITTYPE
        {
            LJIF_INITTYPE_PROGRAM = 0 , // Program
            LJIF_INITTYPE_WITHOUT_ENVIRONMENT = 1 , // Environment setting
            LJIF_INITTYPE_ALL = 2 // All settings
        }

        // The target head
        public enum LJIF_HEAD
        {
            LJIF_HEAD_A = 0 , // Head A
            LJIF_HEAD_B = 1 , // Head B
            LJIF_HEAD_BOTH = 2 , // Both heads
            LJIF_HEAD_CALC = 3 // After calculation
        }

        // OUT setting
        public enum LJIF_OUTNO
        {
            LJIF_OUTNO_1 = 1 , // OUT1
            LJIF_OUTNO_2 = 2 , // OUT2
            LJIF_OUTNO_3 = 4 , // OUT3
            LJIF_OUTNO_4 = 8 , // OUT4
            LJIF_OUTNO_5 = 16 , // OUT5
            LJIF_OUTNO_6 = 32 , // OUT6
            LJIF_OUTNO_7 = 64 , // OUT7
            LJIF_OUTNO_8 = 128 , // OUT8
            LJIF_OUTNO_ALL = 255 // All OUT
        }

        // Setting type
        public enum LJIF_TYPE_SETTING
        {
            LJIF_TYPE_TRIGGER = 1 , // Trigger setting
            LJIF_TYPE_HEAD = 2 , // Head setting
            LJIF_TYPE_PROFILE = 3 , // Profile setting
            LJIF_TYPE_OUT = 6 , // OUT setting
            LJIF_TYPE_COMMON = 7 , // Common setting
            LJIF_TYPE_ENVIRONMENT = 9 // Environment setting
        }

        // Trigger setting
        public enum LJIF_ITEM_TRIGGER
        {
            LJIF_ITEM_TRIGGER_MODE = 0 , // Trigger mode
            LJIF_ITEM_INTERFERENCE = 1 , // Mutual interference prevention
            LJIF_ITEM_AUTOMATIC = 2 , // Multiple trigger ON/OFF
            LJIF_ITEM_AUTO_SETTING = 3 , // Multiple trigger setting
            LJIF_ITEM_TRIGGER_SYNC = 4 , // Trigger synch
            LJIF_ITEM_ASAP_TRIGGER = 5 , // ASAP Trigger
            LJIF_ITEM_ASAP_MODE = 6 // ASAP Mode
        }

        // Head setting
        internal enum LJIF_ITEM_HEAD
        {
            LJIF_ITEM_MEAS_RANGE_X = 0 , // Measurement range (X direction)
            LJIF_ITEM_MEAS_RANGE_Z_FROM = 1 , // Measurement range (Z direction:
            LJIF_ITEM_MEAS_RANGE_Z_TO = 2 , // Measurement range (Z direction: Stop)
            LJIF_ITEM_SAMPLING_TIME = 3 , // Sampling time
            LJIF_ITEM_MFL_MODE = 6 , // MFL(AUTO/MANUAL)
            LJIF_ITEM_MFL_MANUAL = 7 , // MFL MANUAL range
            LJIF_ITEM_MFL_TUNING = 8 , // MFL calibration
            LJIF_ITEM_ALARM_TIMES = 9 , // No. of alarm warnings
            LJIF_ITEM_ALARM_LEVEL = 10 , // Alarm level
            LJIF_ITEM_FILTER = 11 //Filter
        }

        // Profile setting
        internal enum LJIF_ITEM_PROFILE
        {
            LJIF_ITEM_CALCULATION = 4 , // Calculation
            LJIF_ITEM_DELAY_COUNT = 5 , // Delay count
            LJIF_ITEM_SMOOTHING = 6 , // Smoothing
            LJIF_ITEM_AVERAGING = 7 // Averaging
        }

        // OUT setting
        internal enum LJIF_ITEM_OUT
        {
            LJIF_ITEM_ALARM = 3 , // Measurement value alarm
            LJIF_ITEM_SCALING = 4 , // Scaling setting
            LJIF_ITEM_AVERAGE = 5 , // Average measurement value
            LJIF_ITEM_MODE = 6 , // Measurement mode
            LJIF_ITEM_OFFSET = 7 , // Offset
            LJIF_ITEM_TOLERANCE = 8 , // Tolerance
            LJIF_ITEM_UNIT = 9 // Minimum display unit
        }

        // Common setting
        internal enum LJIF_ITEM_COMMON
        {
            LJIF_ITEM_TIMING = 0 , // TIMING
            LJIF_ITEM_ZERO = 1 , // ZERO
            LJIF_ITEM_BINARY = 2 , // Binary output
            LJIF_ITEM_ANA_CH = 3 , // Analog output CH setting
            LJIF_ITEM_ANA_SCALING = 4 , // Analog output scaling
            LJIF_ITEM_ANA_CYCLE = 5 , // Analog output profile update cycle
            LJIF_ITEM_STORAGE = 6 , // Storage setting
            LJIF_ITEM_DATA_STORAGE = 9 , // Data storage setting
            LJIF_ITEM_PROFILE_STORAGE = 10 // Profile storage setting
        }

        // Environment setting
        internal enum LJIF_ITEM_ENVIRONMENT
        {
            LJIF_ITEM_ETHERNET_IP_ADDRESS = 3 , // Ethernet IP address
            LJIF_ITEM_ETHERNET_SUBNET_MASK = 4 , // Ethernet subnet mask
            LJIF_ITEM_ETHERNET_GATEWAY = 5 , // Ethernet gateway
            LJIF_ITEM_STROBE_CYCLE = 7 , // Strobe time
            LJIF_ITEM_LANGUAGE = 8 , // Language setting
            LJIF_ITEM_CABLE_EXTEND_MODE = 10 // Cable extension mode
        }

        //Parameter
        // The target head
        internal enum LJIF_SETTING_TARGET_HEAD
        {
            LJIF_SETTING_HEAD_A = 0 , // Head A
            LJIF_SETTING_HEAD_B = 1 , // Head B
            LJIF_SETTING_HEAD_BOTH = 255 // Both heads
        }

        //Trigger mode
        internal enum LJIF_TRIGGER_MODE
        {
            LJIF_TRIGGER_CONTINU = 0 , //Cont trigger
            LJIF_TRIGGER_OUTER = 1 //External trigger
        }

        //Measurement range
        internal enum LJIF_METROLOGYAREA
        {
            LJIF_METROLOGYAREA_FULL = 0 ,
            LJIF_METROLOGYAREA_1 = 1 ,
            LJIF_METROLOGYAREA_2 = 2 ,
            LJIF_METROLOGYAREA_3 = 3 ,
            LJIF_METROLOGYAREA_4 = 4 ,
            LJIF_METROLOGYAREA_5 = 5 ,
            LJIF_METROLOGYAREA_6 = 6 ,
            LJIF_METROLOGYAREA_7 = 7 ,
            LJIF_METROLOGYAREA_8 = 8 ,
            LJIF_METROLOGYAREA_9 = 9 ,
            LJIF_METROLOGYAREA_10 = 10
        }

        //Sampling time
        internal enum LJIF_SAMPLINGTIME
        {
            LJIF_SAMPLINGTIME_0_3 = 0 ,
            LJIF_SAMPLINGTIME_1 = 1 ,
            LJIF_SAMPLINGTIME_2 = 2 ,
            LJIF_SAMPLINGTIME_3 = 3 ,
            LJIF_SAMPLINGTIME_4 = 4 ,
            LJIF_SAMPLINGTIME_5 = 5 ,
            LJIF_SAMPLINGTIME_6 = 6 ,
            LJIF_SAMPLINGTIME_7 = 7 ,
            LJIF_SAMPLINGTIME_8 = 8 ,
            LJIF_SAMPLINGTIME_9 = 9 ,
            LJIF_SAMPLINGTIME_10 = 10
        }

        //MFL
        internal enum LJIF_TUNING
        {
            LJIF_MFL_TUNING_START = 0 , //Start
            LJIF_MFL_TUNING_STOP = 1 , //Stop
            LJIF_MFL_TUNING_CANCEL = 2 , //Cancel
            LJIF_MFL_TUNING_INIT = 3 , //Initialization
            LJIF_MFL_TUNING_START2 = 4 //Start2
        }

        //Alarm level
        internal enum LJIF_ALARM_LEVEL
        {
            LJIF_ALARM_LEVEL_0 = 0 ,
            LJIF_ALARM_LEVEL_1 = 1 ,
            LJIF_ALARM_LEVEL_2 = 2 ,
            LJIF_ALARM_LEVEL_3 = 3 ,
            LJIF_ALARM_LEVEL_4 = 4 ,
            LJIF_ALARM_LEVEL_5 = 5 ,
            LJIF_ALARM_LEVEL_6 = 6 ,
            LJIF_ALARM_LEVEL_7 = 7 ,
            LJIF_ALARM_LEVEL_8 = 8 ,
            LJIF_ALARM_LEVEL_9 = 9
        }

        //Filter
        internal enum LJIF_FILTER
        {
            LJIF_FILTER_OFF = 0 , //OFF
            LJIF_FILTER_F1 = 1 , //F1
            LJIF_FILTER_F2 = 2 //F2
        }

        // Calculation
        internal enum LJIF_HEADCALC
        {
            LJIF_PROFILE_HEADCALC_NONE = 0 , //OFF
            LJIF_PROFILE_HEADCALC_A_P_B = 1 , //A+B
            LJIF_PROFILE_HEADCALC_A_M_B = 2 , //A-B
            LJIF_PROFILE_HEADCALC_WIDE = 3 , //Wide
            LJIF_PROFILE_HEADCALC_HORZ = 4 , //Linking (H)
            LJIF_PROFILE_HEADCALC_VERT = 5 //Linking (V)
        }

        // Smoothing
        internal enum LJIF_SMOOTHING
        {
            LJIF_PROFILE_SMOOTHING_OFF = 0 ,
            LJIF_PROFILE_SMOOTHING_2 = 1 ,
            LJIF_PROFILE_SMOOTHING_4 = 2 ,
            LJIF_PROFILE_SMOOTHING_8 = 3 ,
            LJIF_PROFILE_SMOOTHING_16 = 4 ,
            LJIF_PROFILE_SMOOTHING_32 = 5 ,
            LJIF_PROFILE_SMOOTHING_64 = 6
        }

        // Averaging
        internal enum LJIF_AVERAGING
        {
            LJIF_PROFILE_AVERAGING_OFF = 0 ,
            LJIF_PROFILE_AVERAGING_2 = 1 ,
            LJIF_PROFILE_AVERAGING_4 = 2 ,
            LJIF_PROFILE_AVERAGING_8 = 3 ,
            LJIF_PROFILE_AVERAGING_16 = 4 ,
            LJIF_PROFILE_AVERAGING_32 = 5 ,
            LJIF_PROFILE_AVERAGING_64 = 6
        }

        // Average measurement value
        internal enum LJIF_METE_AVERAGE
        {
            LJIF_METE_AVERAGE_1 = 0 ,
            LJIF_METE_AVERAGE_2 = 1 ,
            LJIF_METE_AVERAGE_4 = 2 ,
            LJIF_METE_AVERAGE_8 = 3 ,
            LJIF_METE_AVERAGE_16 = 4 ,
            LJIF_METE_AVERAGE_32 = 5 ,
            LJIF_METE_AVERAGE_64 = 6 ,
            LJIF_METE_AVERAGE_128 = 7 ,
            LJIF_METE_AVERAGE_256 = 8 ,
            LJIF_METE_AVERAGE_512 = 9 ,
            LJIF_METE_AVERAGE_1024 = 10 ,
            LJIF_METE_AVERAGE_2048 = 11 ,
            LJIF_METE_AVERAGE_4096 = 12
        }

        // Measurement mode
        internal enum LJIF_METROLOGYMODE
        {
            LJIF_METROLOGYMODE_NORMAL = 0 , //Normal
            LJIF_METROLOGYMODE_PHOLD1 = 1 , //Peak hold 1
            LJIF_METROLOGYMODE_BHOLD1 = 2 , //Bottom hold 1
            LJIF_METROLOGYMODE_DHOLD1 = 3 , //P-to-p hold 1
            LJIF_METROLOGYMODE_AVHOLD1 = 4 , //Average hold 1
            LJIF_METROLOGYMODE_PHOLD2 = 5 , //Peak hold 2
            LJIF_METROLOGYMODE_BHOLD2 = 6 , //Bottom hold 2
            LJIF_METROLOGYMODE_DHOLD2 = 7 , //P-to-p hold 2
            LJIF_METROLOGYMODE_AVHOLD2 = 8 , //Average hold 2
            LJIF_METROLOGYMODE_SHOLD1 = 9 //Sample hold
        }

        // Analog output CH setting
        internal enum LJIF_SETTING_ANALOG_CH
        {
            LJIF_SETTING_CH1 = 0 , // CH1
            LJIF_SETTING_CH2 = 1 , // CH2
            LJIF_SETTING_BOTH_CH = 255 // Both CH
        }

        //Analog output selection
        internal enum LJIF_ANALOG_CH
        {
            LJIF_ANALOG_CH_OUTNO_1 = 0 , // OUT1
            LJIF_ANALOG_CH_OUTNO_2 = 1 , // OUT2
            LJIF_ANALOG_CH_OUTNO_3 = 2 , // OUT3
            LJIF_ANALOG_CH_OUTNO_4 = 3 , // OUT4
            LJIF_ANALOG_CH_OUTNO_5 = 4 , // OUT5
            LJIF_ANALOG_CH_OUTNO_6 = 5 , // OUT6
            LJIF_ANALOG_CH_OUTNO_7 = 6 , // OUT7
            LJIF_ANALOG_CH_OUTNO_8 = 7 , // OUT8
            LJIF_ANALOG_CH_PROFILE_A = 16 ,
            LJIF_ANALOG_CH_PROFILE_B = 17 ,
            LJIF_ANALOG_CH_PROFILE_CALC = 18
        }

        // Prof out update
        internal enum LJIF_COMMON_UPDATECYCLE
        {
            LJIF_COMMON_UPDATECYCLE_20US = 0 ,
            LJIF_COMMON_UPDATECYCLE_50US = 1 ,
            LJIF_COMMON_UPDATECYCLE_100US = 2 ,
            LJIF_COMMON_UPDATECYCLE_500US = 3
        }

        // Storage
        internal enum LJIF_COMMON_STORAGETARGET
        {
            LJIF_COMMON_STORAGETARGET_OFF = 0 , //OFF
            LJIF_COMMON_STORAGETARGET_DATA = 1 , //Data storage
            LJIF_COMMON_STORAGETARGET_PROFILE = 2 //Profile storage
        }

        //Skipping
        internal enum LJIF_DATA_STORAGE_SKIP
        { // Skipping
            LJIF_ITEM_DATA_SKIP_01 = 1 ,
            LJIF_ITEM_DATA_SKIP_02 = 2 ,
            LJIF_ITEM_DATA_SKIP_05 = 5 ,
            LJIF_ITEM_DATA_SKIP_10 = 10 ,
            LJIF_ITEM_DATA_SKIP_20 = 20 ,
            LJIF_ITEM_DATA_SKIP_50 = 50 ,
            LJIF_ITEM_DATA_SKIP_100 = 100
        }

        //Storage mode
        internal enum LJIF_STORAGE_STOCK
        {
            LJIF_PROFILE_STORAGE_STOCK_ANY , //Always
            LJIF_PROFILE_STORAGE_STOCK_NG , //NG
            LJIF_PROFILE_STORAGE_STOCK_MANUAL //Manual
        }

        //Language
        internal enum LJIF_LANGUAGE
        {
            LJIF_LANG_JP = 0 , //Japanese
            LJIF_LANG_EN = 1 , //English
            LJIF_LANG_DE = 2 //German
        }

        #endregion enum

        #region DLL Method

        //''''''''''''''''''''''''''''''''''''''''''''''
        // Measurement Control Command
        //
        //USBOpen Device
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_OpenDeviceUSB();
        //EtherOpen Device
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_OpenDeviceETHER(ref LJIF_OPENPARAM_ETHERNET OPENPARAM);
        //Close Device
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_CloseDevice();
        //Move to Measurement Mode
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetRunMode(int bNotSaveSetting);
        //Move to Communication Mode
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetCommMode();
        //Move to Storage Mode
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetStorageMode();
        //Load Program File
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_LoadProgramFile([MarshalAs(UnmanagedType.VBByRefStr)] ref string MessageText);
        //Save Program File
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SaveProgramFile([MarshalAs(UnmanagedType.VBByRefStr)] ref string MessageText);
        //Load Environment Setting File
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_LoadEnvironmentFile([MarshalAs(UnmanagedType.VBByRefStr)] ref string MessageText);
        //Save Environment Setting File
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SaveEnvironmentFile([MarshalAs(UnmanagedType.VBByRefStr)] ref string MessageText);
        //Set Specified Parameter
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref byte pData, int nData);
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref short pData, int nData);
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref int pData, int nData);
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref LJIF_SETTING_DATA_STORAGE pData, int nData);
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref LJIF_SETTING_PROFILE_STORAGE pData, int nData);
        //Get Specified Parameter
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref byte pData, int nData);
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref short pData, int nData);
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref int pData, int nData);
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref LJIF_SETTING_DATA_STORAGE pData, int nData);
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetSettingItem(LJIF_TYPE_SETTING nType, int nItem, int nTarget, ref LJIF_SETTING_PROFILE_STORAGE pData, int nData);
        //Initialize Settings
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_InitSetting(LJIF_INITTYPE InitType, LJIF_SETTINGTARGET InitTarget);
        //Copy Program
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_CopyProgram(LJIF_SETTINGTARGET SettingFrom, LJIF_SETTINGTARGET SettingTo);
        //Load Program File
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_ChangeProgram(LJIF_SETTINGTARGET SetTarge);
        //Change Program with Saved Program in the CF
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_ChangeProgramInCF(LJIF_FILETARGET nFileTarget, LJIF_SETTINGTARGET SetTarget);
        //Get Active Program No.
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetActiveProgramNo(ref byte pData);
        //Trigger
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetTrigger(LJIF_HEAD Head);
        //Auto Zero
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetAutoZero(int bIsOn, LJIF_OUTNO OutNo);
        //Timing
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetTiming(int bIsOn, LJIF_OUTNO OutNo);
        //Reset
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetReset();
        //Get Measurement Value
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetMeasureValue(ref LJIF_MEASUREDATA pMesureData, int nCount);
        //Get Measurement Value (ExTension)
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetMeasureValueEx(ref LJIF_MEASUREDATAEX pMesureData, int nCount);
        //Get Profile Data
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetProfileData(int ProfileTarget, ref LJIF_PROFILE_INFO pProfileInfo, ref float pProfile, int nCount);
        //Get Precalculation Profile Data
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetPrecalcProfileData(int ProfileTarget, ref LJIF_PROFILE_INFO pProfileInfo, ref float pProfile, int nCount);
        //Change Measurement Screen
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_ChangeDisplay(LJIF_DISPLAY_TARGET DisplayTarget);
        //Set Keylock
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SetKeyLock(int bIsOn);
        //Clear Storage Data
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_ClearStorageData();
        //Get Data from Data Storage
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetDataStorage(int OutNo, ref int pDataCount, ref LJIF_STIME pTime, ref LJIF_DATA_STORAGE pData, int nCount);
        //Get Data from Profile Storage
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetProfileStorage(int ProfileTarget, int nIndex, ref LJIF_STIME pTime, ref LJIF_PROFILE_INFO pProfileInfo, ref LJIF_MEASUREDATA pData, int nCountOfData, ref float pData_Renamed, int nCountOfProfile);
        //Start Storage
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_StartStorage();
        //Stop Storage
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_StopStorage();
        //Get Storage Status
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetStorageStatus(ref LJIF_GET_STORAGE_STATUS pData);
        //Save Storage Data
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_SaveStorage();
        //Get Head Type Information
        [DllImport("LJIF.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int LJIF_GetHeadType(ref byte pHeadTypeA, int nCountHeadA, ref byte pHeadTypeB, int nCountHeadB);

        #endregion DLL Method
    }
}