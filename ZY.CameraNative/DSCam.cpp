#include "DSCam.h"


CDSCam::CDSCam(void)
{
}


CDSCam::~CDSCam(void)
{
}

int CDSCam::OpenDevice(char* index, int width, int height, bool xflip, bool yflip)
{
	return 0;
}

int CDSCam::GetAllSerioNum(vector<char*>& vectornum)
{
	return 0;
}

int CDSCam::CloseDevice()
{
	return 0;
}

int CDSCam::SetVideoOn(bool bOn)
{
	return 0;
}

int CDSCam::SoftShot(int numToGrab)
{
	return 0;
}

int CDSCam::SetExposal(int nExpVal)
{
	return 0;
}

int CDSCam::GetExposal()
{
	return 0;
}

int CDSCam::SetGain(int nGainVal)
{
	return 0;
}

int CDSCam::GetGain()
{
	return 0;
}

int CDSCam::GetBufferImagesCnt()
{
	return 0;
}

int CDSCam::GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages)
{
	return 0;
}
