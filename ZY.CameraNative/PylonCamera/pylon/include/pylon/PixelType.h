//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Basler Components
//  Project: PYLON
//  Author:  Hartmut Nebelung, Edgar Katzer, AG
//  $Header:  $
//-----------------------------------------------------------------------------
/*!
\file
\brief    Definition of types of helper functions concerning image format and pixel type.
\ingroup  PYLON_PUBLIC
*/

#ifndef INCLUDED_PIXELTYPE_H_1534845
#define INCLUDED_PIXELTYPE_H_1534845

#if _MSC_VER > 1000
#pragma once
#endif

#include <pylon/Platform.h>

#ifdef _MSC_VER
#   pragma pack(push, PYLON_PACKING)
#endif /* _MSC_VER */


#include <pylon/PylonBase.h>

namespace Pylon
{
     /** \addtogroup Pylon_ImageHandlingSupport
     * @{
     */

    // Bitmask value of the monochrome type. Internal use only.
#define PIXEL_MONO  0x01000000
    // Bitmask value of the color pixel type. Internal use only.
#define PIXEL_COLOR 0x02000000
    // Sets the bit count of pixel type. Internal use only.
#define PIXEL_BIT_COUNT(n) ((n) << 16)

    /// Lists the available pixel types. These pixel types are returned by a grab result and are used by the Image Handling Support classes.
    enum EPixelType
    {
        PixelType_Undefined = -1,
        PixelType_Mono1packed     = 0x80000000 | PIXEL_MONO | PIXEL_BIT_COUNT(1)  | 0x000c, ///<...
        PixelType_Mono2packed     = 0x80000000 | PIXEL_MONO | PIXEL_BIT_COUNT(2)  | 0x000d, ///<...
        PixelType_Mono4packed     = 0x80000000 | PIXEL_MONO | PIXEL_BIT_COUNT(4)  | 0x000e, ///<...

        PixelType_Mono8           = PIXEL_MONO | PIXEL_BIT_COUNT(8)  | 0x0001, ///<...
        PixelType_Mono8signed     = PIXEL_MONO | PIXEL_BIT_COUNT(8)  | 0x0002, ///<...
        PixelType_Mono10          = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0003, ///<...
        PixelType_Mono10packed    = PIXEL_MONO | PIXEL_BIT_COUNT(12) | 0x0004, ///<...
        PixelType_Mono12          = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0005,  ///<...
        PixelType_Mono12packed    = PIXEL_MONO | PIXEL_BIT_COUNT(12) | 0x0006,  ///<...
        PixelType_Mono16          = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0007,  ///<...

        PixelType_BayerGR8        = PIXEL_MONO | PIXEL_BIT_COUNT(8)  | 0x0008,  ///<...
        PixelType_BayerRG8        = PIXEL_MONO | PIXEL_BIT_COUNT(8)  | 0x0009,  ///<...
        PixelType_BayerGB8        = PIXEL_MONO | PIXEL_BIT_COUNT(8)  | 0x000a,  ///<...
        PixelType_BayerBG8        = PIXEL_MONO | PIXEL_BIT_COUNT(8)  | 0x000b,  ///<...

        PixelType_BayerGR10       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x000c,  ///<...
        PixelType_BayerRG10       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x000d,  ///<...
        PixelType_BayerGB10       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x000e,  ///<...
        PixelType_BayerBG10       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x000f,  ///<...

        PixelType_BayerGR12       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0010,  ///<...
        PixelType_BayerRG12       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0011,  ///<...
        PixelType_BayerGB12       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0012,  ///<...
        PixelType_BayerBG12       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0013,  ///<...

        PixelType_RGB8packed      = PIXEL_COLOR | PIXEL_BIT_COUNT(24) | 0x0014,  ///<...
        PixelType_BGR8packed      = PIXEL_COLOR | PIXEL_BIT_COUNT(24) | 0x0015,  ///<...

