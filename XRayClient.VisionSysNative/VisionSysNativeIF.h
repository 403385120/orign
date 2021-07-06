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
* @brief	����Ӿ�ϵͳ����C#��C++�޷�ʹ�ö���ָ�봫���������ôˣ�
* @param	pVisionSysParamsList	һ���Ӿ�ϵͳһ�ݲ���
* @param	nIndex			�Ӿ�ϵͳ���
* @return ����Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_Add(const CameraParams* pVisionSysParamsList);
/**
  * @brief	��ʼ��ģ��
  * @param	pVisionSysParamsList	һ���Ӿ�ϵͳһ�ݲ���
  * @param	pResults				�Ӿ�ϵͳ�ĳ�ʼ���������
  * @return ����Error Code
  */
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_Init(int* pResults, bool isHigh);


/**
* @brief	��ȡ������ع�ʱ��
* @param	cameraIndex		��������
* @return ����������ع�ʱ��
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_GetExposure(int cameraIndex);

/**
* @brief	����������ع�ʱ��
* @param	cameraIndex	��������
* @param	nvalue		�ع�ʱ��ֵ
* @return ����Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_SetExposure(int cameraIndex, int nvalue);


/**
* @brief	��ȡ���������
* @param	cameraIndex	��������
* @return �������������ֵ
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_GetGain(int cameraIndex);

/**
* @brief	��������ĻҶ�ֵ
* @param	minGray	��С�Ҷ�ֵ
* @param	minGray	���Ҷ�ֵ
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF void VisionSysNativeIF_SetSreachTable(int cameraIndex, int minGray, int maxGray);

/**
* @brief	��ȡ������������к�
* @param	cameraIndex	������ַ���
* @param	stringwidth	�ַ�������
* @param	stringheight	�ַ������
* @return �������������ֵ
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_GetAllSerioNum(char* cameraIndex, int stringwidth, int stringheight);


/**
* @brief	�������������
* @param	cameraIndex	��������
* @param	nvalue		����ֵ
* @return ����Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_SetGain(int cameraIndex, int nvalue);


/**
  * @brief	����ʼ��ģ��
  * @param
  * @return	����Error Code
  */
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_UnInit();

/**
  * @brief	ǰ�����ͼ�����
  * @param	frontcameraIndex			�������
  *			frontpOutImage			���ͼƬ
  *	@note	�˺���Ϊ����ʽ�������벻Ҫ�ں����ڲ������߳�
  * @return	���ؼ��������
  */
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int VisionSysNativeIF_shot(int frontcameraIndex, int imgcount, ZYImageStruct* frontpOutImage, int iAlgoType,int &imageNo);

////////////////////////////////////�������㷨����//////////////////////////////////////////////////

ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_Init();

/**
* @brief	����ʼ��ģ��
* @param
* @return	����Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_UnInit();


/**
* @brief	ǰ��ͼƬ���
* @param
* @return	����Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_InspectPre(const InspectParams* params, const ZYImageStruct* image);

/**
* @brief	���ͼƬ���
* @param
* @return	����Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_InspectBack(const InspectParams* params, const ZYImageStruct* image);


/**
* @brief	��ȡ���Ǽ��������
* @param
* @return	����Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF void ImageProcessNativeIF_InspectResult(ZYResultData* preResData, ZYResultData* backResData);


/**
* @brief	��ȡ���Ǽ��������(��Աչ����C#����)
* @param
* @return	����Error Code
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
* @brief	����ͼƬ
* @param
* @return	����Error Code
*/
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_SaveImage(const ZYImageStruct* image, char* filePath,int iType, int imageNo);

//�������ѧϰ�㷨�ӿ�
//�㷨��ʼ��
//�ر�ע��:Ҫ��һ������̨���ڣ�������ʱ
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_Init_DL(const InspectParams* params);

//ͼ����
ZYAPI_EXPORT_VISION_SYS_NATIVE_IF int ImageProcessNativeIF_Inspect_DL(char* barcode, const InspectParams* params, const ZYImageStruct* image,
	float* pVecDis, PairDis* pVecPairDis, float* pVecAngs, ZYResultDataMin* ResData, int iMethode,int imagNo);

#endif
