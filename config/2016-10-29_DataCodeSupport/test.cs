//
// File generated by HDevelop for HALCON/.NET (C#) Version 12.0.1
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
  public HDevelopExport()
  {
    // Default settings used in HDevelop 
    HOperatorSet.SetSystem("width", 512);
    HOperatorSet.SetSystem("height", 512);
    if (HalconAPI.isWindows)
      HOperatorSet.SetSystem("use_window_thread","true");
    action();
  }
#endif

#if !NO_EXPORT_MAIN
  // Main procedure 
  private void action()
  {


    // Local iconic variables 

    HObject ho_Image, ho_SymbolXLDs;

    // Local control variables 

    HTuple hv_DataCodeHandle = null, hv_ResultHandles = null;
    HTuple hv_DecodedDataStrings = null;
    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_Image);
    HOperatorSet.GenEmptyObj(out ho_SymbolXLDs);
    ho_Image.Dispose();
    HOperatorSet.ReadImage(out ho_Image, "B:/Gold.tif");
    HOperatorSet.CreateDataCode2dModel("QR Code", "default_parameters", "maximum_recognition", 
        out hv_DataCodeHandle);
    ho_SymbolXLDs.Dispose();
    HOperatorSet.FindDataCode2d(ho_Image, out ho_SymbolXLDs, hv_DataCodeHandle, new HTuple(), 
        new HTuple(), out hv_ResultHandles, out hv_DecodedDataStrings);
    HOperatorSet.ClearDataCode2dModel(hv_DataCodeHandle);

    ho_Image.Dispose();
    ho_SymbolXLDs.Dispose();

  }

#endif


}
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
public class HDevelopExportApp
{
  static void Main(string[] args)
  {
    new HDevelopExport();
  }
}
#endif

