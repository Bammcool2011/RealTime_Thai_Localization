// PrisonerAIPatch.cs

namespace RealTime.Patches
{
    using ColossalFramework;
    using HarmonyLib;
    using RealTime.Localization;
    using SkyTools.Localization;

    [HarmonyPatch]
    public static class PrisonerAIPatch
    {
        /// <summary>Gets or sets the mod localization.</summary>
        public static ILocalizationProvider localizationProvider { get; set; }

        [HarmonyPatch(typeof(PrisonerAI), "GetLocalizedStatus",
            [typeof(ushort), typeof(CitizenInstance), typeof(InstanceID)],
            [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Out])]
        [HarmonyPostfix]
        public static void Postfix1(PrisonerAI __instance, ushort instanceID, ref CitizenInstance data, out InstanceID target, ref string __result)
        {
            ushort targetBuilding = data.m_targetBuilding;
            if (targetBuilding != 0)
            {
                target = InstanceID.Empty;
                target.Building = targetBuilding;
                __result = localizationProvider.Translate(TranslationKeys.ServingTimeAt);
            }
            else
            {
                target = InstanceID.Empty;
            }
        }

        [HarmonyPatch(typeof(PrisonerAI), "GetLocalizedStatus",
            [typeof(uint), typeof(Citizen), typeof(InstanceID)],
            [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Out])]
        [HarmonyPostfix]
        public static void Postfix2(PrisonerAI __instance, uint citizenID, ref Citizen data, out InstanceID target, ref string __result)
        {
            var instance = Singleton<CitizenManager>.instance;
            ushort instance2 = data.m_instance;
            if (instance2 != 0)
            {
                Postfix1(__instance, instance2, ref instance.m_instances.m_buffer[instance2], out target, ref __result);
            }
            else if (data.m_visitBuilding != 0)
            {
                target = InstanceID.Empty;
                target.Building = data.m_visitBuilding;
                __result = localizationProvider.Translate(TranslationKeys.ServingTimeAt);
            }
            else
            {
                target = InstanceID.Empty;
                __result = ColossalFramework.Globalization.Locale.Get("CITIZEN_STATUS_CONFUSED");
            }
        }
    }
}
