#pragma once

#include <fstream>
#include <string>
#include <direct.h>
#include <vector>
#include "zy_xray_basic.h"
#include <opencv2/opencv.hpp>

using namespace std;

typedef bool(*Xray_inspect_initial)(const stInputParams& params);

typedef int(*Xray_inspection)(stInputParams& inPara, stResultData& outPara);

class CShape
{
public:
	CShape(void);

	bool learning_init(const stInputParams& params);

	int Defect_Judge(stInputParams& inPara, stResultData& outPara);

	~CShape(void);

	std::string getTime();

	std::string getYMD();

private:
	HINSTANCE hdll;
	Xray_inspect_initial init;
	Xray_inspection xInspect;
	fstream of;
};
