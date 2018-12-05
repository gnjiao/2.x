using System;
using Hdc.Boot;
using Hdc.Mercury;
using Hdc.Mercury.Communication;
using Hdc.Mercury.Communication.OPC.Xi;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using UnityServiceLocator = Hdc.Unity.UnityServiceLocator;

namespace Hdc.Measuring
{
    [Serializable]
    public class OpcInitializer : IInitializer
    {
        private BootstrapperRunner _bootstrapperRunner;
        private static IDeviceGroupProvider _deviceGroupProvider;
        private static bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException();

            _isInitialized = true;

            _bootstrapperRunner = new BootstrapperRunner()
                .AddExtension(
                    new ActionBootstrapperExtension(
                        x =>
                        {
                            x.RegisterType<IServiceLocator, UnityServiceLocator>(
                                new ContainerControlledLifetimeManager());
                        }))
                .AddExtension<MercuryBootstrapperExtension>()
                .AddExtension<XiBootstrapperExtension>()
                .AddExceptionHandler(ex =>
                {
                    //MessageBox.Show("Unhandled Exception!\n\n" + ex.Message + ex.StackTrace);
                    Environment.Exit(1);
                })
                .Run();

            var accessChannelController = _bootstrapperRunner.Container.Resolve<IAccessChannelController>();

            _deviceGroupProvider = _bootstrapperRunner.Container.Resolve<IDeviceGroupProvider>();

            accessChannelController
                .Config(cfg =>
                {
                    cfg.ServerUrl = OpcXi_ServerUrl; //"http://192.168.100.100/XiTOCO"; //"da:Takebishi.Dxp.1";
                    cfg.UserName = OpcXi_UserName;// "WS01";
                    cfg.Password = OpcXi_Password;// "hdcrd.com";
                });

            _deviceGroupProvider = _bootstrapperRunner.Container.Resolve<IDeviceGroupProvider>();

            if (!ManualStart)
                accessChannelController.Start(AccessChannelNames.AccessChannelFactory);
        }

        public void Start()
        {
            var accessChannelController = _bootstrapperRunner.Container.Resolve<IAccessChannelController>();
            Console.WriteLine("OpcInitializer.Start(): begin");
            accessChannelController.Start(AccessChannelNames.AccessChannelFactory);
            Console.WriteLine("OpcInitializer.Start(): end");
        }

        public static IDeviceGroupProvider DeviceGroupProvider => _deviceGroupProvider;

        public string OpcXi_ServerUrl { get; set; } = "da:Takebishi.Dxp.1";
        public string OpcXi_UserName { get; set; } = "";
        public string OpcXi_Password { get; set; } = "";
        public bool ManualStart { get; set; }
    }
}