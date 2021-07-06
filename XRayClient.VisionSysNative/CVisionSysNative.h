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
	  * @brief	�������ͼ
	  * @return ����Error Code
	  */
	int Init(int num, bool isHigh);

	/**
	  * @brief	�ر����
	  * @return ����Error Code
	  */
	int UnInit();

	/**
	  * @brief	ʵʱ��Ƶ����
	  * @return ����Error Code
	  */
	int SetVideoMode(bool bOn);

	/**
	  * @brief	��ͼ�����
	  * @return ����Error Code
	  */
	int ShotAndCheck(int imgcount,ZYImageStruct* pOutImage, int iAlgoType,int &imgNo);
	
	/**
	* @brief	��ȡ������ع�ʱ��
	* @return ����������ع�ʱ��
	*/
	int GetExposureTime();

	/**
	* @brief	����������ع�ʱ��
	* @return ����Erorrcode
	*/
	int SetExposureTime(int nValue);

	/**
	* @brief	��ȡ���������
	* @return �������������ֵ
	*/
	int GetGain();

	/**
	* @brief	�������������
	* @return ����Erorrcode
	*/
	int SetGain(int nvalue);

	/**
	* @brief	����λ���ұ�
	* @return ����Erorrcode
	*/
	void SetSreachTable(int iMinVal, int iMaxVal);

	/**
	* @brief	��ȡ������������к�
	* @return ����Erorrcode
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
