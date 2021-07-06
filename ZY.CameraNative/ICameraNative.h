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
	  * @brief	���豸
	  * @param	index	��������к�
	  * @param	width	ͼƬ���
	  * @param	height	ͼƬ�߶�
	  * @param	xflip	Xƫ��
	  * @param	yflip	Yƫ��
	  * @return ����Error Code
	  */
	virtual int OpenDevice(char* index, int width, int height, bool xflip, bool yflip) = 0;
	virtual int CloseDevice() = 0;

	/**
	  * @brief	��/�ر���Ƶ��
	  *			��Ϊ������ͼ/�ر�Ϊ������ͼ
	  * @param	index	�������
	  * @param	width	ͼƬ���
	  * @param	height	ͼƬ�߶�
	  * @return ����Error Code
	  */
	virtual int SetVideoOn(bool bOn) = 0;

	/**
	  * @brief	����һ��
	  * @param	numToGrab	��Ҫ�ɶ�����ͼ(һ�β�����20��)
	  * @return ����Error Code
	  */
	virtual int SoftShot(int numToGrab) = 0;

	/**
	  * @brief	�����ع�
	  * @param	nExpVal		�ع�ֵ(ע�ⱶ��)
	  * @return ����Error Code
	  */
	virtual int SetExposal(int nExpVal) = 0;
	virtual int GetExposal() = 0;

	/**
	  * @brief	��������
	  * @param	nGainVal	����ֵ
	  * @return	����Error Code
	  */
	virtual int SetGain(int nGainVal) = 0;
	virtual int GetGain() = 0;

	virtual int GetBufferImagesCnt() = 0;

	/**
	 * @brief	��ȡ��֡ƽ�����ͼ��
	 * @param	pImgCnt		ƽ��������
	 * @param	pImages		ͼƬ
	 * @return	����Error Code
	 */
	virtual int GetBufferImages(const int* pImgCnt, ZYImageStruct* pImages) = 0;

	/**
	* @brief	��ȡ������������к�
	* @param	vectornum		��������кŵ�����
	* @return	����Error Code
	*/
	virtual int GetAllSerioNum(vector<char*>& vectornum ) = 0;
};

class ZYAPI_EXPORT_CAMERA_NATIVE CameraFactory
{
public:
	ICameraNative* CreateCamera(int camType);
};

#endif // __I_CAMERA_NATIVE_H__
