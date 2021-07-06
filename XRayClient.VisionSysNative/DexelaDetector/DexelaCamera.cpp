#include "DexelaCamera.h"

CDexelaCamera g_DexelaCamera;		
void myCallback(int fc, int buf, DexelaDetector * det);
void ConvetShort2Byte(int dexelcameraNum, WORD * buffer, BYTE* data, int width, int height, int minVal=400, int maxVal=4800);
void AverageImages(int dexelcameraNum, WORD ** psrc, WORD * pdst, int inum, int iw, int ih);

struct CBdata
{
	int callbackNum;
};
CBdata pcbData[4];

 typedef struct MapData
{
	DexelaDetector *m_det;			//对应相机硬件
	DexelaDetectorGE *m_det2;
	DexImage m_img;					//相机硬件对应的图像
	WORD* m_Map;					//查找表
	UINT* m_Average;				//多帧平均后中间变量
	BYTE* m_Map16_8;				//16位图转8位的中间变量
	WORD* m_SrcImg;					//多帧平均后16位图像
	WORD** m_PData;					//每一帧的图像
	BYTE* m_data;					//最终8位图
	BOOL  m_bRuning;				//当前相机是否结束
	int m_Frames;					//当前采集帧数
	int m_iSetFrams;				//总共需要采集帧数
	float m_fSetExposures;			//单帧采集时间（曝光值）
	WORD m_iminVal;					//灰度最小值
	WORD m_imaxVal;					//灰度最大值
	bool m_bLost;                    //当前平板是否掉线
	bool m_bCaptureImage;
	int m_iCatchTime;

	MapData()
	{
		//m_img = DexImage("C:\\1\\input.smv");
		m_det = NULL;
		m_Average = NULL;	
		m_Map16_8 = NULL;	
		m_SrcImg= NULL;
		m_PData= NULL;
		m_data= NULL;	
		m_bRuning = FALSE;
		m_Frames = 0;
		m_iSetFrams = 10;
		m_fSetExposures = 60.0;
		m_iminVal = 0;
		m_imaxVal = 4000;
		m_bLost = false;
		m_bCaptureImage=false;
		m_iCatchTime = 0;
	}
	
}DexelMapData;
 vector<DexelMapData> g_MapData;

void myCallback(int fc, int buf, DexelaDetector * det)
{	
	CBdata * data = (CBdata *)det->GetCallbackData();
	int dexelcameraNum = data->callbackNum;
	
	//g_MapData[dexelcameraNum].m_bLost = false;
	if (!g_MapData[dexelcameraNum].m_bRuning)
	{
		return;
	}

	WORD *pData;

	det->ReadBuffer(buf, g_MapData[dexelcameraNum].m_img);

	g_MapData[dexelcameraNum].m_img.UnscrambleImage();
	g_MapData[dexelcameraNum].m_img.SubtractDark();
	g_MapData[dexelcameraNum].m_img.FloodCorrection();
	//g_MapData[dexelcameraNum].m_img.DefectCorrection();
	pData = (WORD *)g_MapData[dexelcameraNum].m_img.GetDataPointerToPlane();
	memcpy(g_MapData[dexelcameraNum].m_PData[g_MapData[dexelcameraNum].m_Frames], pData, DEXELA_IMAGE_WIDTH*DEXELA_IMAGE_HEIGHT*sizeof(WORD));
	g_MapData[dexelcameraNum].m_Frames++;
	if (g_MapData[dexelcameraNum].m_iCatchTime<1)
	{
		if (g_MapData[dexelcameraNum].m_Frames >= 2)
		{
			g_MapData[dexelcameraNum].m_iCatchTime++;
			g_MapData[dexelcameraNum].m_Frames = 0;
		}
	}
	
	if (g_MapData[dexelcameraNum].m_Frames >= g_MapData[dexelcameraNum].m_iSetFrams)
	{
		AverageImages(dexelcameraNum, g_MapData[dexelcameraNum].m_PData, g_MapData[dexelcameraNum].m_SrcImg,
			g_MapData[dexelcameraNum].m_iSetFrams, DEXELA_IMAGE_WIDTH, DEXELA_IMAGE_HEIGHT);
		ConvetShort2Byte(dexelcameraNum, g_MapData[dexelcameraNum].m_SrcImg, g_MapData[dexelcameraNum].m_data,
			DEXELA_IMAGE_HEIGHT, DEXELA_IMAGE_WIDTH, g_MapData[dexelcameraNum].m_iminVal, g_MapData[dexelcameraNum].m_imaxVal);//g_MapData[dexelcameraNum].m_SrcImg
		g_MapData[dexelcameraNum].m_Frames = 0;
		g_MapData[dexelcameraNum].m_iCatchTime++;
	}
	Sleep(10);
}

