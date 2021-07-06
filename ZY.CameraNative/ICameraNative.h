#ifndef __I_CAMERA_NATIVE_H__
#define __I_CAMERA_NATIVE_H__

#include "../packages/Native/ZYShape.h"

#ifdef _DLL_EXPORT_CAMERA_NATIVE
	#define ZYAPI_EXPORT_CAMERA_NATIVE __declspec(dllexport) 
#else
	#define ZYAPI_EXPORT_CAMERA_NATIVE __declspec(dllimport) 

	#if defined(_DEBUG) || defined(_FAKE_DEBUG)
		#pragma comment(lib, "../PTF UI/Debug/ZY.CameraNative.lib")
	#else
		#pragma comment(lib, "../PTF UI/Debug/ZY.CameraNative.lib")
	#endif // DEBUG
#endif // !DLL_EXPORT

#include <vector>
using namespace std;

class ZYAPI_EXPORT_CAMERA_NATIVE ICameraNative
{
public:
	/**
	  * @brief	打开设备
	  * @param	index	相机的序列号
	  * @param	width	图片宽度
	  * @param	height	图片高度
	  * @param	xflip	X偏移
	  * @param	yflip	Y偏移
	  * @return 返回Error Code
	  */
	virtual int OpenDevice(char* index, int width, int height, bool xflip, bool yflip) = 0;
	virtual int CloseDevice() = 0;

	/**
	  * @brief	打开/关闭视频流
	  *			打开为连续采图/关闭为软触发采图
	  * @param	index	相机索引
	  * @param	width	图片宽度
	  * @param	height	图片高度
	  * @return 返回Error Code
	  */
	virtual int SetVideoOn(bool bOn) = 0;

	/**
	  * @brief	软触发一次
	  * @param	numToGrab	需要采多少张图(一次不超过20张)
	  * @return 返回Error Code
	  */
	virtual int SoftShot(int numToGrab) = 0;

	/**
	  * @brief	设置曝光
	  * @param	nExpVal		曝光值(注意倍率)
	  * @return 返回Error Code
	  */
	virtual int SetExposal(int nExpVal) = 0;
	virtual int GetExposal() = 0;

	/**
	  * @brief	设置增益
	  * @param	nGainVal	增益值
	  * @return	返回Error Code
	  */
	virtual int SetGain(int nGainVal) = 0;
	virtual int GetGain() = 0;

	virtual int GetBufferImagesCnt() = 0;

	/**
	 * @brief	获取多帧平均后的图像
	 * @param	pImgCnt		平均的数量
	 * @param	pImages		图片
	 * @return	返回Error Code
	 */
	virtual int GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages) = 0;

	/**
	* @brief	获取所有相机的序列号
	* @param	vectornum		相机的序列号的容器
	* @return	返回Error Code
	*/
	virtual int GetAllSerioNum(vector<char*>& vectornum ) = 0;
};

class ZYAPI_EXPORT_CAMERA_NATIVE CameraFactory
{
public:
	ICameraNative* CreateCamera(int camType);
};

#endif // __I_CAMERA_NATIVE_H__
