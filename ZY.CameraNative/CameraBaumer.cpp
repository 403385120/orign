#include "CameraBaumer.h"
#include "BGDLL.h"
#pragma comment(lib, "BGDLL.lib")

CCameraBaumer::CCameraBaumer(void)
{
	m_tcamera = NULL;
	m_pImage = NULL;
	m_binit = false;
	m_LvboValue = 1;
	m_pinValue = 8;
	m_imagebuffer = NULL;

	m_nindex = -1;

	m_CapturedImage = false;

}

CCameraBaumer::~CCameraBaumer(void)
{

	
}


BGAPI_RESULT BGAPI_CALLBACK CCameraBaumer::imageCallback(void * callBackOwner, BGAPI::Image* pImage)
{
	BGAPI_RESULT res = BGAPI_RESULT_OK;
	

	CCameraBaumer* pcam = (CCameraBaumer*)callBackOwner;


	pcam->m_imagebuffer = NULL;
	//if (pcam->m_imagebuffer != NULL)
//	{
	//	delete pcam->m_imagebuffer;
	//	pcam->m_imagebuffer = NULL;
	//}

	
	pImage->get(&pcam->m_imagebuffer);
	//Now you have the Imagebuffer and can do with it whatever you want

	int swc = 0;
	int hwc = 0;
	pImage->getNumber(&swc, &hwc);

	//after you are ready with this image, return it to the camera for the next image
	res = pcam->m_tcamera->setImage(pImage);

	pcam->m_CapturedImage = true;

	return res;
}

// BGAPI_RESULT BGAPI_CALLBACK CCameraBaumer::imageCallback1(void * callBackOwner, BGAPI::Image* pImage)
// {
// 	BGAPI_RESULT res = BGAPI_RESULT_OK;
// 	m_imagebuffer1 = NULL;
// 
// 	/*DWORD dwRet = WaitForSingleObject(m_hStartCaptured1, 0);
// 	if (dwRet != WAIT_OBJECT_0)
// 	{
// 		return res;
// 	}
// 
// 	ResetEvent(m_hStartCaptured1);*/
// 	//m_hStartCaptured
// 
// 	pImage->get(&m_imagebuffer1);
// 	//Now you have the Imagebuffer and can do with it whatever you want
// 
// 	int swc = 0;
// 	int hwc = 0;
// 	pImage->getNumber(&swc, &hwc);
// 
// 	//after you are ready with this image, return it to the camera for the next image
// 	res = ((BGAPI::Camera*)callBackOwner)->setImage(pImage);
// 
// 	//SetEvent(m_hCapturedImage1);
// 
// 	return res;
// }
BGAPI::System* CCameraBaumer::pSystem = NULL;
bool CCameraBaumer::m_initflag = false; 
int  CCameraBaumer::initsystems()
{
	//CString LDtemp;
	int syscount = 0;
	BGAPI_RESULT res;

	//
	res = BGAPI::countSystems(&syscount);  //计算系统数

	if (res != BGAPI_RESULT_OK)
	{
		//AfxMessageBox(_T("计算相机系统数异常，请检查...."));
		return -1;
	}

	res = BGAPI::createSystem(0, &pSystem);   //创建系统

	if (res != BGAPI_RESULT_OK)
	{
		//AfxMessageBox(_T("创建相机系统异常，请检查...."));
		return -1;
	}

	res = pSystem->open();					//打开系统
	if (res != BGAPI_RESULT_OK)
	{
	//	AfxMessageBox(_T("打开相机系统异常，请检查...."));
		return -1;
	}


	int camcount = 0;
	res = pSystem->countCameras(&camcount);  //计算相机个数

	

	if (res != BGAPI_RESULT_OK)
	{
	//	AfxMessageBox(_T("计算相机个数异常，请检查...."));
		return -1;
	}
	//add by ljm
	if (camcount <= 0)
	{
		//AfxMessageBox(_T("计算相机个数异常，请检查...."));
		return -1;
	}

	return camcount;
}

int CCameraBaumer::GetIndex(char* serionum)  //根据序列号获取相机的序号
{
	for (int i = 0; i < baumeraserio.size(); i++)
	{
		if (0 == strcmp(serionum, baumeraserio[i]))
		{
			return i;
		}
	}

	return -1;
}

