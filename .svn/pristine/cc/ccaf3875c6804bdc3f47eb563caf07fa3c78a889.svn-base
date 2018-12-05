using System.Runtime.InteropServices;

namespace Hdc.Measuring
{
    public static class NativeMethods
    {
        private const string Dll = "mwLibWrap.dll";


        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern bool mwLibInit();

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern unsafe bool mwReadRefcsv(string filename, ref int nPts, double[] xRef, double[] yRef, double scalingRate);

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern unsafe bool mwReadFScsv(string filename, ref int nPts, double[] xTest, double[] yTest);


        /*
        double mwProfErrCal(  //轮廓度计算函数，返回值为最大残差绝对值，若为负则计算错误
		const int nPtsRef, const double* xRef, const double* yRef,  //输入参考轮廓点数、x坐标、y坐标
		const int nPtsTestIn, const double* xTestIn, const double* yTestIn, //输入待计算轮廓点数、x坐标、y坐标
		int& nPtsTestOut, double* xTestOut, double* yTestOut, double* yTestResOut, //输出计算轮廓点数、x坐标、y坐标及残差
		const double needFlip = 0.0L, //计算控制参数: 是否需要将测试轮廓线反向 1 yes，0 no
		const double outlierDist = 0.05,  //计算控制参数: 异常值距离阈值（去除边界异常值） 0~inf（单位与输入参考及测试数据坐标单位一致）
		const double numBoundtoRemove = 5, //计算控制参数: 去除不稳定的边界点数量（左右两边均删除该数目测量点）
		const double edgeonRight = 1.0L, //计算控制参数: 参考玻璃轮廓的边界在右边？ 1 yes，0 no （通过制定该参数，将测试轮廓首先与参考轮廓在边界处进行初步配准，然后使用迭代就近点拟合
		const double fittingStopVar = 1e-6, //计算控制参数：控制拟合迭代精度，数值越小，精度越高，迭代次数也越多
		const char* savepath= NULL);
        */
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern unsafe double mwProfErrCal( 
        int nPtsRef, double[] xRef, double[] yRef,  
        int nPtsTestIn, double[] xTestIn, double[] yTestIn, 
        ref int nPtsTestOut, double[] xTestOut, double[] yTestOut, double[] yTestResOut, 
        double needFlip, 
        double outlierDist,  
        double numBoundtoRemove, 
        double edgeonRight, 
        double fittingStopVar,
        string savepath);

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern bool mwLibAbort();
    }
}