void CDexelaCamera::SetCameraFrams(int Frams, int dexelcameraNum)
{
	if (Frams > 2)
	{
		g_MapData[dexelcameraNum].m_iSetFrams = Frams - 2;
	}
	else
	{
		g_MapData[dexelcameraNum].m_iSetFrams = Frams;
	}
}

void CDexelaCamera::SetSreachTable(int minVal, int maxVal, int dexelcameraNum)
{
	g_MapData[dexelcameraNum].m_iminVal = minVal;
	g_MapData[dexelcameraNum].m_imaxVal = maxVal;

	//生成查找表
	float gap = (float)(g_MapData[dexelcameraNum].m_imaxVal - g_MapData[dexelcameraNum].m_iminVal );
	for (int i = 0; i < SEARCH_TABLE_MAX; i++)
	{
		g_MapData[dexelcameraNum].m_Map[i] = i;
		if (g_MapData[dexelcameraNum].m_Map[i] <= g_MapData[dexelcameraNum].m_iminVal)
		{
			g_MapData[dexelcameraNum].m_Map[i] = 0;
		}
		else 
		{
			if (g_MapData[dexelcameraNum].m_Map[i] >= g_MapData[0].m_imaxVal)
			{
				g_MapData[dexelcameraNum].m_Map[i] = SEARCH_TABLE_MAX - 1;
			}
			else
			{
				g_MapData[dexelcameraNum].m_Map[i] = (WORD)((i - minVal)/gap*65535.0f + 0.5f);
			}
		}
	}
}

void CDexelaCamera::SetThreshParam()
{
	int iGrayMin = 0;
	int iGrayMax = 100;
	for (int i = 0; i < 256; i++)
	{
		int value = i;
		value = (int)(((float)value - iGrayMin) / (iGrayMax - iGrayMin) * 255);
		value = (value < 0 ? 0 : value);
		value = (value > 255 ? 255 : value);
		g_MapData[0].m_Map[i] = value;
	}
}

CDexelaCamera::CDexelaCamera()
{

	m_hIsImageLost = NULL;
	//m_bCaptureImage = false;
}

CDexelaCamera::~CDexelaCamera()
{
	closeCamera(0);

	for (int index = 0; index < g_MapData.size(); index++)
	{
		g_MapData[index].m_bRuning = false;
	}
	
}

