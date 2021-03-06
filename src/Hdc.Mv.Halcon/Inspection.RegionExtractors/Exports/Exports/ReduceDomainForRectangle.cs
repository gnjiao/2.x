//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 11.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
#endif

  // Procedures 
    public void ChangeDomainForRectangle(HObject ho_InputImage, out HObject ho_ImageResult, 
      HTuple hv_LineStartPoint_Row, HTuple hv_LineStartPoint_Column, HTuple hv_LineEndPoint_Row, 
      HTuple hv_LineEndPoint_Column, HTuple hv_RoiWidthLen, HTuple hv_DilationWidth, 
      HTuple hv_DilationHeight)
  {



    // Local iconic variables 

    HObject ho_Rectangle, ho_RegionDilation;


    // Local control variables 

    HTuple hv_TmpCtrl_Row = null, hv_TmpCtrl_Column = null;
    HTuple hv_TmpCtrl_Dr = null, hv_TmpCtrl_Dc = null, hv_TmpCtrl_Phi = null;
    HTuple hv_TmpCtrl_Len1 = null, hv_TmpCtrl_Len2 = null;

    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_ImageResult);
    HOperatorSet.GenEmptyObj(out ho_Rectangle);
    HOperatorSet.GenEmptyObj(out ho_RegionDilation);

    //Measure 01: Convert coordinates to rectangle2 type
    hv_TmpCtrl_Row = 0.5*(hv_LineStartPoint_Row+hv_LineEndPoint_Row);
    hv_TmpCtrl_Column = 0.5*(hv_LineStartPoint_Column+hv_LineEndPoint_Column);
    hv_TmpCtrl_Dr = hv_LineStartPoint_Row-hv_LineEndPoint_Row;
    hv_TmpCtrl_Dc = hv_LineEndPoint_Column-hv_LineStartPoint_Column;
    hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(hv_TmpCtrl_Dc);
    hv_TmpCtrl_Len1 = 0.5*((((hv_TmpCtrl_Dr*hv_TmpCtrl_Dr)+(hv_TmpCtrl_Dc*hv_TmpCtrl_Dc))).TupleSqrt()
        );
    hv_TmpCtrl_Len2 = hv_RoiWidthLen.Clone();
    ho_Rectangle.Dispose();
    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_TmpCtrl_Row, hv_TmpCtrl_Column, 
        hv_TmpCtrl_Phi, hv_TmpCtrl_Len1, hv_TmpCtrl_Len2);

    ho_RegionDilation.Dispose();
    HOperatorSet.DilationRectangle1(ho_Rectangle, out ho_RegionDilation, hv_DilationWidth, 
        hv_DilationHeight);

    ho_ImageResult.Dispose();
    HOperatorSet.ChangeDomain(ho_InputImage, ho_RegionDilation, out ho_ImageResult
        );

    ho_Rectangle.Dispose();
    ho_RegionDilation.Dispose();

    return;
  }

  // Procedures 
    public void ChangeDomainForRectangle(HObject ho_InputImage, out HObject ho_ImageResult, 
      HTuple hv_LineStartPoint_Row, HTuple hv_LineStartPoint_Column, HTuple hv_LineEndPoint_Row, 
      HTuple hv_LineEndPoint_Column, HTuple hv_RoiWidthLen)
  {



    // Local iconic variables 

        HObject ho_Rectangle;


    // Local control variables 

    HTuple hv_TmpCtrl_Row = null, hv_TmpCtrl_Column = null;
    HTuple hv_TmpCtrl_Dr = null, hv_TmpCtrl_Dc = null, hv_TmpCtrl_Phi = null;
    HTuple hv_TmpCtrl_Len1 = null, hv_TmpCtrl_Len2 = null;

    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_ImageResult);
    HOperatorSet.GenEmptyObj(out ho_Rectangle);

    //Measure 01: Convert coordinates to rectangle2 type
    hv_TmpCtrl_Row = 0.5*(hv_LineStartPoint_Row+hv_LineEndPoint_Row);
    hv_TmpCtrl_Column = 0.5*(hv_LineStartPoint_Column+hv_LineEndPoint_Column);
    hv_TmpCtrl_Dr = hv_LineStartPoint_Row-hv_LineEndPoint_Row;
    hv_TmpCtrl_Dc = hv_LineEndPoint_Column-hv_LineStartPoint_Column;
    hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(hv_TmpCtrl_Dc);
    hv_TmpCtrl_Len1 = 0.5*((((hv_TmpCtrl_Dr*hv_TmpCtrl_Dr)+(hv_TmpCtrl_Dc*hv_TmpCtrl_Dc))).TupleSqrt()
        );
    hv_TmpCtrl_Len2 = hv_RoiWidthLen.Clone();
    ho_Rectangle.Dispose();
    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_TmpCtrl_Row, hv_TmpCtrl_Column, 
        hv_TmpCtrl_Phi, hv_TmpCtrl_Len1, hv_TmpCtrl_Len2);

    ho_ImageResult.Dispose();
    HOperatorSet.ChangeDomain(ho_InputImage, ho_Rectangle, out ho_ImageResult
        );

    ho_Rectangle.Dispose();

    return;
  }


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

