//-----------------------------------------------------------------------------
//  (c) 2008 by Basler Vision Technologies
//  Section: Basler Components
//  Project: PYLON
//  Author:  Thomas Koeller
//  $Header:  $
//-----------------------------------------------------------------------------
/*!
\file 
\brief Device class definitions
*/

#ifndef __DEVICE_CLASS_H__
#define __DEVICE_CLASS_H__

#if _MSC_VER > 1000
#pragma once
#endif //_MSC_VER > 1000

#include <pylon/PylonBase.h>

namespace Pylon
{
    /** \addtogroup Pylon_TransportLayer
     * @{
     */
    const char* const Basler1394DeviceClass = "Basler1394"; ///< This device class can be used to create the corresponding Transport Layer object or when creating Devices with the Transport Layer Factory.
    const char* const BaslerGigEDeviceClass = "BaslerGigE"; ///< This device class can be used to create the corresponding Transport Layer object or when creating Devices with the Transport Layer Factory.
    const char* const BaslerCamEmuDeviceClass ="BaslerCamEmu";
    const char* const BaslerIpCamDeviceClass = "BaslerIPCam";
    const char* const BaslerCameraLinkDeviceClass = "BaslerCameraLink"; ///< This device class can be used to create the corresponding Transport Layer object or when creating Devices with the Transport Layer Factory.
    const char* const BaslerGenTlDeviceClass = "BaslerGenTLConsumer";
    const char* const BaslerUsbDeviceClass = "BaslerUsb";
    PYLON_DEPRECATED("Use BaslerCameraLinkDeviceClass") static const char* BaslerCLSerDeviceClass = "BaslerCameraLink";
    PYLON_DEPRECATED("Use BaslerIpCamDeviceClass") static const char* IpCamDeviceClass = "BaslerIPCam";
    /**
     * @}
     */
}
#endif
