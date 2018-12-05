using System;
using System.IO;
using System.Text;
using System.Threading;
using Hdc.FA.KeyenceLasers.LJG;
using Hdc.IO;
using Hdc.Measuring.Keyence;
using Hdc.Reflection;

namespace Hdc.Measuring
{
    [Serializable]
    public class LJGInitializer : IInitializer
    {
        private const int SUCCESS = 0;
        private static LJGInitializer _singletone;
        private static bool _isInitialized;

        private readonly string _assemblyDirectoryPath;

        public LJGInitializer()
        {
            _assemblyDirectoryPath = typeof (LJGInitializer).Assembly.GetAssemblyDirectoryPath();
        }

        public void Initialize()
        {
            if (_singletone != null)
            {
                _singletone = this;
                return;
            }

            if (_isInitialized)
                return;

            _isInitialized = true;

            Console.WriteLine($"{nameof(LJGInitializer)}.Initialize(), begin");

            int ret = -1;
            switch (OpenType)
            {
                case OpenType.USB:
                    ret = LJIF.LJIF_OpenDeviceUSB();
                    if (ret != 0)
                        throw new InvalidOperationException($"{nameof(LJGInitializer)}.LJIF_OpenDeviceUSB error");
                    Console.WriteLine($"{nameof(LJGInitializer)}.LJIF_OpenDeviceUSB() OK");
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
                        throw new InvalidOperationException($"{nameof(LJGInitializer)}.LJIF_OpenDeviceETHER error");
                    Console.WriteLine($"{nameof(LJGInitializer)}.LJIF_OpenDeviceETHER() OK");
                    break;
            }

            if (ProgramFile.IsNotNullOrEmpty())
                TransferProgramToController();
        }

        private void TransferProgramToController()
        {
            if (ProgramFile.IsNullOrEmpty()) return;

            string fullPath = null;
            var relativePath = _assemblyDirectoryPath.CombilePath(ProgramFile);

            if (File.Exists(ProgramFile))
            {
                fullPath = ProgramFile;
            }
            else if (File.Exists(relativePath))
            {
                fullPath = relativePath;
            }
            else
            {
                Console.WriteLine($"{nameof(TransferProgramToController)} error. ProgramFile is not exist. " +
                                  $"ProgramFile: {ProgramFile}");
            }


            int rc = 0;

            // LJIF_SetRunMode
            rc = LJIF.LJIF_SetRunMode(0);

            if (rc == SUCCESS)
                Console.WriteLine($"{nameof(LJIF.LJIF_SetRunMode)} ended normally.");
            else
                Console.WriteLine($"{nameof(LJIF.LJIF_SetRunMode)} terminated abnormally."
                                  + "Error Code:0x" + rc.ToString("X"));

            // LJIF_GetHeadType
            byte[] headA = new byte[16];
            byte[] headB = new byte[16];
            string strHeadA = String.Empty;
            string strHeadB = String.Empty;

            rc = LJIF.LJIF_GetHeadType(ref headA[0], 16, ref headB[0], 16);

            if (rc == SUCCESS)
            {
                strHeadA = Encoding.Default.GetString(headA);
                strHeadB = Encoding.Default.GetString(headB);
                Console.WriteLine($"{nameof(LJIF.LJIF_GetHeadType)} ended normally."
                                  + "headA: " + strHeadA
                                  + "headB: " + strHeadB);
            }
            else
            {
                Console.WriteLine($"{nameof(LJIF.LJIF_GetHeadType)} terminated abnormally."
                                  + "Error Code:0x" + rc.ToString("X"));
            }

            // LJIF_SetRunMode
            rc = LJIF.LJIF_SetCommMode();

            if (rc == SUCCESS)
                Console.WriteLine($"{nameof(LJIF.LJIF_SetCommMode)} ended normally.");
            else
                Console.WriteLine($"{nameof(LJIF.LJIF_SetCommMode)} terminated abnormally."
                                  + "Error Code:0x" + rc.ToString("X"));


            // LJIF_LoadProgramFile
            rc = LJIF.LJIF_LoadProgramFile(ref fullPath);
            Console.WriteLine($"{nameof(LJIF.LJIF_LoadProgramFile)} ProgramFile: {fullPath}.");

            if (rc == SUCCESS)
                Console.WriteLine($"{nameof(LJIF.LJIF_LoadProgramFile)} ended normally.");
            else
                Console.WriteLine($"{nameof(LJIF.LJIF_LoadProgramFile)} terminated abnormally."
                                  + "Error Code:0x" + rc.ToString("X"));

            // LJIF_SetRunMode
            rc = LJIF.LJIF_SetRunMode(0);

            if (rc == SUCCESS)
                Console.WriteLine($"{nameof(LJIF.LJIF_SetRunMode)} ended normally.");
            else
                Console.WriteLine($"{nameof(LJIF.LJIF_SetRunMode)} terminated abnormally."
                                  + "Error Code:0x" + rc.ToString("X"));

            // 
            Thread.Sleep(500);
        }

        public static LJGInitializer Singletone => _singletone;

        public OpenType OpenType { get; set; } = OpenType.USB; //
        public int IPAddress1 { get; set; }
        public int IPAddress2 { get; set; }
        public int IPAddress3 { get; set; }
        public int IPAddress4 { get; set; }
        public string ProgramFile { get; set; }
    }
}