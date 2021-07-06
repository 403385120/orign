#pragma once
#include "BusScanner.h"
#include "DexelaDetector.h"
#include "DexelaException.h"
#include "DexDefs.h"
#include "DexelaDetectorGE.h"
#include <Vector>

#define DEXELA_IMAGE_WIDTH 1536
#define DEXELA_IMAGE_HEIGHT 864

#define DEXELA_MAX_NUM 4
#define DEXELA_MAX_FRAMS 20
#define SEARCH_MAX 256

#define SEARCH_TABLE_MAX 65536

class CDexelaCamera
{
public:
	CDexelaCamera();
	~CDexelaCamera();

public:
	//
	bool InitialDexelaCamera(int dexelcameraNum, int iExposureTime, int iDeviceSN);

	//
	void closeCamera(int dexelcameraNum);

	//
	bool CaptureOneImage(BYTE * pImageBuffer, int dexelcameraNum);

	//
	void SetSreachTable(int minVal, int maxVal, int dexelcameraNum);

	//
	void SetCameraFrams(int Frams, int dexelcameraNum);

	//
	void AcquireCallbackSequence(DexelaDetector &detector, int dexelcameraNum);

	void SetThreshParam();
	//¼à¿ØÆ½°åÏß³Ì
	HANDLE m_hIsImageLost;
	static DWORD WINAPI IsImageLostThread(LPVOID lpdata);
	DWORD IsImageLostThreadDo();

	int m_iFrams;
	int m_iCameraMin;
	int m_iCameraMax;
	int m_iDeviceUserSN;
	int m_iExposureTime;

	bool m_bExitSystem;	
};

extern CDexelaCamera g_DexelaCamera;											