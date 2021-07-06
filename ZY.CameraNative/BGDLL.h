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

/* ���ã���һ�������ͼ����С���ƽ����ȥ�봦��
����:

pvImgBuffer:	�����ͼ�����е�����ָ�������, Ҫ�������и�ͼ��Ĵ�С�ߴ����һ��
nTrainImgNum:   ����ͼ�����Ŀ��ȡֵ��1~15. ֵ==1��ԭͼ; ֵ==2, �����������ͼ��ƽ��; ֵ��3, �������ͼ�������޳��������������ƽ��.
nWidth:			����ͼ��Ŀ��
nHeight:		����ͼ��ĸ߶�
nBytesPerRow	����ͼ��ÿ�е��ֽ���

���:

pResult:		������ͼ�������ָ��.


����ֵ:		������TRUE�� �쳣��FALSE��һ��Ϊ��ȡ����ͼ�����쳣

*/


BG_DLL_API bool ImageSmoothAndSharpen(UINT8* pSrc, INT32 nWidth, INT32 nHeight, INT32 nBytesPerRow, UINT8* pDst, INT32 masksize=5, INT32 scale=3);

/* ���ã�������ͼ�����ƽ�����񻯴���
����:

pSrc:			����ͼ�������ָ��.
nWidth:			����ͼ��Ŀ��
nHeight:		����ͼ��ĸ߶�
nBytesPerRow	����ͼ��ÿ�е��ֽ���
masksize:		ƽ����ģ�ĳߴ��С����λ���أ�ȡֵ��3�� 5��7
scale:			�񻯵ȼ���ȡֵ��1, 2, 3

���:

pDst:		������ͼ�������ָ��.


����ֵ:		������TRUE�� �쳣��FALSE��һ��Ϊ��ȡ����ͼ�����쳣

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
