// AcademicYearManager.cs

namespace RealTime.Managers
{
    using RealTime.GameConnection;
    using SkyTools.Tools;

    internal static class AcademicYearManager
    {
        public struct AcademicYearData
        {
            public static bool DidLastYearEnd;
            public static bool DidGraduationStart;
            public static float GraduationStartTime;
            public static uint ActualAcademicYearEndFrame;
        }

        // calculate hours since last year ended
        public static float CalculateHoursSinceLastYearEnded() => (SimulationManager.instance.m_currentFrameIndex - AcademicYearData.ActualAcademicYearEndFrame) * SimulationManager.DAYTIME_FRAME_TO_HOUR;

        // dont start or end academic year if night time or weekend or the hour is not between 9 am and 10 am
        public static bool CanAcademicYearEndorBegin(TimeInfo TimeInfo)
        {
            if (TimeInfo.IsNightTime || TimeInfo.Now.IsWeekend())
            {
                return false;
            }
            if (TimeInfo.CurrentHour < 9f || TimeInfo.CurrentHour > 10f)
            {
                return false;
            }
            return true;
        }
    }
}
