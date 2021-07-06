#pragma once 

#define COUNT_OF_IMAGES_TO_CAPTURE 8

#define CAMERA_IMAGE_WIDTH 1000
#define CAMERA_IMAGE_HEIGHT 1000
#define CAMERA_IMAGE_SIZE  (CAMERA_IMAGE_WIDTH * CAMERA_IMAGE_HEIGHT)

bool  CaptureOneImage(BYTE * pImageBuffer);

void CaptureAndSaveImage();

void BeginCaptureThread();

bool AverageValue(BYTE * pByteDataBuffer,int * pIntDataBuffer,long lSize,int iCycle);
bool ConvertIntToBYTE(int * iBuffer,BYTE * bBuffer,long lSize);
