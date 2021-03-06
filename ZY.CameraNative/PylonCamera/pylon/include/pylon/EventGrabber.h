//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Basler Components
//  Project: PYLON
//  Author:  Hartmut Nebelung, AH
//  $Header:  $
//-----------------------------------------------------------------------------
/*!
\file
\brief Low Level API: Definition of IEventGrabber interface
*/
#ifndef __IEVENTGRABBER_H__
#define __IEVENTGRABBER_H__

#if _MSC_VER > 1000
#pragma once
#endif //_MSC_VER > 1000

#include <pylon/Platform.h>

#ifdef _MSC_VER
#   pragma pack(push, PYLON_PACKING)
#endif /* _MSC_VER */

#include <pylon/stdinclude.h>

namespace GenApi
{
    interface INodeMap;
}

namespace Pylon
{

    class EventResult;
    class WaitObject;

    /*!
    \interface IEventGrabber
    \ingroup Pylon_LowLevelApi
    \brief Low Level API: Interface of an object receiving asynchronous events.

    Asynchronous event messages are received from the camera. Internal Buffers are filled
    and stored in an output queue. While the output queue contains data the associated
    waitobject is signaled.

    With RetrieveEvent() the first event message is copied into a user buffer.
    */
    interface PUBLIC_INTERFACE IEventGrabber
    {
        /// Open the event grabber
        virtual void Open() = 0;
        /// Close the event grabber
        virtual void Close() = 0;
        /// Retrieve whether the event grabber is open
        virtual bool IsOpen() const = 0;
        /// Retrieve an event message from the output queue
        /*!
           \return When the event was available true is returned
           and the event message is copied into the EventResult.
        */
        virtual bool RetrieveEvent( EventResult& ) = 0;
        /// Return the event object associated with the grabber
        /*!
            This object get signaled as soon as a event has occurred.
            It will be reset when the output queue is empty.
        */
        virtual WaitObject& GetWaitObject() const = 0;
        /// Return the associated event grabber parameters
        /*! If no parameters are available, NULL is returned. */
        virtual GenApi::INodeMap* GetNodeMap() = 0;
    };

}

#ifdef _MSC_VER
#   pragma pack(pop)
#endif /* _MSC_VER */

#endif //__IEVENTGRABBER_H__
