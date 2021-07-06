#ifndef __VISION_SYS_NATIVE_IF_H__
#define __VISION_SYS_NATIVE_IF_H__

#include "../packages/Native/ZYShape.h"
#include "../packages/Native/ZYVisionParams.h"

#ifdef _DLL_EXPORT_VISION_SYS_NATIVE_IF
	#define ZYAPI_EXPORT_VISION_SYS_NATIVE_IF extern "C" __declspec(dllexport) 
#else
	#define ZYAPI_EXPORT_VISION_SYS_NATIVE_IF extern "C" __declspec(dllimport) 

	#ifdef _DEBUG
		#pragma comment(lib, "../../bin/Debug/XRayClient.VisionSysNative.lib")
	#else
		#pragma comment(lib, "../../bin/Release/XRayClient.VisionSysNative.lib")
	#endif // DEBUG
#endif // !DLL_EXPORT

/**
* @brief	添加视觉系统（因C#与C++无法使用二级指针传参数，改用此）
* @param	pVisionSysParamsList	一个视觉系统一份参数
* @param	nIndex			视觉系统序号
* @return 返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_Add(const CameraParams* pVisionSysParamsList);
/**
  * @brief	初始化模块
  * @param	pVisionSysParamsList	一个视觉系统一份参数
  * @param	pResults				视觉系统的初始化结果数组
  * @return 返回Error Code
  */
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_Init(int* pResults, bool isHigh);


/**
* @brief	获取相机的曝光时间
* @param	cameraIndex		相机的序号
* @return 返回相机的曝光时间
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_GetExposure(int cameraIndex);

/**
* @brief	设置相机的曝光时间
* @param	cameraIndex	相机的序号
* @param	nvalue		曝光时间值
* @return 返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_SetExposure(int cameraIndex, int nvalue);


/**
* @brief	获取相机的增益
* @param	cameraIndex	相机的序号
* @return 返回相机的增益值
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_GetGain(int cameraIndex);

/**
* @brief	设置相机的灰度值
* @param	minGray	最小灰度值
* @param	minGray	最大灰度值
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF void VisionSysNativeIF_SetSreachTable(int cameraIndex, int minGray, int maxGray);

/**
* @brief	获取所有相机的序列号
* @param	cameraIndex	输出的字符串
* @param	stringwidth	字符串长度
* @param	stringheight	字符串宽度
* @return 返回相机的增益值
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_GetAllSerioNum(char* cameraIndex, int stringwidth, int stringheight);


/**
* @brief	设置相机的增益
* @param	cameraIndex	相机的序号
* @param	nvalue		增益值
* @return 返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_SetGain(int cameraIndex, int nvalue);


/**
  * @brief	反初始化模块
  * @param
  * @return	返回Error Code
  */
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_UnInit();

/**
  * @brief	前相机采图并检测
  * @param	frontcameraIndex			相机索引
  *			frontpOutImage			结果图片
  *	@note	此函数为阻塞式函数，请不要在函数内部启用线程
  * @return	返回检测结果代码
  */
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_shot(int frontcameraIndex, int imgcount, ZYImageStruct* frontpOutImage, int iAlgoType,int &imageNo);

////////////////////////////////////下面是算法部分//////////////////////////////////////////////////

ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_Init();

/**
* @brief	反初始化模块
* @param
* @return	返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_UnInit();


/**
* @brief	前角图片检测
* @param
* @return	返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_InspectPre(const InspectParams* params, const ZYImageStruct* image);

/**
* @brief	后角图片检测
* @param
* @return	返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_InspectBack(const InspectParams* params, const ZYImageStruct* image);


/**
* @brief	获取两角检测结果数据
* @param
* @return	返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF void ImageProcessNativeIF_InspectResult(ZYResultData* preResData, ZYResultData* backResData);


/**
* @brief	获取两角检测结果数据(成员展开供C#调用)
* @param
* @return	返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF void ImageProcessNativeIF_InspectResultMin(float* pVecDisPre, PairDis* pVecPairDisPre, float* pVecAngsPre, ZYResultDataMin* preResData,
	float* pVecDisBack, PairDis* pVecPairDisBack, float* pVecAngsBack, ZYResultDataMin* backResData);


ZYAPI_EXPORT_VISION_SYS_NATIVE_IF void ImageProcessNativeIF_GetResultImage(const ZYImageStruct* image1, const ZYImageStruct* image2,
	const ZYImageStruct* image3, const ZYImageStruct* image4,
	const ZYImageStruct* imagecenter, bool Resultflag,
	char* barcode, ZYImageStruct* ReslutImage);

ZYAPI_EXPORT_VISION_SYS_NATIVE_IF void ImageProcessNativeIF_GetResultImage(const ZYImageStruct* image1, const ZYImageStruct* image2,
	const ZYImageStruct* image3, const ZYImageStruct* image4,
	const ZYImageStruct* imagecenter, bool Resultflag,
	char* barcode, ZYImageStruct* ReslutImage);


/**
* @brief	保存图片
* @param
* @return	返回Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_SaveImage(const ZYImageStruct* image, char* filePath,int iType, int imageNo);

//新增深度学习算法接口
//算法初始化
//特别注意:要开一个控制台窗口，阻塞耗时
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_Init_DL(const InspectParams* params);

//图像检测
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_Inspect_DL(char* barcode, const InspectParams* params, const ZYImageStruct* image,
	float* pVecDis, PairDis* pVecPairDis, float* pVecAngs, ZYResultDataMin* ResData, int iMethode,int imagNo);

#endif
