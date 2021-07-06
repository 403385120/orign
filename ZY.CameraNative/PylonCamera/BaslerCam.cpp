#include "stdafx.h"
#include "BaslerCam.h"
#include <fstream>

std::ofstream of_DMC_Log("D://TestLog//Data.txt");

BaslerCam::BaslerCam(void)
{
	m_nCurrentIndex = -1;
	m_bSaveImage = false;
	bNeedImageConverter = false;
	m_bframeStartAvailable = false;
	m_bAcquisitionStartAvailable = false;
	m_bFrameBurstStartAvailable = false;
	bIsGige = false;
	bIsUSB3 = false;
	triggerSelectorValue = "FrameStart";

	m_pImage1 = new BYTE[CAMERA_IMAGE_SIZE];
	m_pTotalImage = new BYTE[CAMERA_IMAGE_SIZE * 8];
	m_iCaptureNum = 8;
	m_iNowCaptureNum = 0; 
	bCaptureFinish = false;

	g_pImageData = NULL;
	g_pDataBufferInt = NULL;
}

BaslerCam::~BaslerCam(void)
{
	MyCloseDevice();

	DestroyDevice();
}

void BaslerCam::RunPylonAutoInitTerm()
{
	Pylon::PylonAutoInitTerm autoInitTerm;

}

int BaslerCam::EnumateAllDevices()
{
	int nTotalCamNum = 0;

	try
	{
		Pylon::CTlFactory& tlFactory = CTlFactory::GetInstance();

		nTotalCamNum = tlFactory.EnumerateDevices(g_DeviceInfoList);

	}
	catch (GenICam::GenericException &e)
	{	
		CString str;
		str.Format(_T("InitAll() error: %s"),e.GetDescription());
		TRACE(str);

	}
  
	g_nTotalDeviceNum = nTotalCamNum;

	return nTotalCamNum ;

}
int  BaslerCam::GetDeviceIndexFromUserID(CString UserID)
{
	int nIndex = -1;

	// Create an instant camera object with the camera found first.
	/*CDeviceInfo info;
		info.SetDeviceClass( Camera_t::DeviceClass());
	info.SetUserDefinedName("Device1");
	CInstantCamera camera( CTlFactory::GetInstance().CreateFirstDevice(info));*/

	/*int j = -1;
	for (DeviceInfoList_t::iterator it = g_DeviceInfoList.begin (); it != g_DeviceInfoList.end (); it++)
	{
		j++;
		CString str = (*it).GetSerialNumber().c_str ();
		
		str = (*it).GetModelName().c_str ();

		str = (*it).GetUserDefinedName().c_str ();


		if( str == UserID)
		{
			nIndex = j;
			break;
		}	
	}*/

	return nIndex;
}
bool BaslerCam::SetDeviceIndex(int nIndex)
{
	bool btn = false;

	if (nIndex>=0 && nIndex<g_nTotalDeviceNum)
	{
		m_nCurrentIndex = nIndex;
		btn = true;
	}
	else
	{
		CString str;
		str.Format(_T("SetDeviceIndex error:  nIndex = %d > g_nTotalDeviceNum = %d"), nIndex, g_nTotalDeviceNum);
		AfxMessageBox(str);
	}

	return btn;
}


CString BaslerCam::GetDeviceSN()
{
	CString strDeviceSN;
	strDeviceSN="";

	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		DeviceInfoList_t::iterator it = g_DeviceInfoList.begin () + m_nCurrentIndex;
		strDeviceSN = (*it).GetSerialNumber().c_str ();
		int sn = atol((*it).GetSerialNumber().c_str ());
	}

	return strDeviceSN;
}

CString BaslerCam::GetDeviceUserID()
{
	CString strDeviceUserID;
	strDeviceUserID="";

	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		DeviceInfoList_t::iterator it = g_DeviceInfoList.begin () + m_nCurrentIndex;
		strDeviceUserID = (*it).GetUserDefinedName ().c_str ();
	}
	return strDeviceUserID;
}

bool BaslerCam::OpenDevice()
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		if (m_InstantCamera.IsPylonDeviceAttached())
		{
			m_InstantCamera.DetachDevice();
			m_InstantCamera.DestroyDevice();
		}				
			m_InstantCamera.Attach( CTlFactory::GetInstance().CreateDevice(g_DeviceInfoList[m_nCurrentIndex]));
			m_InstantCamera.Open();

			//判断是USB or Gige 
			/*CString strModelName = m_InstantCamera.GetDeviceInfo().GetModelName().c_str ();
			if(strModelName.Find("g") != -1)
			{
				bIsGige = true;
			}
			else if(strModelName.Find("u") != -1)
			{
				bIsUSB3 = true;
			}*/

			bIsGige = true;

			// For demonstration purposes only, register another image event handler.
		//	m_InstantCamera.RegisterImageEventHandler( new CSampleImageEventHandler, RegistrationMode_Append, Cleanup_Delete);
		//	m_InstantCamera.RegisterConfiguration( this, RegistrationMode_Append, Ownership_ExternalOwnership);


			m_InstantCamera.RegisterImageEventHandler(this, RegistrationMode_Append, Ownership_ExternalOwnership);//内部virtual注册一个函数

			//// Get the trigger selector node.
			CEnumerationPtr triggerSelector(m_InstantCamera.GetNodeMap().GetNode("TriggerSelector"));
			// Frame start trigger mode available?
			GenApi::IEnumEntry* frameStart = triggerSelector->GetEntryByName("FrameStart");
			m_bframeStartAvailable = frameStart && IsAvailable(frameStart);

			GenApi::IEnumEntry* acquisitionStart = triggerSelector->GetEntryByName("AcquisitionStart");
			m_bAcquisitionStartAvailable = acquisitionStart && IsAvailable(acquisitionStart);
		
			// Acquisition start trigger mode available?
			GenApi::IEnumEntry* FrameBurstStart = triggerSelector->GetEntryByName("FrameBurstStart");
			m_bFrameBurstStartAvailable = FrameBurstStart && IsAvailable(FrameBurstStart);

			btn = true;
	}

	return btn;
}