bool CCameraBaumer::GetCamerSerioNum()
{
	if (baumeraserio.size() == 0)
	{
		BGAPI_RESULT res;
		m_initflag = true;
		int ncount = initsystems();

		for (int n = 0; n < ncount; n++)
		{
			BGAPI::Camera* ptemp = NULL;

			char* szBuf = new char[255];

			res = pSystem->createCamera(n, &ptemp);
			if (res != BGAPI_RESULT_OK)
			{
				//AfxMessageBox(_T("初始化相机异常，请检查...."));
				return false;
			}
			BGAPIX_CameraInfo camdeviceinfo;					// 4.2 variable for Device Information
			camdeviceinfo.cbSize = sizeof(BGAPIX_CameraInfo);
			BGAPI_FeatureState state;							// 4.2 variable for Device Information
			state.cbSize = sizeof(BGAPI_FeatureState);			// 4.2 variable for Device Information
			res = ptemp->getDeviceInformation(&state, &camdeviceinfo); // check model and SN of camera
			if (res != BGAPI_RESULT_OK)
			{
				//printf("BGAPI::Camera::getDeviceInformation Errorcode: %d Camera index: %d\n", res, cam);
				//AfxMessageBox(_T("获取相机参数失败，请检查...."));
				return false;
			}
			strcpy(szBuf, camdeviceinfo.serialNumber);
			baumeraserio.push_back(szBuf);

			res = pSystem->releaseCamera(ptemp);
		}
	}


}


bool CCameraBaumer::Initial(char* serionum, int width, int height, bool xflip, bool yflip)
{
	m_width = width;
	m_height = height;

	//CString LDtemp;
	BGAPI_RESULT res;

	GetCamerSerioNum();

	int cameraindex = GetIndex(serionum);

	if (-1 == cameraindex)
	{
		return false;
	}

	
	

		//初始化相机
		res = pSystem->createCamera(cameraindex, &m_tcamera);
		if (res != BGAPI_RESULT_OK)
		{
			//AfxMessageBox(_T("初始化相机异常，请检查...."));
			return false;
		}

		//res = pSystem->releaseCamera(m_tcamera);

		//打开相机
		res = m_tcamera->open();
		if (res != BGAPI_RESULT_OK)
		{
			//AfxMessageBox(_T("打开相机异常，请检查...."));
			return false;
		}

#pragma region add

		res = m_tcamera->setTriggerSource(BGAPI_TRIGGERSOURCE_SOFTWARE);
		if (res != BGAPI_RESULT_OK)
		{
			//AfxMessageBox(_T("打开相机异常，请检查...."));
			return false;
		}
		//m_chkTrgEnabled.SetCheck(0);
		//m_bEnabled = false;
		//m_pCamera->setTrigger(false);

		
		res = m_tcamera->setTrigger(true);
		if (res != BGAPI_RESULT_OK)
		{
			//AfxMessageBox(_T("打开相机异常，请检查...."));
			return false;
		}
#pragma endregion



		//创建图像对象
		res = BGAPI::createImage(&m_pImage);
		if (res != BGAPI_RESULT_OK)
		{
			//AfxMessageBox(_T("创建图像对象异常，请检查...."));
			return false;
		}

		

		
			res = m_tcamera->registerNotifyCallback(this, (BGAPI::BGAPI_NOTIFY_CALLBACK)&imageCallback);
			if (res != BGAPI_RESULT_OK)
			{
// 				CString  str;
// 				str.Format(_T("BGAPI::Camera::registerNotifyCallback Errorcode: %d for Camera1"), res);
// 				AfxMessageBox(str);
			}
		

		//图像对象和相机关联
		res = m_tcamera->setImage(m_pImage);

		if (res != BGAPI_RESULT_OK)
		{
			//AfxMessageBox(_T("图像对象和相机关联异常，请检查...."));
			return false;
		}

#pragma region XY偏移
		if (xflip == true && yflip == true)
		{
			res = m_tcamera->setFlipType(BGAPI_FLIPTYPE_XY);
		}
		else if (xflip == true)
		{
			res = m_tcamera->setFlipType(BGAPI_FLIPTYPE_X);
		}
		else if (yflip == true)
		{
			res = m_tcamera->setFlipType(BGAPI_FLIPTYPE_Y);
		}
		else
		{
			res = m_tcamera->setFlipType(BGAPI_FLIPTYPE_NONE);
		}

		if (res != BGAPI_RESULT_OK)
		{
			//XY偏移设置错误
			return false;
		}
#pragma endregion
	
		BGAPI_FeatureState fs;

		fs.cbSize = sizeof(BGAPI_FeatureState);

		res = m_tcamera->getGVSHeartBeatEnable(&fs);

		m_tcamera->setGVSHeartBeatEnable(false);

		//res = m_tcamera->setbi(m_pImage);

		if (res != BGAPI_RESULT_OK)
		{
			//AfxMessageBox(_T("图像对象和相机关联异常，请检查...."));
			return false;
		}
	//设置宽高1 靠右相机
	res = m_tcamera->setPartialScan(true, 0, 0, width, height);

	if (res != BGAPI_RESULT_OK)
	{
	
// 		LDtemp.Format(TEXT("%d"), res);
// 		AfxMessageBox(LDtemp);
// 		AfxMessageBox(_T("设置相机1宽高异常，请检查...."));
		return false;
	}

	//相机开始1
	res = m_tcamera->setStart(true);
	if (res != BGAPI_RESULT_OK)
	{
		//AfxMessageBox(_T("相机开始异常，请检查...."));
		return false;
	}

	return true;
}

