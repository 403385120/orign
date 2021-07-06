//-----------------------------------------------------------------------------
//  (c) 2010 by Basler Vision Technologies
//  Section: Basler Components
//  Project: PYLON
//  Author:  Andreas Gau
//  $Header:  $
//-----------------------------------------------------------------------------
/*!
\file
\brief Pylon GigE specific grab result data class.
*/

#ifndef INCLUDED_BASLERGIGEGRABRESULTPTR_H_4564157
#define INCLUDED_BASLERGIGEGRABRESULTPTR_H_4564157

#include <pylon/gige/BaslerGigEGrabResultData.h>
#include <pylon/private/DeviceSpecificGrabResultPtr.h>

namespace Pylon
{
    /** \addtogroup Pylon_InstantCameraApiGigE
     * @{
     */

    /// The GigE specific grab result smart pointer.
    PYLON_DEFINE_DEVICE_SPECIFIC_GRAB_RESULT_PTR( CBaslerGigEGrabResultPtr, CBaslerGigEGrabResultData)

    /** 
     * @}
     */
} // namespace Pylon

#endif /* INCLUDED_BASLERGIGEGRABRESULTPTR_H_4564157 */
