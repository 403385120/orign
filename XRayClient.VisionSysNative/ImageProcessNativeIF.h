#ifndef __IMAGE_PROCESS_NATIVE_IF_H__
#define __IMAGE_PROCESS_NATIVE_IF_H__

#include "../packages/Native/ZYShape.h"
#include "../packages/Native/ZYVisionParams.h"

#ifdef _DLL_EXPORT_IMAGE_PROCESS_NATIVE_IF
	#define ZYAPI_IMAGE_PROCESS_NATIVE_IF extern "C" __declspec(dllexport) 
#else
	#define ZYAPI_IMAGE_PROCESS_NATIVE_IF extern "C" __declspec(dllimport) 

	#if defined(_DEBUG) || defined(_FAKE_DEBUG)
		#pragma comment(lib, "../bin/Debug/XRayClient.ImageProcessNative.lib")
	#else
		#pragma comment(lib, "../bin/Release/XRayClient.ImageProcessNative.lib")
	#endif // DEBUG
#endif // !DLL_EXPORT

/**
  * @brief	初始化模块
  * @param
  * @return 返回Error Code
  */
ZYAPI_IMAGE_PROCESS_NATIVE_IF int ImageProcessNativeIF_Init();

/**
  * @brief	反初始化模块
  * @param
  * @return	返回Error Code
  */
ZYAPI_IMAGE_PROCESS_NATIVE_IF int ImageProcessNativeIF_UnInit();


/**
* @brief	前角图片检测
* @param
* @return	返回Error Code
*/
ZYAPI_IMAGE_PROCESS_NATIVE_IF int ImageProcessNativeIF_InspectPre(const InspectParams* params, const ZYImageStruct* image);

/**
* @brief	后角图片检测
* @param
* @return	返回Error Code
*/
ZYAPI_IMAGE_PROCESS_NATIVE_IF int ImageProcessNativeIF_InspectBack(const InspectParams* params, const ZYImageStruct* image);


/**
* @brief	获取两角检测结果数据
* @param
* @return	返回Error Code
*/
ZYAPI_IMAGE_PROCESS_NATIVE_IF void ImageProcessNativeIF_InspectResult(ZYResultData* preResData, ZYResultData* backResData);


/**
* @brief	获取两角检测结果数据(成员展开供C#调用)
* @param
* @return	返回Error Code
*/
ZYAPI_IMAGE_PROCESS_NATIVE_IF void ImageProcessNativeIF_InspectResultMin(float* pVecDisPre, PairDis* pVecPairDisPre, float* pVecAngsPre, ZYResultDataMin* preResData,
																		float* pVecDisBack, PairDis* pVecPairDisBack, float* pVecAngsBack, ZYResultDataMin* backResData);


ZYAPI_IMAGE_PROCESS_NATIVE_IF void ImageProcessNativeIF_GetResultImage(const ZYImageStruct* image1, const ZYImageStruct* image2,
																	   const ZYImageStruct* image3, const ZYImageStruct* image4, 
																	   const ZYImageStruct* imagecenter, bool Resultflag, 
																	   char* barcode, ZYImageStruct* ReslutImage);

ZYAPI_IMAGE_PROCESS_NATIVE_IF void ImageProcessNativeIF_GetResultImage(const ZYImageStruct* image1, const ZYImageStruct* image2,
																	   const ZYImageStruct* image3, const ZYImageStruct* image4, 
																	   const ZYImageStruct* imagecenter, bool Resultflag, 
																	   char* barcode, ZYImageStruct* ReslutImage);


/**
* @brief	保存图片
* @param
* @return	返回Error Code
*/
ZYAPI_IMAGE_PROCESS_NATIVE_IF int ImageProcessNativeIF_SaveImage(const ZYImageStruct* image, char* filePath);

//新增深度学习算法接口
//算法初始化
//特别注意:要开一个控制台窗口，阻塞耗时
ZYAPI_IMAGE_PROCESS_NATIVE_IF int ImageProcessNativeIF_Init_DL(const InspectParams* params);

//图像检测
ZYAPI_IMAGE_PROCESS_NATIVE_IF int ImageProcessNativeIF_Inspect_DL(char* barcode, const InspectParams* params, const ZYImageStruct* image,
																  float* pVecDis, PairDis* pVecPairDis, float* pVecAngs, ZYResultDataMin* ResData, int iMethode);

#endif