bool BaslerCam::MyCloseDevice()
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		if (m_InstantCamera.IsOpen())
		{
	//		m_InstantCamera.DeregisterImageEventHandler(this);
			m_InstantCamera.Close();
		}

	}
	return btn;
}
int BaslerCam::OpenDevice(char* index, int width, int height, bool xflip, bool yflip)
{
	m_nDeviceNum = EnumateAllDevices();

	m_bCam1Valid = false;

	if (m_nDeviceNum == 1)
	{
		if (!m_bCam1Valid)
		{
			SetDeviceIndex(0);
			m_bCam1Valid = OpenDevice();
			SetWidth(CAMERA_IMAGE_WIDTH);
			SetHeight(CAMERA_IMAGE_HEIGHT);
			SetToSoftwareTrigerMode();
			SetPixelFormatToMono8();
			GrabImageStart();
		}
	}
	else
	{
		AfxMessageBox(_T(" 打开相机失败，请确定相机连接正常？"));
	}

	return m_bCam1Valid == true ? 0 : -1;
}

int BaslerCam::CloseDevice()
{

	if (m_bCam1Valid)
	{
		GrabImageStop();
		MyCloseDevice();
		DestroyDevice();
	}

	return 0;
}


int BaslerCam::SetVideoOn(bool bOn)
{
	//未使用
	return 0;
}

int BaslerCam::SoftShot(int numToGrab)
{
	//未使用
	return 0;
}

int BaslerCam::SetExposal(int nExpVal)
{
	if (false == SetExposureTimeRaw(nExpVal))
	{
		return -1;
	}

	return 0;
}

int BaslerCam::GetExposal()
{
	return 0;
}

int BaslerCam::SetGain(int nGainVal)
{
	if (false == SetGainAbs(nGainVal))
	{
		return -1;
	}

	return 0;
}

int BaslerCam::GetGain()
{
	return 0;
}

int BaslerCam::GetBufferImagesCnt()
{
	return 0;
}

int BaslerCam::GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages, int flag)
{
	if (pImages->data == NULL)
	{
		//InsertMessageToList(L"if(pImageBuffer==NULL");
		return false;

	}

	try
	{
		if (g_pImageData == NULL)
		{
			g_pImageData = new BYTE[COUNT_OF_IMAGES_TO_CAPTURE*CAMERA_IMAGE_SIZE];
		}

		if (g_pDataBufferInt == NULL)
		{
			g_pDataBufferInt = new int[CAMERA_IMAGE_SIZE];
		}
		memset(g_pImageData, 0, CAMERA_IMAGE_SIZE);
		memset(g_pDataBufferInt, 0, sizeof(int)*CAMERA_IMAGE_SIZE);
	}
	catch (...)
	{
		//InsertMessageToList(L"Eror In CaptureOneImage");
		return false;
	}
	/*if (g_bUseImageFileAsSource == TRUE)
	{
	int w = 0, h = 0;
	LoadImageData(pImageBuffer, w, h);
	return true;
	}*/

	for (int i = 0; i < COUNT_OF_IMAGES_TO_CAPTURE; i++)
	{
		SendSoftwareTrigerCommand();
		while (!bCaptureFinish)
		{
			Sleep(10);
		}
		memcpy_s((g_pImageData + i*CAMERA_IMAGE_SIZE), CAMERA_IMAGE_SIZE, m_pImage1, CAMERA_IMAGE_SIZE);
	}
	memset(m_pImage1, 0, CAMERA_IMAGE_SIZE);
	bool bRet = false;
	try
	{
		bRet = AverageValue((BYTE*)g_pImageData, g_pDataBufferInt, CAMERA_IMAGE_SIZE, COUNT_OF_IMAGES_TO_CAPTURE);
		if (bRet)
		{
			bRet = ConvertIntToBYTE(g_pDataBufferInt, pImages->data, CAMERA_IMAGE_SIZE);
			if (flag != 0)
			{
				Rotate(pImages->data, CAMERA_IMAGE_WIDTH, CAMERA_IMAGE_HEIGHT, flag);
			}
		}
	}
	catch (...)
	{
		bRet = false;
		//InsertMessageToList(L"Eror In CaptureOneImage");

	}
	return bRet ? 0 : -1;
}

int BaslerCam::GetAllSerioNum(vector<char*>& vectornum)
{
	return 0;
}