bool CDexelaCamera::InitialDexelaCamera(int dexelcameraNum, int iExposureTime, int iDeviceSN)
{
	//return false;

	if(dexelcameraNum > DEXELA_MAX_NUM || dexelcameraNum < 1)
	{
		//AfxMessageBox(_T("相机个数初始化参数异常，初始化相机失败"));
		return false;
	}

	g_MapData.resize(dexelcameraNum);

	for(int i = 0 ;i < dexelcameraNum; i++)
	{
		g_MapData[i].m_bRuning = FALSE;
		g_MapData[i].m_fSetExposures = (float)iExposureTime;
		g_MapData[i].m_Frames = 0;
		g_MapData[i].m_iSetFrams = 8;
		g_MapData[i].m_data = new BYTE[DEXELA_IMAGE_WIDTH*DEXELA_IMAGE_HEIGHT];
		g_MapData[i].m_Map = new WORD[SEARCH_TABLE_MAX];
		g_MapData[i].m_Map16_8 = new BYTE[SEARCH_TABLE_MAX];
		g_MapData[i].m_Average = new UINT[DEXELA_IMAGE_WIDTH*DEXELA_IMAGE_HEIGHT];
		g_MapData[i].m_SrcImg = new WORD[DEXELA_IMAGE_WIDTH*DEXELA_IMAGE_HEIGHT];
		g_MapData[i].m_PData = new WORD* [DEXELA_MAX_FRAMS];
		for(int j = 0; j < DEXELA_MAX_FRAMS; j++)
		{
			g_MapData[i].m_PData[j] = new unsigned short[DEXELA_IMAGE_WIDTH*DEXELA_IMAGE_HEIGHT];
		}
		for (int k = 0; k < SEARCH_TABLE_MAX; k++)
		{
			g_MapData[i].m_Map16_8[k] = (BYTE)((k + SEARCH_MAX)/SEARCH_MAX - 1);
		}

	}
	
	//自动暗校正
	DexImage input = DexImage("C:\\CameraSetting\\darksAverage.smv");
	input.FindMedianofPlanes();
	input.SetAveragedFlag(true);
	input.SetImageType(Offset);
	input.WriteImage("C:\\CameraSetting\\darks.smv");

	//自动亮校准
	DexImage inputflood1 = DexImage("C:\\CameraSetting\\floodsAverage.smv");
	inputflood1.LoadDarkImage("C:\\CameraSetting\\darks.smv");
	inputflood1.FindMedianofPlanes();
	inputflood1.SetAveragedFlag(true);
	inputflood1.SubtractDark();
	inputflood1.SetDarkCorrectedFlag(true);
	inputflood1.FixFlood();
	inputflood1.SetFixedFlag(true);
	inputflood1.SetImageType(Gain);
	inputflood1.WriteImage("C:\\CameraSetting\\floods.smv");

	SetCameraFrams(8, 0);
	SetSreachTable(0, 4000, 0);

	g_MapData[0].m_img.LoadDarkImage("C:\\CameraSetting\\darks.smv");
	g_MapData[0].m_img.LoadFloodImage("C:\\CameraSetting\\floods.smv");
	//g_MapData[0].m_img.LoadDefectMap("D:\\CameraSetting\\DefectMap.smv");

	try
	{
		BusScanner scanner;
		//int count = scanner.EnumerateDevices();
		/*int count = scanner.EnumerateCLDevices();
		if (count < 1)
		{
			AfxMessageBox(_T("查找平板异常,请重启设备"));
			return false;
		}*/
		int count = 1;

		for (int i = 0; i<count; i++)
		{
			if (count != dexelcameraNum)
			{
				//AfxMessageBox(_T("相机个数参数异常，初始化相机失败"));
			}

			DevInfo info = scanner.GetDevice(i);
			//DevInfo info = scanner.GetDeviceGE(i);
			//根据 SN号初始化平板
			int iDevice = 99;
			for (int index = 0; index < count; index++)
			{
				if (iDeviceSN == info.serialNum)
				{
					iDevice = index;
				}
			}
			if (99 == iDevice)
			{
				return false;
			}

			g_MapData[iDevice].m_det = new DexelaDetector(info);
			
			AcquireCallbackSequence(*g_MapData[iDevice].m_det, iDevice);
			
		}
		
	}
	catch (DexelaException ex)
	{
		//AfxMessageBox(_T("平板异常,请重启设备"));
	}

	
	return true;
}

void CDexelaCamera::closeCamera(int dexelcameraNum)
{
	for(int i = 0 ;i < dexelcameraNum; i++)
	{
		delete []g_MapData[i].m_data;
		delete []g_MapData[i].m_Map;
		delete []g_MapData[i].m_Map16_8;
		delete []g_MapData[i].m_Average;
		delete []g_MapData[i].m_PData;
		delete []g_MapData[i].m_PData;
		for(int j = 0; j < DEXELA_MAX_FRAMS; j++)
		{
			delete []g_MapData[i].m_PData[j];
		}
	}
}


void CDexelaCamera::AcquireCallbackSequence(DexelaDetector &detector, int dexelcameraNum)
{
	ExposureModes expMode = Expose_and_read;
	FullWellModes wellMode = Low;//Low
	float exposureTime = 50.0f;
	ExposureTriggerSource trigger = Internal_Software;;
	int imCnt = 0; 
	bins binFmt = x11;
	
	detector.OpenBoard();	
	detector.SetFullWellMode(wellMode);
	detector.SetExposureTime((float)g_MapData[dexelcameraNum].m_fSetExposures);
	detector.SetBinningMode(binFmt);
	detector.SetExposureMode(expMode);
	detector.SetTriggerSource(trigger);
	detector.EnablePulseGenerator(); 
	detector.GetModelNumber();

	pcbData[dexelcameraNum].callbackNum = dexelcameraNum;
	g_MapData[dexelcameraNum].m_bRuning = TRUE;
	detector.SetCallback(myCallback);
	detector.SetCallbackData(&pcbData[dexelcameraNum]);

	Sleep(50);

	detector.GoLiveSeq();
	//bool isLive = false;
	//detector.ToggleGenerator(true);
	//if (dexelcameraNum == 0)
	//{
	//	m_hIsImageLost = CreateThread(NULL, 0, IsImageLostThread, this, 0, NULL);
	//	CloseHandle(m_hIsImageLost);
	//}
}

