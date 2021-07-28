using JewelleryStore.Models.CodeModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using JewelleryStore.Models.DBModels;
using System.Reflection;

namespace JewelleryStore.HelperMethods
{
    public class ApplicationHelper
    {
        public static string GetProperUnits(string unit)
        {
            if(unit.Contains(Globals.Gram))
            {
                return Globals.Gram;
            }
            else if(unit.Contains(Globals.KiloGram))
            {
                return Globals.KiloGram;
            }
            return Globals.OtherUnit;
        }
        public static string GetProperItemType(string itemType)
        {
            if (itemType.Equals(Globals.Gold))
            {
                return Globals.Gold;
            }
            else if (itemType.Equals(Globals.Diamond))
            {
                return Globals.Diamond;
            }
            else if (itemType.Equals(Globals.Platinum))
            {
                return Globals.Platinum;
            }
            else if (itemType.Equals(Globals.Silver))
            {
                return Globals.Silver;
            }
            else
            {
                return Globals.OthersItemType;
            }
        }
    }
}
