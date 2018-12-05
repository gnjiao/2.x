using System;
using System.Collections.ObjectModel;
using System.Windows;

// ReSharper disable InconsistentNaming

namespace Hdc.Mv.RobotVision.HandEyeCalibrator
{
    [Serializable]
    public class Config
    {
        public string Mv_CalibInspectionSchemaDirctory { get; set; } = "InspectionSchema_Calib";
        public string Mv_OriInspectionSchemaDirctory { get; set; } = "InspectionSchema_Ori";
        public string Mv_RefInspectionSchemaDirctory { get; set; } = "InspectionSchema_Ref";

//        public Collection<Vector> CalibPlate_CameraVectors { get; set; } = new Collection<Vector>();
//        public Collection<Vector> CalibPlate_WorldVectors { get; set; } = new Collection<Vector>();
//        public Vector CalibPlate_ToolInBaseVector { get; set; } = new Vector();
//
//        public Vector Source_OriToolInBaseVector { get; set; } = new Vector();
//        public Vector Source_RefToolInBaseVector { get; set; } = new Vector();
//
//        public Vector Source_OriInCamVector{ get; set; } = new Vector();
//        public Vector Source_RefInCamVector{ get; set; } = new Vector();

        /// <summary>
        /// GigEVision
        /// File
        /// </summary>
        public string OpenFramegrabber_Name { get; set; } = "File";

        /// <summary>
        /// GtlForceIP=c42f90ff00d6,192.168.55.50/24
        /// -1
        /// </summary>
        public string OpenFramegrabber_Generic { get; set; } = "-1";

        /// <summary>
        /// default
        /// B:\
        /// </summary>
        public string OpenFramegrabber_CameraType { get; set; } = @"B:/";

        public double ExposureTime { get; set; } = 10000;
    }
}

// ReSharper restore InconsistentNaming