//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Basler Components
//  Project: PYLON
//  Author:  AH
//  $Header:  $
//-----------------------------------------------------------------------------
/*!
\file
\brief  Function and classes for initializing the pylon runtime.
*/

#ifndef __PYLONBASE_H__
#define __PYLONBASE_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <pylon/Platform.h>

#ifdef _MSC_VER
#   pragma pack(push, PYLON_PACKING)
#endif /* _MSC_VER */


#include <pylon/stdinclude.h>

#ifdef PYLONBASE_EXPORTS
#  define PYLONBASE_API APIEXPORT
#else
#  define PYLONBASE_API APIIMPORT
#endif

#if defined(PYLON_BASE_3_0_NO_DEPRECATE)
#   define PYLON_BASE_3_0_DEPRECATED(message)
#else
#   define PYLON_BASE_3_0_DEPRECATED(message) PYLON_DEPRECATED(message)
#endif

namespace Pylon
{
/**
  \brief Initializes the pylon runtime system.

  You must call PylonInitialize before calling any other pylon functions.
  When finished you must call PylonTerminate to free up all resources used by pylon.

  You can use the helperclass PylonAutoInitTerm to let the compiler call
  PylonInitialze and PylonTerminate.

  Just create a local object on the stack in your main function and
  the constructor and destructor will call the functions.
  See PylonAutoInitTerm for a sample.

  \if windows
  For MFC applications we recommend to call PylonInitialize and PylonTerminate in the
  application's InitInstance() and ExitInstance() methods.
  This prevents the MFC runtime from reporting a huge number of pretended memory leaks.
  \endif

  */
PYLONBASE_API void CDECL PylonInitialize(void);

/**
    \brief Frees resources allocated by the pylon runtime system.

    Call this function before terminating the application. Don't use any pylon methods or pylon objects after
    having called PylonTerminate().

    */
PYLONBASE_API void CDECL PylonTerminate(bool ShutDownLogging = true);

/**
    \brief Returns the version number of pylon

    It is possible to pass a NULL pointer for a version number category if the value is not of interest.
    */
extern "C" PYLONBASE_API void CDECL GetPylonVersion( unsigned int* major, unsigned int* minor, unsigned int* subminor, unsigned int* build);


/**
    \brief helperclass to automagically call PylonInitialize and PylonTerminate in constructor and destructor

    \code

    #include <pylon/TlFactory.h>

    // ...

    using namespace Pylon;

    int main(int argc, char* argv[])
    {

        PylonAutoInitTerm autoInitTerm;

        // Get the transport layer factory
        CTlFactory& TlFactory = CTlFactory::GetInstance();

        // Get all attached cameras and exit application if no camera is found
        DeviceInfoList_t devices;
        if ( 0 == TlFactory.EnumerateDevices( devices ) )
        {
            cerr << "No camera present!" << endl;
            return 1;
        }
        else
        {
            // do something with devices ...
            //...
        }


        return 0;

    }  // When leaving the main function, the desctructor of the PylonAutoInitTerm object will be called
       // and it will in turn call PylonTerminate.
    \endcode
*/
    class PylonAutoInitTerm
    {
    public:
        PYLONBASE_API PylonAutoInitTerm(void)
        {
            PylonInitialize();
        }

        PYLONBASE_API ~PylonAutoInitTerm(void)
        {
            PylonTerminate();
        }
    private:
        // unimplemented new operator to prevent creation on the heap which is a bad idea.
        // This object must be created in the local stackframe (see sample above)
        void* operator new(size_t);

    };

}

#ifdef _MSC_VER
#   pragma pack(pop)
#endif /* _MSC_VER */

#endif /* __PYLONBASE_H__ */
