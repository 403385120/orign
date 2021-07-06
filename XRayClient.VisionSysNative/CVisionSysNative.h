#ifndef __C_VISION_SYS_NATIVE_H__
#define __C_VISION_SYS_NATIVE_H__

#include "../packages/Native/ZYShape.h"
#include "../packages/Native/ZYVisionParams.h"
#include "../Zy.CameraNative/ICameraNative.h"
//#include "../zy.CameraNative/CameraBaumer.h"
//#include "DexelaCamera.h"
#include "zyDexelaDetector.h"


class CVisionSysNative
{
public:
	explicit CVisionSysNative(const CameraParams* visionSysParams);
	~CVisionSysNative();

	/**
	  * @brief	打开相机采图
	  * @return 返回Error Code
	  */
	int Init(int num, bool isHigh);

	/**
	  * @brief	关闭相机
	  * @return 返回Error Code
	  */
	int UnInit();

	/**
	  * @brief	实时视频开关
	  * @return 返回Error Code
	  */
	int SetVideoMode(bool bOn);

	/**
	  * @brief	采图并检测
	  * @return 返回Error Code
	  */
	int ShotAndCheck(int imgcount,ZYImageStruct* pOutImage, int iAlgoType,int &imgNo);
	
	/**
	* @brief	获取相机的曝光时间
	* @return 返回相机的曝光时间
	*/
	int GetExposureTime();

	/**
	* @brief	设置相机的曝光时间
	* @return 返回Erorrcode
	*/
	int SetExposureTime(int nValue);

	/**
	* @brief	获取相机的增益
	* @return 返回相机的增益值
	*/
	int GetGain();

	/**
	* @brief	设置相机的增益
	* @return 返回Erorrcode
	*/
	int SetGain(int nvalue);

	/**
	* @brief	窗宽窗位查找表
	* @return 返回Erorrcode
	*/
	void SetSreachTable(int iMinVal, int iMaxVal);

	/**
	* @brief	获取所有相机的序列号
	* @return 返回Erorrcode
	*/
	int GetAllSerioNum(vector<char*>& pvector);

	CameraParams m_pVisionSysParams;
protected:
private:
	zyDexelaDetector* m_pCameraNative;
	int m_iFrame;
	int m_fExposureTime;
};

#endif // __C_VISION_SYS_NATIVE_H__
