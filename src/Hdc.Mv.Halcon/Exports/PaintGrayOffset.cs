//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 12.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
#endif

  // Procedures 
  public void PaintGrayOffset (HObject ho_ImageSource, HObject ho_ImageDestination, 
      out HObject ho_MixedImage, HTuple hv_OffsetRow, HTuple hv_OffsetColumn)
  {




    // Local iconic variables 

    HObject ho_Images;

    // Local control variables 

    HTuple hv_SourceWidth = null, hv_SourceHeight = null;
    HTuple hv_DestinationWidth = null, hv_DestinationHeight = null;
    HTuple hv_OffsetCol = null, hv_Row1 = null, hv_Col1 = null;
    HTuple hv_Row2 = null, hv_Col2 = null;
    HTuple   hv_OffsetRow_COPY_INP_TMP = hv_OffsetRow.Clone();

    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_MixedImage);
    HOperatorSet.GenEmptyObj(out ho_Images);
    HOperatorSet.GetImageSize(ho_ImageSource, out hv_SourceWidth, out hv_SourceHeight);
    HOperatorSet.GetImageSize(ho_ImageDestination, out hv_DestinationWidth, out hv_DestinationHeight);

    ho_Images.Dispose();
    HOperatorSet.ConcatObj(ho_ImageDestination, ho_ImageSource, out ho_Images);

    hv_OffsetRow_COPY_INP_TMP = (new HTuple(0)).TupleConcat(hv_OffsetRow_COPY_INP_TMP);
    hv_OffsetCol = new HTuple();
    hv_OffsetCol[0] = 0;
    hv_OffsetCol = hv_OffsetCol.TupleConcat(hv_OffsetColumn);
    hv_Row1 = new HTuple();
    hv_Row1[0] = 0;
    hv_Row1[1] = 0;
    hv_Col1 = new HTuple();
    hv_Col1[0] = 0;
    hv_Col1[1] = 0;
    hv_Row2 = new HTuple();
    hv_Row2 = hv_Row2.TupleConcat(hv_DestinationHeight);
    hv_Row2 = hv_Row2.TupleConcat(hv_SourceHeight);
    hv_Col2 = new HTuple();
    hv_Col2 = hv_Col2.TupleConcat(hv_DestinationWidth);
    hv_Col2 = hv_Col2.TupleConcat(hv_SourceWidth);

    ho_MixedImage.Dispose();
    HOperatorSet.TileImagesOffset(ho_Images, out ho_MixedImage, hv_OffsetRow_COPY_INP_TMP, 
        hv_OffsetCol, hv_Row1, hv_Col1, hv_Row2, hv_Col2, hv_DestinationWidth, hv_DestinationHeight);

    ho_Images.Dispose();

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

