#include "DahHengCam.h"


CDahHengCam::CDahHengCam(void)
{
}


CDahHengCam::~CDahHengCam(void)
{
}

int CDahHengCam::OpenDevice(char* index, int width, int height, bool xflip, bool yflip)
{
	return 0;
}

int CDahHengCam::GetAllSerioNum(vector<char*>& vectornum)
{
	return 0;
}


int CDahHengCam::CloseDevice()
{
	return 0;
}

int CDahHengCam::SetVideoOn(bool bOn)
{
	return 0;
}

int CDahHengCam::SoftShot(int numToGrab)
{
	return 0;
}

int CDahHengCam::SetExposal(int nExpVal)
{
	return 0;
}

int CDahHengCam::GetExposal()
{
	return 0;
}

int CDahHengCam::SetGain(int nGainVal)
{
	return 0;
}

int CDahHengCam::GetGain()
{
	return 0;
}

int CDahHengCam::GetBufferImagesCnt()
{
	return 0;
}

int CDahHengCam::GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages)
{
	return 0;
}
