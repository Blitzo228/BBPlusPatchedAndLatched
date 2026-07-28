using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using PatchedAndLatched.Patches;
using PatchedAndLatched.Patches.OldTheTest;

namespace PatchedAndLatched
{
    [BepInPlugin("blitzo.baldiplus.patchedandlatched", "Patched and Latched", "2.0.0")]
    public class PatchedAndLatchedPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool>? CutGrapplingHook;
        public static ConfigEntry<bool>? ColoredActivities;
        public static ConfigEntry<bool>? RunningInRooms;
        public static ConfigEntry<bool>? StaminaOnPoints;
        public static ConfigEntry<bool>? PointsBonus;
        public static ConfigEntry<bool>? ReplaceDietBSODA;
        public static ConfigEntry<bool>? ClassicArtsAndCrafters;
        public static ConfigEntry<bool>? NoPrincipalFacultyKnock;
        public static ConfigEntry<bool>? OldConveyorBelt;
        public static ConfigEntry<bool>? NametagForFieldTrip;
        public static ConfigEntry<bool>? OnlyBaldiEveryFloor;
        public static ConfigEntry<bool>? BootsSnapRope;
        public static ConfigEntry<bool>? StaminaSpeedModifier;
        public static ConfigEntry<bool>? BootsClassicDuration;
        public static ConfigEntry<bool>? NotebookRestoreStamina;
        public static ConfigEntry<bool>? GottaSweepAcceleration;
        public static ConfigEntry<bool>? CustomInventorySlots;
        public static ConfigEntry<int>? InventorySlotCount;
        public static ConfigEntry<bool>? InfiniteSodaMachine;
        public static ConfigEntry<bool>? EnableSeedLetters;
        public static ConfigEntry<bool>? FirstPrizeBreakByBSODA;
        public static ConfigEntry<bool>? InfiniteReach;
        public static ConfigEntry<float>? ReachDistance;
        public static ConfigEntry<bool>? EnableDropItem;
        public static ConfigEntry<bool>? ToggleableDoors;

        public static ConfigEntry<bool>? GrapplingHookBreakWindows;
        public static ConfigEntry<bool>? GrapplingHookOpenDoors;
        public static ConfigEntry<bool>? GrapplingHookPushNPCs;
        public static ConfigEntry<bool>? GrapplingHookHitGum;
        public static ConfigEntry<bool>? GrapplingHookBreakBalder;
        public static ConfigEntry<bool>? GrapplingHookUnlockDoors;
        public static ConfigEntry<bool>? GrapplingHookBreakPlaytime;

        public static ConfigEntry<bool>? FastModeEnabled;
        public static ConfigEntry<bool>? LethalTouchEnabled;
        public static ConfigEntry<bool>? LightsOutEnabled;
        public static ConfigEntry<bool>? AllKnowingPrincipalEnabled;

        public static ConfigEntry<bool>? RandomJumpsEnabled;
        public static ConfigEntry<int>? MinJumps;
        public static ConfigEntry<int>? MaxJumps;
        public static ConfigEntry<bool>? FasterJumpropeEnabled;
        public static ConfigEntry<bool>? BaldiKillsNPCs;
        public static ConfigEntry<bool>? FinalLevelPreEndingEnabled;
        public static ConfigEntry<bool>? AlwaysClosedValves;
        public static ConfigEntry<float>? LockdownDoorSpeedMultiplier;
        public static ConfigEntry<bool>? EnableBaldiPushBack;
        public static ConfigEntry<float>? BaldiPushForce;
        public static ConfigEntry<float>? BaldiPushCooldown;
        public static ConfigEntry<int>? BaldiMaxPushes;
        public static ConfigEntry<bool>? EnableOldTestBehavior;
        public static ConfigEntry<bool>? EnableNewTestFeatures;
        public static ConfigEntry<bool>? OldTestTimeStop;
        public static ConfigEntry<bool>? OldTestFastForward;
        public static ConfigEntry<bool>? OldTestDisappear;
        public static ConfigEntry<bool>? OldTestMovingItems;
        public static ConfigEntry<bool>? EnableMrsPompTimeControl;
        public static ConfigEntry<float>? MrsPompClassTime;
        public static ConfigEntry<bool>? MrsPompRandomizeTime;
        public static ConfigEntry<float>? MrsPompMinTime;
        public static ConfigEntry<float>? MrsPompMaxTime;
        public static ConfigEntry<bool>? EnableYTPSMultiplier;
        public static ConfigEntry<int>? YTPSMultiplier;
        public static ConfigEntry<bool>? EnableCampingItemLimit;
        public static ConfigEntry<float>? CampingItemPickupLimit;
        public static ConfigEntry<bool>? EnableJohnnyBringCount;
        public static ConfigEntry<int>? JohnnyBringItemCount;

