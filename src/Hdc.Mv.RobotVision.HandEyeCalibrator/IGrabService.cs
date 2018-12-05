using System;
using System.Reactive;
using HalconDotNet;

namespace Hdc.Mv.RobotVision.HandEyeCalibrator
{
    public interface IGrabService
    {
        void Initialize();
        void EnableGrabContinue();
        void DisableGrabContinue();
        HImage Grab();
        HImage GrabAsync();
        void Uninitialize();
        object GetParamerter(string paraName);
        void SetParamerter(string paraName, string value);
        void SetParamerter(string paraName, int value);

        IObservable<Unit> InitializedEvent { get; }
    }
}