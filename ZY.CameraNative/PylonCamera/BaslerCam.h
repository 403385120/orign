#pragma once
#include "afxwin.h"
#include <pylon/PylonIncludes.h>
#include <pylon/PylonGUI.h>
#include "..\ICameraNative.h"
//#include "CameraConfiguration.h"
#define COUNT_OF_IMAGES_TO_CAPTURE 8
#define CAMERA_IMAGE_WIDTH 1000
#define CAMERA_IMAGE_HEIGHT 1000
#define CAMERA_IMAGE_SIZE (CAMERA_IMAGE_WIDTH * CAMERA_IMAGE_HEIGHT)
using namespace Pylon;
using namespace GenApi;
using namespace std;


static DeviceInfoList_t g_DeviceInfoList;
static int				g_nTotalDeviceNum = 0;

const UINT CAMERA_NUM = 4;
//csDeviceUserID 相机用户ID
const CString csDeviceUserID[CAMERA_NUM] = {_T("CameraUp01"), _T("CameraUp02"),_T("CameraDown01"),_T("CameraDown02")};

//逆时针方向
enum DirecEnum
{
	ROTATE90 = 1,
	ROTATE180,
	ROTATE270,
};
//Example of an image event handler.
class CSampleImageEventHandler : public CImageEventHandler
{
public:
	virtual void OnImageGrabbed( CInstantCamera& camera, const CGrabResultPtr& ptrGrabResult)
	{
#ifdef PYLON_WIN_BUILD
		size_t ndispIndex = 0;
		for ( size_t i = 0; i < CAMERA_NUM; i++)
		{
			if (camera.GetDeviceInfo().GetUserDefinedName().c_str () == csDeviceUserID[i])
			{
				
				ndispIndex = i;
				break;
			}
		}
		// Display the image

		Pylon::DisplayImage(ndispIndex, ptrGrabResult);
#endif

		////cout << "CSampleImageEventHandler::OnImageGrabbed called." << std::endl;
		////cout << std::endl;
		////cout << std::endl;

		static int ncount=0;
	//	CString str = camera.GetDeviceInfo().GetUserDefinedName().c_str ();
	//	if (str == "CameraDown01")
		{
			ncount++;
			TRACE("OnImageGrabbed called \n");
		}
		
	}
};

class  BaslerCam: public ICameraNative, public CImageEventHandler
{
public:
	BaslerCam(void);

	~BaslerCam(void);

public:
	//static函数，不管使用多少相机，也只能被调用一次
	static void RunPylonAutoInitTerm(); 
	static int  EnumateAllDevices();	//返回相机数

public:
	//根据自定义的相机的UserID，得到枚举的相机列表中的相机序号
	int  GetDeviceIndexFromUserID(CString UserID);
	//根据序号，初始化相机
	bool SetDeviceIndex(int nIndex);

	CString GetDeviceSN();
	CString GetDeviceUserID();


	bool OpenDevice();
	bool MyCloseDevice();
	bool DestroyDevice();

	bool  SaveSettingFile(CString strFileName);
	bool  LoadSettingFile(CString strFileName);
	
	bool SetWidth( int32_t nWidth);	// 设置指定相机的图像宽和高
	bool SetHeight(int32_t nHeight);	// 设置指定相机的图像宽和高

	int64_t GetWidth();	//获得指定相机的宽
	int64_t GetHeight();// 获得指定相机的高

	int64_t GetWidthMax();	//获得指定相机的宽
	int64_t GetHeightMax();// 获得指定相机的高

	bool SetOffsetX(int32_t nOffsetX);	//设置X方向偏移
	bool SetOffsetY(int32_t nOffsetY);  // 设置Y方向偏移

	bool SetCenterX(bool bSetValue); //设置X方向中心对齐
	bool SetCenterY(bool bSetValue); //设置Y方向中心对齐

	bool SetPixelFormatToMono8();
	bool SetPixelFormatToBayerBG8();

	bool  GrabImageStart();
//	bool  GrabImageStartContinuousThread();
	bool  GrabOneImage( unsigned int timeoutMs = 1000);
	bool  GrabOneImage(uint8_t* pImageBuffer, unsigned int timeoutMs = 1000);
	bool  GrabImageStop();

	bool  SetHeartbeatTimeout(int64_t NewValue_ms);

	bool  SetGainAbs(int64_t nValue);
	bool  SetExposureTimeRaw(int64_t nValue);

	bool  SetToFreeRunMode();
	bool  SetToSoftwareTrigerMode();
	bool  SendSoftwareTrigerCommand();
	bool  SetToHardwareTrigerMode();

	void  ADLog(CString strMsg);

	bool  Rotate(unsigned char* src, const int &SrcWidth, const int &SrcHeight, int iRotateDirec);

	virtual  void OnImageGrabbed( CInstantCamera& camera, const CGrabResultPtr& ptrGrabResult);

	BYTE *m_pImage1;
	BYTE *m_pTotalImage;
	int m_iCaptureNum;	//采图的数量
	int m_iNowCaptureNum;	//当前采到的图的数量
	bool AverageValue(BYTE * pByteDataBuffer,int * pIntDataBuffer,long lSize,int iCycle);
	bool ConvertIntToBYTE(int * iBuffer,BYTE * bBuffer,long lSize);
	bool bCaptureFinish;


	//接口
	int OpenDevice(char* index, int width, int height, bool xflip, bool yflip);
	int CloseDevice();
	int SetVideoOn(bool bOn);
	int SoftShot(int numToGrab);
	int SetExposal(int nExpVal);
	int GetExposal();
	int SetGain(int nGainVal);
	int GetGain();
	int GetBufferImagesCnt();
	int GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages, int flag = 0);
	int GetAllSerioNum(vector<char*>& vectornum);

	int m_nDeviceNum;     //当前相机的数量
	bool m_bCam1Valid;
	BYTE * g_pImageData;
	int * g_pDataBufferInt;

private:
	int64_t Adjust(int64_t val, int64_t minimum, int64_t maximum, int64_t inc);
	int	 m_nCurrentIndex;
	CInstantCamera m_InstantCamera;
	bool m_bSaveImage;
	bool bNeedImageConverter;
	// Create a target image.
	CPylonImage m_targetImage;
	// Create the converter and set parameters.
	CImageFormatConverter m_ImageConverter;

	bool m_bframeStartAvailable ;
	bool m_bAcquisitionStartAvailable ;
	bool m_bFrameBurstStartAvailable;
	// Preselect the trigger mode for image acquisition.
	char* triggerSelectorValue;

	bool bIsGige;
	bool bIsUSB3;
};
