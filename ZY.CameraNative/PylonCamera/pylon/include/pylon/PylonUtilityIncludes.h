//-----------------------------------------------------------------------------
//  (c) 2008 by Basler Vision Technologies
//  Section: Basler Components
//  Project: PYLON
//  Author:  JS
//  $Header:  $
//-----------------------------------------------------------------------------
/*!
\file
\brief  Master include for the PylonUtility library
*/

#ifndef PYLONUTILITIESINCLUDES_H_INCLUDED__
#define PYLONUTILITIESINCLUDES_H_INCLUDED__

#include <pylon/Platform.h>
#include <pylon/PylonUtility.h>
#include <pylon/ImageFormat.h>
#include <pylon/PylonImage.h>
#include <pylon/PylonBitmapImage.h>
#include <pylon/ImageFormatConverter.h>
#include <pylon/FeaturePersistence.h>
#include <pylon/AviWriter.h>
#include <pylon/ImagePersistence.h>

// The image format converter classes based on Pylon::CPixelFormatConverter are now deprecated
// and replaced by wrapper classes that use the new, much more convenient, Pylon::CImageFormatConverter class internally.
#include <pylon/PixelFormatConverter.h>
#include <pylon/PixelFormatConverterBayer.h>
#include <pylon/PixelFormatConverterBayer16.h>
#include <pylon/PixelFormatConverterMonoXX.h>
#include <pylon/PixelFormatConverterGamma.h>
#include <pylon/PixelFormatConverterMonoPacked.h>
#include <pylon/PixelFormatConverterGammaPacked.h>
#include <pylon/PixelFormatConverterRGB.h>
#include <pylon/PixelFormatConverterTruncate.h>
#include <pylon/PixelFormatConverterTruncatePacked.h>
#include <pylon/PixelFormatConverterYUV422.h>

#if defined (PYLON_WIN_BUILD) && !defined (PYLON_NO_AUTO_IMPLIB)
#include <pylon/PylonLinkage.h>
#pragma comment(lib, PYLON_LIB_NAME( "PylonUtility" ))
#endif

#endif
