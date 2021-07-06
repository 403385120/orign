#include "ImageProcessNativeIF.h"
#include "zy_xray_basic.h"
#include "CShape.h"

static bool bInited_ImageProcessNativeIF = false;
HMODULE hdll;
typedef int(*Xray_inspect_pre)(const cv::Mat& img, const stInputParams& params);
typedef int(*Xray_inspect_back)(const cv::Mat& img, const stInputParams& params);
typedef void(*Xray_inspect_result)(stResultData& preResData, stResultData&  backResData);
Xray_inspect_pre xi_pre;
Xray_inspect_back xi_back;
Xray_inspect_result xi_result;

CShape g_Shape;

int ImageProcessNativeIF_Init()
{

	//if (bInited_ImageProcessNativeIF) return 0;

	//hdll = LoadLibrary(L"zy_Xray_inspection.dll");
	//if (hdll == NULL)
	//{
	//	return -1;
	//}

	//xi_pre = (Xray_inspect_pre)GetProcAddress(hdll, "Xray_inspection_pre");
	//xi_back = (Xray_inspect_back)GetProcAddress(hdll, "Xray_inspection_back");
	//xi_result = (Xray_inspect_result)GetProcAddress(hdll, "Xray_inspection_result");
	//if (xi_pre == NULL || xi_back == NULL || xi_result == NULL)
	//{
	//	//加载Xray_inspect.dll中的函数失败
	//	FreeLibrary(hdll);
	//	return -1;
	//}

	//bInited_ImageProcessNativeIF = true;

	return 0;
}

//深度学习算法单一接口
int ImageProcessNativeIF_Init_DL(const InspectParams* params)
{

	if (bInited_ImageProcessNativeIF) return 0;

		stInputParams InitParams;
		InitParams.detected_rect.height = (int)params->detected_rect.height;
		InitParams.total_layer = params->total_layer;
		InitParams.min_length = params->min_length;
		InitParams.max_length = params->max_length;
		InitParams.pixel_to_mm = params->pixel_to_mm;
		InitParams.thread_num = 4;
		InitParams.strBarcode = "init";
		InitParams.iMethode = 1;
		InitParams.bInitial = true;

		stResultData outParaOne;
		
		int iRes = g_Shape.Defect_Judge(InitParams, outParaOne);  //深度学习算法接口初始化接口换成这个了
		if (iRes == 1)
		{
			bInited_ImageProcessNativeIF = true;
		}
		else
		{
			bInited_ImageProcessNativeIF = false;
		}

	return 0;
}

int ImageProcessNativeIF_UnInit()
{

	/*if (!bInited_ImageProcessNativeIF) return -1;

	if (hdll)
		FreeLibrary(hdll);
	bInited_ImageProcessNativeIF = false;*/

	return 0;
}

//深度学习算法单一接口
int ImageProcessNativeIF_Inspect_DL(char* barcode, const InspectParams* params, const ZYImageStruct* image,
									float* pVecDis, PairDis* pVecPairDis, float* pVecAngs, ZYResultDataMin* ResData)
{
	if (!bInited_ImageProcessNativeIF) return -1;

	stInputParams input;
	Mat srcImage;
	stResultData resultData;

	input.bInitial = false;
	//input.iMethode = iMethode;
	input.iLine = 1;
	input.iCorner = params->iCorner;
	input.thread_num = params->iCorner;
	input.total_layer = params->total_layer;
	input.pixel_to_mm = params->pixel_to_mm;
	input.min_length = params->min_length;
	input.max_length = params->max_length;
	input.max_angle_thresh = params->max_angle_thresh;
	input.isDetectAngle = params->isDetectAngle;
	input.isDrawLine = params->isDrawLine;
	input.bEnhanceImage = false;
	input.isShowData = params->isShowData;
	input.isShowAngle = params->isShowAngle;
	input.detected_rect.x = 0;
	input.detected_rect.y = 0;
	input.detected_rect.width = params->detected_rect.width;
	input.detected_rect.height = params->detected_rect.height;
	input.strBarcode = barcode;
	//if (3 == input.iMethode)
	//{
	//	input.Src = cv::Mat::zeros(cvSize(image->width, image->height), CV_8UC1);
	//}
	//else
	//{
		input.Src = cv::Mat::zeros(cvSize(image->width, image->height), CV_8UC1);
	//}

	//input.Src = cv::Mat::zeros(cvSize(image->width, image->height), CV_16UC1);
	
	memcpy(input.Src.data, image->data, image->width*image->height);

	g_Shape.Defect_Judge(input, resultData);

	
	for (int i = 0; i < resultData.vecDis.size(); i++)
	{
		pVecDis[i] = resultData.vecDis[i];
	}
		
	for (int i = 0; i < resultData.vecDis.size(); i++)
	{
		pVecAngs[i] = resultData.vecAngles[i];
	}
		
	ResData->fMinDis = resultData.fMinDis;
	ResData->fMaxDis = resultData.fMaxDis;
	ResData->fMeanDis = resultData.fMeanDis;
	ResData->fContrast = resultData.fContrast;
	ResData->iResult = resultData.iResult;

	ResData->colorMat.width = resultData.colorMat.cols;
	ResData->colorMat.height = resultData.colorMat.rows;
	ResData->colorMat.channel = resultData.colorMat.channels();
	memcpy(ResData->colorMat.data, resultData.colorMat.data, resultData.colorMat.cols*resultData.colorMat.rows*resultData.colorMat.channels());

	return resultData.iResult;
}