bool BaslerCam::DestroyDevice()
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		if (m_InstantCamera.IsPylonDeviceAttached())
		{
			m_InstantCamera.DetachDevice();
			m_InstantCamera.DestroyDevice();

		}
		btn = true;
	}
	return btn;
}

bool BaslerCam::SaveSettingFile(CString strFileName)
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		// Save the content of the camera's node map into the file.
		char chName[255];
		sprintf_s(chName, "%s", strFileName);
		CFeaturePersistence::Save( chName, &m_InstantCamera.GetNodeMap());

		btn = true;
	}
	return btn;
}

bool BaslerCam::LoadSettingFile(CString strFileName)
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{  
//		WIN32_FIND_DATA fd;  
//		TCHAR chName[255];
//		sprintf_s(chName, "%s", strFileName);
//		HANDLE  hFind = FindFirstFile(chName, &fd);
//		 if (hFind != INVALID_HANDLE_VALUE)		
		{
			// Just for demonstration, read the content of the file back to the camera's node map with enabled validation.
//			CFeaturePersistence::Load( chName, &m_InstantCamera.GetNodeMap(), true );	
			btn = true;
		}
		
	}
	return btn;
}

bool BaslerCam::SetWidth( int32_t nWidth)
{
	//INodeMap& nodemap = m_InstantCamera.GetNodeMap();

	bool btn = true;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{

		CIntegerPtr Ptrwidth( m_InstantCamera.GetNodeMap().GetNode( "Width"));

		if ( IsWritable( Ptrwidth))
		{
			int64_t newwidth = Adjust(nWidth, Ptrwidth->GetMin(), Ptrwidth->GetMax(), Ptrwidth->GetInc());
			Ptrwidth->SetValue(newwidth);
		}
		
	}
	return btn;

}

bool BaslerCam::SetHeight(int32_t nHeight)
{
	bool btn = true;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{

		CIntegerPtr Ptrheight( m_InstantCamera.GetNodeMap().GetNode( "Height"));

		if ( IsWritable( Ptrheight))
		{
			int64_t newHeight = Adjust(nHeight, Ptrheight->GetMin(), Ptrheight->GetMax(), Ptrheight->GetInc());
			Ptrheight->SetValue(newHeight);
		}

	}
	return btn;
}

int64_t BaslerCam::GetWidth()
{
	int64_t nWidth = 0;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{

		CIntegerPtr Ptrwidth( m_InstantCamera.GetNodeMap().GetNode( "Width"));

		if ( IsReadable( Ptrwidth))
		{
			nWidth = Ptrwidth->GetValue();
		}

	}
	return nWidth;
}
int64_t BaslerCam::GetHeight()
{
	int64_t nHeight = 0;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{

		CIntegerPtr Ptrheight( m_InstantCamera.GetNodeMap().GetNode( "Height"));

		if ( IsReadable( Ptrheight))
		{
			nHeight = Ptrheight->GetValue();
		}
	}
	return nHeight;
}

int64_t BaslerCam::GetWidthMax()
{
	int64_t nWidthMax = 0;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{

		CIntegerPtr Ptrwidth( m_InstantCamera.GetNodeMap().GetNode( "Width"));

		if ( IsReadable( Ptrwidth))
		{
			nWidthMax = Ptrwidth->GetMax();
		}

	}
	return nWidthMax;
}
int64_t BaslerCam::GetHeightMax()
{
	int64_t nHeightMax = 0;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{

		CIntegerPtr Ptrheight( m_InstantCamera.GetNodeMap().GetNode( "Height"));

		if ( IsReadable( Ptrheight))
		{
			nHeightMax = Ptrheight->GetMax();
		}
	}
	return nHeightMax;
}

bool BaslerCam::SetOffsetX(int32_t nOffsetX)
{
	bool btn = true;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		CIntegerPtr PtroffsetX(  m_InstantCamera.GetNodeMap().GetNode( "OffsetX"));
		if ( IsWritable( PtroffsetX))
		{
			int64_t newoffsetX = Adjust(nOffsetX, PtroffsetX->GetMin(), PtroffsetX->GetMax(), PtroffsetX->GetInc());
			PtroffsetX->SetValue(newoffsetX);
		}
	}	
	return btn;
}

bool BaslerCam::SetOffsetY(int32_t nOffsetY)
{
	bool btn = true;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		CIntegerPtr PtroffsetY(  m_InstantCamera.GetNodeMap().GetNode( "OffsetY"));
		if ( IsWritable( PtroffsetY))
		{
			int64_t newoffsetY = Adjust(nOffsetY, PtroffsetY->GetMin(), PtroffsetY->GetMax(), PtroffsetY->GetInc());
			PtroffsetY->SetValue(newoffsetY);
		}
	}	
	return btn;
}

bool BaslerCam::SetCenterX(bool bSetValue)
{
	bool btn = true;

	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		CBooleanPtr PtrCenterX(m_InstantCamera.GetNodeMap().GetNode("CenterX"));
		if ( IsWritable( PtrCenterX))
		{
			PtrCenterX->SetValue(true);
		}
	}	
	return btn;
}
bool BaslerCam::SetCenterY(bool bSetValue)
{
	bool btn = true;

	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		CBooleanPtr PtrCenterY(m_InstantCamera.GetNodeMap().GetNode("CenterY"));
		if ( IsWritable( PtrCenterY))
		{
			PtrCenterY->SetValue(true);
		}
	}	
	return btn;
}

