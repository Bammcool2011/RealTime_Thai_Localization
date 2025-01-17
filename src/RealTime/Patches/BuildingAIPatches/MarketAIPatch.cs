namespace RealTime.Patches.BuildingAIPatches
{
    using HarmonyLib;
    using RealTime.CustomAI;

    internal class MarketAIPatch
    {
        /// <summary>Gets or sets the custom AI object for buildings.</summary>
        public static RealTimeBuildingAI RealTimeBuildingAI { get; set; }

        [HarmonyPatch(typeof(MarketAI), "SimulationStep")]
        [HarmonyPrefix]
        public static bool SimulationStepPrefix(ref Building buildingData, ref byte __state)
        {
            __state = buildingData.m_outgoingProblemTimer;
            if (buildingData.m_customBuffer2 > 0)
            {
                // Simulate some goods become spoiled; additionally, this will cause the buildings to never reach the 'stock full' state.
                // In that state, no visits are possible anymore, so the building gets stuck
                --buildingData.m_customBuffer2;
            }

            return true;
        }

        [HarmonyPatch(typeof(MarketAI), "SimulationStep")]
        [HarmonyPostfix]
        public static void SimulationStepPostfix(ushort buildingID, ref Building buildingData, byte __state)
        {
            if (__state != buildingData.m_outgoingProblemTimer && RealTimeBuildingAI != null)
            {
                RealTimeBuildingAI.ProcessBuildingProblems(buildingID, __state);
            }
        }
    }
}
