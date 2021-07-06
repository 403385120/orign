// ******************************************************
//
// Copyright (c) 2015, PerkinElmer Inc., All rights reserved
// 
// ******************************************************
//
// Header for DexImage C Wrapper
//
// ******************************************************

#pragma once
#ifndef DEX_BUILD_C
#ifdef _DEBUG
#pragma comment(lib,"DexImageC-d.lib")
#else
#pragma comment(lib,"DexImageC.lib")
#endif
#endif

#include "DexDefs.h"

#ifdef __cplusplus
#include "DexImage.h"
extern "C" {
#endif

/// <summary>
/// Handle to the DexImageC object
/// </summary>
typedef void DexImageC;

DllExportC DexImageC* DexImageC_Create(void);
DllExportC DexImageC* DexImageC_CreateFromFile(const char* filename);
DllExportC DexImageC* DexImageC_Copy(DexImageC* dexImage);
DllExportC void DexImageC_Delete(DexImageC* dexImage);

DllExportC Derr DexImageC_ReadImage(DexImageC* dexImage, const char* filename);
DllExportC Derr DexImageC_WriteImage(DexImageC* dexImage, const char* filename);
DllExportC Derr DexImageC_WriteImagePlane(DexImageC* dexImage, const char* filename, int iZ);
DllExportC Derr DexImageC_Build(DexImageC* dexImage, int iWidth, int iHeight, int iDepth, pType iPxType);
DllExportC Derr DexImageC_BuildModel(DexImageC* dexImage, int model, bins binFmt, int iDepth);
DllExportC Derr DexImageC_isEmpty(DexImageC* dexImage, int* empty);

DllExportC void* DexImageC_GetDataPointerToPlane(DexImageC* dexImage, int iZ);
DllExportC int DexImageC_GetImageXdim(DexImageC* dexImage);
DllExportC int DexImageC_GetImageYdim(DexImageC* dexImage);
DllExportC int DexImageC_GetImageDepth(DexImageC* dexImage);
DllExportC pType DexImageC_GetImagePixelType(DexImageC* dexImage);
DllExportC float DexImageC_PlaneAvg(DexImageC* dexImage, int iZ);
DllExportC Derr DexImageC_FixFlood(DexImageC* dexImage);
DllExportC Derr DexImageC_FindMedianofPlanes(DexImageC* dexImage);
DllExportC Derr DexImageC_FindAverageofPlanes(DexImageC* dexImage);
DllExportC Derr DexImageC_LinearizeData(DexImageC* dexImage);
DllExportC Derr DexImageC_SubtractDark(DexImageC* dexImage);
DllExportC Derr DexImageC_FloodCorrection(DexImageC* dexImage);
DllExportC Derr DexImageC_DefectCorrection(DexImageC* dexImage);
DllExportC Derr DexImageC_FullCorrection(DexImageC* dexImage);
DllExportC Derr DexImageC_UnscrambleImage(DexImageC* dexImage);
DllExportC Derr DexImageC_AddImage(DexImageC* dexImage);

DllExportC Derr DexImageC_LoadDarkImage(DexImageC* dexImage, DexImageC* dark);
DllExportC Derr DexImageC_LoadDarkImageFromFile(DexImageC* dexImage, const char* filename);
DllExportC Derr DexImageC_LoadFloodImage(DexImageC* dexImage, DexImageC* flood);
DllExportC Derr DexImageC_LoadFloodImageFromFile(DexImageC* dexImage, const char* filename);
DllExportC Derr DexImageC_LoadDefectMap(DexImageC* dexImage, DexImageC* defect);
DllExportC Derr DexImageC_LoadDefectMapFromFile(DexImageC* dexImage, const char* filename);

DllExportC DexImageC* DexImageC_GetDarkImage(DexImageC* dexImage);
DllExportC DexImageC* DexImageC_GetFloodImage(DexImageC* dexImage);
DllExportC DexImageC* DexImageC_GetDefectMap(DexImageC* dexImage);
DllExportC DexImageC* DexImageC_GetImagePlane(DexImageC* dexImage, int iZ);

DllExportC Derr DexImageC_GetImageType(DexImageC* dexImage, DexImageTypes* type);
DllExportC Derr DexImageC_SetImageType(DexImageC* dexImage, DexImageTypes type);

DllExportC Derr DexImageC_SetDarkOffset(DexImageC* dexImage, int offset);
DllExportC Derr DexImageC_GetDarkOffset(DexImageC* dexImage, int* offset);

DllExportC Derr DexImageC_SetLinearizationStarts(DexImageC* dexImage, unsigned int* measuredStarts, int MSLen);
DllExportC Derr DexImageC_GetLinearizationStarts(DexImageC* dexImage, unsigned int** measuredStarts, int* MSLen);

DllExportC Derr DexImageC_SetImageParameters(DexImageC* dexImage, bins binningMode, int modelNumber);

DllExportC Derr DexImageC_GetImageModel(DexImageC* dexImage,int* model);
DllExportC Derr DexImageC_GetImageBinning(DexImageC* dexImage,bins* binFmt);

DllExportC Derr DexImageC_IsDefectCorrected(DexImageC* dexImage,BOOL* onOff);
DllExportC Derr DexImageC_IsDarkCorrected(DexImageC* dexImage,BOOL* onOff);
DllExportC Derr DexImageC_IsFloodCorrected(DexImageC* dexImage,BOOL* onOff);
DllExportC Derr DexImageC_IsAveraged(DexImageC* dexImage,BOOL* onOff);
DllExportC Derr DexImageC_IsFixed(DexImageC* dexImage,BOOL* onOff);
DllExportC Derr DexImageC_IsLinearized(DexImageC* dexImage,BOOL* onOff);
DllExportC Derr DexImageC_IsSorted(DexImageC* dexImage,BOOL* onOff);
DllExportC Derr DexImageC_SetFloodCorrectedFlag(DexImageC* dexImage,BOOL onOff);
DllExportC Derr DexImageC_SetDarkCorrectedFlag(DexImageC* dexImage,BOOL onOff);
DllExportC Derr DexImageC_SetDefectCorrectedFlag(DexImageC* dexImage,BOOL onOff);
DllExportC Derr DexImageC_SetAveragedFlag(DexImageC* dexImage,BOOL onOff);
DllExportC Derr DexImageC_SetFixedFlag(DexImageC* dexImage,BOOL onOff);
DllExportC Derr DexImageC_SetLinearizedFlag(DexImageC* dexImage,BOOL onOff);
DllExportC Derr DexImageC_SetSortedFlag(DexImageC* dexImage,BOOL onOff);
DllExportC Derr DexImageC_SubImageDefectCorrection(DexImageC* dexImage, int iStartCol, int iStartRow, int iWidth, int iHeight, int iCorrectionsFlag);

#ifdef __cplusplus
}
#endif