        PixelType_RGBA8packed     = PIXEL_COLOR | PIXEL_BIT_COUNT(32) | 0x0016,  ///<...
        PixelType_BGRA8packed     = PIXEL_COLOR | PIXEL_BIT_COUNT(32) | 0x0017,  ///<...

        PixelType_RGB10packed     = PIXEL_COLOR | PIXEL_BIT_COUNT(48) | 0x0018,  ///<...
        PixelType_BGR10packed     = PIXEL_COLOR | PIXEL_BIT_COUNT(48) | 0x0019,  ///<...

        PixelType_RGB12packed     = PIXEL_COLOR | PIXEL_BIT_COUNT(48) | 0x001a,  ///<...
        PixelType_BGR12packed     = PIXEL_COLOR | PIXEL_BIT_COUNT(48) | 0x001b,  ///<...

        PixelType_RGB16packed     = PIXEL_COLOR | PIXEL_BIT_COUNT(48) | 0x0033,  ///<...

        PixelType_BGR10V1packed   = PIXEL_COLOR | PIXEL_BIT_COUNT(32) | 0x001c,  ///<...
        PixelType_BGR10V2packed   = PIXEL_COLOR | PIXEL_BIT_COUNT(32) | 0x001d,  ///<...

        PixelType_YUV411packed    = PIXEL_COLOR | PIXEL_BIT_COUNT(12) | 0x001e,  ///<...
        PixelType_YUV422packed    = PIXEL_COLOR | PIXEL_BIT_COUNT(16) | 0x001f,  ///<...
        PixelType_YUV444packed    = PIXEL_COLOR | PIXEL_BIT_COUNT(24) | 0x0020,  ///<...

        PixelType_RGB8planar      = PIXEL_COLOR | PIXEL_BIT_COUNT(24) | 0x0021,  ///<...
        PixelType_RGB10planar     = PIXEL_COLOR | PIXEL_BIT_COUNT(48) | 0x0022,  ///<...
        PixelType_RGB12planar     = PIXEL_COLOR | PIXEL_BIT_COUNT(48) | 0x0023,  ///<...
        PixelType_RGB16planar     = PIXEL_COLOR | PIXEL_BIT_COUNT(48) | 0x0024,  ///<...

        PixelType_YUV422_YUYV_Packed = PIXEL_COLOR | PIXEL_BIT_COUNT(16) | 0x0032,  ///<...

        PixelType_BayerGR12Packed = PIXEL_MONO | PIXEL_BIT_COUNT(12) | 0x002A,  ///<...
        PixelType_BayerRG12Packed = PIXEL_MONO | PIXEL_BIT_COUNT(12) | 0x002B,  ///<...
        PixelType_BayerGB12Packed = PIXEL_MONO | PIXEL_BIT_COUNT(12) | 0x002C,  ///<...
        PixelType_BayerBG12Packed = PIXEL_MONO | PIXEL_BIT_COUNT(12) | 0x002D,  ///<...

        PixelType_BayerGR16       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x002E,  ///<...
        PixelType_BayerRG16       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x002F,  ///<...
        PixelType_BayerGB16       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0030,  ///<...
        PixelType_BayerBG16       = PIXEL_MONO | PIXEL_BIT_COUNT(16) | 0x0031,  ///<...
    
        PixelType_RGB12V1packed   = PIXEL_COLOR | PIXEL_BIT_COUNT(36) | 0X0034,  ///<...

        PixelType_Double =  0x80000000 | PIXEL_MONO | PIXEL_BIT_COUNT(48) | 0x100
    };

    // pylon 2.x compatibility.
    typedef EPixelType PixelType;

    /// Returns true if the pixel type is Mono and the pixel values are not byte aligned.
    inline bool IsMonoPacked(EPixelType pixelType)
    {
        if (PixelType_Mono1packed == pixelType)   return true;
        if (PixelType_Mono2packed == pixelType)   return true;
        if (PixelType_Mono4packed == pixelType)   return true;
        if (PixelType_Mono10packed == pixelType)   return true;
        if (PixelType_Mono12packed == pixelType)   return true;

        return false;
    }

