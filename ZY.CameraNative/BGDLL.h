// BGDLL.h : main header file for the BGDLL DLL
//

#pragma once
#ifdef BG_DLL_API
#else
#define BG_DLL_API extern"C" _declspec(dllexport)
#endif

// #ifndef __AFXWIN_H__
// 	#error "include 'stdafx.h' before including this file for PCH"
// #endif

//#include "resource.h"		// main symbols
#include <vector>
#include <math.h>
#include "emmintrin.h"
using namespace std;


// CBGDLLApp
// See BGDLL.cpp for the implementation of this class
//

BG_DLL_API bool Training(vector<UINT8*> *pvImgBuffer, INT32 nTrainImgNum, INT32 nWidth, INT32 nHeight, INT32 nBytesPerRow, UINT8* pResult);

/* 作用：对一组输入的图像进行“求平均”去噪处理
输入:

pvImgBuffer:	输入的图像序列的数据指针的容器, 要求序列中各图像的大小尺寸必须一致
nTrainImgNum:   输入图像的数目，取值：1~15. 值==1，原图; 值==2, 对输入的两幅图求平均; 值≥3, 对输入的图像序列剔除奇异噪声点后求平均.
nWidth:			输入图像的宽度
nHeight:		输入图像的高度
nBytesPerRow	输入图像每行的字节数

输出:

pResult:		输出结果图像的数据指针.


返回值:		正常：TRUE； 异常：FALSE，一般为获取输入图像有异常

*/


BG_DLL_API bool ImageSmoothAndSharpen(UINT8* pSrc, INT32 nWidth, INT32 nHeight, INT32 nBytesPerRow, UINT8* pDst, INT32 masksize=5, INT32 scale=3);

/* 作用：对输入图像进行平滑和锐化处理
输入:

pSrc:			输入图像的数据指针.
nWidth:			输入图像的宽度
nHeight:		输入图像的高度
nBytesPerRow	输入图像每行的字节数
masksize:		平滑掩模的尺寸大小，单位像素，取值：3， 5，7
scale:			锐化等级，取值：1, 2, 3

输出:

pDst:		输出结果图像的数据指针.


返回值:		正常：TRUE； 异常：FALSE，一般为获取输入图像有异常

*/


// class CBGDLLApp : public CWinApp
// {
// public:
// 	CBGDLLApp();
// 
// // Overrides
// public:
// 	virtual BOOL InitInstance();
// 
// 	DECLARE_MESSAGE_MAP()
// };
