#pragma once
#include "ICameraNative.h"

class ZYAPI_EXPORT_CAMERA_NATIVE CDSCam:public ICameraNative
{
public:
	CDSCam(void);
	~CDSCam(void);

	int OpenDevice(char* index, int width, int height, bool xflip, bool yflip);
	int CloseDevice();
	int SetVideoOn(bool bOn);
	int SoftShot(int numToGrab);
	int SetExposal(int nExpVal);
	int GetExposal();
	int SetGain(int nGainVal);
	int GetGain();
	int GetBufferImagesCnt();
	int GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages);
	int GetAllSerioNum(vector<char*>& vectornum);

};

