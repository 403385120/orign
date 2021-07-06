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
	int64_t GetWidthMax();	//获得指定相机的宽
	int64_t GetHeightMax();// 获得指定相机的高
	bool CaptureOneImage(BYTE * pImageBuffer);
	bool SetGainAbs(int64_t nValue);
	bool SetExposureTimeRaw(int64_t nValue);
public:
	BaslerCam m_BaslerCam1;
	int m_nDeviceNum;     //当前相机的数量
	bool m_bCam1Valid;
	CSize m_stAOI;
};