bool BaslerCam::SetPixelFormatToMono8()
{
	bool btn = true;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		// Access the PixelFormat enumeration type node.
		CEnumerationPtr PtrpixelFormat( m_InstantCamera.GetNodeMap().GetNode( "PixelFormat"));

		// Set the pixel format to Mono8 if available.
		if ( IsAvailable( PtrpixelFormat->GetEntryByName( "Mono8")))
		{
			PtrpixelFormat->FromString( "Mono8");
			m_ImageConverter.OutputPixelFormat = PixelType_Mono8;
			bNeedImageConverter = false;
		}
		else
		{
			btn = false;
		}
	}

	return btn;
}

bool BaslerCam::SetPixelFormatToBayerBG8()
{
	bool btn = true;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		// Access the PixelFormat enumeration type node.
		CEnumerationPtr PtrpixelFormat( m_InstantCamera.GetNodeMap().GetNode( "PixelFormat"));

		// Set the pixel format to Mono8 if available.
		if ( IsAvailable( PtrpixelFormat->GetEntryByName( "BayerBG8")))
		{
			PtrpixelFormat->FromString( "BayerBG8");
			m_ImageConverter.OutputPixelFormat = PixelType_RGB8planar;
			bNeedImageConverter = true;
		}
		else
		{
			btn = false;
		}
	}

	return btn;
}


bool  BaslerCam::GrabImageStart()
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		if(m_InstantCamera.IsOpen())
		{
			if(!m_InstantCamera.IsGrabbing())
			{
				// The parameter MaxNumBuffer can be used to control the count of buffers
				// allocated for grabbing. The default value of this parameter is 10.
				m_InstantCamera.MaxNumBuffer = 10;

				// Enable the continuous acquisition mode.
				//CEnumerationPtr(m_InstantCamera.GetNodeMap().GetNode("AcquisitionMode"))->FromString("Continuous");

				m_InstantCamera.StartGrabbing(GrabStrategy_OneByOne, GrabLoop_ProvidedByInstantCamera);//这个是正确的，
			}
			btn = true;
		}
	}
	return btn;
}

//bool  BaslerCam::GrabImageStartContinuousThread()
//{
//		bool btn = false;
//	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
//	{
//		if(m_InstantCamera.IsOpen())
//		{
//
//			if(!m_InstantCamera.IsGrabbing())
//			{
//				// The parameter MaxNumBuffer can be used to control the count of buffers
//				// allocated for grabbing. The default value of this parameter is 10.
//				m_InstantCamera.MaxNumBuffer =10;
//
//				// Enable the continuous acquisition mode.
//				CEnumerationPtr(m_InstantCamera.GetNodeMap().GetNode("AcquisitionMode"))->FromString("Continuous");
//
//				//grab stratery upcoming is not supported for USB camera devices.
//				//m_InstantCamera .StartGrabbing(GrabStrategy_UpcomingImage,GrabLoop_ProvidedByInstantCamera);
//
//				m_InstantCamera.StartGrabbing(GrabStrategy_OneByOne, GrabLoop_ProvidedByInstantCamera);
//
//				 unsigned int timeoutMs;
//				 CGrabResultPtr pGrabResult;
//				m_InstantCamera .RetrieveResult( timeoutMs, pGrabResult, TimeoutHandling_ThrowException);
//
//				
//			}
//			btn = true;
//		}
//	
//		
//	}
//	return btn;
//}


bool  BaslerCam::GrabOneImage(/*uint8_t* pImageBuffer,*/ unsigned int timeoutMs)
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		CGrabResultPtr pGrabResult;
		// Wait for an image and then retrieve it. A timeout of 5000 ms is used.
		m_InstantCamera .RetrieveResult( 50000, pGrabResult, TimeoutHandling_ThrowException);

		//m_InstantCamera .RetrieveResult( timeoutMs, pGrabResult, TimeoutHandling_ThrowException);

		// Image grabbed successfully?
		if(pGrabResult->GrabSucceeded())
		{
			btn = true;

			uint8_t* buffer = (uint8_t *)pGrabResult->GetBuffer();

			// Display the grabbed image.
			Pylon::DisplayImage(1, pGrabResult);

			static int GrabImagesNum = 0;
			if (m_bSaveImage)
			{
				EPixelType pixelType = pGrabResult->GetPixelType();
				uint32_t width = pGrabResult->GetWidth();
				uint32_t height = pGrabResult->GetHeight();
				size_t paddingX = pGrabResult->GetPaddingX();
				EImageOrientation orientation = ImageOrientation_TopDown; //pGrabResult->GetOrientation();
				size_t bufferSize = pGrabResult->GetImageSize();
				char strName[200];
				sprintf_s(strName, " GrabImagesNum%d-%d.bmp", m_nCurrentIndex, GrabImagesNum);					
				GrabImagesNum++;

				CImagePersistence::Save( 
					ImageFileFormat_Bmp, 
					strName, 
					buffer, 
					bufferSize, 
					pixelType, 
					width, 
					height, 
					paddingX, 
					orientation);
			}
		}
		else//if(pGrabResult->GrabSucceeded())
		{
			CString str;
			str.Format(_T("grab failed"));
			TRACE(str);
			//AfxMessageBox(str);
			
		}
	}
	return btn;
}

