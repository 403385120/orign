using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    public static class CheckLogicExtFactory
    {
        public static ICheckLogicExt CreateCheckLogicExt(ECheckExtensions ext)
        {
            switch(ext)
            {
                case ECheckExtensions.STF: return new CheckLogicExtSTF(); 
                case ECheckExtensions.Test: return new CheckLogicExtTest(); 
                case ECheckExtensions.RunEmpty: return new CheckLogicExtTest(); 
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
