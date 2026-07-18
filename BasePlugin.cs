using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Mono.Cecil;
using PatchedAndLatched.Patches;
using PatchedAndLatched.Patches.OldTheTest;
using SmallChanges.Patches;
using System.Linq;
using UnityEngine;

namespace PatchedAndLatched
{
    [BepInPlugin("blitzo.baldiplus.patchedandlatched", "Patched and Latched", "1.3.1")]
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
        public static ConfigEntry<bool> EnableBaldiPushBack = null!;
        public static ConfigEntry<float> BaldiPushForce = null!;
        public static ConfigEntry<float> BaldiPushCooldown = null!;
        public static ConfigEntry<int> BaldiMaxPushes = null!;
        public static ConfigEntry<bool> EnableOldTestBehavior = null!;
        public static ConfigEntry<bool> EnableNewTestFeatures = null!;
        public static ConfigEntry<bool> OldTestTimeStop = null!;
        public static ConfigEntry<bool> OldTestFastForward = null!;
        public static ConfigEntry<bool> OldTestDisappear = null!;
        public static ConfigEntry<bool> OldTestMovingItems = null!;
        public static ConfigEntry<bool> EnableHUDShadows = null!;
        public static ConfigEntry<bool> EnableMrsPompTimeControl = null!;
        public static ConfigEntry<float> MrsPompClassTime = null!;
        public static ConfigEntry<bool> MrsPompRandomizeTime = null!;
        public static ConfigEntry<float> MrsPompMinTime = null!;
        public static ConfigEntry<float> MrsPompMaxTime = null!;
        public static ConfigEntry<bool> EnableYTPSMultiplier = null!;
        public static ConfigEntry<int> YTPSMultiplier = null!;
        public static ConfigEntry<bool> EnableCampingItemLimit = null!;
        public static ConfigEntry<float> CampingItemPickupLimit = null!;
        public static ConfigEntry<bool> EnableJohnnyBringCount = null!;
        public static ConfigEntry<int> JohnnyBringItemCount = null!;
        public static ConfigEntry<bool> EnableMathMachineMultiplication = null!;
        public static ConfigEntry<bool> EnableMathMachineDivision = null!;
        public static ConfigEntry<bool> ReplaceMathMachineCompletely = null!;
        public static ConfigEntry<bool> EnableStaminaNoLimit = null!;
        public static ConfigEntry<bool> EnableStaminaText = null!;
        public static ConfigEntry<bool> EnablePickupDissolve = null!;
        public static ConfigEntry<float> PickupDissolveDuration = null!;
        public static ConfigEntry<bool> EnableNotebookSpin = null!;
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
            EnableBaldiPushBack = Config.Bind("Gameplay", "EnableBaldiPushBack", true, "Push Baldi back on catch");
            BaldiPushForce = Config.Bind("Gameplay", "BaldiPushForce", 20f, "Force of push");
            BaldiPushCooldown = Config.Bind("Gameplay", "BaldiPushCooldown", 1.5f, "Cooldown between pushes");
            BaldiMaxPushes = Config.Bind("Gameplay", "BaldiMaxPushes", 3, "Max pushes before Baldi catches you");
            EnableOldTestBehavior = Config.Bind("Gameplay", "EnableOldTestBehavior", true, "Enable old behavior for The Test");
            EnableNewTestFeatures = Config.Bind("Gameplay", "EnableNewTestFeatures", false, "Use new test features (head bobbing, speed scaling)");
            OldTestTimeStop = Config.Bind("Gameplay", "OldTestTimeStop", true, "Time stop or slow down when looking at The Test");
            OldTestFastForward = Config.Bind("Gameplay", "OldTestFastForward", false, "Fast forward time while looking at The Test");
            OldTestDisappear = Config.Bind("Gameplay", "OldTestDisappear", false, "The Test disappears when not in sight");
            OldTestMovingItems = Config.Bind("Gameplay", "OldTestMovingItems", true, "Items and entities can move while looking at The Test");
            EnableHUDShadows = Config.Bind("Visuals", "EnableHUDShadows", true, "Add shadows to HUD text");
            EnableMrsPompTimeControl = Config.Bind("Gameplay", "EnableMrsPompTimeControl", true, "Override Mrs. Pomp's class arrival time");
            MrsPompClassTime = Config.Bind("Gameplay", "MrsPompClassTime", 300f, "Fixed class time in seconds (default 300 = 5 min)");
            MrsPompRandomizeTime = Config.Bind("Gameplay", "MrsPompRandomizeTime", true, "Randomize class time between Min and Max");
            MrsPompMinTime = Config.Bind("Gameplay", "MrsPompMinTime", 60f, "Minimum random class time in seconds (default 60 = 1 min)");
            MrsPompMaxTime = Config.Bind("Gameplay", "MrsPompMaxTime", 540f, "Maximum random class time in seconds (default 540 = 9 min)");
            EnableYTPSMultiplier = Config.Bind("Gameplay", "EnableYTPSMultiplier", true, "Multiply points from YTPS item");
            YTPSMultiplier = Config.Bind("Gameplay", "YTPSMultiplier", 3, "Multiplier for YTPS points (default 3)");
            EnableCampingItemLimit = Config.Bind("Gameplay", "EnableCampingItemLimit", true, "Override max pickups during camping (set to large number to disable limit)");
            CampingItemPickupLimit = Config.Bind("Gameplay", "CampingItemPickupLimit", 999f, "Max items you can collect during camping before others disappear");
            EnableJohnnyBringCount = Config.Bind("Gameplay", "EnableJohnnyBringCount", true, "Override how many items Johnny brings to lobby");
            JohnnyBringItemCount = Config.Bind("Gameplay", "JohnnyBringItemCount", 999, "Number of items Johnny brings (default 3)");
            EnableMathMachineMultiplication = Config.Bind("MathMachine", "EnableMultiplication", true, "Allow multiplication problems on math machines");
            EnableMathMachineDivision = Config.Bind("MathMachine", "EnableDivision", true, "Allow division problems on math machines");
            ReplaceMathMachineCompletely = Config.Bind("MathMachine", "ReplaceCompletely", false, "Replace all problems with multiplication/division (if true, no addition/subtraction)");
            EnableStaminaNoLimit = Config.Bind("Visuals", "EnableStaminaNoLimit", true, "Remove stamina needle limit, allowing it to go beyond the scale");
            EnableStaminaText = Config.Bind("Visuals", "EnableStaminaText", true, "Show stamina percentage text next to staminometer");
            EnablePickupDissolve = Config.Bind("Visuals", "EnablePickupDissolve", true, "Smooth dissolve effect when picking up items");
            PickupDissolveDuration = Config.Bind("Visuals", "PickupDissolveDuration", 0.5f, "Duration of dissolve effect in seconds");
            EnableNotebookSpin = Config.Bind("Visuals", "EnableNotebookSpin", true, "Enable notebook spinning animation");


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