bool CDexelaCamera::CaptureOneImage(BYTE * pImageBuffer, int dexelcameraNum)
{
	g_MapData[dexelcameraNum].m_det->ToggleGenerator(true);
	int index = 0;
	DWORD begin = GetTickCount();
	while (g_MapData[dexelcameraNum].m_iCatchTime < 2)
	{
		Sleep(1);
		index++;
		if (3000 <= GetTickCount() - begin)
		{
			//AfxMessageBox(_T("采图超过3秒"));
			begin = GetTickCount();
			break;
		}
	}
	g_MapData[dexelcameraNum].m_det->ToggleGenerator(false);
	g_MapData[dexelcameraNum].m_iCatchTime = 0;

	memcpy(pImageBuffer,g_MapData[dexelcameraNum].m_data,DEXELA_IMAGE_WIDTH*DEXELA_IMAGE_HEIGHT);

	return true;
}

void ConvetShort2Byte(int dexelcameraNum, WORD* buffer, BYTE * data, int width, int height, int minVal, int maxVal)
{

	int ilen = width* height;
	int i;
	WORD * psrc = buffer;
	BYTE * pdst = data;	
	for (i = 0; i < ilen; i++)
	{
		*psrc = g_MapData[dexelcameraNum].m_Map[*psrc];
		*pdst = g_MapData[dexelcameraNum].m_Map16_8[*psrc];
		psrc++;
		pdst++;
	}
}
void AverageImages(int dexelcameraNum, WORD ** psrc, WORD * pdst, int inum, int iw, int ih)
{
	if(inum <= 0)
	{
		//AfxMessageBox(_T("平板采集帧数设置异常,请重启设备"));

	}

	int ilen = iw*ih;
	memset(g_MapData[dexelcameraNum].m_Average, 0, ilen*sizeof(UINT));

	WORD * psrc1;
	UINT * pdst1;

	int i;
	int j;
	for(i = 0; i < inum; i++)
	{
		psrc1 = psrc[i];
		pdst1 = g_MapData[dexelcameraNum].m_Average;
		for(j = 0; j < ilen; j++)
		{
			*pdst1 += *psrc1;
			pdst1++;
			psrc1++;
		}
	}

	UINT * psrc2 = g_MapData[dexelcameraNum].m_Average;
	WORD * pdst2 = pdst;
	for(j = 0; j < ilen; j++)
	{
		*pdst2 = *psrc2/inum;
		pdst2++;
		psrc2++;
	}
}

DWORD CDexelaCamera::IsImageLostThread(LPVOID lpdata)
{
	return ((CDexelaCamera*)lpdata)->IsImageLostThreadDo();
}

DWORD CDexelaCamera::IsImageLostThreadDo()
{
	while (!m_bExitSystem)
	{
		for (int index = 0; index < 1; index++)
		{

			if (g_MapData[index].m_bRuning == TRUE)
			{
				if (g_MapData[index].m_bLost)
				{
					//CString str;
					//str.Format(_T("%d号平板异常，请重启软件连接"), index+1);
					//g_MapData[index].m_bRuning = FALSE;
					//AfxMessageBox(str);
					g_MapData[index].m_det->ToggleGenerator(false);
					g_MapData[index].m_det->GoUnLive();
					g_MapData[index].m_det->GoLiveSeq();
					g_MapData[index].m_det->ToggleGenerator(true);
				}
				else
				{
					g_MapData[index].m_bLost = true;
					//CString str;
					//str.Format(_T("%d号平板正常运转"), index);
					//WriteLog(str);
				}
			}
		}
		Sleep(500);
	}
	return 0;
}