bool  BaslerCam::GrabOneImage(uint8_t* pImageBuffer, unsigned int timeoutMs)
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		CGrabResultPtr pGrabResult;
		// Wait for an image and then retrieve it. A timeout of 5000 ms is used.
		
		for(int c=0;c<20;c++)
		{
			m_InstantCamera .RetrieveResult( 50000, pGrabResult, TimeoutHandling_ThrowException);

			//m_InstantCamera .RetrieveResult( timeoutMs, pGrabResult, 500);
			// Image grabbed successfully?
			if(pGrabResult->GrabSucceeded())
			{
				btn = true;

				if ( m_ImageConverter.ImageHasDestinationFormat( pGrabResult))
				{
					uint8_t* buffer = (uint8_t *)pGrabResult->GetBuffer();
					memcpy(pImageBuffer, buffer, pGrabResult->GetPayloadSize());
				}
				else
				{
					m_ImageConverter.Convert( m_targetImage, pGrabResult);
					memcpy(pImageBuffer, m_targetImage.GetBuffer(), m_targetImage.GetAllocatedBufferSize());
					

					//CImagePersistenceOptions additionalOptions;
					//// Set the lowest quality value.
					//additionalOptions.SetQuality(0);

					//CImagePersistence::Save( ImageFileFormat_Jpeg, "m_targetImage.jpg", m_targetImage, &additionalOptions);

				}

			}
			else//if(pGrabResult->GrabSucceeded())
			{
				CString str;
				str.Format(_T("grab failed"));
				TRACE(str);
				//AfxMessageBox(str);

			}
		}
	}
	return btn;
}



bool  BaslerCam::GrabImageStop()
{
	bool btn = false;
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		if(m_InstantCamera.IsOpen())
		{

			if(m_InstantCamera.IsGrabbing())
			{
				m_InstantCamera .StopGrabbing();
				Sleep(1000);
				btn = true;

			}
		}
	}
	return btn;
}

bool  BaslerCam::SetHeartbeatTimeout(int64_t NewValue_ms)
{
	bool btn = false;

	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		//String_t m_InstantCamera.GetDeviceInfo().GetModelName()；
		if (bIsGige)
		{
			GenApi::CIntegerPtr m_pHeartbeatTimeout = NULL;
			m_pHeartbeatTimeout = m_InstantCamera.GetTLNodeMap().GetNode("HeartbeatTimeout");
			if (m_pHeartbeatTimeout.IsValid())
			{
				// int64_t maxVal = m_pHeartbeatTimeout->GetMax();
				// Apply the increment and cut off invalid values if neccessary.
				int64_t correctedValue = NewValue_ms - (NewValue_ms % m_pHeartbeatTimeout->GetInc());
				m_pHeartbeatTimeout->SetValue(correctedValue);
			
				btn = true;
			}
		}
	}

	return btn;

}
//bool btn = false;
//if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
//{
//
//	btn = true;
//}
//return btn;


// Adjust value to make it comply with range and increment passed.
//
// The parameter's minimum and maximum are always considered as valid values.
// If the increment is larger than one, the returned value will be: min + (n * inc).
// If the value doesn't meet these criteria, it will be rounded down to ensure compliance.
int64_t BaslerCam::Adjust(int64_t val, int64_t minimum, int64_t maximum, int64_t inc)
{
	// Check the input parameters.
	if (inc <= 0)
	{
		// Negative increments are invalid.
		throw LOGICAL_ERROR_EXCEPTION("Unexpected increment %d", inc);
	}
	if (minimum > maximum)
	{
		// Minimum must not be bigger than or equal to the maximum.
		throw LOGICAL_ERROR_EXCEPTION("minimum bigger than maximum.");
	}

	// Check the lower bound.
	if (val < minimum)
	{
		return minimum;
	}

	// Check the upper bound.
	if (val > maximum)
	{
		return maximum;
	}

	// Check the increment.
	if (inc == 1)
	{
		// Special case: all values are valid.
		return val;
	}
	else
	{
		// The value must be min + (n * inc).
		// Due to the integer division, the value will be rounded down.
		return minimum + ( ((val - minimum) / inc) * inc );
	}
}

bool  BaslerCam::SetGainAbs(int64_t nValue)
{

	bool btn = true;
	try
	{
		if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
		{
			INodeMap& nodemap = m_InstantCamera.GetNodeMap();
			CEnumerationPtr PtrgainAuto( nodemap.GetNode( "GainAuto"));
			if ( IsWritable( PtrgainAuto))
			{
				PtrgainAuto->FromString("Off");
			}

			// Access the GainRaw integer type node.
			if(bIsGige)
			{
				//CIntegerPtr PtrgainRaw( nodemap.GetNode( "GainRaw"));
				//if ( PtrgainRaw.IsValid())
				//{
				//	// Make sure the calculated value is valid.
				//	int64_t newGainRaw = Adjust(nValue, PtrgainRaw->GetMin(), PtrgainRaw->GetMax(), PtrgainRaw->GetInc());
				//	PtrgainRaw->SetValue(newGainRaw);
				//}

				CFloatPtr PtrgainAbs( nodemap.GetNode( "GainAbs"));
				if ( PtrgainAbs.IsValid())
				{
					// Make sure the calculated value is valid.

					PtrgainAbs->SetValue(nValue);
				}

			}
			else if (bIsUSB3)
			{
				CFloatPtr Ptrgain( nodemap.GetNode( "Gain"));
				if ( Ptrgain.IsValid())
				{
					// Make sure the calculated value is valid.
					
					Ptrgain->SetValue(nValue);
				}
			}
			
			else
			{
				btn = false;
			}
		}

	}
	catch (GenICam::GenericException &e)
	{
		CString str;			
		str.Format(_T("SetGainRaw() error: %s"),e.GetDescription());
		TRACE(str);
		//AfxMessageBox(str);
		btn = false;
	}

	return btn;
}

