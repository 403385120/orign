#pragma once
#include <windows.h>
#include "ICameraNative.h"
#include "bgapi.hpp"
#include <vector>
using namespace std;

#pragma comment(lib, "../ZY.CameraNative/bgapi.lib")

// ��ʱ�г�1200*1200 
// #define CAMERA_IMAGE_CAMERABAUMER_WEIGHT 1200
// #define CAMERA_IMAGE_CAMERABAUMER_HEIGHT 1200
// 
// #define CAMERA1_IMAGE_CAMERABAUMER_WEIGHT 1600
// #define CAMERA1_IMAGE_CAMERABAUMER_HEIGHT 1200
// 
// 
// #define CAMERA2_IMAGE_CAMERABAUMER_WEIGHT 1600
// #define CAMERA2_IMAGE_CAMERABAUMER_HEIGHT 1200



class ZYAPI_EXPORT_CAMERA_NATIVE CCameraBaumer: public ICameraNative
{
public:
	CCameraBaumer(void);
	~CCameraBaumer(void);

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
	int GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages);
	int GetAllSerioNum(vector<char*>& vectornum);


	//add
	int GetIndex(char* serionum);  //�������кŻ�ȡ��������
	bool Initial(char* serionum, int width, int height, bool xflip, bool yflip);
	static bool GetCamerSerioNum();
	static vector<char*> baumeraserio;
	void CloseCam();
	int GetWidthMax();	//���ָ������Ŀ�
	int GetHeightMax();// ���ָ������ĸ�
	bool CaptureOneImage(BYTE * pImageBuffer,bool bcapture); //�������

	bool SaveImage(BYTE* bdata, int width, int height);

	bool SetGainAbs(float nValue);
	bool SetExposureTimeRaw(int nValue);

	int GetGainMaxAndMin(float& MaxValue, float& MinValue);

	int GetExposure();
	int GetExposureMaxAndMin(float& MaxValue, float& Minvalue);

	int SetLvBoValue(int value);
	int GetLvBovalue();
	int GetLvBoMaxAndMin(int& maxvalue, int& minvalue);


	int m_LvboValue;
	int m_pinValue;
	int SetPinValue(int value);
	int GetPinValue();
	int GetPinMaxAndMin(int& maxvalue, int& minvalue);


	int init_systems(int* system_count, vector<BGAPI::System*>* externppSystem);
	int init_camera(int system_count, vector<BGAPI::System*>* externppSystem, int* pCurrSystem, BGAPI::Camera** externppCamera);
	int release_systems(vector<BGAPI::System*>* ppSystem );
	int release_images(vector<BGAPI::Image*>* ppImage );

	static BGAPI_RESULT BGAPI_CALLBACK imageCallback(void * callBackOwner, BGAPI::Image* pImage);
	unsigned char* m_imagebuffer;
	vector<BGAPI::Image*> ppImage;
public:
	BGAPI::Camera* m_tcamera;		 //�����������ָ������
	bool m_binit;

	static BGAPI::System* pSystem;            
	BGAPI::Image* m_pImage;           //����ͼ��ָ��
	int m_nindex;   //������

	bool m_CapturedImage;
	//������

	int m_width;
	int m_height;


	static int initsystems();
	static bool m_initflag; 

};

