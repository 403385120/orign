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

	//��ʼ��
	bool Init(std::string strSN,bool isHigh);

	//
	void close();

	//
	void CaptureOneImage(BYTE * pImageBuffer, cv::Mat& mImage16, int iAlgoType);
	
	//
	void SetSreachTable(int iMinVal, int iMaxVal);

	//���ò���
	void SetParam(int iDetNo, int iHeight, int iWidth, int iFrame, float fExposureTime);

	void SetThreshParam();

private:
	int m_iHeight;
	int m_iWidth;
	int m_iFrame;
	float m_fExposureTime;

	WORD* m_pMap;				//���ұ�
	BYTE* m_pMap16_8;			//16ת8ӳ���
	WORD** m_pData;				//ÿһ֡��ͼ��
	UINT* m_pAverage;			//��֡ƽ�����м����
	WORD* m_pImg16;				//��֡ƽ�����16λͼ
	int m_iMinVal;				//��λ��Сֵ
	int m_iMaxVal;				//��λ���ֵ
	DexelaDetector* m_pDet;		//̽��������
	DexImage m_img;				//���Ӳ����Ӧ��У׼ͼ��
	bool bOnce;

	void AverageImages(WORD ** psrc, WORD * pdst, int iFrame, int iWidth, int Height);
	void ConvetShort2Byte(WORD* pSrc, BYTE * pDst, int iWidth, int Height, int iMinVal, int iMaxVal);
};

