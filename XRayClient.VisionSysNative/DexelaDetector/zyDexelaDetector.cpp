#include "zyDexelaDetector.h"
#include <conio.h>
#include <iostream>
#include <string>


zyDexelaDetector::zyDexelaDetector()
{
	m_iHeight = 1000;
	m_iWidth = 1000;
	m_iFrame = 8;
	m_fExposureTime = 250.0f;
	m_pData = new WORD*[20];
	bOnce = true;
}


zyDexelaDetector::~zyDexelaDetector()
{
}

bool zyDexelaDetector::Init(std::string strSN,bool isHigh)
{
	try
	{
		BusScanner scanner;
		DevInfo info;
		int iCount = scanner.EnumerateDevices();
		if (iCount < 0) 
		{
			return false;
		}

		//根据 SN号初始化平板
		int iDevice = -1;
		for (int index = 0; index < iCount; index++)
		{
			info = scanner.GetDevice(index);
			if (info.serialNum == std::atoi(strSN.c_str()))
			{
				iDevice = index;
				break;
			}
		}
		if (-1 == iDevice)
		{
			return false;
		}

		m_pDet = new DexelaDetector(info);

		// local variables
		ExposureModes expMode = Expose_and_read;
		bins binFmt = x11;
		FullWellModes wellMode = Low;
		if (isHigh)
		{
			FullWellModes wellMode = High;
		}
		ExposureTriggerSource trigger = Internal_Software;
		float exposureTime = 50.0f;
		char filename[50];
		int model;
		int buf = 1;

		// connect to detector
		m_pDet->OpenBoard();
		m_pDet->SetFullWellMode(wellMode);
		m_pDet->SetExposureTime(exposureTime);
		m_pDet->SetBinningMode(binFmt);
		m_pDet->SetTriggerSource(trigger);
		m_pDet->SetExposureMode(expMode);
		model = m_pDet->GetModelNumber();
	}
	catch (DexelaException ex)
	{
		return false;
	}
	return true;
}


void zyDexelaDetector::CaptureOneImage(BYTE * pImageBuffer, cv::Mat& mImage16, int iAlgoType)
{
	int iIndex = 1;
	for (int index = 0; index < m_iFrame; index++)
	{
		m_pDet->Snap(1, (int)m_fExposureTime + 1000);
		m_pDet->ReadBuffer(1, m_img);
		m_img.UnscrambleImage();
		m_img.SubtractDark();
		m_img.FloodCorrection();
		//m_img.DefectCorrection();

		memcpy(m_pData[index], (WORD *)m_img.GetDataPointerToPlane(), m_iHeight*m_iWidth*sizeof(WORD));
	}

	//多帧平均和转换
	AverageImages(m_pData, m_pImg16, m_iFrame, m_iHeight, m_iWidth);
	if (1)
	{
		//memcpy(pImage16, m_pImg16, m_iHeight*m_iWidth*sizeof(WORD));
		cv::Mat TempImage16 = cv::Mat(m_iWidth, m_iHeight, CV_16UC1, m_pImg16);
		TempImage16.copyTo(mImage16);
	}
	ConvetShort2Byte(m_pImg16, pImageBuffer, m_iHeight, m_iWidth, m_iMinVal, m_iMaxVal);
}

