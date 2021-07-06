#include "VisionSysNativeIF.h"
#include "CVisionSysNative.h"

#include <vector>

static bool bInited = false;

static int nSVisionSysCnt = 0;
static std::vector<CVisionSysNative*> pSVisionSysNativeList;


int VisionSysNativeIF_Add(const CameraParams* pVisionSysParamsList)
{
	
	nSVisionSysCnt++;

	//return 1;

	CVisionSysNative* visionSys = new CVisionSysNative(pVisionSysParamsList);

	

	pSVisionSysNativeList.push_back(visionSys);

	return 0;

}

int VisionSysNativeIF_Init(int* pResults, bool isHigh)
{
	if (bInited) return true;
	if (0 == pSVisionSysNativeList.size() || nSVisionSysCnt <= 0) return -1;

	// 然后逐个初始化
	int nRet = 0;
	unsigned int retCode = 0;
	for (int i = 0; i < nSVisionSysCnt; i++)
	{
		// 如果相机初始化失败，我们需要返回一个错误代号以便识别
		// 约定为1左移相机编号
		nRet = pSVisionSysNativeList[i]->Init(i+1, isHigh);
		if (NULL != pResults[i]) {
			pResults[i] = nRet;
		}
		if (nRet != 1) {
			retCode = -1;
		}
	}

	if (1 == nRet && retCode == 0)
	{
		bInited = true;
	}

	return (retCode==0? 0: -1);

}


int VisionSysNativeIF_UnInit()
{

	//ImageProcessNativeIF::UnInit();
	if (!bInited || nSVisionSysCnt == 0) return -1;

	// 逐个反初始化
	for (int i = 0; i < nSVisionSysCnt; i++)
	{
		pSVisionSysNativeList[i]->UnInit();

		delete pSVisionSysNativeList[i];
		pSVisionSysNativeList[i] = NULL;
	}

	return 0;
}


int VisionSysNativeIF_shot(int frontcameraIndex, int imgcount, ZYImageStruct* frontpOutImage, int iAlgoType ,int &imgNO)
{
	if (!bInited || frontcameraIndex >= nSVisionSysCnt || frontcameraIndex < 0)
	{
		return -1;
	}

	if (NULL == frontpOutImage->data)
	{
		return -1;
	}

	return pSVisionSysNativeList[frontcameraIndex]->ShotAndCheck(imgcount,frontpOutImage, iAlgoType, imgNO);
}

int VisionSysNativeIF_GetExposure(int cameraIndex)
{
	if (!bInited || cameraIndex >= nSVisionSysCnt || cameraIndex < 0)
	{
		return -1;
	}

	return pSVisionSysNativeList[cameraIndex]->GetExposureTime();
}

int VisionSysNativeIF_SetExposure(int cameraIndex, int nvalue)
{
	if (!bInited || cameraIndex >= nSVisionSysCnt || cameraIndex < 0)
	{
		return -1;
	}

	return pSVisionSysNativeList[cameraIndex]->SetExposureTime(nvalue);
}

int VisionSysNativeIF_GetGain(int cameraIndex)
{
	if (!bInited || cameraIndex >= nSVisionSysCnt || cameraIndex < 0)
	{
		return -1;
	}

	return pSVisionSysNativeList[cameraIndex]->GetGain();
}

void VisionSysNativeIF_SetSreachTable(int cameraIndex, int minGray, int maxGray)
{
	if (maxGray < minGray)
	{
		return;
	}
	pSVisionSysNativeList[cameraIndex]->SetSreachTable(minGray, maxGray);
}

int VisionSysNativeIF_GetAllSerioNum(char* cameraIndex, int stringwidth, int stringheight)
{
	if (NULL == cameraIndex || stringwidth <= 0 || stringheight <= 0) {
		return -1;
	}

	memset(cameraIndex, 0, stringwidth * stringheight);
	
	vector<char*> pserionum;

	pSVisionSysNativeList[0]->GetAllSerioNum(pserionum);

	if (stringheight < pserionum.size())
	{
		return -1;
	}

	for (int n = 0; n < pserionum.size(); n++)
	{
		memcpy((char*)(cameraIndex + n * stringwidth), pserionum[n], strlen(pserionum[n]));
	}

	return 0;
}

int VisionSysNativeIF_SetGain(int cameraIndex, int nvalue)
{
	if (!bInited || cameraIndex >= nSVisionSysCnt || cameraIndex < 0)
	{
		return -1;
	}

	return pSVisionSysNativeList[cameraIndex]->SetGain(nvalue);
}