            if (EnableBaldiPushBack.Value)
                Harmony.CreateAndPatchAll(typeof(BaldiPushPatch));
            if (EnableOldTestBehavior.Value)
            {
                Harmony.CreateAndPatchAll(typeof(TheTestActivatePatch));
                Harmony.CreateAndPatchAll(typeof(TheTestBlindPatch));
                Harmony.CreateAndPatchAll(typeof(TheTestFleePlayerPatch));
                Harmony.CreateAndPatchAll(typeof(TheTestFleeUpdateHeadPositionPatch));
                Harmony.CreateAndPatchAll(typeof(TheTestFreezePatch));
                Harmony.CreateAndPatchAll(typeof(TheTestInitializePatch));
                Harmony.CreateAndPatchAll(typeof(TheTestRespawnPatch));
                Harmony.CreateAndPatchAll(typeof(TheTestVirtualUpdatePatch));
            }
            if (EnableHUDShadows.Value) Harmony.CreateAndPatchAll(typeof(HUDShadowPatch));
            if (EnableMrsPompTimeControl.Value) Harmony.CreateAndPatchAll(typeof(MrsPompTimePatch));
            if (EnableYTPSMultiplier.Value && YTPSMultiplier.Value != 1) Harmony.CreateAndPatchAll(typeof(YTPSPointsPatch));
            if (EnableCampingItemLimit.Value) Harmony.CreateAndPatchAll(typeof(CampingItemLimitPatch));

            if (EnableJohnnyBringCount.Value) Harmony.CreateAndPatchAll(typeof(JohnnyBringItemCountPatch));
            if (EnableMathMachineMultiplication.Value || EnableMathMachineDivision.Value) Harmony.CreateAndPatchAll(typeof(MathMachinePatch));
            if (EnableStaminaNoLimit.Value) Harmony.CreateAndPatchAll(typeof(StaminaNoLimitPatch));
            if (EnableStaminaText.Value) Harmony.CreateAndPatchAll(typeof(StaminaTextPatch));
            if (EnablePickupDissolve.Value) Harmony.CreateAndPatchAll(typeof(PickupDissolvePatch));
            if (EnableNotebookSpin.Value) Harmony.CreateAndPatchAll(typeof(NotebookSpinPatch));
        }
    }
}