    /// Returns true if the pixel type is Bayer and the pixel values are not byte aligned.
    inline bool IsBayerPacked(EPixelType pixelType)
    {
        if (PixelType_BayerGB12Packed == pixelType)   return true;
        if (PixelType_BayerGR12Packed == pixelType)   return true;
        if (PixelType_BayerRG12Packed == pixelType)   return true;
        if (PixelType_BayerBG12Packed == pixelType)   return true;

        return false;
    }

    /// Returns true if the pixel type is RGB and the pixel values are not byte aligned.
    inline bool IsRGBPacked(EPixelType pixelType)
    {
        if (PixelType_RGB12V1packed == pixelType) return true;

        return false;
    }

    /// Returns true if the pixel type is BGR and the pixel values are not byte aligned.
    inline bool IsBGRPacked(EPixelType pixelType)
    {
        if (PixelType_BGR10V1packed == pixelType) return true;
        if (PixelType_BGR10V2packed == pixelType) return true;

        return false;
    }

    /// Returns true if the pixels of the given pixel type are not byte aligned.
    inline bool IsPacked(EPixelType pixelType)
    {
        return (IsMonoPacked(pixelType) || IsBayerPacked(pixelType) || IsRGBPacked(pixelType) || IsBGRPacked(pixelType));
    }

    /// Returns number of planes in the image composed of the pixel type.
    inline uint32_t PlaneCount(EPixelType pixelType)
    {
        switch (pixelType)
        {
        case PixelType_RGB8planar:
        case PixelType_RGB10planar:
        case PixelType_RGB12planar:
        case PixelType_RGB16planar:
            return 3;
        }

        return 1;
    }

    /// Returns the pixel type of a plane.
    inline EPixelType GetPlanePixelType(EPixelType pixelType)
    {
        switch (pixelType)
        {
        case PixelType_RGB8planar:
            return PixelType_Mono8;
        case PixelType_RGB10planar:
            return PixelType_Mono10;
        case PixelType_RGB12planar:
            return PixelType_Mono12;
        case PixelType_RGB16planar:
            return PixelType_Mono16;
        }

        return pixelType;
    }

    /// Returns true if images of the pixel type are divided into multiple planes.
    inline bool IsPlanar(EPixelType pixelType)
    {
        return PlaneCount( pixelType) > 1;
    }

    /// Lists the Bayer color filter types.
    enum EPixelColorFilter
    {
        PCF_BayerRG, ///<red green
        PCF_BayerGB, ///<green blue
        PCF_BayerGR, ///<green red
        PCF_BayerBG, ///<blue green
        PCF_Undefined ///< undefined color filter or not applicable
    };

    // pylon 2.x compatibility.
    typedef EPixelColorFilter PixelColorFilter;

    /// Returns the Bayer color filter type.
    inline EPixelColorFilter GetPixelColorFilter(EPixelType pixelType)
    {
        if (PixelType_BayerGR8 == pixelType)   return PCF_BayerGR;
        if (PixelType_BayerRG8 == pixelType)   return PCF_BayerRG;
        if (PixelType_BayerGB8 == pixelType)   return PCF_BayerGB;
        if (PixelType_BayerBG8 == pixelType)   return PCF_BayerBG;

        if (PixelType_BayerGR10 == pixelType)   return PCF_BayerGR;
        if (PixelType_BayerRG10 == pixelType)   return PCF_BayerRG;
        if (PixelType_BayerGB10 == pixelType)   return PCF_BayerGB;
        if (PixelType_BayerBG10 == pixelType)   return PCF_BayerBG;

        if (PixelType_BayerGR12 == pixelType)   return PCF_BayerGR;
        if (PixelType_BayerRG12 == pixelType)   return PCF_BayerRG;
        if (PixelType_BayerGB12 == pixelType)   return PCF_BayerGB;
        if (PixelType_BayerBG12 == pixelType)   return PCF_BayerBG;

        if (PixelType_BayerGR12Packed == pixelType)   return PCF_BayerGR;
        if (PixelType_BayerRG12Packed == pixelType)   return PCF_BayerRG;
        if (PixelType_BayerGB12Packed == pixelType)   return PCF_BayerGB;
        if (PixelType_BayerBG12Packed == pixelType)   return PCF_BayerBG;

        if (PixelType_BayerGR16 == pixelType)   return PCF_BayerGR;
        if (PixelType_BayerRG16 == pixelType)   return PCF_BayerRG;
        if (PixelType_BayerGB16 == pixelType)   return PCF_BayerGB;
        if (PixelType_BayerBG16 == pixelType)   return PCF_BayerBG;

        return PCF_Undefined;
    }