bool CCameraBaumer::SaveImage(BYTE* bdata, int width, int height)
{
	//if (str.IsEmpty()) return false;
	/*long x = 0, y = 0, width = 0, height = 0;
	m_Camera->GetProperty()->GetROI(x, y, width, height);*/
	//m_Camera->GetProperty()->
	//if (m_Width <= 0 || m_Height <= 0) return 0;

	HANDLE hFile = CreateFile(TEXT("123.bmp"), GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	//DWORD dwError = GetLastError();
	if (!hFile || hFile == INVALID_HANDLE_VALUE)
		return false;

	// 
	BITMAPFILEHEADER	FileHeader;
	BITMAPINFOHEADER	InfoHeader;
	RGBQUAD rgbQuad[256];

	DWORD dwImageSize = width*height;
	FileHeader.bfType = 0x4D42; // "BM"
	FileHeader.bfSize = sizeof(FileHeader)+sizeof(InfoHeader)+dwImageSize;
	FileHeader.bfReserved1 = FileHeader.bfReserved2 = 0;
	FileHeader.bfOffBits = sizeof(FileHeader)+sizeof(InfoHeader);

	ZeroMemory(&InfoHeader, sizeof(InfoHeader));
	InfoHeader.biWidth = width;
	InfoHeader.biHeight = -height;
	InfoHeader.biSizeImage = dwImageSize;
	InfoHeader.biBitCount = 8;
	InfoHeader.biCompression = BI_RGB;
	InfoHeader.biPlanes = 1;
	InfoHeader.biSize = sizeof(BITMAPINFOHEADER);
	InfoHeader.biXPelsPerMeter = 0xEC4;
	InfoHeader.biYPelsPerMeter = 0xEC4;

	for (int i = 0; i<256; i++)
	{
		rgbQuad[i].rgbBlue = (BYTE)i;
		rgbQuad[i].rgbGreen = (BYTE)i;
		rgbQuad[i].rgbRed = (BYTE)i;
		rgbQuad[i].rgbReserved = 0;
	}

	DWORD Written;
	WriteFile(hFile, (LPVOID)&FileHeader, sizeof(BITMAPFILEHEADER), &Written, NULL);
	WriteFile(hFile, (LPVOID)&InfoHeader, sizeof(BITMAPINFOHEADER), &Written, NULL);
	WriteFile(hFile, (LPVOID)rgbQuad, 256 * sizeof(RGBQUAD), &Written, NULL);
	BYTE* src = bdata;
	//int stride = m_Width*4;


	//DWORD bytes = m_Width * 4;
	//for (unsigned long y = 0; y < NHEIGHT; y++) {
	WriteFile(hFile, (LPVOID)src, dwImageSize, &Written, NULL);
	//}
	CloseHandle(hFile);
	return true;
}

//关闭
void CCameraBaumer::CloseCam()
{
	
	
		int res = m_tcamera->setStart(false);
		if (res != BGAPI_RESULT_OK)
		{
			return;
		}

		res = pSystem->releaseImage(m_pImage);
		if (res != BGAPI_RESULT_OK)
		{
			return;
		}

		res = pSystem->releaseCamera(m_tcamera);
		if (res != BGAPI_RESULT_OK)
		{
			return;
		}
	
}

int CCameraBaumer::GetWidthMax()
{
	BGAPI_FeatureState pstate;
	BGAPIX_TypeRangeINT tWidth;
	BGAPIX_TypeRangeINT tHeight;
	m_tcamera->getImageResolution(&pstate,&tWidth,&tHeight);
	return tWidth.maximum;
}

int CCameraBaumer::GetHeightMax()
{
	BGAPI_FeatureState pstate;
	BGAPIX_TypeRangeINT tWidth;
	BGAPIX_TypeRangeINT tHeight;
	m_tcamera->getImageResolution(&pstate,&tWidth,&tHeight);
	return tHeight.maximum; 
}

bool CCameraBaumer::CaptureOneImage(BYTE * pImageBuffer,bool bcapture)
{
	if (m_width <= 0 || m_height <= 0 || pImageBuffer == NULL)
	{
		return false;
	}

	//unsigned char* imageRet = new unsigned char[m_width * m_height];

	int ncount = 0;

	const int captureTimeout = 1000;
	int elpsTime = 0;

	vector <UINT8*> pvector;

	int temp = 0;


		if (bcapture == true)
		{
			temp = 1;
		}
		else
		{
			temp = m_pinValue;
		}

		ncount = 0;
		while (ncount < temp && ncount < 15)
		{

#pragma region add
			int n  = m_tcamera->doTrigger();
			if (n != BGAPI_RESULT_OK)
			{
				//CString  str;
				//str.Format(_T("BGAPI::Camera::doTrigger Errorcode: %d"), m_Result);
				//AfxMessageBox(str);
				//return FALSE;

				//delete imageRet;
				for (int i = 0; i < pvector.size(); i++)
				{
					delete pvector[i];
				}

				return false;
			}
#pragma endregion


			while (false == m_CapturedImage && elpsTime < captureTimeout)
			{
				Sleep(2);
				elpsTime += 2;
			}
			if (elpsTime >= captureTimeout)
			{
				//delete imageRet;
				for (int i = 0; i < pvector.size(); i++)
				{
					delete pvector[i];
				}

				return false;
			}


			m_CapturedImage = false;
			unsigned char* imagebuffer = new unsigned char[m_width * m_height];


			m_CapturedImage = false;

			memcpy_s(imagebuffer, m_width*m_height, m_imagebuffer, m_width*m_height);
			pvector.push_back(imagebuffer);
			ncount++;
		}

		bool bRet = Training(&pvector, temp, m_width, m_height, m_width, pImageBuffer);
			//ImageSmoothAndSharpen(imageRet,ldwidth, ldheight, ldwidth, imageRet1,5, m_LvboValue);
			//做锐化处理
		//memcpy_s(pImageBuffer, m_width*m_height, imageRet, m_width*m_height);


	//delete imageRet;

	for (int i = 0; i < pvector.size(); i++)
	{
		delete pvector[i];
	}

	//SaveImage(pImageBuffer, 1600, 1200);
	pvector.clear();

	//num++;
	
	return true;
}

bool CCameraBaumer::SetGainAbs(float nValue)
{
	int res1 = m_tcamera->setGain(nValue);
	if (res1 != BGAPI_RESULT_OK)
	{
		return false;
	}

	return true;
}

int CCameraBaumer::GetGain()
{
	BGAPI_FeatureState bf;
	BGAPIX_TypeRangeFLOAT sfloat;

	bf.cbSize = sizeof( BGAPI_FeatureState );
	sfloat.cbSize = sizeof( BGAPIX_TypeRangeFLOAT );

	m_tcamera->getGain(&bf,&sfloat);

	return sfloat.current;

	return 0;
}

int CCameraBaumer::GetGainMaxAndMin(float& MaxValue, float& MinValue)
{
	BGAPI_RESULT res = BGAPI_RESULT_FAIL;
	BGAPI_FeatureState bf;
	BGAPIX_TypeRangeFLOAT sfloat;

	bf.cbSize = sizeof( BGAPI_FeatureState );
	sfloat.cbSize = sizeof( BGAPIX_TypeRangeFLOAT );

	res = m_tcamera->getGain(&bf,&sfloat);

	MaxValue = sfloat.maximum;
	MinValue = sfloat.minimum;

	return 0;
}

int CCameraBaumer::GetExposure()
{
	BGAPI_RESULT res = BGAPI_RESULT_FAIL;
	BGAPI_FeatureState bf;
	BGAPIX_TypeRangeINT sfloat;

	bf.cbSize = sizeof( BGAPI_FeatureState );
	sfloat.cbSize = sizeof( BGAPIX_TypeRangeINT );

	m_tcamera->getExposure(&bf,&sfloat);

	return sfloat.current;

}
int CCameraBaumer::GetExposureMaxAndMin(float& MaxValue, float& Minvalue)
{
	//BGAPI_RESULT res = BGAPI_RESULT_FAIL;
	//BGAPI_FeatureState bf;
	//BGAPIX_TypeRangeINT sfloat;

	//bf.cbSize = sizeof( BGAPI_FeatureState );
	//sfloat.cbSize = sizeof( BGAPIX_TypeRangeINT );

	//m_tcamera->getExposure(&bf,&sfloat);

	//MaxValue = sfloat.maximum;
	//Minvalue = sfloat.minimum;

	return 0;
}

int CCameraBaumer::SetLvBoValue(int value)
{
	m_LvboValue = value;
	return 0;
}

int CCameraBaumer::GetLvBovalue()
{
	return m_LvboValue;
}

int CCameraBaumer::GetLvBoMaxAndMin(int& maxvalue, int& minvalue)
{
	maxvalue = 3;
	minvalue = 1;
	return 0;
}


int CCameraBaumer::SetPinValue(int value)
{
	m_pinValue = value;
	return 0;
}

int CCameraBaumer::GetPinValue()
{
	return m_pinValue;
}

int CCameraBaumer::GetPinMaxAndMin(int& maxvalue, int& minvalue)
{
	maxvalue = 15;
	minvalue = 1;
	return 0;
}

//曝光
bool CCameraBaumer::SetExposureTimeRaw(int nValue)
{
	int res1 = m_tcamera->setExposure(nValue);
	if (res1 != BGAPI_RESULT_OK)
	{
		return false;
	}

	return true;
}

vector<char*> CCameraBaumer::baumeraserio;

int CCameraBaumer::init_systems(int* system_count, vector<BGAPI::System*>* externppSystem)
{
	BGAPI_RESULT res = BGAPI_RESULT_FAIL;
	int i = 0;
	res = BGAPI::countSystems( system_count );

	if( res != BGAPI_RESULT_OK )
	{
		return res;
	}

	for( i = 0; i < *system_count; i++ )
	{
		BGAPI::System * pSystem = NULL;
		res = BGAPI::createSystem( i, &pSystem );
		if( res != BGAPI_RESULT_OK )
		{
			externppSystem->clear();
			return res;
		}
		res = pSystem->open();		
		if( res != BGAPI_RESULT_OK )
		{
			externppSystem->clear();
			return res;
		}

		externppSystem->push_back(pSystem);
	}
	return res;
}


int CCameraBaumer::init_camera(int system_count, vector<BGAPI::System*>* externppSystem, int* pCurrSystem, BGAPI::Camera** externppCamera)
{
	BGAPI_RESULT res = BGAPI_RESULT_FAIL;
	int cam = 0;
	int camera_count = 0;
	vector<int> cameras;
	vector<int>::iterator camIter;
	BGAPI_FeatureState state;
	BGAPIX_CameraInfo cameradeviceinfo;
	int inputVal = 0;
	vector<BGAPI::System*>::iterator systemIter;

	for( systemIter = externppSystem->begin(); systemIter != externppSystem->end(); systemIter++ )
	{		
		int count = 0;
		res = (*systemIter)->countCameras( &count );		
		if( res != BGAPI_RESULT_OK )
		{
			return res;
		}
		cameras.push_back( count );

		for( cam = 0; cam < count; cam++ )
		{
			camera_count++;

			res = (*systemIter)->createCamera( cam, externppCamera );
			if( res != BGAPI_RESULT_OK )
			{
				return res;
			}

			state.cbSize = sizeof( BGAPI_FeatureState );
			cameradeviceinfo.cbSize = sizeof( BGAPIX_CameraInfo );
			res = (*externppCamera)->getDeviceInformation( &state, &cameradeviceinfo );
			if( res != BGAPI_RESULT_OK )
			{	
				return res;
			}
			(*systemIter)->releaseCamera( *externppCamera );
		}
	}

// 	do
// 	{
// 		fflush( stdin );fflush( stdout );
// 		scanf( "%d", &inputVal );
// 	}
// 	while( inputVal < 0 || inputVal > camera_count );
	if(camera_count > 0) inputVal = 1;

	camera_count = 0;
	for( systemIter = externppSystem->begin(); systemIter != externppSystem->end(); systemIter++ )
	{		
		for( cam = 0; cam < cameras[systemIter - externppSystem->begin()]; cam++ )
		{
			camera_count++;
			if( camera_count == inputVal )
			{	
				*pCurrSystem = (int)(systemIter - externppSystem->begin());

				res = (*externppSystem)[*pCurrSystem]->createCamera( cam, externppCamera );
				if( res != BGAPI_RESULT_OK )
				{
					return res;
				}

				res = (*externppCamera)->open();
				if( res != BGAPI_RESULT_OK )
				{
					return res;
				}
				break;
			}
		}
	}
	return res;
}

int CCameraBaumer::release_systems( vector<BGAPI::System*> * externppSystem )
{
	BGAPI_RESULT res = BGAPI_RESULT_FAIL;
	vector<BGAPI::System*>::iterator systemIter;

	for( systemIter = externppSystem->begin(); systemIter != externppSystem->end(); systemIter++ )
	{
		res = (*systemIter)->release();
		if( res != BGAPI_RESULT_OK )
		{
		}
	}
	externppSystem->clear();
	return res;
}


int CCameraBaumer::release_images( vector<BGAPI::Image*>* ppImage )
{
	BGAPI_RESULT res = BGAPI_RESULT_FAIL;
	vector<BGAPI::Image*>::iterator imageIter;
	bool tmpExtern = false;
	unsigned char* tmpBuffer = NULL;

	for( imageIter = ppImage->begin(); imageIter != ppImage->end(); imageIter++ )
	{
		res = ((BGAPI::Image*)(*imageIter))->isExternBuffer( &tmpExtern );
		if( res != BGAPI_RESULT_OK )
		{
		}

		if( tmpExtern )
		{
			res = ((BGAPI::Image*)(*imageIter))->getBuffer( &tmpBuffer );
			if( res != BGAPI_RESULT_OK )
			{
			}
			else
			{
				free( tmpBuffer );
			}
		}
		res = BGAPI::releaseImage( *imageIter );
		if( res != BGAPI_RESULT_OK )
		{
		}
	}
	ppImage->clear();
	return res;
}

int CCameraBaumer::OpenDevice(char* index, int width, int height, bool xflip, bool yflip)
{
	if (false == Initial(index, width, height, xflip, yflip))
	{
		return -1;
	}

	m_width = width;
	m_height = height;
	
	return 0;
}

int CCameraBaumer::CloseDevice()
{
	CloseCam();
	return 0;
}

int CCameraBaumer::SetVideoOn(bool bOn)
{
	if (true == bOn)
	{
		m_tcamera->setStart(true);
	}
	else
	{
		m_tcamera->setStart(false);
	}

	return 0;
}

int CCameraBaumer::SoftShot(int numToGrab)
{
	return 0;
}

int CCameraBaumer::SetExposal(int nExpVal)
{
	if (false == SetExposureTimeRaw(nExpVal))
	{
		return -1;
	}

	return 0;
}

int CCameraBaumer::GetExposal()
{
	return GetExposure();
}

int CCameraBaumer::SetGain(int nGainVal)
{
	if (false == SetGainAbs(nGainVal))
	{
		return -1;
	}

	return 0;
}

int CCameraBaumer::GetBufferImagesCnt()
{
	return 0;
}

int CCameraBaumer::GetAllSerioNum(vector<char*>& vectornum)
{
	if (baumeraserio.size() == 0)
	{
		GetCamerSerioNum();
	}

	vectornum.clear();

	for (int n = 0; n < baumeraserio.size(); n++)
	{
		char* pstr = new char[255];

		strcpy_s(pstr, 100, baumeraserio[n]);

		vectornum.push_back(pstr);
	}
	return 0;
}

int CCameraBaumer::GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages)
{
	//unsigned char *imgtemp = new unsigned char[m_width * m_height];
	m_pinValue = *pImgCnt;
	bool bRet = CaptureOneImage(pImages->data, false);


	memset(pImages->data, 0, pImages->width * pImages->height * pImages->channel);

	if (!bRet)
	{
		return -1;
	}


	//memcpy((pImages)->data, imgtemp, m_width * m_height);

	//(pImages)->width = m_width;
	//(pImages)->height = m_height;
	//(pImages)->channel = 1;


	//delete imgtemp;

	
	return 0;
}
