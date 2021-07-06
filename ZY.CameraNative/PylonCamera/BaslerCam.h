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
//csDeviceUserID ����û�ID
const CString csDeviceUserID[CAMERA_NUM] = {_T("CameraUp01"), _T("CameraUp02"),_T("CameraDown01"),_T("CameraDown02")};

//��ʱ�뷽��
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
	//static����������ʹ�ö��������Ҳֻ�ܱ�����һ��
	static void RunPylonAutoInitTerm(); 
	static int  EnumateAllDevices();	//���������

public:
	//�����Զ���������UserID���õ�ö�ٵ�����б��е�������
	int  GetDeviceIndexFromUserID(CString UserID);
	//������ţ���ʼ�����
	bool SetDeviceIndex(int nIndex);

	CString GetDeviceSN();
	CString GetDeviceUserID();


	bool OpenDevice();
	bool MyCloseDevice();
	bool DestroyDevice();

	bool  SaveSettingFile(CString strFileName);
	bool  LoadSettingFile(CString strFileName);
	
	bool SetWidth( int32_t nWidth);	// ����ָ�������ͼ���͸�
	bool SetHeight(int32_t nHeight);	// ����ָ�������ͼ���͸�

	int64_t GetWidth();	//���ָ������Ŀ�
	int64_t GetHeight();// ���ָ������ĸ�

	int64_t GetWidthMax();	//���ָ������Ŀ�
	int64_t GetHeightMax();// ���ָ������ĸ�

	bool SetOffsetX(int32_t nOffsetX);	//����X����ƫ��
	bool SetOffsetY(int32_t nOffsetY);  // ����Y����ƫ��

	bool SetCenterX(bool bSetValue); //����X�������Ķ���
	bool SetCenterY(bool bSetValue); //����Y�������Ķ���

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
	int m_iCaptureNum;	//��ͼ������
	int m_iNowCaptureNum;	//��ǰ�ɵ���ͼ������
	bool AverageValue(BYTE * pByteDataBuffer,int * pIntDataBuffer,long lSize,int iCycle);
	bool ConvertIntToBYTE(int * iBuffer,BYTE * bBuffer,long lSize);
	bool bCaptureFinish;


	//�ӿ�
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

	int m_nDeviceNum;     //��ǰ���������
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
