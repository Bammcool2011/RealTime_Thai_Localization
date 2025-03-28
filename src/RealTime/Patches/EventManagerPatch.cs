// EventManagerPatch.cs

namespace RealTime.Patches
{
    using ColossalFramework;
    using HarmonyLib;
    using RealTime.CustomAI;
    using RealTime.GameConnection;
    using RealTime.Managers;

    [HarmonyPatch]
    internal static class EventManagerPatch
    {
        public static RealTimeBuildingAI RealTimeBuildingAI { get; set; }

        public static TimeInfo TimeInfo { get; set; }

        [HarmonyPatch(typeof(EventManager), "CreateEvent")]
        [HarmonyPrefix]
        public static bool CreateEvent(EventManager __instance, out ushort eventIndex, ushort building, EventInfo info, ref bool __result)
        {
            if(info.GetAI() is AcademicYearAI)
            {
                var buildingManager = Singleton<BuildingManager>.instance;
                var eventData = default(EventData);
                eventData.m_flags = EventData.Flags.Created;
                eventData.m_startFrame = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                eventData.m_ticketPrice = (ushort)info.m_eventAI.m_ticketPrice;
                eventData.m_securityBudget = (ushort)info.m_eventAI.m_securityBudget;
                if (building != 0)
                {
                    eventData.m_building = building;
                    eventData.m_nextBuildingEvent = buildingManager.m_buildings.m_buffer[building].m_eventIndex;
                }
                eventData.Info = info;
                if (__instance.m_events.m_size < 256)
                {
                    eventIndex = (ushort)__instance.m_events.m_size;

                    bool can_start_new_year = false;
                    if (eventIndex != 0)
                    {
                        if (!RealTimeBuildingAI.IsBuildingWorking(building))
                        {
                            __result = false;
                            return false;
                        }
                        if (!AcademicYearManager.CanAcademicYearEndorBegin(TimeInfo))
                        {
                            __result = false;
                            return false;
                        }

                        var academicYearData = AcademicYearManager.GetAcademicYearData(building);

                        if(academicYearData.IsFirstAcademicYear)
                        {
                            can_start_new_year = true;
                            academicYearData.IsFirstAcademicYear = false;
                            AcademicYearManager.SetAcademicYearData(building, academicYearData);
                        }
                        else
                        {
                            float hours_since_last_year_ended = AcademicYearManager.CalculateHoursSinceLastYearEnded(building);

                            if (hours_since_last_year_ended >= 24f)
                            {
                                can_start_new_year = true;
                                academicYearData.DidLastYearEnd = false;
                            }
                            else
                            {
                                academicYearData.DidLastYearEnd = true;
                                academicYearData.IsFirstAcademicYear = false;
                            }
                            AcademicYearManager.SetAcademicYearData(building, academicYearData);
                        }
                    }
                    else
                    {
                        can_start_new_year = true;
                    }
                    
                    if (can_start_new_year)
                    {
                        __instance.m_events.Add(eventData);
                        __instance.m_eventCount++;
                        info.m_eventAI.CreateEvent(eventIndex, ref __instance.m_events.m_buffer[eventIndex]);
                        buildingManager.m_buildings.m_buffer[building].m_eventIndex = eventIndex;
                        __result = true;
                        return false;
                    }
                }
                eventIndex = 0;
                __result = false;
                return false;
            }
            eventIndex = 0;
            return true;
        }
    }
}