bool  BaslerCam::SetExposureTimeRaw(int64_t nValue)
{
	bool btn = true;
	try
	{
		if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
		{
			INodeMap& nodemap = m_InstantCamera.GetNodeMap();
			CEnumerationPtr ptrExposureMode( nodemap.GetNode ("ExposureMode"));
			ptrExposureMode->FromString ("Timed");

			if (bIsGige)
			{
				CIntegerPtr ptrExposureTimeRaw( nodemap.GetNode ("ExposureTimeRaw"));
				if ( IsWritable( ptrExposureTimeRaw))
				{
					int64_t newExposureTimeRaw= Adjust(nValue, ptrExposureTimeRaw->GetMin(), ptrExposureTimeRaw->GetMax(), ptrExposureTimeRaw->GetInc());
					ptrExposureTimeRaw->SetValue(newExposureTimeRaw);
				}
			}
			else if(bIsUSB3)
			{
				CFloatPtr ptrExposureTime( nodemap.GetNode ("ExposureTime"));
				if ( IsWritable( ptrExposureTime))
				{
					int64_t newExposureTime= Adjust(nValue, ptrExposureTime->GetMin(), ptrExposureTime->GetMax(), ptrExposureTime->GetInc());
					ptrExposureTime->SetValue(newExposureTime);
				}
			}

		}

	}
	catch (GenICam::GenericException &e)
	{
		CString str;			
		str.Format(_T("SetExposureTimeRaw() error: %s"),e.GetDescription());
		TRACE(str);
		//AfxMessageBox(str);
		btn = false;
	}

	return btn;
}

bool  BaslerCam::SetToFreeRunMode()
{
	bool btn = true;

	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		//// Get the trigger selector node.
		CEnumerationPtr triggerSelector(m_InstantCamera.GetNodeMap().GetNode("TriggerSelector"));
		// Get the trigger mode node.
		CEnumerationPtr triggerMode(m_InstantCamera.GetNodeMap().GetNode("TriggerMode"));

		if ( m_bFrameBurstStartAvailable)
		{
			// Camera uses the acquisition start trigger as the only trigger mode.
			triggerSelector->FromString("FrameBurstStart");
			triggerMode->FromString("Off");
		}

		if (m_bframeStartAvailable)
		{
			triggerSelector->FromString("FrameStart");
			triggerMode->FromString("Off");
		}

		if (m_bAcquisitionStartAvailable)
		{
			triggerSelector->FromString("AcquisitionStart");
			triggerMode->FromString("Off");
		}
	}


	return btn;
}


bool  BaslerCam::SetToSoftwareTrigerMode()
{
	bool btn = true;

	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		//// Get the trigger selector node.
		CEnumerationPtr triggerSelector(m_InstantCamera.GetNodeMap().GetNode("TriggerSelector"));
		// Get the trigger mode node.
		CEnumerationPtr triggerMode(m_InstantCamera.GetNodeMap().GetNode("TriggerMode"));

		if (bIsGige)
		{
			// Check to see if the camera implements the acquisition start trigger mode only.
			if ( m_bAcquisitionStartAvailable && !m_bframeStartAvailable)
			{
				// Camera uses the acquisition start trigger as the only trigger mode.
				triggerSelector->FromString("AcquisitionStart");
				triggerMode->FromString("On");
				triggerSelectorValue = "AcquisitionStart";
			}
			else  
			{   
				// Camera may have the acquisition start trigger mode and the frame start trigger mode implemented.
				// In this case, the acquisition trigger mode must be switched off.
				if ( m_bAcquisitionStartAvailable )
				{
					triggerSelector->FromString("AcquisitionStart");
					triggerMode->FromString("Off");
				}
				// To trigger each single frame by software or external hardware trigger: Enable the frame start trigger mode.
				assert( m_bframeStartAvailable); //Frame start trigger mode must be available here.
				triggerSelector->FromString("FrameStart");
				triggerMode->FromString("On");
				triggerSelectorValue = "FrameStart";
			}
		}
		else if (bIsUSB3)
		{
			// Check to see if the camera implements the acquisition start trigger mode only.
			if ( m_bFrameBurstStartAvailable && !m_bframeStartAvailable)
			{
				// Camera uses the acquisition start trigger as the only trigger mode.
				triggerSelector->FromString("FrameBurstStart");
				triggerMode->FromString("On");
				triggerSelectorValue = "FrameBurstStart";
			}
			else  
			{   
				// Camera may have the acquisition start trigger mode and the frame start trigger mode implemented.
				// In this case, the acquisition trigger mode must be switched off.
				if ( m_bFrameBurstStartAvailable )
				{
					triggerSelector->FromString("FrameBurstStart");
					triggerMode->FromString("Off");
				}
				// To trigger each single frame by software or external hardware trigger: Enable the frame start trigger mode.
				assert( m_bframeStartAvailable); //Frame start trigger mode must be available here.
				triggerSelector->FromString("FrameStart");
				triggerMode->FromString("On");
				triggerSelectorValue = "FrameStart";
			}
		}


		// Note: The trigger selector must be set to the appropriate trigger mode 
		// before setting the trigger source or issuing software triggers.
		// Frame start trigger mode for newer cameras (i.e. for cameras supporting the standard image acquisition control mode).
		triggerSelector->FromString( triggerSelectorValue);

		// The trigger source must be set to 'Software'.
		CEnumerationPtr(m_InstantCamera.GetNodeMap().GetNode("TriggerSource"))->FromString("Software");	

	}


	return btn;
}

