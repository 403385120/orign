#include "stdafx.h"
#include "CCamera.h"


BYTE * g_pImageData = NULL;
int * g_pDataBufferInt = NULL;
CCamera::CCamera()
{

}
CCamera::~CCamera()
{

}

bool CCamera::Initial()
{
	m_nDeviceNum = BaslerCam::EnumateAllDevices();

	m_bCam1Valid = false;

	if (m_nDeviceNum == 1)
	{
		if (!m_bCam1Valid)
		{
			m_BaslerCam1.SetDeviceIndex(0);
			m_bCam1Valid = m_BaslerCam1.OpenDevice();
			//m_BaslerCam1.SetHeartbeatTimeout(6000);
			//m_BaslerCam1.SetOffsetX(0);
			//m_BaslerCam1.SetOffsetY(0);
			//m_stAOI.cx = (long)m_BaslerCam1.GetWidthMax();
			//m_stAOI.cx = (long)(m_stAOI.cx & ~3);
			//m_stAOI.cy = (long)m_BaslerCam1.GetHeightMax();
			m_BaslerCam1.SetWidth(CAMERA_IMAGE_WIDTH);
			m_BaslerCam1.SetHeight(CAMERA_IMAGE_HEIGHT);
			m_BaslerCam1.SetToSoftwareTrigerMode();
			//m_BaslerCam1.SetExposureTimeRaw(7000);
			//m_BaslerCam1.SetToFreeRunMode();
			//如果是黑白相机
			m_BaslerCam1.SetPixelFormatToMono8();
			//mGrabImage = CPylonImage::Create(PixelType_Mono8, m_stAOI.cx, m_stAOI.cy);

			//如果是彩色相机
			//m_BaslerCam1.SetPixelFormatToBayerBG8();
			//mGrabImage = CPylonImage::Create(PixelType_RGB8packed, m_stAOI.cx, m_stAOI.cy);
			//size_t mSize = mGrabImage.GetAllocatedBufferSize();
			m_BaslerCam1.GrabImageStart();
		}
	}
	else
	{
		AfxMessageBox(_T(" 打开相机失败，请确定相机连接正常？"));
	}

	return m_bCam1Valid;
}
void CCamera::CloseCam()
{
	if (m_bCam1Valid)
	{
		m_BaslerCam1.GrabImageStop();
		m_BaslerCam1.CloseDevice();
		m_BaslerCam1.DestroyDevice();
	}
}
int64_t CCamera::GetWidthMax()
{
	int64_t nWidthMax = 0;
	if (m_bCam1Valid)
	{
		nWidthMax = m_BaslerCam1.GetWidthMax();
	}
	return nWidthMax;
}
int64_t CCamera::GetHeightMax()
{
	int64_t nHeightMax = 0;
	if (m_bCam1Valid)
	{
		nHeightMax = m_BaslerCam1.GetHeightMax();
	}
	return nHeightMax;
}
bool CCamera::CaptureOneImage(BYTE * pImageBuffer)
{
	if (pImageBuffer == NULL)
	{
		//InsertMessageToList(L"if(pImageBuffer==NULL");
		return false;

	}

	try
	{
		if (g_pImageData == NULL)
		{
			g_pImageData = new BYTE[COUNT_OF_IMAGES_TO_CAPTURE*CAMERA_IMAGE_SIZE];
		}

		if (g_pDataBufferInt == NULL)
		{
			g_pDataBufferInt = new int[CAMERA_IMAGE_SIZE];
		}
		memset(g_pImageData, 0, CAMERA_IMAGE_SIZE);
		memset(g_pDataBufferInt, 0, sizeof(int)*CAMERA_IMAGE_SIZE);
	}
	catch (...)
	{
		//InsertMessageToList(L"Eror In CaptureOneImage");
		return false;
	}
	/*if (g_bUseImageFileAsSource == TRUE)
	{
		int w = 0, h = 0;
		LoadImageData(pImageBuffer, w, h);
		return true;
	}*/

	for (int i = 0; i < COUNT_OF_IMAGES_TO_CAPTURE; i++)
	{
		m_BaslerCam1.SendSoftwareTrigerCommand();
		while (!m_BaslerCam1.bCaptureFinish)
		{
			Sleep(5);
		}
		memcpy_s((g_pImageData + i*CAMERA_IMAGE_SIZE), CAMERA_IMAGE_SIZE, m_BaslerCam1.m_pImage1, CAMERA_IMAGE_SIZE);
	}
	memset(m_BaslerCam1.m_pImage1, 0, CAMERA_IMAGE_SIZE);
	bool bRet = false;
	try
	{
		bRet = m_BaslerCam1.AverageValue((BYTE*)g_pImageData, g_pDataBufferInt, CAMERA_IMAGE_SIZE, COUNT_OF_IMAGES_TO_CAPTURE);
		if (bRet)
		{
			bRet = m_BaslerCam1.ConvertIntToBYTE(g_pDataBufferInt, pImageBuffer, CAMERA_IMAGE_SIZE);
		}
	}
	catch (...)
	{
		bRet = false;
		//InsertMessageToList(L"Eror In CaptureOneImage");

	}
	return bRet;
}
bool  CCamera::SetGainAbs(int64_t nValue)
{
	bool btn = false;
	btn = m_BaslerCam1.SetGainAbs(nValue);
	return btn;
}
bool  CCamera::SetExposureTimeRaw(int64_t nValue)
{
	bool btn = false;
	btn = m_BaslerCam1.SetExposureTimeRaw(nValue);
	return btn;
}

