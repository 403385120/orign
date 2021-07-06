#include <pylon/PylonIncludes.h>
#include <pylon/PylonGUI.h>
#include "CameraConfiguration.h"
#include "ImageEventPrinter.h"

using namespace Pylon;
// Namespace for using cout.
using namespace std;
using namespace GenApi;


#define BASLER_USED_CAM  1
//const CString csDeviceUserID[BASLER_USED_CAM] = {_T("1"), _T("2")};


class BaslerCamera: public CImageEventHandler, public CConfigurationEventHandler
{

public:
	BaslerCamera();
	~BaslerCamera();

public:

	static int    InitAll();

	bool  SetCurrentIndex(int index);

	bool  ReInit();

	bool  OpenCamera();

	bool  CloseCamera();

	bool  GetCameraSN(int &sn); 

	bool  SettingFileLoadAndSave(const String_t& FileName);

	bool  SetPixelFormat();

	bool  SetImageAOI(int64_t nWidth, int64_t nHeight, int64_t nOffsetX, int64_t nOffsetY);

	bool  GetImageAOI(int64_t& nWidth, int64_t& nHeight, int64_t& nOffsetX, int64_t& nOffsetY);

	bool  SetGainRaw(int64_t nValue);

	bool  SetAcquisitionTrigger(bool bTriggerOn);

	bool  SetAcquisitionTriggerSource(int nSource);

	bool  SoftwareAcquisitionTrigger();

	bool  SetFrameTrigger(bool bTriggerOn);

	bool  SetFrameTriggerSource(int nSource);

	bool  SoftwareFrameTrigger();

	bool  SetExposureTimeRaw(int64_t nValue);

	bool  GrabImageStart();

	bool  GrabImages(uint8_t* pImageBuffer, unsigned int timeoutMs = 5000);

	bool  GrabImageStop();

	bool  IsCameraRemoved();

	bool  SetHeartbeatTimeout(int64_t NewValue_ms);

	bool CaptureImageToBuffer(int Index,BYTE * _pDataBuffer);

	static void CloseAllCamera();

private:
	int64_t Adjust(int64_t val, int64_t minimum, int64_t maximum, int64_t inc);
	int     nCurrentIndex;

	BYTE * m_pDataBufferCamOne;
	BYTE * m_pDataBufferCamTwo;

	int * m_pIntDataBufferOne;
	int * m_pIntDataBufferTwo;


};