bool BaslerCam::SendSoftwareTrigerCommand()
{
	bool btn = true;
	
	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		if ( m_InstantCamera.WaitForFrameTriggerReady(500, TimeoutHandling_ThrowException))
		{
			bCaptureFinish = false;
			m_InstantCamera.ExecuteSoftwareTrigger();
		}
	}

	return btn;
}

bool  BaslerCam::SetToHardwareTrigerMode()
{
	bool btn = true;

	if (m_nCurrentIndex>=0 && m_nCurrentIndex<g_nTotalDeviceNum)
	{
		// Get the camera control object.
		INodeMap &Control = m_InstantCamera.GetNodeMap();

		//// Get the trigger selector node.
		CEnumerationPtr triggerSelector(Control.GetNode("TriggerSelector"));
		// Get the trigger mode node.
		CEnumerationPtr triggerMode(Control.GetNode("TriggerMode"));

		// Check to see if the camera implements the acquisition start trigger mode only.

		if (bIsGige)
		{
			// Check to see if the camera implements the acquisition start trigger mode only.
			if ( m_bAcquisitionStartAvailable && !m_bframeStartAvailable)
			{
				// Camera uses the acquisition start trigger as the only trigger mode.
				triggerSelector->FromString("AcquisitionStart");
				triggerMode->FromString("On");
				triggerSelectorValue = "AcquisitionStart";


				CIntegerPtr PtrAcquisitionFrameCount( m_InstantCamera.GetNodeMap().GetNode( "AcquisitionFrameCount"));

				if ( IsWritable( PtrAcquisitionFrameCount))
				{
					PtrAcquisitionFrameCount->SetValue(1);
				}
			}
			else  
			{   
				// Camera may have the acquisition start trigger mode and the frame start trigger mode implemented.
				// In this case, the acquisition trigger mode must be switched off.
				if ( m_bAcquisitionStartAvailable )
				{
					triggerSelector->FromString("AcquisitionStart");
					triggerMode->FromString("Off");
				}
				// To trigger each single frame by software or external hardware trigger: Enable the frame start trigger mode.
				assert( m_bframeStartAvailable); //Frame start trigger mode must be available here.
				triggerSelector->FromString("FrameStart");
				triggerMode->FromString("On");
			}
		}
		else if (bIsUSB3)
		{
			// Check to see if the camera implements the acquisition start trigger mode only.
			if ( m_bFrameBurstStartAvailable && !m_bframeStartAvailable)
			{
				// Camera uses the acquisition start trigger as the only trigger mode.
				triggerSelector->FromString("FrameBurstStart");
				triggerMode->FromString("On");
				triggerSelectorValue = "FrameBurstStart";
			}
			else  
			{   
				// Camera may have the acquisition start trigger mode and the frame start trigger mode implemented.
				// In this case, the acquisition trigger mode must be switched off.
				if ( m_bFrameBurstStartAvailable )
				{
					triggerSelector->FromString("FrameBurstStart");
					triggerMode->FromString("Off");
				}
				// To trigger each single frame by software or external hardware trigger: Enable the frame start trigger mode.
				assert( m_bframeStartAvailable); //Frame start trigger mode must be available here.
				triggerSelector->FromString("FrameStart");
				triggerMode->FromString("On");
			}
		}

		// Note: The trigger selector must be set to the appropriate trigger mode 
		// before setting the trigger source or issuing software triggers.
		// Frame start trigger mode for newer cameras (i.e. for cameras supporting the standard image acquisition control mode).
		triggerSelector->FromString( triggerSelectorValue);

		// The trigger source must be set to 'Software'.
		CEnumerationPtr(Control.GetNode("TriggerSource"))->FromString("Line1");	

		////The trigger activation must be set to e.g. 'RisingEdge'.
		CEnumerationPtr(Control.GetNode("TriggerActivation"))->FromString("RisingEdge");

		////Set TriggerDelay in microseconds
		if (bIsGige)
		{
			CFloatPtr(Control.GetNode("TriggerDelayAbs"))->SetValue(100);	

		}
		else if (bIsUSB3)
		{
			CFloatPtr(Control.GetNode("TriggerDelay"))->SetValue(100);	

		}
		
		////Set LineSelector to Line1
		CEnumerationPtr(Control.GetNode("LineSelector"))->FromString("Line1");
		////Set Line1 DebouncerTime in microseconds

		if (bIsGige)
		{
			CFloatPtr(Control.GetNode("LineDebouncerTimeAbs"))->SetValue(10);

		}
		else if (bIsUSB3)
		{
			CFloatPtr(Control.GetNode("LineDebouncerTime"))->SetValue(10);

		}

		
		//////set line1 Invert
		//CBooleanPtr(Control.GetNode("LineInverter"))->SetValue(true);


		//---------------------trigger set end--------------------------------------------------------


	}

	return btn;
}

