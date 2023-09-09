// <copyright file="ResidentAIPatch.cs" company="dymanoid">Copyright (c) dymanoid. All rights reserved.</copyright>

namespace RealTime.Patches
{
    using System;
    using HarmonyLib;
    using RealTime.CustomAI;
    using SkyTools.Tools;
    using RealTime.GameConnection;
    using static RealTime.GameConnection.HumanAIConnectionBase<ResidentAI, Citizen>;
    using static RealTime.GameConnection.ResidentAIConnection<ResidentAI, Citizen>;
    using RealTime.Core;
    using static MessageInfo;

    /// <summary>
    /// A static class that provides the patch objects and the game connection objects for the resident AI .
    /// </summary>
    [HarmonyPatch]
    internal static class ResidentAIPatch
    {
        /// <summary>Gets or sets the custom AI object for resident citizens.</summary>
        public static RealTimeResidentAI<ResidentAI, Citizen> RealTimeAI { get; set; }

        /// <summary>Creates a game connection object for the resident AI class.</summary>
        /// <returns>A new <see cref="ResidentAIConnection{ResidentAI, Citizen}"/> object.</returns>
        public static ResidentAIConnection<ResidentAI, Citizen> GetResidentAIConnection()
        {
            try
            {
                var doRandomMove = AccessTools.MethodDelegate<DoRandomMoveDelegate>(AccessTools.Method(typeof(ResidentAI), "DoRandomMove"));

                var findEvacuationPlace = AccessTools.MethodDelegate<FindEvacuationPlaceDelegate>(AccessTools.Method(typeof(HumanAI), "FindEvacuationPlace"));

                var findHospital = AccessTools.MethodDelegate<FindHospitalDelegate>(AccessTools.Method(typeof(ResidentAI), "FindHospital"));

                var findVisitPlace = AccessTools.MethodDelegate<FindVisitPlaceDelegate>(AccessTools.Method(typeof(HumanAI), "FindVisitPlace"));

                var getEntertainmentReason = AccessTools.MethodDelegate<GetEntertainmentReasonDelegate>(AccessTools.Method(typeof(ResidentAI), "GetEntertainmentReason"));

                var getEvacuationReason = AccessTools.MethodDelegate<GetEvacuationReasonDelegate>(AccessTools.Method(typeof(ResidentAI), "GetEvacuationReason"));

                var getShoppingReason = AccessTools.MethodDelegate<GetShoppingReasonDelegate>(AccessTools.Method(typeof(ResidentAI), "GetShoppingReason"));

                var startMoving = AccessTools.MethodDelegate<StartMovingDelegate>(AccessTools.Method(typeof(HumanAI), "StartMoving", new Type[] { typeof(uint), typeof(Citizen).MakeByRefType(), typeof(ushort), typeof(ushort)}));

                var startMovingWithOffer = AccessTools.MethodDelegate<StartMovingWithOfferDelegate>(AccessTools.Method(typeof(HumanAI), "StartMoving", new Type[] { typeof(uint), typeof(Citizen).MakeByRefType(), typeof(ushort), typeof(TransferManager.TransferOffer)}));

                var attemptAutodidact = AccessTools.MethodDelegate<AttemptAutodidactDelegate>(AccessTools.Method(typeof(ResidentAI), "AttemptAutodidact"));

                return new ResidentAIConnection<ResidentAI, Citizen>(
                    doRandomMove,
                    findEvacuationPlace,
                    findHospital,
                    findVisitPlace,
                    getEntertainmentReason,
                    getEvacuationReason,
                    getShoppingReason,
                    startMoving,
                    startMovingWithOffer,
                    attemptAutodidact);
            }
            catch (Exception e)
            {
                Log.Error("The 'Real Time' mod failed to create a delegate for type 'ResidentAI', no method patching for the class: " + e);
                return null;
            }
        }

        [HarmonyPatch]
        private sealed class ResidentAI_UpdateLocation
        {
            [HarmonyPatch(typeof(ResidentAI), "UpdateLocation")]
            [HarmonyPrefix]
            private static bool Prefix(ResidentAI __instance, uint citizenID, ref Citizen data)
            {
                RealTimeAI.UpdateLocation(__instance, citizenID, ref data);
                return false;
            }
        }

        [HarmonyPatch]
        private sealed class HumanAI_ArriveAtTarget
        {
            [HarmonyPatch(typeof(HumanAI), "ArriveAtTarget")]
            [HarmonyPostfix]
            private static void Postfix(ushort instanceID, ref CitizenInstance citizenData, bool __result)
            {
                if (__result && citizenData.m_citizen != 0)
                {
                    RealTimeAI.RegisterCitizenArrival(citizenData.m_citizen);
                }
            }
        }

        [HarmonyPatch]
        private sealed class ResidentAI_UpdateAge
        {
            [HarmonyPatch(typeof(ResidentAI), "UpdateAge")]
            [HarmonyPrefix]
            private static bool Prefix(ref bool __result)
            {
                if(!RealTimeCore.ApplyCitizenPatch)
                {
                    return true;
                }

                if (RealTimeAI.CanCitizensGrowUp)
                {
                    return true;
                }

                __result = false;
                return false;
            }
        }

        [HarmonyPatch]
        private sealed class ResidentAI_CanMakeBabies
        {
            [HarmonyPatch(typeof(ResidentAI), "CanMakeBabies")]
            [HarmonyPrefix]
            private static bool Prefix(uint citizenID, ref Citizen data, ref bool __result)
            {
                if(!RealTimeCore.ApplyCitizenPatch)
                {
                    return true;
                }

                __result = RealTimeAI.CanMakeBabies(citizenID, ref data);
                return false;
            }
        }

        [HarmonyPatch]
        private sealed class HumanAI_StartMoving
        {
            [HarmonyPatch(typeof(HumanAI), "StartMoving",
                new Type[] { typeof(uint), typeof(Citizen), typeof(ushort), typeof(ushort) },
                new ArgumentType[] {ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal})]
            [HarmonyPostfix]
            private static void Postfix(uint citizenID, bool __result)
            {
                if (__result && citizenID != 0)
                {
                    RealTimeAI.RegisterCitizenDeparture(citizenID);
                }
            }
        }

        [HarmonyPatch]
        private sealed class ResidentAI_SimulationStep
        {
            [HarmonyPatch(typeof(ResidentAI), "SimulationStep",
                new Type[] { typeof(ushort), typeof(CitizenInstance), typeof(CitizenInstance.Frame), typeof(bool) },
                new ArgumentType[] {ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Normal})]
            [HarmonyPostfix]
            private static void Postfix(ResidentAI __instance, ushort instanceID, ref CitizenInstance citizenData)
            {
                if (instanceID == 0)
                {
                    return;
                }

                if ((citizenData.m_flags & (CitizenInstance.Flags.WaitingTaxi | CitizenInstance.Flags.WaitingTransport)) != 0)
                {
                    RealTimeAI.ProcessWaitingForTransport(__instance, citizenData.m_citizen, instanceID);
                }
            }
        }
    }
}