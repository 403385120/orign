// ******************************************************
//
// Copyright (c) 2015, PerkinElmer Inc., All rights reserved
// 
// ******************************************************
//
// Header for BusScanner C Wrapper
//
// ******************************************************

#pragma once
#ifndef DEX_BUILD_C
#ifdef _DEBUG
#pragma comment(lib,"BusScannerC-d.lib")
#else
#pragma comment(lib,"BusScannerC.lib")
#endif
#endif

#ifdef __cplusplus
#include "BusScanner.h"
extern "C" {
#endif

	/// <summary>
	/// Handle to the BusScannerC object
	/// </summary>
	typedef void BusScannerC;

	DllExportC BusScannerC* BusScannerC_Create(void);
	DllExportC void BusScannerC_Delete(BusScannerC* scanner);

	DllExportC Derr BusScannerC_EnumerateDevs(BusScannerC* scanner, int* numDevs);
	DllExportC Derr BusScannerC_EnumerateGEDevs(BusScannerC* scanner, int* numDevs);
	DllExportC Derr BusScannerC_EnumerateCLDevs(BusScannerC* scanner, int* numDevs);

	DllExportC Derr BusScannerC_GetDevice(BusScannerC* scanner, int index, DevInfo* devInfo);
	DllExportC Derr BusScannerC_GetDeviceGE(BusScannerC* scanner, int index, DevInfo* devInfo);
	DllExportC Derr BusScannerC_GetDeviceCL(BusScannerC* scanner, int index, DevInfo* devInfo);

#ifdef __cplusplus
}
#endif