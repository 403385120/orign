//------------------------------------------------------------------------------
//  (c) 2010 by Basler Vision Technologies
//  Section: Basler Components
//  Project: PYLON
//  Author:  Andreas Gau
//  $Header:  $
//------------------------------------------------------------------------------
/*!
    \file
    \brief  Contains an interface for a buffer factory.
*/

#ifndef INCLUDED_BUFFERFACTORY_H_264599
#define INCLUDED_BUFFERFACTORY_H_264599

#include <pylon/stdinclude.h>

#ifdef _MSC_VER
#   pragma pack(push, PYLON_PACKING)
#endif /* _MSC_VER */


namespace Pylon
{
    /*!
    \class  IBufferFactory
    \brief  Usable to create a custom buffer factory when needed.
    \ingroup Pylon_InstantCameraApiGeneric
    */
    interface IBufferFactory
    {
    public:
        /// Ensure proper destruction by using a virtual destructor.
        virtual ~IBufferFactory() = 0
        {
        }

        /*!
        \brief Allocates a buffer and provides additional context information.

         
        \param[in] bufferSize      The size of the buffer that has to be allocated.
        \param[out] pCreatedBuffer The pointer to the allocated buffer. May return NULL if the allocation fails.
        \param[out] bufferContext  Context information that belongs to the buffer. 
                                   This context information is provided when FreeBuffer() is called.
                                   The value can be left unchanged if not needed.

        \error
            May throw an exception if the allocation fails.
        */        
        virtual void AllocateBuffer( size_t bufferSize, void** pCreatedBuffer, intptr_t& bufferContext) = 0;


        /*!
        \brief Frees a previously allocated buffer.

        \param[in] pCreatedBuffer The pointer to the allocated buffer. Created by this factory.
        \param[in] bufferContext  Context information of the buffer returned by AllocateBuffer(). 

        \error
            Does not throw C++ exceptions.
        */        
        virtual void FreeBuffer( void* pCreatedBuffer, intptr_t bufferContext) = 0;


        /*!
        \brief Destroys the buffer factory.

        This method is called when the buffer factory is not needed any longer.
        The object implementing IBufferFactory can be deleted by calling: delete this.

        \error
            C++ exceptions from this call will be caught and ignored.
        */        
        virtual void DestroyBufferFactory() = 0;
    };
}

#ifdef _MSC_VER
#   pragma pack(pop)
#endif /* _MSC_VER */

#endif /* INCLUDED_BUFFERFACTORY_H_264599 */
