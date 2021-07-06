#pragma  once
#include <Windows.h>
#include "opencv2/opencv.hpp"
#include <vector>
#include <string>

using namespace  std;
using namespace cv;

struct detected_pole_info
{
	Point  anode_pt;
	Point cathode_pt;
	vector<Point> pole_line;
	float pole_length;

	float angle;   //09.27 ���صĽǶȾ���ֵ

	Rect rect;       // �����㷨ʹ��  add by fh 2020.1.2
};

struct output_para
{//����ṹ��
	Mat drawing_img;    //���Ƶ�ͼ��
	Mat undrawing_img;
	bool is_fold;

	bool is_cathode_fold;

	vector<detected_pole_info> poles; //�缫���ȼ��Ƕ���Ϣ
};

/// ����Խṹ��
/// lenght �������ϲ���������
/// length2 �������²���������
typedef struct stPairDis
{
	float length;
	float length2;

	stPairDis()
	{
		length = 0;
		length2 = 0;
	}
	stPairDis operator=(const stPairDis& params)
	{
		length = params.length;
		length2 = params.length2;
		return (*this);
	}
}stPairDis;


/// ��������ṹ��
///  iLine ��е�ֱ��
///  iCorner �Ǳ�
///  detected_rect ���ROI����
/// total_layer �ܲ���
/// min_length ��С������ֵ
/// max_length ��󳤶���ֵ
/// max_angle_thresh �Ƕ���ֵ
/// isDiagonal;   �Ƿ���ԶԽ�  1���Խ�  0��ͬ��2��
/// isDrawBarcode �Ƿ���ͼƬ�ϻ������� 1�����ƣ�0��������
/// isDrawLine  �Ƿ���
/// isShowData  �Ƿ���ʾ����
/// isShowAngle �Ƿ���ʾ�Ƕ�
/// isDetectAngle  �Ƿ���нǶȲ���
/// strBarcode ����
typedef struct stInputParams
{
	int iMethode;

	int iLine;
	int iCorner;
	int total_layer;
	int thread_num;

	float pixel_to_mm;
	float min_length;
	float max_length;
	float max_angle_thresh;

	bool isDrawLine;
	bool isShowData;
	bool isShowAngle;
	bool isDetectAngle;
	bool bEnhanceImage;
	bool bInitial;

	cv::Rect detected_rect;
	std::string strBarcode;
	std::string strMIcode;

	Mat Src;

	stInputParams()
	{
		iMethode = 1;
		iLine = 1;
		iCorner = 1;
		total_layer = 5;
		thread_num = 1;

		pixel_to_mm = 0.01f;
		min_length = 0.1;
		max_length = 1.4f;
		max_angle_thresh = 45.0f;

		isDrawLine = true;
		isShowData = true;
		isDetectAngle = true;
		isShowAngle = true;
		bEnhanceImage = false;
		bInitial = false;
		detected_rect = cv::Rect(0, 0, 0, 0);
		strBarcode = std::string("ERROR");
		strMIcode = std::string("W00");
		Src = NULL;
	}

	stInputParams operator=(const stInputParams& params)
	{
		iMethode = params.iMethode;
		iLine = params.iLine;
		iCorner = params.iCorner;
		total_layer = params.total_layer;
		thread_num = params.thread_num;

		pixel_to_mm = params.pixel_to_mm;
		min_length = params.min_length;
		max_length = params.max_length;
		max_angle_thresh = params.max_angle_thresh;

		isDrawLine = params.isDrawLine;
		isShowData = params.isShowData;
		isDetectAngle = params.isDetectAngle;
		isShowAngle = params.isShowAngle;
		detected_rect = params.detected_rect;
		strBarcode = params.strBarcode;
		strMIcode = params.strMIcode;
		return(*this);
	}

}stInputParams;


// ���ؽ���ṹ��
typedef struct stResultData
{
	vector<float> vecDis;
	vector<stPairDis> vecPairDis;
	vector<float> vecAngles;
	float fMinDis;
	float fMaxDis;
	float fMeanDis;
	float fContrast; // �Աȶ�
	int iResult; // �����־ 1->OK, 0->NG
	cv::Mat colorMat;  // ���ؽ��ͼ
	std::string strBarcode;

	stResultData()
	{
		vecDis.clear();
		vecAngles.clear();
		vecPairDis.clear();
		fMinDis = 0.0f;
		fMaxDis = 0.0f;
		fMeanDis = 0.0f;
		fContrast = 0.0f;
		iResult = 0;
		strBarcode = std::string("ERROR");
	}

	stResultData operator=(const stResultData& resData)
	{
		this->fMinDis = resData.fMinDis;
		this->fMaxDis = resData.fMaxDis;
		this->fMeanDis = resData.fMeanDis;
		this->fContrast = resData.fContrast;
		this->iResult = resData.iResult;
		for (int i = 0; i < resData.vecDis.size(); i++)
		{
			this->vecDis.push_back(resData.vecDis[i]);
			this->vecAngles.push_back(resData.vecAngles[i]);			
		}
		for (int i = 0; i < resData.vecPairDis.size(); i++)
		{			
			this->vecPairDis.push_back(resData.vecPairDis[i]);
		}		

		if (!resData.colorMat.empty())
			resData.colorMat.copyTo(this->colorMat);

		strBarcode = resData.strBarcode;
		return (*this);
	}

	void init()
	{
		this->vecDis.clear();
		this->vecAngles.clear();
		this->vecPairDis.clear();
		this->fMinDis = 0.0f;
		this->fMaxDis = 0.0f;
		this->fMeanDis = 0.0f;
		this->fContrast = 0.0f;
		this->iResult = 0;
		if (!colorMat.empty())
			colorMat.release();

		strBarcode = std::string("ERROR");
	}

}stResultData;