#pragma once

#include "BusScanner.h"
#include "DexImage.h"
#include "DexelaDetector.h"
#include "DexelaException.h"
#include "BadPixelCorrection.h"
#include <iostream>
#include <conio.h>
#include "opencv2/core/core.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv/cv.h"

class zyDexelaDetector
{
public:
	zyDexelaDetector();
	~zyDexelaDetector();

	//初始化
	bool Init(std::string strSN,bool isHigh);

	//
	void close();

	//
	void CaptureOneImage(BYTE * pImageBuffer, cv::Mat& mImage16, int iAlgoType);
	
	//
	void SetSreachTable(int iMinVal, int iMaxVal);

	//设置参数
	void SetParam(int iDetNo, int iHeight, int iWidth, int iFrame, float fExposureTime);

	void SetThreshParam();

private:
	int m_iHeight;
	int m_iWidth;
	int m_iFrame;
	float m_fExposureTime;

	WORD* m_pMap;				//查找表
	BYTE* m_pMap16_8;			//16转8映射表
	WORD** m_pData;				//每一帧的图像
	UINT* m_pAverage;			//多帧平均后中间变量
	WORD* m_pImg16;				//多帧平均后的16位图
	int m_iMinVal;				//窗位最小值
	int m_iMaxVal;				//窗位最大值
	DexelaDetector* m_pDet;		//探测器对象
	DexImage m_img;				//相机硬件对应的校准图像
	bool bOnce;

	void AverageImages(WORD ** psrc, WORD * pdst, int iFrame, int iWidth, int Height);
	void ConvetShort2Byte(WORD* pSrc, BYTE * pDst, int iWidth, int Height, int iMinVal, int iMaxVal);
};

