// ******************************************************
//
// Copyright (c) 2015, PerkinElmer Inc., All rights reserved
// 
// ******************************************************
//
// Header for DexelaDetector C Wrapper
//
// ******************************************************

#pragma once
#ifndef DEX_BUILD_C
#ifdef _DEBUG
#pragma comment(lib,"DexelaDetectorC-d.lib")
#else
#pragma comment(lib,"DexelaDetectorC.lib")
#endif
#endif

#include "DexImageC.h"

#ifdef __cplusplus
#include "DexelaDetector.h"
extern "C" {
#endif

/// <summary>
/// Handle to the Dexela Detector
/// </summary>
typedef void DexelaDetectorC;

DllExportC DexelaDetectorC* DexelaDetectorC_Create(DevInfo* devInfo);
DllExportC DexelaDetectorC* DexelaDetectorC_CreateFromParams(DetectorInterface transport, int unit, const char* params);
DllExportC Derr DexelaDetectorC_Delete(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_GetTransportMethod(DexelaDetectorC* dexDetector, DetectorInterface* transportMethod);
DllExportC Derr DexelaDetectorC_Snap(DexelaDetectorC* dexDetector, int buffer , int timeout);
DllExportC Derr DexelaDetectorC_QueryOnBoardLinearization(DexelaDetectorC* dexDetector, int* isSupported);
DllExportC Derr DexelaDetectorC_ToggleOnBoardLinearization(DexelaDetectorC* dexDetector, BOOL onOff);
DllExportC Derr DexelaDetectorC_GetOnBoardLinearizationState(DexelaDetectorC* dexDetector, BOOL *isSupported);
DllExportC Derr DexelaDetectorC_QueryOnBoardXTalkCorrection(DexelaDetectorC* dexDetector, int* isSupported);
DllExportC Derr DexelaDetectorC_ToggleOnBoardXTalkCorrection(DexelaDetectorC* dexDetector, BOOL onOff);
DllExportC Derr DexelaDetectorC_GetOnBoardXTalkCorrectionState(DexelaDetectorC* dexDetector, BOOL *isSupported);
DllExportC Derr DexelaDetectorC_GetCapturedBuffer(DexelaDetectorC* dexDetector, int* captBuf);
DllExportC Derr DexelaDetectorC_GetFieldCount(DexelaDetectorC* dexDetector, int* iFieldCount);
DllExportC Derr DexelaDetectorC_ReadRegister(DexelaDetectorC* dexDetector,int address,int* returnVal, int sensorNum);
DllExportC Derr DexelaDetectorC_WriteRegister(DexelaDetectorC* dexDetector,int address, int value, int sensorNum);
DllExportC Derr DexelaDetectorC_ReadBuffer(DexelaDetectorC* dexDetector, int bufNum, byte* buffer);
DllExportC Derr DexelaDetectorC_ReadBufferImage(DexelaDetectorC* dexDetector, int bufNum, DexImageC* dexIm, int iZ);
DllExportC Derr DexelaDetectorC_WriteBuffer(DexelaDetectorC* dexDetector, int bufNum, byte* buffer, int size);
DllExportC Derr DexelaDetectorC_GetBufferXdim(DexelaDetectorC* dexDetector, int* xDim);
DllExportC Derr DexelaDetectorC_GetBufferYdim(DexelaDetectorC* dexDetector, int* yDim);
DllExportC Derr DexelaDetectorC_GetNumBuffers(DexelaDetectorC* dexDetector, int* numBufs);
DllExportC Derr DexelaDetectorC_SetFullWellMode(DexelaDetectorC* dexDetector, FullWellModes fwm);
DllExportC Derr DexelaDetectorC_SetExposureMode(DexelaDetectorC* dexDetector, ExposureModes mode);
DllExportC Derr DexelaDetectorC_SetExposureTime(DexelaDetectorC* dexDetector, float timems);
DllExportC Derr DexelaDetectorC_GetExposureMode(DexelaDetectorC* dexDetector, ExposureModes* expMode);
DllExportC Derr DexelaDetectorC_GetExposureTime(DexelaDetectorC* dexDetector, float* expTime);
DllExportC Derr DexelaDetectorC_SetBinningMode(DexelaDetectorC* dexDetector, bins flag);
DllExportC Derr DexelaDetectorC_ClearCameraBuffer(DexelaDetectorC* dexDetector, int i);
DllExportC Derr DexelaDetectorC_ClearBuffers(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_SoftwareTrigger(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_GetDetectorStatus(DexelaDetectorC* dexDetector, DetStatus* detStatus);
DllExportC Derr DexelaDetectorC_SetTestMode(DexelaDetectorC* dexDetector, BOOL SetTestOn);
DllExportC Derr DexelaDetectorC_LoadSensorConfigFile(DexelaDetectorC* dexDetector, char* filename);
DllExportC Derr DexelaDetectorC_OpenBoardDefault(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_OpenBoard(DexelaDetectorC* dexDetector, int NumBufs);
DllExportC Derr DexelaDetectorC_CloseBoard(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_SoftReset(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_PowerSwitch(DexelaDetectorC* dexDetector, BOOL onOff);
DllExportC Derr DexelaDetectorC_GetTriggerSource(DexelaDetectorC* dexDetector, ExposureTriggerSource* trigSrc);
DllExportC Derr DexelaDetectorC_SetTriggerSource(DexelaDetectorC* dexDetector,  ExposureTriggerSource ets);
DllExportC Derr DexelaDetectorC_GetTestMode(DexelaDetectorC* dexDetector, BOOL* onOff);
DllExportC Derr DexelaDetectorC_GetBinningMode(DexelaDetectorC* dexDetector, bins* binMode);
DllExportC Derr DexelaDetectorC_GetFullWellMode(DexelaDetectorC* dexDetector, FullWellModes* wellMode);
DllExportC Derr DexelaDetectorC_GoLiveSeq(DexelaDetectorC* dexDetector, int start, int stop,int numBuf);
DllExportC Derr DexelaDetectorC_GoLiveSeqDefault(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_GoUnLive(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_SetNumOfExposures(DexelaDetectorC* dexDetector, int num);
DllExportC Derr DexelaDetectorC_GetNumOfExposures(DexelaDetectorC* dexDetector, int* num);
DllExportC Derr DexelaDetectorC_SetGapTime(DexelaDetectorC* dexDetector, float timems);
DllExportC Derr DexelaDetectorC_GetGapTime(DexelaDetectorC* dexDetector, float* timems);
DllExportC Derr DexelaDetectorC_GetSerialNumber(DexelaDetectorC* dexDetector, int* serial);
DllExportC Derr DexelaDetectorC_GetModelNumber(DexelaDetectorC* dexDetector, int* model);
DllExportC Derr DexelaDetectorC_GetFirmwareVersion(DexelaDetectorC* dexDetector, int* version);
DllExportC Derr DexelaDetectorC_GetFirmwareBuild(DexelaDetectorC* dexDetector, int* iDayAndMonth,int* iYear, int* iTime);
DllExportC Derr DexelaDetectorC_EnablePulseGenerator(DexelaDetectorC* dexDetector, float frequency);
DllExportC Derr DexelaDetectorC_EnablePulseGeneratorDefault(DexelaDetectorC* detector);
DllExportC Derr DexelaDetectorC_DisablePulseGenerator(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_ToggleGenerator(DexelaDetectorC* dexDetector, BOOL onOff);
DllExportC Derr DexelaDetectorC_WaitImage(DexelaDetectorC* dexDetector, int timeout);
DllExportC Derr DexelaDetectorC_IsConnected(DexelaDetectorC* dexDetector, BOOL *isSupported);
DllExportC Derr DexelaDetectorC_IsLive(DexelaDetectorC* dexDetector, BOOL *isSupported);
DllExportC Derr DexelaDetectorC_IsCallbackActive(DexelaDetectorC* dexDetector, BOOL *isSupported);
DllExportC Derr DexelaDetectorC_SetPreProgrammedExposureTimes(DexelaDetectorC* dexDetector, float* exposuretimes_ms, int numExposures);
DllExportC Derr DexelaDetectorC_GetPreProgrammedExposureTimes(DexelaDetectorC* dexDetector, float** fltArray, int* MSLen);
DllExportC Derr DexelaDetectorC_GetSensorHeight(DexelaDetectorC* dexDetector, unsigned short uiSensorID, int* iSensorWidth);
DllExportC Derr DexelaDetectorC_GetSensorWidth(DexelaDetectorC* dexDetector, unsigned short uiSensorID, int* iSensorHeight);
DllExportC Derr DexelaDetectorC_GetReadOutTime(DexelaDetectorC* dexDetector,float* readOutTime);
DllExportC Derr DexelaDetectorC_CheckForCallbackError(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_CheckForLiveError(DexelaDetectorC* dexDetector);
DllExportC Derr DexelaDetectorC_SetSlowed(DexelaDetectorC* dexDetector,BOOL flag);
DllExportC Derr DexelaDetectorC_SetReadoutMode(DexelaDetectorC* dexDetector,ReadoutModes mode);
DllExportC Derr DexelaDetectorC_GetReadoutMode(DexelaDetectorC* dexDetector,ReadoutModes* mode);
DllExportC Derr DexelaDetectorC_QueryReadoutMode(DexelaDetectorC* dexDetector,ReadoutModes mode, int* isSupported);
DllExportC Derr DexelaDetectorC_QueryExposureMode(DexelaDetectorC* dexDetector,ExposureModes mode, int* isSupported);
DllExportC Derr DexelaDetectorC_QueryTriggerSource(DexelaDetectorC* dexDetector,ExposureTriggerSource ets, int* isSupported);
DllExportC Derr DexelaDetectorC_QueryFullWellMode(DexelaDetectorC* dexDetector,FullWellModes fwm, int* isSupported);
DllExportC Derr DexelaDetectorC_QueryBinningMode(DexelaDetectorC* dexDetector,bins flag, int* isSupported);
DllExportC Derr DexelaDetectorC_SendCommand(DexelaDetectorC* dexDetector, char* cmd, char* response);
DllExportC Derr DexelaDetectorC_QueryTempReporting(DexelaDetectorC* dexDetector, BOOL* isSupported);
DllExportC Derr DexelaDetectorC_GetDetectorTemp(DexelaDetectorC* dexDetector, int SensorNum, float* temperature);

#ifndef __cplusplus
typedef void (*IMAGE_CALLBACK)(int fc, int buf, DexelaDetectorC* det);
#endif
DllExportC Derr DexelaDetectorC_SetCallback(DexelaDetectorC* dexDetector, IMAGE_CALLBACK func);
DllExportC Derr DexelaDetectorC_SetCallbackData(DexelaDetectorC* dexDetector, void* cbData);
DllExportC Derr DexelaDetectorC_GetCallbackData(DexelaDetectorC* dexDetector, void** cbData);
DllExportC Derr DexelaDetectorC_StopCallback(DexelaDetectorC* dexDetector);

#ifdef __cplusplus
}
#endif