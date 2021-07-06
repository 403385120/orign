using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    public static class CheckLogicFactory
    {
        public static ICheckLogic CreateCheckLogic(ECheckModes checkMode)
        {
            switch(checkMode)
            {
                case ECheckModes.FourSides: return new FourSidesCheckLogic(); break;
                case ECheckModes.Diagonal_1_2: return new TwoSides13CheckLogic(); break;   //recompose by fjy
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
