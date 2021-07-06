//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Basler Components
//  Project: PYLON
//  Author:  AH
//  $Header:  $
//-----------------------------------------------------------------------------
/*!
\file 
\internal
\brief Declaration of the IXmlFileProvider interface
*/

#ifndef __PYLONXMLFILEPROVIDER_H__
#define __PYLONXMLFILEPROVIDER_H__

#if _MSC_VER > 1000
#pragma once
#endif

#include <pylon/Platform.h>

#ifdef _MSC_VER
#   pragma pack(push, PYLON_PACKING)
#endif /* _MSC_VER */

#include <pylon/stdinclude.h>


namespace Pylon
{
    class CDeviceInfo;

    /** 
    \internal 
    */
    interface PUBLIC_INTERFACE IXmlFileProvider
    {
        virtual bool CanProvideXmlFile( const CDeviceInfo& ) = 0;
        virtual void ProvideXmlFile( 
            const CDeviceInfo& di, 
            String_t& RecommendedFileName, 
            String_t& XmlString 
         ) = 0;
    };

}


#ifdef _MSC_VER
#   pragma pack(pop)
#endif /* _MSC_VER */

#endif /* __PYLONXMLFILEPROVIDER_H__ */

