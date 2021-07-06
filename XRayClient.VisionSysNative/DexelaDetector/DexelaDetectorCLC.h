// ******************************************************
//
// Copyright (c) 2015, PerkinElmer Inc., All rights reserved
// 
// ******************************************************
//
// Header for DexelaDetectorCL C Wrapper
//
// ******************************************************
#pragma once

#ifndef DEX_BUILD_C
#define DllExportC __declspec( dllimport )
#else
#define DllExportC __declspec( dllexport )
#endif

#ifdef __cplusplus
#include "DexelaDetector.h"
#include "DexelaDetectorCL.h"
extern "C" {
#endif

	/// <summary>
	/// Handle to the CameraLink Dexela Detector
	/// </summary>
	typedef void DexelaDetectorCLC;

	DllExportC DexelaDetectorCLC* DexelaDetectorCLC_Create(DevInfo* devInfo);
	DllExportC DexelaDetectorCLC* DexelaDetectorCLC_CreateFromParams(DetectorInterface transport, int unit, const char* params);
	DllExportC void DexelaDetectorCLC_Delete(DexelaDetectorCLC* dexCL);
	DllExportC Derr DexelaDetectorCLC_PowerCLInterface(DexelaDetectorCLC* dexCL, BOOL flag);

	DllExportC Derr DexelaDetectorCLC_OpenBoardDefault(DexelaDetectorCLC* dexCL);
	DllExportC Derr DexelaDetectorCLC_OpenBoard(DexelaDetectorCLC* dexCL, int NumBufs);

#ifdef __cplusplus
}
#endif