int ImageProcessNativeIF_InspectPre(const InspectParams* params, const ZYImageStruct* image)
{
	if (!bInited_ImageProcessNativeIF) return -1;

	stInputParams input;
	Mat srcImage;

	input.iLine = params->iLine;
	input.iCorner = params->iCorner;
	input.total_layer = params->total_layer;
	input.pixel_to_mm = params->pixel_to_mm;
	input.min_length = params->min_length;
	input.max_length = params->max_length;
	input.max_angle_thresh = params->max_angle_thresh;
	//input.isDiagonalMode = params->isDiagonalMode;
	//input.isDrawBarcode = params->isDrawBarcode;
	input.isDrawLine = params->isDrawLine;
	input.isShowData = params->isShowData;
	input.isShowAngle = params->isShowAngle;
	input.isDetectAngle = params->isDetectAngle;
	
	//debug 版本测试不能通过
	//input.strBarcode.assign(params.strBarcode);
	input.detected_rect.x = params->detected_rect.x;
	input.detected_rect.y = params->detected_rect.y;
	input.detected_rect.width = params->detected_rect.width;
	input.detected_rect.height = params->detected_rect.height;
	srcImage = cv::Mat::zeros(cvSize(image->width, image->height), CV_8UC1);
	//srcImage.data = image->data;
	memcpy(srcImage.data, image->data, image->width*image->height);

	return xi_pre(srcImage, input);
}

int ImageProcessNativeIF_InspectBack(const InspectParams* params, const ZYImageStruct* image)
{

	if (!bInited_ImageProcessNativeIF) return -1;

	stInputParams input;
	Mat srcImage;

	input.iLine = params->iLine;
	input.iCorner = params->iCorner;
	input.total_layer = params->total_layer;
	input.pixel_to_mm = params->pixel_to_mm;
	input.min_length = params->min_length;
	input.max_length = params->max_length;
	input.max_angle_thresh = params->max_angle_thresh;
	//input.isDiagonalMode = params->isDiagonalMode;
	//input.isDrawBarcode = params->isDrawBarcode;
	input.isDrawLine = params->isDrawLine;
	input.isShowData = params->isShowData;
	input.isShowAngle = params->isShowAngle;
	input.isDetectAngle = params->isDetectAngle;
	//debug 版本测试不能通过
	//input.strBarcode.assign(params.strBarcode);
	input.detected_rect.x = params->detected_rect.x;
	input.detected_rect.y = params->detected_rect.y;
	input.detected_rect.width = params->detected_rect.width;
	input.detected_rect.height = params->detected_rect.height;
	srcImage = cv::Mat::zeros(cvSize(image->width, image->height), CV_8UC1);
	memcpy(srcImage.data, image->data, image->width*image->height);

	return xi_back(srcImage, input);
}