        public static ConfigEntry<bool>? EnableMathMachineMultiplication;
        public static ConfigEntry<bool>? EnableMathMachineDivision;
        public static ConfigEntry<bool>? EnableMathMachineExponent;
        public static ConfigEntry<bool>? ReplaceMathMachineCompletely;

        public static ConfigEntry<bool>? SchoolHouseEscape;
        public static ConfigEntry<bool>? NoTransparentMap;
        public static ConfigEntry<bool>? EnableHUDShadows;
        public static ConfigEntry<bool>? EnableStaminaNoLimit;
        public static ConfigEntry<bool>? EnableStaminaText;
        public static ConfigEntry<bool>? EnableStaminaRestText;
        public static ConfigEntry<bool>? EnablePickupDissolve;
        public static ConfigEntry<float>? PickupDissolveDuration;
        public static ConfigEntry<bool>? EnableNotebookSpin;
        public static ConfigEntry<bool>? EnableMysteryRoomMap;
        public static ConfigEntry<bool>? EnableCustomLightRadius;
        public static ConfigEntry<float>? AmbientDarknessLevel;
        public static ConfigEntry<float>? CustomLightRadiusMultiplier;
        public static ConfigEntry<bool>? DisableBananasInCafeteria;


        private void Awake()
        {
            CutGrapplingHook = Config.Bind("Gameplay", "CutGrapplingHook", true, "You can cut the grappling hook with scissors");
            RunningInRooms = Config.Bind("Gameplay", "RunningInRooms", true, "Principal doesn't detention for running in rooms");
            PointsBonus = Config.Bind("Gameplay", "PointsBonus", true, "Every 30 points gives +5 bonus points");
            ReplaceDietBSODA = Config.Bind("Gameplay", "ReplaceDietBSODA", false, "Regular BSODA completely replaces diet BSODA");
            ClassicArtsAndCrafters = Config.Bind("Gameplay", "ClassicArtsAndCrafters", true, "Classic ArtsAndCrafters: no spinning, instant teleport on touch");
            NoPrincipalFacultyKnock = Config.Bind("Gameplay", "NoPrincipalFacultyKnock", false, "Principal doesn't knock on faculty doors, just opens them");
            OldConveyorBelt = Config.Bind("Gameplay", "OldConveyorBelt", false, "Old conveyor belt speed");
            NametagForFieldTrip = Config.Bind("Gameplay", "NametagForFieldTrip", true, "You can use nametag to field trip");
            OnlyBaldiEveryFloor = Config.Bind("Gameplay", "OnlyBaldiEveryFloor", false, "Only Baldi spawns on every floor");
            BootsSnapRope = Config.Bind("Gameplay", "BootsSnapRope", true, "Boots snap the jumprope");
            StaminaSpeedModifier = Config.Bind("Gameplay", "StaminaSpeedModifier", true, "Speed scales with stamina (low stamina = slower, high stamina = faster)");
            BootsClassicDuration = Config.Bind("Gameplay", "BootsClassicDuration", false, "Boots duration is 15 seconds");
            NotebookRestoreStamina = Config.Bind("Gameplay", "NotebookRestoreStamina", true, "Restore full stamina when collect a notebook");
            GottaSweepAcceleration = Config.Bind("Gameplay", "GottaSweepAcceleration", true, "Gotta Sweep starts slow and accelerates over time");
            CustomInventorySlots = Config.Bind("Gameplay", "CustomInventorySlots", false, "Enable custom inventory slot count");
            InventorySlotCount = Config.Bind("Gameplay", "InventorySlotCount", 9, "Number of inventory slots (1-9)");
            InfiniteSodaMachine = Config.Bind("Gameplay", "InfiniteSodaMachine", false, "Vendings machines never run out of uses");
            EnableSeedLetters = Config.Bind("Gameplay", "EnableSeedLetters", true, "Enable letterseed input (A-Z)");
            FirstPrizeBreakByBSODA = Config.Bind("Gameplay", "FirstPrizeBreakByBSODA", true, "BSODA can stun FirstPrize on hit");
            InfiniteReach = Config.Bind("Gameplay", "InfiniteReach", false, "Allows picking up items from any distance");
            ReachDistance = Config.Bind("Gameplay", "ReachDistance", 10000f, "Maximum reach distance for picking up items (10000 = infinite)");
            EnableDropItem = Config.Bind("Gameplay", "EnableDropItem", true, "Drop item with R key");
            ToggleableDoors = Config.Bind("Gameplay", "ToggleableDoors", false, "Allows doors to be toggled open and closed by clicking them (like DFACR)");

            GrapplingHookBreakWindows = Config.Bind("Gameplay", "GrapplingHookBreakWindows", true, "Grappling Hook can break windows");
            GrapplingHookOpenDoors = Config.Bind("Gameplay", "GrapplingHookOpenDoors", true, "Grappling Hook can open doors with clickables");
            GrapplingHookPushNPCs = Config.Bind("Gameplay", "GrapplingHookPushNPCs", true, "Grappling Hook pushes NPCs on hit");
            GrapplingHookHitGum = Config.Bind("Gameplay", "GrapplingHookHitGum", true, "Grappling Hook can hit flying gum");
            GrapplingHookBreakBalder = Config.Bind("Gameplay", "GrapplingHookBreakBalder", true, "Grappling Hook can breakBalder on hit");
            GrapplingHookUnlockDoors = Config.Bind("Gameplay", "GrapplingHookUnlockDoors", true, "Grappling Hook can break locks and open locked doors");

            FastModeEnabled = Config.Bind("FunSettings", "FastMode", false, "Everything moves faster");
            LethalTouchEnabled = Config.Bind("FunSettings", "LethalTouch", false, "Any NPC touching the player kills them instantly");
            LightsOutEnabled = Config.Bind("FunSettings", "LightsOut", false, "Darkness anywhere");
            AllKnowingPrincipalEnabled = Config.Bind("FunSettings", "AllKnowingPrincipal", false, "Principal instantly knows where you are, chases you");

            SchoolHouseEscape = Config.Bind("Visuals", "SchoolHouseEscape", true, "Play SchoolHouse Escape music when all notebooks are collected");
            NoTransparentMap = Config.Bind("Visuals", "NoTransparentMap", true, "Remove transparent from the map");
            ColoredActivities = Config.Bind("Visuals", "ColoredActivities", true, "Colored balloons in activities (balloon bster only)");
            EnableHUDShadows = Config.Bind("Visuals", "EnableHUDShadows", true, "Add shadows to HUD text");
            EnableStaminaNoLimit = Config.Bind("Visuals", "EnableStaminaNoLimit", false, "Remove stamina needle limit, allowing it to go beyond the scale");
            EnableStaminaText = Config.Bind("Visuals", "EnableStaminaText", true, "Show stamina percentage text next to staminometer");
            EnableStaminaRestText = Config.Bind("Visuals", "EnableStaminaRestText", true, "Show 'YOU NEED REST!' text when stamina is empty");
            EnablePickupDissolve = Config.Bind("Visuals", "EnablePickupDissolve", true, "Smooth dissolve effect when picking up items");
            PickupDissolveDuration = Config.Bind("Visuals", "PickupDissolveDuration", 0.5f, "Duration of dissolve effect in seconds");
            EnableNotebookSpin = Config.Bind("Visuals", "EnableNotebookSpin", true, "Enable notebook spinning animation");

            StaminaOnPoints = Config.Bind("Stamina", "StaminaOnPoints", true, "Restore stamina when getting points");

            RandomJumpsEnabled = Config.Bind("Gameplay", "RandomJumpsEnabled", false, "Enable random jump count in Playtime minigame");
            MinJumps = Config.Bind("Gameplay", "MinJumps", 3, "Minimum number of jumps required");
            MaxJumps = Config.Bind("Gameplay", "MaxJumps", 10, "Maximum number of jumps required");
            FasterJumpropeEnabled = Config.Bind("Gameplay", "FasterJumpropeEnabled", false, "Makes jumprope 1.5x faster");
            BaldiKillsNPCs = Config.Bind("Gameplay", "BaldiKillsNPCs", false, "Baldi can kill other NPCs when touching them");
            FinalLevelPreEndingEnabled = Config.Bind("Gameplay", "FinalLevelPreEndingEnabled", true, "On the final level, when breaking the pre last elevator, despawn other NPCs, and Baldi accelerates faster over time");
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
            EnableMrsPompTimeControl = Config.Bind("Gameplay", "EnableMrsPompTimeControl", true, "Override Mrs. Pomp's class arrival time");
            MrsPompClassTime = Config.Bind("Gameplay", "MrsPompClassTime", 300f, "Fixed class time in seconds (default 300 = 5 min)");
            MrsPompRandomizeTime = Config.Bind("Gameplay", "MrsPompRandomizeTime", true, "Randomize class time between Min and Max");
            MrsPompMinTime = Config.Bind("Gameplay", "MrsPompMinTime", 60f, "Minimum random class time in seconds (default 60 = 1 min)");
            MrsPompMaxTime = Config.Bind("Gameplay", "MrsPompMaxTime", 540f, "Maximum random class time in seconds (default 540 = 9 min)");
            EnableYTPSMultiplier = Config.Bind("Gameplay", "EnableYTPSMultiplier", false, "Multiply points from YTPS item");
            YTPSMultiplier = Config.Bind("Gameplay", "YTPSMultiplier", 3, "Multiplier for YTPS points (default 3)");
            EnableCampingItemLimit = Config.Bind("Gameplay", "EnableCampingItemLimit", true, "Override max pickups during camping (set to large number to disable limit)");
            CampingItemPickupLimit = Config.Bind("Gameplay", "CampingItemPickupLimit", 999f, "Max items you can collect during camping before others disappear");
            EnableJohnnyBringCount = Config.Bind("Gameplay", "EnableJohnnyBringCount", true, "Override how many items Johnny brings to lobby");
            JohnnyBringItemCount = Config.Bind("Gameplay", "JohnnyBringItemCount", 999, "Number of items Johnny brings (default 3)");
            EnableMysteryRoomMap = Config.Bind("Visuals", "EnableMysteryRoomMap", true, "Show Mystery Room on map with color changes (gray inactive, dark green active)");
            EnableCustomLightRadius = Config.Bind("Visuals", "EnableCustomLightRadius", true, "Enable the light changes.");
            AmbientDarknessLevel = Config.Bind("Visuals", "AmbientDarknessLevel", 0.1f, "Base light level for unlit tiles (0 = pitch black, 1 = fully lit)");
            CustomLightRadiusMultiplier = Config.Bind("Visuals", "CustomLightRadiusMultiplier", 0.5f, "Multiplier for light source radius. Lower values create more dark spots.");
            DisableBananasInCafeteria = Config.Bind("Gameplay", "DisableBananasInCafeteria", true, "Remove nana peels from Cafeteria if you really hate that");

            EnableMathMachineMultiplication = Config.Bind("MathMachine", "EnableMultiplication", true, "Allow multiplication problems on math machines");
            EnableMathMachineDivision = Config.Bind("MathMachine", "EnableDivision", true, "Allow division problems on math machines");
            EnableMathMachineExponent = Config.Bind("MathMachine", "EnableExponent", true, "Allow exponentiation problems on math machines");
            ReplaceMathMachineCompletely = Config.Bind("MathMachine", "ReplaceCompletely", false, "Replace all problems with multiplication/division/exponentiation (if true, no addition/subtraction)");

            if (CutGrapplingHook.Value)
                Harmony.CreateAndPatchAll(typeof(GrapplingHookCutPatch));

            if (ColoredActivities.Value)
                Harmony.CreateAndPatchAll(typeof(BalloonBusterColorsPatch));

            if (RunningInRooms.Value)
                Harmony.CreateAndPatchAll(typeof(PrincipalPatch));

            if (StaminaOnPoints.Value)
                Harmony.CreateAndPatchAll(typeof(StaminaOnPointsPatch));

            if (PointsBonus.Value)
                Harmony.CreateAndPatchAll(typeof(PointsBonusPatch));

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
                Harmony.CreateAndPatchAll(typeof(PrincipalNoFacultyKnockPatch));

            if (OldConveyorBelt.Value)
                Harmony.CreateAndPatchAll(typeof(ConveyorBeltSpeedPatch));

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

            if (SchoolHouseEscape.Value)
                Harmony.CreateAndPatchAll(typeof(SchoolHouseEscapePatch));

            if (NoTransparentMap.Value)
                Harmony.CreateAndPatchAll(typeof(NoTransparentMapPatch));

            if (BootsSnapRope.Value)
                Harmony.CreateAndPatchAll(typeof(BootsSnapRopePatch));

            if (StaminaSpeedModifier.Value)
                Harmony.CreateAndPatchAll(typeof(StaminaSpeedModifierPatch));

            if (BootsClassicDuration.Value)
                Harmony.CreateAndPatchAll(typeof(BootsClassicDurationPatch));

            if (NotebookRestoreStamina.Value)
                Harmony.CreateAndPatchAll(typeof(NotebookStaminaPatch));

            if (GottaSweepAcceleration.Value)
                Harmony.CreateAndPatchAll(typeof(GottaSweepAccelerationPatch));

            if (CustomInventorySlots.Value)
                Harmony.CreateAndPatchAll(typeof(InventorySlotCountPatch));

            if (InfiniteSodaMachine.Value)
                Harmony.CreateAndPatchAll(typeof(InfiniteSodaMachinePatch));

            if (GrapplingHookBreakWindows.Value || GrapplingHookOpenDoors.Value || GrapplingHookPushNPCs.Value || GrapplingHookHitGum.Value || GrapplingHookUnlockDoors.Value || GrapplingHookBreakBalder.Value)
            {
                Harmony.CreateAndPatchAll(typeof(GrapplingHookPatch));
            }

            if (EnableSeedLetters.Value)
            {
                Harmony.CreateAndPatchAll(typeof(SeedHelper));
                Harmony.CreateAndPatchAll(typeof(SeedInputPatch));
                Harmony.CreateAndPatchAll(typeof(ElevatorScreenSeedPatch));
                Harmony.CreateAndPatchAll(typeof(UseSeedPatch));
                Harmony.CreateAndPatchAll(typeof(PauseMenuSeedPatch));
                Harmony.CreateAndPatchAll(typeof(FixSaves));
            }

            if (FirstPrizeBreakByBSODA.Value)
                Harmony.CreateAndPatchAll(typeof(BSODABreakFirstPrizePatch));

            if (InfiniteReach.Value)
                Harmony.CreateAndPatchAll(typeof(InfiniteReachPatch));

            if (EnableDropItem.Value)
                Harmony.CreateAndPatchAll(typeof(DropItemPatch));

            if (FastModeEnabled.Value)
                Harmony.CreateAndPatchAll(typeof(FastModePatch));

            if (LethalTouchEnabled.Value)
                Harmony.CreateAndPatchAll(typeof(LethalTouchPatch));

            if (LightsOutEnabled.Value)
                Harmony.CreateAndPatchAll(typeof(LightsOutPatch));

            if (AllKnowingPrincipalEnabled.Value)
                Harmony.CreateAndPatchAll(typeof(AllKnowingPrincipalPatch));

            if (RandomJumpsEnabled.Value)
                Harmony.CreateAndPatchAll(typeof(RandomJumpsPatch));

            if (FasterJumpropeEnabled.Value)
                Harmony.CreateAndPatchAll(typeof(FasterJumpropePatch));

            if (BaldiKillsNPCs.Value)
                Harmony.CreateAndPatchAll(typeof(BaldiKillsNPCsPatch));

            if (FinalLevelPreEndingEnabled.Value)
                Harmony.CreateAndPatchAll(typeof(FinalLevelPreEndingPatch));

            if (AlwaysClosedValves.Value)
                Harmony.CreateAndPatchAll(typeof(AlwaysClosedValvesPatch));

            if (LockdownDoorSpeedMultiplier.Value != 1f)
                Harmony.CreateAndPatchAll(typeof(LockdownDoorSpeedPatch));

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

            if (EnableHUDShadows.Value)
                Harmony.CreateAndPatchAll(typeof(HUDShadowPatch));

            if (EnableMrsPompTimeControl.Value)
                Harmony.CreateAndPatchAll(typeof(MrsPompTimePatch));

            if (EnableYTPSMultiplier.Value && YTPSMultiplier.Value != 1)
                Harmony.CreateAndPatchAll(typeof(YTPSPointsPatch));

            if (EnableCampingItemLimit.Value)
                Harmony.CreateAndPatchAll(typeof(CampingItemLimitPatch));

            if (EnableJohnnyBringCount.Value)
                Harmony.CreateAndPatchAll(typeof(JohnnyBringItemCountPatch));

            if (EnableMathMachineMultiplication.Value || EnableMathMachineDivision.Value || EnableMathMachineExponent.Value)
                Harmony.CreateAndPatchAll(typeof(MathMachinePatch));

            if (EnableStaminaNoLimit.Value)
                Harmony.CreateAndPatchAll(typeof(StaminaNoLimitPatch));

            if (EnableStaminaText.Value || EnableStaminaRestText.Value)
                Harmony.CreateAndPatchAll(typeof(StaminaTextPatch));

            if (EnablePickupDissolve.Value)
                Harmony.CreateAndPatchAll(typeof(PickupDissolvePatch));

            if (EnableNotebookSpin.Value)
                Harmony.CreateAndPatchAll(typeof(NotebookSpinPatch));

            if (ToggleableDoors.Value)
                Harmony.CreateAndPatchAll(typeof(ToggleableDoorsPatch));

            if (EnableMysteryRoomMap.Value)
                Harmony.CreateAndPatchAll(typeof(MysteryRoomMapPatch));

            if (EnableCustomLightRadius.Value)
                Harmony.CreateAndPatchAll(typeof(LightGenerationPatch));
            if (DisableBananasInCafeteria.Value)
                Harmony.CreateAndPatchAll(typeof(BananaCafeteriaPatch));
        }
    }
}