    /*!
    \brief Returns the bits needed to store a pixel.

    BitPerPixel(PixelType_Mono12) returns 16 and BitPerPixel(PixelType_Mono12packed)
    returns 12 for example.

    \param[in] pixelType The pixel type.
    \pre The pixel type must be defined.

    \error
        Throws an exception when the pixel type is undefined.
    */
    PYLONBASE_API uint32_t BitPerPixel(EPixelType pixelType);

    /*!
    \brief Returns the number of measured values per pixel.

    SamplesPerPixel(PixelType_Mono8) returns 1 and SamplesPerPixel(PixelType_RGB8packed)
    returns 3 for example.

    \param[in] pixelType The pixel type.
    \pre The pixel type must be defined. The pixel type is not PixelType_YUV411packed.

    \error
        Throws an exception when the pixel type is undefined.
    */
    PYLONBASE_API uint32_t SamplesPerPixel(EPixelType pixelType);

    /// Returns true when the pixel type represents a YUV format.
    inline bool IsYUV( EPixelType pixelType)
    {
        switch (pixelType)
        {
        case PixelType_YUV411packed:
        case PixelType_YUV422_YUYV_Packed:
        case PixelType_YUV422packed:
        case PixelType_YUV444packed:
            return true;
        }
        return false;
    }

    /// Returns true when the pixel type represents an RGBA format.
    inline bool IsRGBA(EPixelType pixelType)
    {
        switch (pixelType)
        {
        case PixelType_RGBA8packed:
            return true;
        }
        return false;
    }

    /// Returns true when the pixel type represents an RGB or RGBA format.
    inline bool IsRGB(EPixelType pixelType)
    {
        switch (pixelType)
        {
        case PixelType_RGB8packed:
        case PixelType_RGB10packed:
        case PixelType_RGB12packed:
        case PixelType_RGB16packed:
        case PixelType_RGB8planar:
        case PixelType_RGB10planar:
        case PixelType_RGB12planar:
        case PixelType_RGB16planar:
        case PixelType_RGB12V1packed:
            return true;
        }
        return IsRGBA( pixelType);
    }

    /// Returns true when the pixel type represents a BGRA format.
    inline bool IsBGRA(EPixelType pixelType)
    {
        switch (pixelType)
        {
        case PixelType_BGRA8packed:
            return true;
        }
        return false;
    }

    /// Returns true when the pixel type represents a BGR or BGRA format.
    inline bool IsBGR(EPixelType pixelType)
    {
        switch (pixelType)
        {
        case PixelType_BGR8packed:
        case PixelType_BGR10packed:
        case PixelType_BGR12packed:
        case PixelType_BGR10V1packed:
        case PixelType_BGR10V2packed:
            return true;
        }
        return IsBGRA( pixelType);
    }

    /// Returns true when the pixel type represents a Bayer format.
    inline bool IsBayer(EPixelType pixelType)
    {
        return GetPixelColorFilter(pixelType) != PCF_Undefined;
    }

    /// Returns true when a given pixel is monochrome, e.g. PixelType_Mono8 or PixelType_BayerGR8.
    inline bool IsMono(EPixelType pixelType)
    {
        return PIXEL_MONO == (pixelType & PIXEL_MONO);
    }

