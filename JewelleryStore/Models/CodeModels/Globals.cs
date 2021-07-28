using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.Models.CodeModels
{
    public class Globals
    {
        #region CutomerType
        public static byte PrivilegedCustomer = 1;
        public static byte NormalCustomer = 0;
        #endregion CustomerType

        #region PriceCurrencies
        public static string IndianRupees = "inr";
        public static string USDollars = "usd";
        public static string BritishPounds = "gbp";
        #endregion PriceCurrencies

        #region ItemType
        public static string Gold = "gold";
        public static string Silver = "silver";
        public static string Platinum = "platinum";
        public static string Diamond = "diamond";
        public static string OthersItemType = "unknow_material";
        #endregion ItemType

        #region PerUnit
        public static string Gram = "gram";
        public static string KiloGram = "kilogram";
        public static string OtherUnit = "unknown_unit";
        #endregion PerUnit

        #region Discount
        public static decimal DefaultDiscountPercentage = 2;
        #endregion Discount

        #region PaymentStatus
        public static byte Paid = 1;
        public static byte UnPaid = 0;
        #endregion PaymentStatus
    }
}
