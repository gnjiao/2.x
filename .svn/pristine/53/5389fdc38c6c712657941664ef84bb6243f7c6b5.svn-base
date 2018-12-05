using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasyNetQ;
using Hdc.Measuring;
using Hdc.Toolkit;
using Vins.ML.Domain;

namespace MqTestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IBus _bus;
        private Thread _mqThread;
        private AutoResetEvent InitEasyNetQEvent = new AutoResetEvent(false);
        private readonly Guid _clientGuid = Guid.NewGuid();
        private int _workpieceCounter;

        //        private bool watchdog;
        private DateTime watchdogDateTime;

        public MainWindow()
        {
            InitializeComponent();
            InitEasyNetQ();
            InitEasyNetQEvent.WaitOne();

            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(x =>
                {
                    var timeSpan = DateTime.Now - watchdogDateTime;
                    if (timeSpan.TotalSeconds > 5)
                    {
                        WatchdogTextBox.Text = "timeout";
                    }
                    else
                    {
                        WatchdogTextBox.Text = "online";
                    }
                });

            this.Closed += (sender, e) => Environment.Exit(0);
        }

        private void InitEasyNetQ()
        {
            _mqThread = new Thread(() =>
            {
                _bus = EasyNetQEx.CreateBusAndWaitForConnnected(
                    ConfigProvider.Config.EasyNetQ_Host,
                    ConfigProvider.Config.EasyNetQ_VirtualHost,
                    ConfigProvider.Config.EasyNetQ_Username,
                    ConfigProvider.Config.EasyNetQ_Password,
                    5000,
                    isConnected => Debug.WriteLine(
                        $"{nameof(MqTestClient)}:: IBus.IsConnected: {isConnected}, at {DateTime.Now}"));

                _bus.Subscribe<GeneralMqEvent>(_clientGuid.ToString(),
                    x => { Debug.WriteLine("GeneralMqEvent raised: " + x.EventName); });

                /*     _bus.Respond<QueryWorkpieceResultMqRequest, QueryWorkpieceResultMqResponse>(request =>
                     {
                         Debug.WriteLine("QueryWorkpieceResultMqRequest raised, at " + DateTime.Now);


                         var workpieceResult = GetWorkpieceResult();
                         workpieceResult.Tag = request.WorkpieceTag;

                         _workpieceCounter++;

                         var response = new QueryWorkpieceResultMqResponse()
                         {
                             ClientGuid = request.ClientGuid,
                             WorkpieceResult = workpieceResult,
                             WorkpieceTag = request.WorkpieceTag,
                         };

                         Debug.WriteLine("QueryWorkpieceResultMqRequest Responsed, at " + DateTime.Now);

                         return response;
                     });*/

                _bus.Subscribe<LauncherWatchdogMqEvent>(_clientGuid.ToString(),
                    x =>
                    {
                        watchdogDateTime = x.DateTime;
                        Debug.WriteLine("LauncherWatchdogMqEvent raised: " + x.StationName);
                    });

                InitEasyNetQEvent.Set();

                System.Windows.Threading.Dispatcher.Run();
            });

            _mqThread.SetApartmentState(ApartmentState.MTA);
            _mqThread.IsBackground = true;
            _mqThread.Start();
        }

        private void WpcInPosition0Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 0,
                DeviceName = "Test0"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition1Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 1,
                DeviceName = "Test01"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition2Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 2,
                DeviceName = "Test02"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition3Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 3,
                DeviceName = "Test03"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition4Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 4,
                DeviceName = "Test04"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition5Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 5,
                DeviceName = "Test05"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition6Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 6,
                DeviceName = "Test06"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition7Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 7,
                DeviceName = "Test07"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition8Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 8,
                DeviceName = "Test08"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition9Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 9,
                DeviceName = "Test09"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition10Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 10,
                DeviceName = "Test10"
            }, p => p.WithExpires(5000));
        }

        private void WpcInPosition11Button_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new WorkpieceInPositionEvent()
            {
                DateTime = DateTime.Now,
                StationIndex = 11,
                DeviceName = "Test11"
            }, p => p.WithExpires(5000));
        }


        WorkpieceResult GetWorkpieceResult()
        {
            return new WorkpieceResult()
            {
                StationResults = new List<StationResult>()
                {
                    new StationResult()
                    {
                        CalculateResults = new List<CalculateResult>()
                        {
                            new CalculateResult()
                            {
                                Definition = new CalculateDefinition() {SipNo = "#1", Name = "比"},
                                //                                
                                Output = new MeasureOutput() {Value = 59}
                            },
                            new CalculateResult()
                            {
                                Definition = new CalculateDefinition() {SipNo = "#2", Name = "Biyadiquanjianxian"},
                                //                                
                                Output = new MeasureOutput() {Value = 509}
                            },
                            new CalculateResult()
                            {
                                Definition = new CalculateDefinition() {SipNo = "#3", Name = "迪全检线"},
                                //                                
                                Output = new MeasureOutput() {Value = 159}
                            },
                            new CalculateResult()
                            {
                                Definition = new CalculateDefinition() {SipNo = "#4", Name = "比亚迪全检线项目"},
                                //                                
                                Output = new MeasureOutput() {Value = 259}
                            },
                            new CalculateResult()
                            {
                                Definition = new CalculateDefinition() {SipNo = "#5", Name = "比比比比比比比比"},
                                //                                
                                Output = new MeasureOutput() {Value = 9}
                            },
                            new CalculateResult()
                            {
                                Definition = new CalculateDefinition() {SipNo = "#6", Name = "BYD"},
                                //                                
                                Output = new MeasureOutput() {Value = 9}
                            },
                            new CalculateResult()
                            {
                                Definition = new CalculateDefinition() {SipNo = "#7", Name = "我"},
                                //                                
                                Output = new MeasureOutput() {Value = 9}
                            },
                            new CalculateResult()
                            {
                                Definition = new CalculateDefinition() {SipNo = "#8", Name = "workpiecename"},
                                //                                
                                Output = new MeasureOutput() {Value = 9}
                            }
                        }
                    }
                }
            };
        }

        private void OpcTestButton_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10000; i++)
            {
                var mod = i % 2;
                if (mod == 0)
                {
                    OpcTestForLeft();
                }
                else if (mod == 1)
                {
                    OpcTestForRight();
                }

                //                Thread.Sleep(3000);
            }
        }

        private void OpcTestForLeftButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpcTestForLeft();
        }

        private void OpcTestForLeft()
        {
            for (int j = 0; j < 100; j++)
            {
                _bus.PublishAsync(new GeneralMqCommand()
                {
                    CommandName = "StationCompletedAppEvent" + j.ToString("D2"),
                }, p => p.WithExpires(5000));
            }
        }

        private void OpcTestForRightButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpcTestForRight();
        }

        private void OpcTestForRight()
        {
            for (int j = 0; j < 100; j++)
            {
                _bus.PublishAsync(new GeneralMqCommand()
                {
                    CommandName = "StationCompletedAppEvent" + (j + 100).ToString("D2"),
                }, p => p.WithExpires(5000));
            }
        }

        private void ForcePrintGo_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new ForcePrintGoMqEvent()
            {
            }, p => p.WithExpires(5000));
        }

        private void ForcePrintNg_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.PublishAsync(new ForcePrintNgMqEvent()
            {
            }, p => p.WithExpires(5000));
        }
    }
}