    /// Returns true when an image using the given pixel type is monochrome, e.g. PixelType_Mono8.
    inline bool IsMonoImage(EPixelType pixelType)
    {
        return IsMono( pixelType) && !IsBayer( pixelType);
    }

    /// Returns the minimum step size expressed in pixels for extracting an AOI.
    inline uint32_t GetPixelIncrementX(EPixelType pixelType)
    {
        if ( IsBayer(pixelType))
        {
            return 2;
        }

        switch (pixelType)
        {
        case PixelType_YUV422packed:
            return 2;
        case PixelType_YUV422_YUYV_Packed:
            return 2;
        case PixelType_YUV411packed:
            return 4;
        }
        return 1;
    }

    /// Returns the minimum step size expressed in pixels for extracting an AOI.
    inline uint32_t GetPixelIncrementY(EPixelType pixelType)
    {
        if ( IsBayer(pixelType))
        {
            return 2;
        }
        return 1;
    }

    /*!
    \brief Returns the bit depth of a value of the pixel in bits.

    This may be less than the size needed to store the pixel.
    BitDepth(PixelType_Mono12) returns 12, BitDepth(PixelType_Mono12packed)
    returns 12, and  BitDepth(PixelType_RGB8packed) returns 8 for example.

    \param[in] pixelType The pixel type.

    \pre The pixel type must be valid.

    \error
        Throws an exception when the pixel type is undefined.
    */
    PYLONBASE_API uint32_t BitDepth( EPixelType pixelType );

    /*!
    \brief Computes the stride in byte.

    The stride indicates the number of bytes between the beginning of one row 
    in an image and the beginning of the next row.
    For planar pixel types the returned value represents the stride of a plane.
    
    The stride in bytes cannot be computed for packed image format when the stride is not byte aligned and paddingX == 0.
    If paddingX is larger than zero and the stride without padding is not byte aligned then the rest of the partially
    filled byte is considered as padding, e.g. pixelType = PixelType_Mono12packed, width = 5, paddingX = 10 results
    in a stride of 18 Bytes (stride without padding is 5 * BitPerPixel( PixelType_Mono12packed) = 5 * 12 = 60 Bits = 7.5 Bytes).

    See also Pylon::IsPacked().

    \param[out] strideBytes  The stride in byte if it can be computed.
    \param[in] pixelType The pixel type.
    \param[in] width     The number of pixels in a row.
    \param[in] paddingX  The number of additional bytes at the end of a row (byte aligned).

    \return Returns true if the stride can be computed.

    \pre 
            The \c width value must be >= 0 and <= _I32_MAX.

    \error
        Throws an exception when the preconditions are not met.
    */
    PYLONBASE_API bool ComputeStride( size_t& strideBytes, EPixelType pixelType, uint32_t width, size_t paddingX = 0);

    /*!
    \brief Computes the padding value from row stride in byte.

    \param[out] strideBytes  The stride in byte.
    \param[in] pixelType The pixel type.
    \param[in] width     The number of pixels in a row.

    \return Returns the paddingX value for the given stride value (byte aligned).

    \pre 
        <ul>
        <li> The value of \c strideBytes must be large enough to contain a line described by \c pixelType and \c width.
        <li> The pixel type must be valid.
        <li> The \c width value must be >= 0 and <= _I32_MAX.
        </ul>

    \error
        Throws an exception when the preconditions are not met.
    */
    PYLONBASE_API size_t ComputePaddingX( size_t strideBytes, EPixelType pixelType, uint32_t width);