void zyDexelaDetector::SetParam(int iDetNo, int iHeight, int iWidth, int iFrame, float fExposureTime)
{
	if (bOnce)
	{
		m_pAverage = new UINT[iHeight*iWidth];
		m_pMap16_8 = new BYTE[65536];
		m_pMap = new WORD[65536];
		m_pImg16 = new WORD[1944*1536];
		for (int index = 0; index < 20; index++)
		{
			m_pData[index] = new WORD[iHeight*iWidth];
		}
		for (int k = 0; k < 65536; k++)
		{
			m_pMap16_8[k] = (BYTE)((k + 256) / 256 - 1);
		}

		if (1 == iDetNo)
		{
			//自动暗校正
			DexImage input = DexImage("C:\\CameraSetting\\camera1\\darksAverage1.smv");
			input.FindMedianofPlanes();
			input.SetAveragedFlag(true);
			input.SetImageType(Offset);
			input.WriteImage("C:\\CameraSetting\\camera1\\darks.smv");

			//自动亮校准
			DexImage inputflood1 = DexImage("C:\\CameraSetting\\camera1\\floodsAverage1.smv");
			inputflood1.LoadDarkImage("C:\\CameraSetting\\camera1\\darks.smv");
			inputflood1.FindMedianofPlanes();
			inputflood1.SetAveragedFlag(true);
			inputflood1.SubtractDark();
			inputflood1.SetDarkCorrectedFlag(true);
			inputflood1.FixFlood();
			inputflood1.SetFixedFlag(true);
			inputflood1.SetImageType(Gain);
			inputflood1.WriteImage("C:\\CameraSetting\\camera1\\floods.smv");

			m_img.LoadDarkImage("C:\\CameraSetting\\camera1\\darks.smv");
			m_img.LoadFloodImage("C:\\CameraSetting\\camera1\\floods.smv");
			//m_img.LoadDefectMap("C:\\CameraSetting\\camera1\\DefectMap.smv");
		}
		else if (2 == iDetNo)
		{
			DexImage input2 = DexImage("C:\\CameraSetting\\camera2\\darksAverage2.smv");
			input2.FindMedianofPlanes();
			input2.SetAveragedFlag(true);
			input2.SetImageType(Offset);
			input2.WriteImage("C:\\CameraSetting\\camera2\\darks.smv");

			DexImage inputflood2 = DexImage("C:\\CameraSetting\\camera2\\floodsAverage2.smv");
			inputflood2.LoadDarkImage("C:\\CameraSetting\\camera2\\darks.smv");
			inputflood2.FindMedianofPlanes();
			inputflood2.SetAveragedFlag(true);
			inputflood2.SubtractDark();
			inputflood2.SetDarkCorrectedFlag(true);
			inputflood2.FixFlood();
			inputflood2.SetFixedFlag(true);
			inputflood2.SetImageType(Gain);
			inputflood2.WriteImage("C:\\CameraSetting\\camera2\\floods.smv");

			m_img.LoadDarkImage("C:\\CameraSetting\\camera2\\darks.smv");
			m_img.LoadFloodImage("C:\\CameraSetting\\camera2\\floods.smv");
			//m_img.LoadDefectMap("C:\\CameraSetting\\camera2\\DefectMap.smv");
		}

		SetSreachTable(0, 4000);
		bOnce = false;
	}
	m_iHeight = iHeight;
	m_iWidth = iWidth;
	m_iFrame = iFrame;
	m_fExposureTime = fExposureTime;
	m_pDet->SetExposureTime(m_fExposureTime);
}

void zyDexelaDetector::AverageImages(WORD ** psrc, WORD * pdst, int iFrame, int iWidth, int Height)
{
	if (iFrame <= 0)
	{
		return;
	}

	int ilen = iWidth*Height;
	memset(m_pAverage, 0, ilen*sizeof(UINT));

	WORD * psrc1;
	UINT * pdst1;

	int i;
	int j;
	for (i = 0; i < iFrame; i++)
	{
		psrc1 = psrc[i];
		pdst1 = m_pAverage;
		for (j = 0; j < ilen; j++)
		{
			*pdst1 += *psrc1;
			pdst1++;
			psrc1++;
		}
	}

	UINT * psrc2 = m_pAverage;
	WORD * pdst2 = pdst;
	for (j = 0; j < ilen; j++)
	{
		*pdst2 = *psrc2 / iFrame;
		pdst2++;
		psrc2++;
	}
}

void zyDexelaDetector::SetSreachTable(int iMinVal, int iMaxVal)
{
	m_iMinVal = iMinVal;
	m_iMaxVal = iMaxVal;

	//生成查找表
	float gap = (float)(iMaxVal - iMinVal);
	for (int i = 0; i < 65536; i++)
	{
		m_pMap[i] = i;
		if (m_pMap[i] <= iMinVal)
		{
			m_pMap[i] = 0;
		}
		else
		{
			if (m_pMap[i] >= iMaxVal)
			{
				m_pMap[i] = 65535;
			}
			else
			{
				m_pMap[i] = (WORD)((i - iMinVal) / gap*65535.0f + 0.5f);
			}
		}
	}
}

void zyDexelaDetector::SetThreshParam()
{
	int iGrayMin = 0;
	int iGrayMax = 100;
	for (int i = 0; i < 256; i++)
	{
		int value = i;
		value = (int)(((float)value - iGrayMin) / (iGrayMax - iGrayMin) * 255);
		value = (value < 0 ? 0 : value);
		value = (value > 255 ? 255 : value);
		m_pMap[i] = value;
	}
}

void zyDexelaDetector::ConvetShort2Byte(WORD* pSrc, BYTE * pDst, int iWidth, int Height, int iMinVal, int iMaxVal)
{
	int ilen = iWidth* Height;
	int i;
	WORD * psrc = pSrc;
	BYTE * pdst = pDst;
	for (i = 0; i < ilen; i++)
	{
		*psrc = m_pMap[*psrc];
		*pdst = m_pMap16_8[*psrc];
		psrc++;
		pdst++;
	}
}
