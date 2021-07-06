#include "ICameraNative.h"

ICameraNative* CameraFactory::CreateCamera(int camType) 
{
	if (camType == 0) {
		//return new BaslerCamera();
	}

	throw "Not implemented!";
}