    /*!
    \brief Computes the buffer size in byte.

    \param[in] pixelType The pixel type.
    \param[in] width     The number of pixels in a row.
    \param[in] height    The number of rows in an image.
    \param[in] paddingX  The number of extra data bytes at the end of each row (byte aligned). 
    \return The buffer size in byte.

    \pre 
        <ul>
        <li> The pixel type must be valid.
        <li> The \c width value must be >= 0 and <= _I32_MAX.
        <li> The \c height value must be >= 0 and <= _I32_MAX.
        </ul>

    \error
        Throws an exception when the preconditions are not met.
    */
    PYLONBASE_API size_t ComputeBufferSize( EPixelType pixelType, uint32_t width, uint32_t height, size_t paddingX = 0);

    //-----------------------------------------------------------------------
    //  Deprecated functions: These functions will be removed in future releases.
    //-----------------------------------------------------------------------

    inline bool PYLON_BASE_3_0_DEPRECATED("This function has been deprecated. Use the IsRGB and IsRGBA functions instead. However, there is no exact replacement available.")
        IsValidRGB(EPixelType pixelType)
    {
        if (PixelType_RGB8packed == pixelType)   return true;
        if (PixelType_RGBA8packed == pixelType)  return true;
        if (PixelType_RGB10packed == pixelType)  return true;
        if (PixelType_RGB12packed == pixelType)  return true;
        if (PixelType_RGB16packed == pixelType)  return true;
        if (PixelType_RGB12V1packed == pixelType)return true;

        return false;
    };


    inline bool PYLON_BASE_3_0_DEPRECATED("This function has been deprecated. Use the IsBGR and IsBGRA functions instead. However, there is no exact replacement available.")
        IsValidBGR(EPixelType pixelType)
    {
        if (PixelType_BGR8packed == pixelType)   return true;
        if (PixelType_BGRA8packed == pixelType)  return true;
        if (PixelType_BGR10packed == pixelType)  return true;
        if (PixelType_BGR12packed == pixelType)  return true;

        return false;
    };

    enum PYLON_BASE_3_0_DEPRECATED("This enumeration has been deprecated. Use the more secure BitPerPixel function which throws exceptions instead.")
    {
        PS_Undefined = -1
    };


    inline int PYLON_BASE_3_0_DEPRECATED("This function has been deprecated. Use the BitDepth and BitPerPixel functions instead. However, there is no exact replacement available.")
        PixelSize(EPixelType pixelType)
    {
        switch (pixelType)
        {
            case PixelType_Mono8:
            case PixelType_Mono8signed:
            case PixelType_BayerGR8:
            case PixelType_BayerRG8:
            case PixelType_BayerGB8:
            case PixelType_BayerBG8:
                return 8;

            case PixelType_Mono10:
            case PixelType_Mono10packed:
            case PixelType_BayerGR10:
            case PixelType_BayerRG10:
            case PixelType_BayerGB10:
            case PixelType_BayerBG10:
                return 10;

            case PixelType_Mono12:
            case PixelType_Mono12packed:
            case PixelType_BayerGR12:
            case PixelType_BayerRG12:
            case PixelType_BayerGB12:
            case PixelType_BayerBG12:
            case PixelType_BayerGB12Packed:
            case PixelType_BayerGR12Packed:
            case PixelType_BayerRG12Packed:
            case PixelType_BayerBG12Packed:
                return 12;

            case PixelType_Mono16:
            case PixelType_BayerGR16:
            case PixelType_BayerRG16:
            case PixelType_BayerGB16:
            case PixelType_BayerBG16:
                return 16;

            case PixelType_RGB8packed:
            case PixelType_BGR8packed:
                return 24;

            case PixelType_RGBA8packed:
            case PixelType_BGRA8packed:
                return 32;

            case PixelType_RGB12V1packed:
                return 36;

            case PixelType_RGB10packed:
            case PixelType_BGR10packed:
            case PixelType_RGB12packed:
            case PixelType_BGR12packed:
                return 48;

        }

        return PS_Undefined;      //  used as invalid code
    };

    /** 
     * @}
     */
}

#ifdef _MSC_VER
#   pragma pack(pop)
#endif /* _MSC_VER */

#endif /* INCLUDED_PIXELTYPE_H_1534845 */