void ImageProcessNativeIF_InspectResult(ZYResultData* preResData, ZYResultData* backResData)
{

	stResultData resultData1;
	stResultData resultData2;

	xi_result(resultData1, resultData2);

	for (int i = 0; i < resultData1.vecDis.size(); i++)
		preResData->vecDis[i] = resultData1.vecDis[i];
	//for (int i = 0; i < resultData1.vecPairDis.size(); i++)
	//	preResData.vecPairDis[i] = resultData1.vecPairDis[i];
	for (int i = 0; i < resultData1.vecDis.size(); i++)
		preResData->vecAngles[i] = resultData1.vecAngles[i];
	preResData->fMinDis = resultData1.fMinDis;
	preResData->fMaxDis = resultData1.fMaxDis;
	preResData->fMeanDis = resultData1.fMeanDis;
	preResData->fContrast = resultData1.fContrast;
	preResData->iResult = resultData1.iResult;
	//debug 版本测试不能通过
	//preResData.strBarcode.assign(resultData1.strBarcode);
	preResData->colorMat.width = resultData1.colorMat.cols;
	preResData->colorMat.height = resultData1.colorMat.rows;
	preResData->colorMat.channel = resultData1.colorMat.channels();
	memcpy(preResData->colorMat.data, resultData1.colorMat.data, resultData1.colorMat.cols*resultData1.colorMat.rows*resultData1.colorMat.channels());

	for (int i = 0; i < resultData2.vecDis.size(); i++)
		backResData->vecDis[i] = resultData2.vecDis[i];
	//for (int i = 0; i < resultData2.vecPairDis.size(); i++)
	//	backResData.vecPairDis[i] = resultData2.vecPairDis[i];
	for (int i = 0; i < resultData2.vecDis.size(); i++)
		backResData->vecAngles[i] = resultData2.vecAngles[i];
	backResData->fMinDis = resultData2.fMinDis;
	backResData->fMaxDis = resultData2.fMaxDis;
	backResData->fMeanDis = resultData2.fMeanDis;
	backResData->fContrast = resultData2.fContrast;
	backResData->iResult = resultData2.iResult;
	//debug 版本测试不能通过
	//backResData.strBarcode.assign(resultData2.strBarcode);
	backResData->colorMat.width = resultData2.colorMat.cols;
	backResData->colorMat.height = resultData2.colorMat.rows;
	backResData->colorMat.channel = resultData2.colorMat.channels();
	memcpy(backResData->colorMat.data, resultData2.colorMat.data, resultData2.colorMat.cols*resultData2.colorMat.rows*resultData2.colorMat.channels());
}

/**
* @brief	获取两角检测结果数据(成员展开供C#调用)
* @param
* @return	返回Error Code
*/
void ImageProcessNativeIF_InspectResultMin(float* pVecDisPre, PairDis* pVecPairDisPre, float* pVecAngsPre, ZYResultDataMin* preResData,
										float* pVecDisBack, PairDis* pVecPairDisBack, float* pVecAngsBack, ZYResultDataMin* backResData)
{
	stResultData resultData1;
	stResultData resultData2;

	xi_result(resultData1, resultData2);

	for (int i = 0; i < resultData1.vecDis.size(); i++)
		pVecDisPre[i] = resultData1.vecDis[i];
	//for (int i = 0; i < resultData1.vecPairDis.size(); i++)
	//	preResData.vecPairDis[i] = resultData1.vecPairDis[i];
	for (int i = 0; i < resultData1.vecDis.size(); i++)
		pVecAngsPre[i] = resultData1.vecAngles[i];
	preResData->fMinDis = resultData1.fMinDis;
	preResData->fMaxDis = resultData1.fMaxDis;
	preResData->fMeanDis = resultData1.fMeanDis;
	preResData->fContrast = resultData1.fContrast;
	preResData->iResult = resultData1.iResult;
	//debug 版本测试不能通过
	//preResData.strBarcode.assign(resultData1.strBarcode);
	preResData->colorMat.width = resultData1.colorMat.cols;
	preResData->colorMat.height = resultData1.colorMat.rows;
	preResData->colorMat.channel = resultData1.colorMat.channels();
	memcpy(preResData->colorMat.data, resultData1.colorMat.data, resultData1.colorMat.cols*resultData1.colorMat.rows*resultData1.colorMat.channels());

	for (int i = 0; i < resultData2.vecDis.size(); i++)
		pVecDisBack[i] = resultData2.vecDis[i];
	//for (int i = 0; i < resultData2.vecPairDis.size(); i++)
	//	backResData.vecPairDis[i] = resultData2.vecPairDis[i];
	for (int i = 0; i < resultData2.vecDis.size(); i++)
		pVecAngsBack[i] = resultData2.vecAngles[i];
	backResData->fMinDis = resultData2.fMinDis;
	backResData->fMaxDis = resultData2.fMaxDis;
	backResData->fMeanDis = resultData2.fMeanDis;
	backResData->fContrast = resultData2.fContrast;
	backResData->iResult = resultData2.iResult;
	//debug 版本测试不能通过
	//backResData.strBarcode.assign(resultData2.strBarcode);
	backResData->colorMat.width = resultData2.colorMat.cols;
	backResData->colorMat.height = resultData2.colorMat.rows;
	backResData->colorMat.channel = resultData2.colorMat.channels();
	memcpy(backResData->colorMat.data, resultData2.colorMat.data, resultData2.colorMat.cols*resultData2.colorMat.rows*resultData2.colorMat.channels());
}


