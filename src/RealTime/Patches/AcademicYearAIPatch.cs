namespace RealTime.Patches
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using ColossalFramework;
    using HarmonyLib;
    using RealTime.CustomAI;
    using RealTime.GameConnection;
    using RealTime.Managers;
    using SkyTools.Tools;

    [HarmonyPatch]
    internal static class AcademicYearAIPatch
    {
        public static RealTimeBuildingAI RealTimeBuildingAI { get; set; }

        public static TimeInfo TimeInfo { get; set; }

        private delegate void StartTogaPartyDelegate(AcademicYearAI __instance, ushort eventID, ref EventData data, DistrictPark.TogaParty party);
        private static readonly StartTogaPartyDelegate StartTogaParty = AccessTools.MethodDelegate<StartTogaPartyDelegate>(typeof(AcademicYearAI).GetMethod("StartTogaParty", BindingFlags.Instance | BindingFlags.NonPublic), null, false);

        [HarmonyPatch(typeof(AcademicYearAI), "GetYearProgress")]
        [HarmonyPrefix]
        public static bool GetYearProgress(ref EventData data, ref float __result)
        {
            var academicYearData = AcademicYearManager.GetAcademicYearData(data.m_building);
            if (academicYearData.IsFirstAcademicYear)
            {
                __result = 0f;
                return false;
            }
            if (academicYearData.DidLastYearEnd)
            {
                __result = 100f;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(AcademicYearAI), "EndEvent")]
        [HarmonyPrefix]
        public static bool EndEvent(ref EventData data)
        {
            if(!RealTimeBuildingAI.IsBuildingWorking(data.m_building))
            {
                return false;
            }
            if (!AcademicYearManager.CanAcademicYearEndorBegin(TimeInfo))
            {
                return false;
            }
            var academicYearData = AcademicYearManager.GetAcademicYearData(data.m_building);
            academicYearData.ActualAcademicYearEndFrame = SimulationManager.instance.m_currentFrameIndex;
            AcademicYearManager.SetAcademicYearData(data.m_building, academicYearData);
            return true;
        }

        [HarmonyPatch(typeof(EventAI), "SimulationStep")]
        public static class SimulationStepBasePatch
        {
            [HarmonyReversePatch]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void SimulationStep(EventAI instance, ushort eventID, ref EventData data)
            {
            }
        }

        [HarmonyPatch(typeof(AcademicYearAI), "SimulationStep")]
        [HarmonyPrefix]
        public static bool SimulationStep(AcademicYearAI __instance, ushort eventID, ref EventData data)
        {
            SimulationStepBasePatch.SimulationStep(__instance, eventID, ref data);
            var instance = Singleton<DistrictManager>.instance;
            var instance2 = Singleton<BuildingManager>.instance;
            var position = instance2.m_buildings.m_buffer[data.m_building].m_position;
            byte park = instance.GetPark(position);
            if (park != 0)
            {
                ushort mainGate = instance.m_parks.m_buffer[park].m_mainGate;
                if (instance.m_parks.m_buffer[park].m_isMainCampus && mainGate != 0)
                {
                    float yearProgress = __instance.GetYearProgress(eventID, ref data);
                    if (yearProgress > 0.495f)
                    {
                        var properties = Singleton<GuideManager>.instance.m_properties;
                        if (properties is not null)
                        {
                            instance.m_academicYearMidterm.Activate(properties.m_academicYearMidterm, mainGate);
                        }
                    }
                }
            }
            if (data.m_togaPartyStartTime1 > 0f || data.m_togaPartyStartTime2 > 0f)
            {
                float yearProgress = __instance.GetYearProgress(eventID, ref data);
                if (data.m_togaPartyStartTime1 > 0f && data.m_togaPartyStartTime1 <= yearProgress &&
                    (TimeInfo.Now.IsWeekend() || !(TimeInfo.CurrentHour > 6f && TimeInfo.CurrentHour < 22f)))
                {
                    StartTogaParty(__instance, eventID, ref data, DistrictPark.TogaParty.First);
                    data.m_togaPartyStartTime1 = 0f;
                }
                if (data.m_togaPartyStartTime2 > 0f && data.m_togaPartyStartTime2 <= yearProgress &&
                    (TimeInfo.Now.IsWeekend() || !(TimeInfo.CurrentHour > 6f && TimeInfo.CurrentHour < 22f)))
                {
                    StartTogaParty(__instance, eventID, ref data, DistrictPark.TogaParty.Second);
                    data.m_togaPartyStartTime2 = 0f;
                }
            }
            return false;
        }

        
    }
}
