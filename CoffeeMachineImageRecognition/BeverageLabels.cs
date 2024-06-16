using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachineImageRecognition
{
    public static class BeverageLabels
    {
        public const string CafeAuLait = "cafeaulait";
        public const string CaffeLatte = "caffelatte";
        public const string Cappuccino = "cappuccino";
        public const string Chocodream = "chocodream";
        public const string Coffee = "coffee";
        public const string Espresso = "espresso";
        public const string HotChocolate = "hotchocolate";
        public const string HotWater = "hotwater";
        public const string LatteMachiato = "lattemachiato";
        public const string Lungo = "lungo";
        public const string Menu = "menu";
        public const string Unknown = "unknown";

        public static readonly List<string> AllLabels = new List<string>() {
            BeverageLabels.CafeAuLait,
            BeverageLabels.CaffeLatte,
            BeverageLabels.Cappuccino,
            BeverageLabels.Chocodream,
            BeverageLabels.Coffee,
            BeverageLabels.Espresso,
            BeverageLabels.HotChocolate,
            BeverageLabels.HotWater,
            BeverageLabels.LatteMachiato,
            BeverageLabels.Lungo,
            BeverageLabels.Menu,
            BeverageLabels.Unknown,
        };

        public static BeverageEnum MapStringToEnum(string label)
        {
            switch (label.ToLower())
            {
                case BeverageLabels.CafeAuLait:
                    return BeverageEnum.CafeAuLait;
                case BeverageLabels.CaffeLatte:
                    return BeverageEnum.CaffeLatte;
                case BeverageLabels.Cappuccino: 
                    return BeverageEnum.Cappuccino;
                case BeverageLabels.Chocodream:
                    return BeverageEnum.Chocodream;
                case BeverageLabels.Coffee:
                    return BeverageEnum.Coffee;
                case BeverageLabels.Espresso:
                    return BeverageEnum.Espresso;
                case BeverageLabels.HotChocolate:
                    return BeverageEnum.HotChocolate;
                case BeverageLabels.HotWater:
                    return BeverageEnum.HotWater;
                case BeverageLabels.LatteMachiato:
                    return BeverageEnum.LatteMachiato;
                case BeverageLabels.Lungo:
                    return BeverageEnum.Lungo;
                case BeverageLabels.Menu:
                    return BeverageEnum.Menu;
                case BeverageLabels.Unknown:
                    return BeverageEnum.Unknown;
                default:
                    throw new ArgumentException("Unknown label: " + label);
            }
        }
    }
}