void BaslerCam::OnImageGrabbed( CInstantCamera& camera, const CGrabResultPtr& ptrGrabResult)
{
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
	//Pylon::DisplayImage(ndispIndex, ptrGrabResult);

	const uint8_t *pImageBuffer = (uint8_t *) ptrGrabResult->GetBuffer();
	memcpy(m_pImage1, pImageBuffer, CAMERA_IMAGE_SIZE);
	bCaptureFinish = true;

	/*memcpy_s((m_pTotalImage + m_iNowCaptureNum*1000*1000), 1000*1000, pImageBuffer, 1000*1000);
	m_iNowCaptureNum++;

	if (m_iNowCaptureNum == m_iCaptureNum)
	{
		m_iNowCaptureNum = 0;
		AverageValue((BYTE*)m_pTotalImage, (int*)m_pImage1, 1000*1000, 8);
	}*/

	//cout << "CSampleImageEventHandler::OnImageGrabbed called." << std::endl;
	//cout << std::endl;
	//cout << std::endl;

	/*static int ncount=0;
	CString str = camera.GetDeviceInfo().GetUserDefinedName().c_str ();
	if (str == "CameraDown01")
	{
		ncount++;
		TRACE("OnImageGrabbed called \n");
	}*/
}


bool BaslerCam::AverageValue(BYTE * pByteDataBuffer,int * pIntDataBuffer,long lSize,int iCycle)
{
	if (iCycle <= 0)
	{
		return false;
	}

	if (pByteDataBuffer == NULL)
	{
		return false;
	}

	if (pIntDataBuffer == NULL)
	{
		return false;
	}

	try
	{
		for (int i = 0; i < iCycle; i ++)
		{
			for (int j = 0 ; j < lSize; j ++)
			{
				pIntDataBuffer[j] += pByteDataBuffer[i * lSize + j]; 
			}
		}

		for (int i = 0; i < lSize; i ++)
		{
			pIntDataBuffer[i] = pIntDataBuffer[i] / iCycle;

			if (*(pIntDataBuffer+i)< 0)
			{
				*(pIntDataBuffer+i) = 0;
			}

			if (*(pIntDataBuffer+i) > 255)
			{
				*(pIntDataBuffer+i) = 255;
			}
		}
	}
	catch (...)
	{

		return false;
	}

	return true;
}

bool BaslerCam::ConvertIntToBYTE(int * iBuffer,BYTE * bBuffer,long lSize)
{
	if (iBuffer == NULL)
	{
		return false;
	}

	if (bBuffer == NULL)
	{
		return false;
	}

	try
	{
		for (long l = 0; l < lSize; l ++)
		{
			*(bBuffer+l) = (BYTE)(*(iBuffer+l));
		}
		return true;
	}
	catch (...)
	{
		return false;
	}
}

void BaslerCam::ADLog(CString strMsg)
{
	CString strSysTime;
	SYSTEMTIME tm;
	GetLocalTime(&tm);
	strSysTime.Format(_T("%04d-%02d-%02d %02d:%02d:%02d "), tm.wYear, tm.wMonth, tm.wDay,
		tm.wHour, tm.wMinute, tm.wSecond);

	std::string strOut;
	USES_CONVERSION;

	strOut = W2A(strSysTime + strMsg);
	of_DMC_Log << strOut.c_str() << std::endl;
}

bool BaslerCam::Rotate(unsigned char* src, const int &SrcWidth, const int &SrcHeight, int iRotateDirec)
{
	if (src == NULL)
	{
		return FALSE;
	}
	if (SrcWidth < 1 || SrcHeight < 1)
	{
		return FALSE;
	}
	unsigned char tempSrc[CAMERA_IMAGE_SIZE];
	memcpy(tempSrc, src, SrcWidth * SrcHeight);
	for (int i = 0; i < SrcHeight; i++)
	{
		for (int j = 0; j < SrcWidth; j++)
		{
			switch (iRotateDirec)
			{
			case ROTATE90:
				*(src + j * SrcWidth + SrcHeight - 1 - i) = *(tempSrc + i * SrcWidth + j);
				break;
			case ROTATE180:
				*(src + (SrcHeight - 1 - i) * SrcWidth + SrcWidth - 1 - j) = *(tempSrc + i * SrcWidth + j);
				break;
			case ROTATE270:
				*(src + (SrcWidth - 1 - j) * SrcHeight + i) = *(tempSrc + i * SrcWidth + j);
				break;
			default:
				break;
			}

		}
	}
	return TRUE;
}
