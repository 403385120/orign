//#include "stdafx.h"
#include "CShape.h"

CShape::CShape(void)
{
	std::string strTime = getYMD();

	std::string strResultPath("D://Save");
	_mkdir(strResultPath.c_str());
	std::string logPath("D://Save//Shape_Log");
	_mkdir(logPath.c_str());

	std::string strLog = logPath +"\\"+strTime +".txt";
	of.open(strLog.c_str(), std::ios::out|std::ios::app);

	hdll = LoadLibrary(L"zy_Xray_inspection.dll");
	if (hdll == NULL)
	{
		FreeLibrary(hdll);		
		of<<"加载zy_Xray_inspection.dll失败，请检测文件及路径...."<<std::endl;
		return ;
	}

	init = (Xray_inspect_initial)GetProcAddress(hdll, "Xray_inspection_initial");
	xInspect = (Xray_inspection)GetProcAddress(hdll, "Xray_inspection");

	if (init == NULL || xInspect == NULL)
	{
		FreeLibrary(hdll);	
		of<<"加载Xray_inspect函数失败，请检测文件及路径...."<<std::endl;
		return;
	}

	of << "1111" << endl;
}

CShape::~CShape(void)
{
	if (hdll)	
		FreeLibrary(hdll);

	of.close();
}

bool CShape::learning_init(const stInputParams& params)
{
	bool result = init(params);
	
	return result;
}

// 返回值说明：
// -4—>表示位置偏差太大； -2—>尾部平极；-1—>头部平极；0—>其他NG； 1：OK
int CShape::Defect_Judge(stInputParams& inPara, stResultData& outPara)
{
	of<<"xi_pre = "<<xInspect<<std::endl;
	of << "layer = " << inPara.total_layer << ", line = " << inPara.iLine << ", iCorer = " << inPara.iCorner << ", mm = " << inPara.pixel_to_mm << std::endl;

	//cv::Mat img/* = cv::cvarrToMat(pre)*/; pre.copyTo(img);

	int result = xInspect(inPara, outPara);

	of << "time: " << getTime() << endl;
	of<<"pre result = "<<result<<std::endl;	

	return result;	
}

std::string CShape::getTime()
{	
	char cs[100]  ;
	SYSTEMTIME st = {0};
	GetLocalTime(&st);
	sprintf_s(cs, "%04d-%02d-%02d-%02d-%02d-%02d", st.wYear, st.wMonth, st.wDay,st.wHour, st.wMinute, st.wSecond);

	std::string result(cs);
	return result;
}

std::string CShape::getYMD()
{
	char cs[100]  ;
	SYSTEMTIME st = {0};
	GetLocalTime(&st);
	sprintf_s(cs , "%04d-%02d-%02d", st.wYear,st.wMonth,st.wDay);	

	std::string result(cs);
	return result;
}