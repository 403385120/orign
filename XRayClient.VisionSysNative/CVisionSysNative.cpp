#include "CVisionSysNative.h"
#include "opencv2/core/core.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv/cv.h"

extern cv::Mat g_pImgData16[20];   // 缓存的16位数据
extern int g_iImgIndex;

CVisionSysNative::CVisionSysNative(const CameraParams* visionSysParams)
{
	//this->m_pVisionSysParams = visionSysParams;
	m_pVisionSysParams.SzBufSeriNum = new char[255];
	strcpy_s(m_pVisionSysParams.SzBufSeriNum, 100, visionSysParams->SzBufSeriNum);
	m_pVisionSysParams.camType = visionSysParams->camType;
	m_pVisionSysParams.PinValue = visionSysParams->PinValue;
	m_pVisionSysParams.nWidth = visionSysParams->nWidth;
	m_pVisionSysParams.nHeight = visionSysParams->nHeight;
	m_pVisionSysParams.xFlip = visionSysParams->xFlip;
	m_pVisionSysParams.yFlip = visionSysParams->yFlip;
	m_pVisionSysParams.exposure = visionSysParams->exposure;

 	if (0 == m_pVisionSysParams.camType)
 	{
 		//此时是瓦里安平板相机
		m_iFrame = 3;
		//m_fExposureTime = 60;
		m_fExposureTime = m_pVisionSysParams.exposure;
 		m_pCameraNative = new zyDexelaDetector();
 	}
	
	static bool bFrist = true;
	if (bFrist)
	{
		bFrist = false;
		for (int index = 0; index < 20; index++)
		{
			g_pImgData16[index] = cv::Mat::zeros(cvSize(m_pVisionSysParams.nWidth, m_pVisionSysParams.nHeight), CV_16UC1);
		}
		g_iImgIndex = 0;
	}
}

CVisionSysNative::~CVisionSysNative()
{
}

int CVisionSysNative::Init(int num, bool isHigh)
{
	// TODO: 
	 bool bRet = m_pCameraNative->Init(m_pVisionSysParams.SzBufSeriNum, isHigh);
	 if (bRet)
	 {
		 m_pCameraNative->SetParam(num, m_pVisionSysParams.nHeight, m_pVisionSysParams.nWidth, m_iFrame, m_fExposureTime);
	 }
	
	 return bRet;
}

int CVisionSysNative::UnInit()
{
	// TODO: 关闭相机
	 return 1;
	//return m_pCameraNative->CloseDevice();
}

int CVisionSysNative::SetVideoMode(bool bOn)
{
	// TODO: 切换视频流
	return 1;
	//return m_pCameraNative->SetVideoOn(bOn);
}

int CVisionSysNative::ShotAndCheck(int imagenum, ZYImageStruct* pOutImage, int iAlgoType,int &imgN0)
{
	// TODO: 拍照、滤波
	//判断平均滤波的值 如果==0改为1
	if (imagenum <= 0)
	{
		imagenum = 1;
	}

	if (imagenum > 15)
	{
		//平均系数在1-15之间
		return -1;
	}

	if (m_iFrame == imagenum)
	{
		m_pCameraNative->SetParam(1, m_pVisionSysParams.nHeight, m_pVisionSysParams.nWidth, m_iFrame, m_fExposureTime);
	}
	else
	{
		m_pCameraNative->SetParam(1, m_pVisionSysParams.nHeight, m_pVisionSysParams.nWidth, imagenum, m_fExposureTime);
	}
	pOutImage->channel = 1;
	pOutImage->height = m_pVisionSysParams.nHeight;
	pOutImage->width = m_pVisionSysParams.nWidth;

	cv::Mat mImgTemp8 = cv::Mat::zeros(m_pVisionSysParams.nWidth, m_pVisionSysParams.nHeight, CV_8UC1);
	cv::Mat mImgTemp16 = cv::Mat::zeros(m_pVisionSysParams.nWidth, m_pVisionSysParams.nHeight, CV_16UC1);

	m_pCameraNative->CaptureOneImage(mImgTemp8.data, mImgTemp16, iAlgoType);

	//cv::flip(mImgTemp8, mImgTemp8, 0);
	cv::transpose(mImgTemp8, mImgTemp8);
	cv::flip(mImgTemp8, mImgTemp8, 1);
	memcpy(pOutImage->data, mImgTemp8.data, pOutImage->width*pOutImage->height);

	g_iImgIndex++;
	pOutImage->imgNo = g_iImgIndex;
	imgN0 = g_iImgIndex;
	//cv::flip(mImgTemp16, mImgTemp16, 0);
	//cv::transpose(mImgTemp16, mImgTemp16);
	//cv::flip(mImgTemp16, mImgTemp16, 1);
	mImgTemp16.copyTo(g_pImgData16[g_iImgIndex % 20]);

	return 1;
}

int CVisionSysNative::GetExposureTime()
{
	return 1;
}


int CVisionSysNative::SetExposureTime(int nValue)
{
	m_pCameraNative->SetParam(1, m_pVisionSysParams.nHeight, m_pVisionSysParams.nWidth, m_iFrame, nValue);
	return 1;
}


int CVisionSysNative::GetGain()
{
	//return m_pCameraNative->GetGain();
	return 1;
}

void CVisionSysNative::SetSreachTable(int iMinVal, int iMaxVal)
{
	m_pCameraNative->SetSreachTable(iMinVal, iMaxVal);
}

int CVisionSysNative::SetGain(int nvalue)
{
	//return m_pCameraNative->SetGain(nvalue);
	return 1;
}

int CVisionSysNative::GetAllSerioNum(vector<char*>& pvector)
{
	//return m_pCameraNative->GetAllSerioNum(pvector);
	return 1;
}
