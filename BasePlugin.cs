using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using SmallChanges.Patches;

namespace SmallChanges
{
    [BepInPlugin("blitzo.baldiplus.smallchanges", "Small Changes", "1.0.0")]
    public class SmallChangesPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> CutGrapplingHook = null!;
        public static ConfigEntry<bool> ColoredActivities = null!;
        public static ConfigEntry<bool> RunningInRooms = null!;
        public static ConfigEntry<bool> StaminaOnPoints = null!;
        public static ConfigEntry<bool> PointsBonus = null!;
        public static ConfigEntry<bool> ReplaceDietBSODA = null!;
        public static ConfigEntry<bool> ClassicArtsAndCrafters = null!;
        public static ConfigEntry<bool> NoPrincipalFacultyKnock = null!;
        public static ConfigEntry<bool> OldConveyorBelt = null!;
        public static ConfigEntry<bool> NametagForFieldTrip = null!;

        private void Awake()
        {
            CutGrapplingHook = Config.Bind("Gameplay", "CutGrapplingHook", true, "You can cut the grappling hook with scissors");
            RunningInRooms = Config.Bind("Gameplay", "RunningInRooms", true, "Principal doesn't detention for running in rooms");
            PointsBonus = Config.Bind("Gameplay", "PointsBonus", true, "Every 30 points gives +5 bonus points");
            ReplaceDietBSODA = Config.Bind("Gameplay", "ReplaceDietBSODA", true, "Regular BSODA completely replaces diet BSODA");
            ClassicArtsAndCrafters = Config.Bind("Gameplay", "ClassicArtsAndCrafters", true, "ArtsAndCrafters no longer spinning, instant teleport on touch");
            NoPrincipalFacultyKnock = Config.Bind("Gameplay", "NoPrincipalFacultyKnock", true, "Principal doesn't knock on faculty doors, just opens them");
            OldConveyorBelt = Config.Bind("Gameplay", "OldConveyorBelt", true, "Old conveyor belt speed");
            NametagForFieldTrip = Config.Bind("Gameplay", "NametagForFieldTrip", true, "You can use nametag to field trip");

            StaminaOnPoints = Config.Bind("Stamina", "StaminaOnPoints", true, "Restore stamina when getting points (1 point = 1%)");
            ColoredActivities = Config.Bind("Visuals", "ColoredActivities", true, "Colored balloons in activities (makes activities easy)");

            if (CutGrapplingHook.Value) Harmony.CreateAndPatchAll(typeof(GrapplingHookCutPatch));
            if (ColoredActivities.Value)
            {
                Harmony.CreateAndPatchAll(typeof(MatchActivityColorsPatch));
                Harmony.CreateAndPatchAll(typeof(BalloonBusterColorsPatch));
            }
            if (RunningInRooms.Value) Harmony.CreateAndPatchAll(typeof(PrincipalPatch));
            if (StaminaOnPoints.Value) Harmony.CreateAndPatchAll(typeof(StaminaOnPointsPatch));
            if (PointsBonus.Value) Harmony.CreateAndPatchAll(typeof(PointsBonusPatch));
            if (ReplaceDietBSODA.Value)
            {
                Harmony.CreateAndPatchAll(typeof(BSODAReplacePatch.ItemManagerPatch));
                Harmony.CreateAndPatchAll(typeof(BSODAReplacePatch.PlayerFileManagerPatch));
            }
            if (ClassicArtsAndCrafters.Value)
            {
                Harmony.CreateAndPatchAll(typeof(ArtsAndCraftersChasingPatch));
                Harmony.CreateAndPatchAll(typeof(ArtsAndCraftersTeleportingPatch));
                Harmony.CreateAndPatchAll(typeof(ArtsAndCraftersPatch));
                Harmony.CreateAndPatchAll(typeof(ArtsAndCraftersReadyPatch));
            }
            if (NoPrincipalFacultyKnock.Value)
            {
                Harmony.CreateAndPatchAll(typeof(PrincipalNoFacultyKnockPatch));
            }
            if (OldConveyorBelt.Value)
            {
                Harmony.CreateAndPatchAll(typeof(ConveyorBeltSpeedPatch));
            }
            if (NametagForFieldTrip.Value)
            {
                Harmony.CreateAndPatchAll(typeof(Patch_StartFieldTrip));
                Harmony.CreateAndPatchAll(typeof(Patch_FieldTripStartMinigame));
                Harmony.CreateAndPatchAll(typeof(Patch_EndMinigame));
            }
        }
    }
}