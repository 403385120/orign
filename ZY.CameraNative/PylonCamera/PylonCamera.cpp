#include "stdafx.h"

#include "PylonCamera.h"
#include "..\\PylonCamera\\PylonCamera.h"
#include "..\\PylonCamera\\pylon\\PylonIncludes.h"

#include <pylon/PylonIncludes.h>
#include <pylon/PylonGUI.h>

//extern void LOGS(char * pText);
extern void CString2Char(CString str, char *pText);

// Namespace for using pylon objects.
using namespace Pylon;

// Namespace for using cout.
using namespace std;

HANDLE g_hEventCapture = NULL;
HANDLE g_hThreadCapture = NULL;

HANDLE g_hEventCaptureTwo = NULL;
HANDLE g_hThreadCaptureTwo = NULL;

extern int g_iMousePointedValue;

//extern CPoint g_CPointMousePoint;

//void PushWindowsImgToView(int index,IplImage* pImage);



bool AverageValue(BYTE * pByteDataBuffer,int * pIntDataBuffer,long lSize,int iCycle)
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
		CString strError;
		char mText[MAX_PATH];
		DWORD mLastError = GetLastError();
		strError.Format(L"Catch Error %d: line = %s Function = %s File = %s",mLastError,__LINE__,__FUNCDNAME__,__FILE__);
		sprintf_s(mText,MAX_PATH,"Catch Error %d: line = %d Function = %s File = %s",mLastError,__LINE__,__FUNCDNAME__,__FILE__);
		//LOGS(mText);
		return false;
	}

	return true;

}

bool ConvertIntToBYTE(int * iBuffer,BYTE * bBuffer,long lSize)
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
		CString strError;
		char mText[MAX_PATH];
		DWORD mLastError = GetLastError();
		strError.Format(L"Catch Error %d: line = %s Function = %s File = %s",mLastError,__LINE__,__FUNCDNAME__,__FILE__);
		sprintf_s(mText,MAX_PATH,"Catch Error %d: line = %d Function = %s File = %s",mLastError,__LINE__,__FUNCDNAME__,__FILE__);
		//LOGS(mText);
		return false;
	}
}

void GetMousePointGrey(BYTE * pBate)
{
	//if (pBate == NULL)
	//{
	//	return ;
	//}

	//if (g_CPointMousePoint.x > CAMERA_IMAGE_WIDTH)
	//{
	//	return ;
	//}

	//if (g_CPointMousePoint.y > CAMERA_IMAGE_HEIGHT)
	//{
	//	return ;
	//}

	//try
	//{
	//	g_iMousePointedValue = *(pBate + g_CPointMousePoint.x * CAMERA_IMAGE_WIDTH + g_CPointMousePoint.y);
	//}
	//catch (...)
	//{
	//	CString strError;
	//	char mText[MAX_PATH];
	//	DWORD mLastError = GetLastError();
	//	strError.Format(L"Catch Error %d: line = %s Function = %s File = %s",mLastError,__LINE__,__FUNCDNAME__,__FILE__);
	//	sprintf_s(mText,MAX_PATH,"Catch Error %d: line = %d Function = %s File = %s",mLastError,__LINE__,__FUNCDNAME__,__FILE__);
	//	LOGS(mText);
	//}

	
	
}


void CaptureAndSaveImage()
{
	//bool bRet = true;
	//BOOL bResult = FALSE;
	//BYTE * mDataBuffer = new BYTE[CAMERA_IMAGE_SIZE];
	//bRet = CaptureOneImage(mDataBuffer);


	////GetMousePointGrey(mDataBuffer);

	////g_CCamera.cam1.GrabImageStart();
	////bRet = g_CCamera.GrabFramesOne(10,mDataBuffer,CAMERA_IMAGE_WIDTH,CAMERA_IMAGE_HEIGHT);
	////g_CCamera.cam1.GrabImageStop();

	//CString strSavePath = g_SaveImage.GenarateOriginalPicPath();
	//char mStr[MAX_PATH] = {0};
	//CString2Char(strSavePath,mStr);

	//if (bRet)
	//{
	//	IplImage * pImage = cvCreateImage(cvSize(CAMERA_IMAGE_WIDTH,CAMERA_IMAGE_HEIGHT),8,1);

	//	bResult = str2IplImage(mDataBuffer ,  CAMERA_IMAGE_WIDTH ,  CAMERA_IMAGE_HEIGHT , pImage);

	//	if (bResult == TRUE &&  g_bSaveCaptureImageFile)
	//	{	
	//		SaveImageByIndex(mStr,pImage);
	//	}

	//	if (pImage)
	//	{
	//		cvReleaseImage(&pImage);
	//	}
	//}

	//

	//delete []  mDataBuffer;
}

void CaptureThreadFunction()
{
	if (g_hEventCapture  == NULL)
	{
		g_hEventCapture  = CreateEvent(FALSE,FALSE,TRUE,L"g_hEventCapture");
	}

	DWORD mWait = WaitForSingleObject(g_hEventCapture,1);

	if (mWait == WAIT_OBJECT_0)
	{
		try
		{
			CaptureAndSaveImage();
		}
		catch (...)
		{
			//CString strError;
			char mText[MAX_PATH];
			DWORD mLastError = GetLastError();
			//strError.Format(L"Catch Error %d: line = %s Function = %s File = %s",mLastError,__LINE__,__FUNCDNAME__,__FILE__);
			sprintf_s(mText,MAX_PATH,"Catch Error %d: line = %d Function = %s File = %s",mLastError,__LINE__,__FUNCDNAME__,__FILE__);
			//LOGS(mText);
			SetEvent(g_hEventCapture);
		}

		SetEvent(g_hEventCapture);
	}
}

void BeginCaptureThread()
{
	g_hThreadCapture = CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)CaptureThreadFunction,0,0,0);
}

