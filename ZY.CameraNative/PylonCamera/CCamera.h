#pragma once
#include "..\ICameraNative.h"
#include "BaslerCam.h"

class ZYAPI_EXPORT_CAMERA_NATIVE CCamera: public ICameraNative
{
public:
	CCamera();
	~CCamera();
	bool Initial();
	void CloseCam();
	int64_t GetWidthMax();	//���ָ������Ŀ�
	int64_t GetHeightMax();// ���ָ������ĸ�
	bool CaptureOneImage(BYTE * pImageBuffer);
	bool SetGainAbs(int64_t nValue);
	bool SetExposureTimeRaw(int64_t nValue);
public:
	BaslerCam m_BaslerCam1;
	int m_nDeviceNum;     //��ǰ���������
	bool m_bCam1Valid;
	CSize m_stAOI;
};
