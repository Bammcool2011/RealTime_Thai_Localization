// OutsideConnectionAIPatch.cs

namespace RealTime.Patches
{
    using HarmonyLib;
    using RealTime.Core;
    using RealTime.CustomAI;

    /// <summary>
    /// A static class that provides the patch objects for the outside connections AI.
    /// </summary>
    [HarmonyPatch]
    internal static class OutsideConnectionAIPatch
    {
        /// <summary>Gets or sets the spare time behavior simulation.</summary>
        public static ISpareTimeBehavior SpareTimeBehavior { get; set; }

        public static Compatibility Compatibility { get; set; }

        public static bool IsInAddConnectionOffers { get; private set; } = false;

        [HarmonyPatch(typeof(OutsideConnectionAI), "DummyTrafficProbability")]
        [HarmonyPostfix]
        private static void DummyTrafficProbabilityPostfix(ref int __result)
        {
            if (SpareTimeBehavior != null)
            {
                __result = SpareTimeBehavior.SetDummyTrafficProbability(__result);
            }
        }

        [HarmonyPatch(typeof(OutsideConnectionAI), "AddConnectionOffers")]
        [HarmonyPrefix]
        public static bool AddConnectionOffersPrefix(ushort buildingID, ref int cargoCapacity, ref int residentCapacity, ref int touristFactor0, ref int touristFactor1, ref int touristFactor2, ref int dummyTrafficFactor)
        {
            IsInAddConnectionOffers = true;

            if (!Compatibility.IsAnyModActive(WorkshopMods.AdvancedOutsideConnections))
            {
                touristFactor0 = 325;
                touristFactor1 = 125;
                touristFactor2 = 50;
            }

            return true;
        }

        [HarmonyPatch(typeof(OutsideConnectionAI), "AddConnectionOffers")]
        [HarmonyPostfix]
        public static void AddConnectionOffersPostfix(ushort buildingID, ref int cargoCapacity, ref int residentCapacity, ref int touristFactor0, ref int touristFactor1, ref int touristFactor2, ref int dummyTrafficFactor) => IsInAddConnectionOffers = false;

    }
}