void ImageProcessNativeIF_GetResultImage(const ZYImageStruct* image1, const ZYImageStruct* image2,
										 const ZYImageStruct* image3, const ZYImageStruct* image4,
										 const ZYImageStruct* imagecenter, bool Resultflag, 
										 char* barcode, ZYImageStruct* ReslutImage)
{
	if ((image1->width != image2->width) || (image2->width != image3->width)||
		(image3->width != image4->width) ||
		(image1->width != image2->width) || (image2->width != image3->width) ||
		(image3->width != image4->width))
	{
		return;
	}

	if (image1->data == NULL
		|| image2->data == NULL
		|| image3->data == NULL
		|| image4->data == NULL
		|| imagecenter->data == NULL
		|| ReslutImage->data == NULL)
	{
		return;
	}

	Mat srcImage1 = cv::Mat::zeros(cvSize(image1->width, image1->height), CV_8UC3);
	//srcImage1.data = image1->data;
	memcpy(srcImage1.data, image1->data, image1->width*image1->height * image1->channel);

	Mat srcImage2 = cv::Mat::zeros(cvSize(image2->width, image2->height), CV_8UC3);
	//srcImage2.data = image2->data;
	memcpy(srcImage2.data, image2->data, image2->width*image2->height * image2->channel);

	Mat srcImage3 = cv::Mat::zeros(cvSize(image3->width, image3->height), CV_8UC3);
	//srcImage3.data = image3->data;
	memcpy(srcImage3.data, image3->data, image3->width*image3->height * image3->channel);

	Mat srcImage4 = cv::Mat::zeros(cvSize(image4->width, image4->height), CV_8UC3);
	//srcImage4.data = image4->data;
	memcpy(srcImage4.data, image4->data, image4->width*image4->height * image4->channel);

	Mat srcImagecenter = cv::Mat::zeros(cvSize(imagecenter->width, imagecenter->height), CV_8UC3);
	//srcImagecenter.data = imagecenter->data;
	memcpy(srcImagecenter.data, imagecenter->data, imagecenter->width*imagecenter->height * imagecenter->channel);

	Mat srctimagetemp = srcImagecenter.clone();

	

	Mat comMat = Mat::zeros(image1->height * 2, image1->width * 2, CV_8UC3);


	
	Rect rect;

	rect.x = 0;
	rect.width = srcImage1.cols;
	rect.y = 0;
	rect.height = srcImage1.rows;
	srcImage1.copyTo(comMat(rect));

	rect.x = srcImage1.cols;
	rect.width = srcImage4.cols;
	rect.y = 0;
	rect.height = srcImage4.rows;
	srcImage4.copyTo(comMat(rect));

	rect.x = 0;
	rect.width = srcImage2.cols;
	rect.y = srcImage1.rows;
	rect.height = srcImage2.rows;
	srcImage2.copyTo(comMat(rect));

	rect.x = srcImage2.cols;
	rect.width = srcImage3.cols;
	rect.y = srcImage4.rows;
	rect.height = srcImage3.rows;
	srcImage3.copyTo(comMat(rect));


	CvFont font1;

	float factor = (700) / ((float)srctimagetemp.cols);
	//string barcode = barcode;

	
	double hscale1 = 2.2;
	double vscale1 = 2.2;
	int linewidth1 = 6;
	//cvInitFont(&font1, CV_FONT_HERSHEY_SIMPLEX, hscale1, vscale1, 0, linewidth1);

	cvInitFont(&font1, CV_FONT_HERSHEY_SIMPLEX, hscale1, vscale1, 0, linewidth1);

	//检测结果标示	
	//cvPutText(&IplImage(srcImagecenter), barcode, cv::Point(70, srcImagecenter.rows / 2 + 5), &font1, CV_RGB(0, 0, 255));
	cvPutText(&IplImage(srctimagetemp), barcode, cv::Point(70, srctimagetemp.rows / 2 + 5), &font1, CV_RGB(0, 0, 255));

	hscale1 = 6;
	vscale1 = 6;
	
	cvInitFont(&font1, CV_FONT_HERSHEY_SIMPLEX, hscale1, vscale1, 0, linewidth1);
	if (Resultflag == true)
	{
		//cvPutText(&IplImage(BatterySample), "OK", cv::Point(280, BatterySample.rows / 2 - 100), &font1, CV_RGB(0, 255, 0));
		cvPutText(&IplImage(srctimagetemp), "OK", cv::Point(200, srctimagetemp.rows / 2 - 60), &font1, CV_RGB(0, 255, 0));
	}
	else
	{
		cvPutText(&IplImage(srctimagetemp), "NG", cv::Point(200, srctimagetemp.rows / 2 - 60), &font1, CV_RGB(255, 0, 0));
		//cvPutText(&IplImage(srcImagecenter), "NG", cv::Point(srcImagecenter.cols / 2, srcImagecenter.rows / 2 - 60), &font1, CV_RGB(255, 0, 0));
	}

	//resize(srcImagecenter, srcImagecenter, cv::Size(srcImagecenter.cols*factor, srcImagecenter.rows*factor));
	//cv::resize();
	//cv::resize(srcImagecenter, srcImagecenter, cv::Size(srcImagecenter.cols*factor, srcImagecenter.rows*factor));

	rect.x = srcImage1.cols - srctimagetemp.cols / 2;
	rect.width = srctimagetemp.cols;
	rect.y = srcImage1.rows - srctimagetemp.rows / 2;
	rect.height = srctimagetemp.rows;
// 	rect.x = inspectRes.pOutImage1.cols - BatterySample.cols / 2;
// 	rect.width = BatterySample.cols;
// 	rect.y = inspectRes.pOutImage1.rows - BatterySample.rows / 2;
// 	rect.height = BatterySample.rows;

	srctimagetemp.copyTo(comMat(rect));

	

	ReslutImage->width = image1->width * 2;
	ReslutImage->height = image1->height * 2;

	ReslutImage->channel = comMat.channels();

	memcpy(ReslutImage->data, comMat.data, ReslutImage->width * ReslutImage->height * ReslutImage->channel);

	return;



}

int ImageProcessNativeIF_SaveImage(const ZYImageStruct* image, char* filePath)
{
	if (NULL == image || NULL == image->data || image->width <= 0 || image->height <= 0)
	{
		return -1;
	}

	int colorDepth = CV_8UC1;
	if (image->channel == 1) {
		colorDepth = CV_8UC1;
	}
	else if (image->channel == 3) {
		colorDepth = CV_8UC3;
	}
	else {
		return -1;
	}

	try
	{
		Mat matImg = cv::Mat::zeros(cvSize(image->width, image->height), colorDepth);
		memcpy(matImg.data, image->data, image->width * image->height * image->channel);

		bool bRet = cv::imwrite(filePath, matImg);
		if (!bRet) {
			return -1;
		}
	}
	catch (const std::exception&)
	{
		return -1;
	}

	return 0;
}
