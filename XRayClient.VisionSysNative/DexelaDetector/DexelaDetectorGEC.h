/// ******************************************************
//
// Copyright (c) 2015, PerkinElmer Inc., All rights reserved
// 
// ******************************************************
//
// Header for DexelaDetectorGE C Wrapper
//
// ******************************************************

#pragma once
#ifdef __cplusplus
#include "DexelaDetector.h"
#include "DexelaDetectorGE.h"
extern "C" {
#endif

	/// <summary>
	/// Handle to the Gigabit Ethernet Dexela Detector
	/// </summary>
	typedef void DexelaDetectorGEC;

	DllExportC DexelaDetectorGEC* DexelaDetectorGEC_Create(DevInfo* devInfo);
	DllExportC DexelaDetectorGEC* DexelaDetectorGEC_CreateFromParams(DetectorInterface transport, int unit, const char* params);
	DllExportC void DexelaDetectorGEC_Delete(DexelaDetectorGEC* dexGE);

	DllExportC Derr DexelaDetectorGEC_SetPersistentIPAddress(DexelaDetectorGEC* dexGE, int firstByte, int secondByte, int thirdByte, int fourthByte);

	DllExportC Derr DexelaDetectorGEC_OpenBoardDefault(DexelaDetectorGEC* dexGE);
	DllExportC Derr DexelaDetectorGEC_OpenBoard(DexelaDetectorGEC* dexGE,int NumBufs);

#ifdef __cplusplus
}
#endif