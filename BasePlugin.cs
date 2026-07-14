using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Mono.Cecil;
using PatchedAndLatched.Patches;
using SmallChanges.Patches;
using UnityEngine;
using System.Linq;

namespace PatchedAndLatched
{
    [BepInPlugin("blitzo.baldiplus.patchedandlatched", "Patched and Latched", "1.2.1")]
    public class PatchedAndLatchedPlugin : BaseUnityPlugin
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
        public static ConfigEntry<bool> OnlyBaldiEveryFloor = null!;
        public static ConfigEntry<bool> SchoolHouseEscape = null!;
        public static ConfigEntry<bool> NoTransparentMap = null!;
        public static ConfigEntry<bool> BootsSnapRope = null!;
        public static ConfigEntry<bool> StaminaSpeedModifier = null!;
        public static ConfigEntry<bool> BootsClassicDuration = null!;
        public static ConfigEntry<bool> NotebookRestoreStamina = null!;
        public static ConfigEntry<bool> GottaSweepAcceleration = null!;
        public static ConfigEntry<bool> CustomInventorySlots = null!;
        public static ConfigEntry<int> InventorySlotCount = null!;
        public static ConfigEntry<bool> InfiniteSodaMachine = null!;
        public static ConfigEntry<bool> GrapplingHookBreakWindows = null!;
        public static ConfigEntry<bool> GrapplingHookOpenDoors = null!;
        public static ConfigEntry<bool> GrapplingHookPushNPCs = null!;
        public static ConfigEntry<bool> GrapplingHookHitGum = null!;
        public static ConfigEntry<bool> EnableSeedLetters = null!;
        public static ConfigEntry<bool> FirstPrizeBreakByBSODA = null!;
        public static ConfigEntry<bool> InfiniteReach = null!;
        public static ConfigEntry<float> ReachDistance = null!;
        public static ConfigEntry<bool> EnableDropItem = null!;
        public static ConfigEntry<bool> FastModeEnabled = null!;
        public static ConfigEntry<bool> LethalTouchEnabled = null!;
        public static ConfigEntry<bool> LightsOutEnabled = null!;
        public static ConfigEntry<bool> AllKnowingPrincipalEnabled = null!;
        public static ConfigEntry<bool> RandomJumpsEnabled = null!;
        public static ConfigEntry<int> MinJumps = null!;
        public static ConfigEntry<int> MaxJumps = null!;
        public static ConfigEntry<bool> FasterJumpropeEnabled = null!;
        public static ConfigEntry<bool> BaldiKillsNPCs = null!;
        public static ConfigEntry<bool> FinalLevelPreEndingEnabled = null!;
        public static ConfigEntry<bool> GrapplingHookBreakBalder = null!;
        public static ConfigEntry<bool> AlwaysClosedValves = null!;
        public static ConfigEntry<float> LockdownDoorSpeedMultiplier = null!;
        public static ConfigEntry<bool> GrapplingHookBreakPlaytime = null!;
        private void Awake()
        {
            CutGrapplingHook = Config.Bind("Gameplay", "CutGrapplingHook", true, "You can cut the grappling hook with scissors");
            RunningInRooms = Config.Bind("Gameplay", "RunningInRooms", true, "Principal doesn't detention for running in rooms");
            PointsBonus = Config.Bind("Gameplay", "PointsBonus", true, "Every 30 points gives +5 bonus points");
            ReplaceDietBSODA = Config.Bind("Gameplay", "ReplaceDietBSODA", true, "Regular BSODA completely replaces diet BSODA");
            ClassicArtsAndCrafters = Config.Bind("Gameplay", "ClassicArtsAndCrafters", true, "Classic ArtsAndCrafters: no spinning, instant teleport on touch");
            NoPrincipalFacultyKnock = Config.Bind("Gameplay", "NoPrincipalFacultyKnock", false, "Principal doesn't knock on faculty doors, just opens them");
            OldConveyorBelt = Config.Bind("Gameplay", "OldConveyorBelt", true, "Old conveyor belt speed");
            NametagForFieldTrip = Config.Bind("Gameplay", "NametagForFieldTrip", true, "You can use nametag to field trip");
            OnlyBaldiEveryFloor = Config.Bind("Gameplay", "OnlyBaldiEveryFloor", false, "Only Baldi spawns on every floor");
            SchoolHouseEscape = Config.Bind("Visuals", "SchoolHouseEscape", true, "Play SchoolHouse Escape music when all notebooks are collected");
            NoTransparentMap = Config.Bind("Visuals", "NoTransparentMap", true, "Remove transparent from the map");
            BootsSnapRope = Config.Bind("Gameplay", "BootsSnapRope", true, "Boots snap the jumprope");
            StaminaSpeedModifier = Config.Bind("Gameplay", "StaminaSpeedModifier", true, "Speed scales with stamina (low stamina = slower, high stamina = faster)");
            BootsClassicDuration = Config.Bind("Gameplay", "BootsClassicDuration", true, "Boots duration is 15 seconds");
            NotebookRestoreStamina = Config.Bind("Gameplay", "NotebookRestoreStamina", true, "Restore full stamina when collect a notebook");
            GottaSweepAcceleration = Config.Bind("Gameplay", "GottaSweepAcceleration", true, "Gotta Sweep starts slow and accelerates over time");
            CustomInventorySlots = Config.Bind("Gameplay", "CustomInventorySlots", false, "Enable custom inventory slot count");
            InventorySlotCount = Config.Bind("Gameplay", "InventorySlotCount", 9, "Number of inventory slots (1-9)");
            InfiniteSodaMachine = Config.Bind("Gameplay", "InfiniteSodaMachine", true, "Vendings machines never run out of uses");
            GrapplingHookBreakWindows = Config.Bind("Gameplay", "GrapplingHookBreakWindows", true, "Grappling Hook can break windows");
            GrapplingHookOpenDoors = Config.Bind("Gameplay", "GrapplingHookOpenDoors", true, "Grappling Hook can open doors with clickables");
            GrapplingHookPushNPCs = Config.Bind("Gameplay", "GrapplingHookPushNPCs", true, "Grappling Hook pushes NPCs on hit");
            GrapplingHookHitGum = Config.Bind("Gameplay", "GrapplingHookHitGum", true, "Grappling Hook can hit flying gum");
            EnableSeedLetters = Config.Bind("Gameplay", "EnableSeedLetters", true, "Enable letter-based seed input (A-Z, 0-9)");
            FirstPrizeBreakByBSODA = Config.Bind("Gameplay", "FirstPrizeBreakByBSODA", true, "BSODA can stun FirstPrize on hit");
            InfiniteReach = Config.Bind("Gameplay", "InfiniteReach", false, "Allows picking up items from any distance");
            ReachDistance = Config.Bind("Gameplay", "ReachDistance", 10000f, "Maximum reach distance for picking up items (10000 = infinite)");
            EnableDropItem = Config.Bind("Gameplay", "EnableDropItem", true, "Drop item with R key");
            StaminaOnPoints = Config.Bind("Stamina", "StaminaOnPoints", true, "Restore stamina when getting points");
            FastModeEnabled = Config.Bind("FunSettings", "FastMode", false, "Everything moves faster");
            LethalTouchEnabled = Config.Bind("FunSettings", "LethalTouch", false, "Any NPC touching the player kills them instantly");
            LightsOutEnabled = Config.Bind("FunSettings", "LightsOut", false, "Darkness anywhere");
            AllKnowingPrincipalEnabled = Config.Bind("FunSettings", "AllKnowingPrincipal", false, "Principal instantly knows where you are, chases you");
            ColoredActivities = Config.Bind("Visuals", "ColoredActivities", true, "Colored balloons in activities (makes activities easy)");
            RandomJumpsEnabled = Config.Bind("Gameplay", "RandomJumpsEnabled", false, "Enable random jump count in Playtime minigame");
            MinJumps = Config.Bind("Gameplay", "MinJumps", 3, "Minimum number of jumps required");
            MaxJumps = Config.Bind("Gameplay", "MaxJumps", 10, "Maximum number of jumps required");
            FasterJumpropeEnabled = Config.Bind("Gameplay", "FasterJumpropeEnabled", false, "Makes jumprope 1.5x faster");
            BaldiKillsNPCs = Config.Bind("Gameplay", "BaldiKillsNPCs", false, "Baldi can kill other NPCs when touching them");
            FinalLevelPreEndingEnabled = Config.Bind("Gameplay", "FinalLevelPreEndingEnabled", true, "On the final level, when breaking the pre last elevator, despawn other NPCs, and Baldi accelerates faster over time");
            GrapplingHookBreakBalder = Config.Bind("Gameplay", "GrapplingHookBreakBalder", true, "Grappling Hook can breakBalder on hit");
            AlwaysClosedValves = Config.Bind("Gameplay", "AlwaysClosedValves", true, "Steam valves always start closed");
            LockdownDoorSpeedMultiplier = Config.Bind("Gameplay", "LockdownDoorSpeedMultiplier", 5f, "Multiplier for Lockdown Door movement speed (default 1)");
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
            if (NoPrincipalFacultyKnock.Value) Harmony.CreateAndPatchAll(typeof(PrincipalNoFacultyKnockPatch));
            if (OldConveyorBelt.Value) Harmony.CreateAndPatchAll(typeof(ConveyorBeltSpeedPatch));
            if (NametagForFieldTrip.Value)
            {
                Harmony.CreateAndPatchAll(typeof(Patch_StartFieldTrip));
                Harmony.CreateAndPatchAll(typeof(Patch_FieldTripStartMinigame));
                Harmony.CreateAndPatchAll(typeof(Patch_EndMinigame));
            }
            if (OnlyBaldiEveryFloor.Value)
            {
                Harmony.CreateAndPatchAll(typeof(OnlyBaldiEveryFloorPatch));
                Harmony.CreateAndPatchAll(typeof(AddNpcsFromPreviousLevelsPatch));
                Harmony.CreateAndPatchAll(typeof(EnvironmentControllerPatch));
                Harmony.CreateAndPatchAll(typeof(OnlyBaldiTimeOutPatch));
            }
            if (SchoolHouseEscape.Value) Harmony.CreateAndPatchAll(typeof(SchoolHouseEscapePatch));
            if (NoTransparentMap.Value) Harmony.CreateAndPatchAll(typeof(NoTransparentMapPatch));
            if (BootsSnapRope.Value) Harmony.CreateAndPatchAll(typeof(BootsSnapRopePatch));
            if (StaminaSpeedModifier.Value) Harmony.CreateAndPatchAll(typeof(StaminaSpeedModifierPatch));
            if (BootsClassicDuration.Value) Harmony.CreateAndPatchAll(typeof(BootsClassicDurationPatch));
            if (NotebookRestoreStamina.Value) Harmony.CreateAndPatchAll(typeof(NotebookStaminaPatch));
            if (GottaSweepAcceleration.Value) Harmony.CreateAndPatchAll(typeof(GottaSweepAccelerationPatch));
            if (CustomInventorySlots.Value) Harmony.CreateAndPatchAll(typeof(InventorySlotCountPatch));
            if (InfiniteSodaMachine.Value) Harmony.CreateAndPatchAll(typeof(InfiniteSodaMachinePatch));
            if (GrapplingHookBreakWindows.Value || GrapplingHookOpenDoors.Value || GrapplingHookPushNPCs.Value || GrapplingHookHitGum.Value)
            {
                Harmony.CreateAndPatchAll(typeof(GrapplingHookPatch));
            }
            if (EnableSeedLetters.Value)
            {
                Harmony.CreateAndPatchAll(typeof(SeedHelper));
                Harmony.CreateAndPatchAll(typeof(SeedInputPatch));
                Harmony.CreateAndPatchAll(typeof(ElevatorScreenSeedPatch));
                Harmony.CreateAndPatchAll(typeof(UseSeedPatch));
            }
            if (FirstPrizeBreakByBSODA.Value) Harmony.CreateAndPatchAll(typeof(BSODABreakFirstPrizePatch));
            if (InfiniteReach.Value) Harmony.CreateAndPatchAll(typeof(InfiniteReachPatch));
            if (EnableDropItem.Value) Harmony.CreateAndPatchAll(typeof(DropItemPatch));
            if (FastModeEnabled.Value) Harmony.CreateAndPatchAll(typeof(FastModePatch));
            if (LethalTouchEnabled.Value) Harmony.CreateAndPatchAll(typeof(LethalTouchPatch));
            if (LightsOutEnabled.Value) Harmony.CreateAndPatchAll(typeof(LightsOutPatch));
            if (AllKnowingPrincipalEnabled.Value) Harmony.CreateAndPatchAll(typeof(AllKnowingPrincipalPatch));
            if (RandomJumpsEnabled.Value) Harmony.CreateAndPatchAll(typeof(RandomJumpsPatch));
            if (FasterJumpropeEnabled.Value) Harmony.CreateAndPatchAll(typeof(FasterJumpropePatch));
            if (BaldiKillsNPCs.Value) Harmony.CreateAndPatchAll(typeof(BaldiKillsNPCsPatch));
            if (FinalLevelPreEndingEnabled.Value) Harmony.CreateAndPatchAll(typeof(FinalLevelPreEndingPatch));
            if (GrapplingHookBreakBalder.Value) Harmony.CreateAndPatchAll(typeof(GrapplingHookBalderPatch));
            if (AlwaysClosedValves.Value) Harmony.CreateAndPatchAll(typeof(AlwaysClosedValvesPatch));
            if (LockdownDoorSpeedMultiplier.Value != 1f) Harmony.CreateAndPatchAll(typeof(LockdownDoorSpeedPatch));
        }
    }
}