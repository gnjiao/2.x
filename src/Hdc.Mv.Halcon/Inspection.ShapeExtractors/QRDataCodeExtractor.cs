using System;
using System.Diagnostics;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class QRDataCodeExtractor : IDataCodeExtractor
    {
        private bool _isInitilized;
        private HDataCode2D _hDataCode2D;

        // _hDataCode2D abnormal
        /*        public string FindDataCode(HImage image)
                {
                    if (!_isInitilized)
                    {
                        _isInitilized = true;

                        _hDataCode2D = new HDataCode2D();
                        _hDataCode2D.CreateDataCode2dModel("QR Code", GenParamNamesOfModel, GenParamValuesOfModel);
                    }

                    int resultHandler;
                    string decodeString;
        //            _hDataCode2D.FindDataCode2d(image, GenParamNamesOfFind, GenParamValuesOfFind, out resultHandler, out decodeString);
                    _hDataCode2D.FindDataCode2d(image, new HTuple(), GenParamValuesOfFind, out resultHandler, out decodeString);

                    return decodeString;
                }*/

        public string FindDataCode(HImage image)
        {
            if (!_isInitilized)
            {
                _isInitilized = true;

//                _hDataCode2D = new HDataCode2D();
//                _hDataCode2D.CreateDataCode2dModel("QR Code", GenParamNamesOfModel, GenParamValuesOfModel);
            }



            // Local iconic variables 

            HObject  ho_SymbolXLDs;

            // Local control variables 

            HTuple hv_DataCodeHandle = null, hv_ResultHandles = null;
            HTuple hv_DecodedDataStrings = null;
            // Initialize local and output iconic variables 
         
            HOperatorSet.GenEmptyObj(out ho_SymbolXLDs);
        
         
            HOperatorSet.CreateDataCode2dModel(SymbolType, GenParamNamesOfModel, GenParamValuesOfModel,
                out hv_DataCodeHandle);
            ho_SymbolXLDs.Dispose();
            HOperatorSet.FindDataCode2d(image, out ho_SymbolXLDs, hv_DataCodeHandle, new HTuple(),
                new HTuple(), out hv_ResultHandles, out hv_DecodedDataStrings);
            HOperatorSet.ClearDataCode2dModel(hv_DataCodeHandle);

        
            ho_SymbolXLDs.Dispose();

            if (hv_DecodedDataStrings.Length == 0)
            {
                Console.WriteLine("FindDataCode2d: not found, at " + DateTime.Now);
                Debug.WriteLine("FindDataCode2d: not found, at " + DateTime.Now);
                return null;
            }
            else
            {
                string decodeString = hv_DecodedDataStrings;

                return decodeString;
            }
            
        }

        public string SymbolType { get; set; } = "[]";
        //Type of the 2D data code.
        //Default value: "Data Matrix ECC 200"
        //List of values: "Aztec Code", "Data Matrix ECC 200", "GS1 Aztec Code", "GS1 DataMatrix", "GS1 QR Code", "Micro QR Code", "PDF417", "QR Code"
        public string GenParamNamesOfModel { get; set; } = "[]";
        //Names of the generic parameters that can be adjusted for the 2D data code model.
        //Default value: []
        //List of values: default_parameters 
        public string GenParamValuesOfModel { get; set; } = "[]";
                                                                                    
        public string GenParamNamesOfFind { get; set; } = "[]";

        public int GenParamValuesOfFind { get; set; } 
    }
}