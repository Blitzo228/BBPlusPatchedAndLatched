using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using PatchedAndLatched.Patches;
using PatchedAndLatched.Patches.OldTheTest;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PatchedAndLatched
{
    [BepInDependency("alexbw145.bbplus.rewiredcompat", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("blayms.tbb.baldiplus.cyrillic", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInIncompatibility("blitzo.baldiplus.smallchanges")] //please stop using that
    [BepInPlugin("blitzo.baldiplus.patchedandlatched", "Patched and Latched", "2.2.0")]
    public class PatchedAndLatchedPlugin : BaseUnityPlugin
    {
        public static bool IsRewiredCompatInstalled { get; private set; }
        private static bool inputsRegistered = false;

        public static bool IsCyrillicPlusInstalled { get; private set; }

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
        public static ConfigEntry<bool>? EnableBaldiQuarterReward;

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

        public static ConfigEntry<bool>? TechnoBootsAnimation;
        public static ConfigEntry<bool>? EnableNametagHudAnimation;
        public static ConfigEntry<bool>? EnableDetentionMessage;
        public static ConfigEntry<bool>? BouncyYTPDisplay;
        public static ConfigEntry<bool>? MapTileFade;
        public static ConfigEntry<bool>? EnableQuarterSpawning;
        public static ConfigEntry<float>? QuarterSpawnChance;
        public static ConfigEntry<int>? QuarterMaxPerFloor;
        public static ConfigEntry<bool>? DisableGaugeVisuals;
        public static ConfigEntry<bool>? EnableScissorsCutRuler;
        public static ConfigEntry<bool>? ZeroStaminaEnabled;
        public static ConfigEntry<bool>? EnableMatchMachineGhosting;
        public static ConfigEntry<bool>? DisableStudentSpawns;
        public static ConfigEntry<bool>? EnableTapePlayerReturnSprite;
        public static ConfigEntry<bool>? EnableQuitCursorHide;
        public static ConfigEntry<bool>? StartSpoopModeOnNotebooks;
        public static ConfigEntry<bool>? DoubleStorePrices;


        private void Awake()
        {
            IsRewiredCompatInstalled = Chainloader.PluginInfos.ContainsKey("alexbw145.bbplus.rewiredcompat");
            IsCyrillicPlusInstalled = Chainloader.PluginInfos.ContainsKey("blayms.tbb.baldiplus.cyrillic");

            CutGrapplingHook = Config.Bind("Gameplay", "CutGrapplingHook", true, "You can cut the Grappling Hook with Scissors.");
            RunningInRooms = Config.Bind("Gameplay", "RunningInRooms", false, "The Principal does not give detention for running in rooms.");
            PointsBonus = Config.Bind("Gameplay", "PointsBonus", false, "Every 30 points awards +5 bonus points.");
            ReplaceDietBSODA = Config.Bind("Gameplay", "ReplaceDietBSODA", false, "Regular BSODA completely replaces Diet BSODA.");
            ClassicArtsAndCrafters = Config.Bind("Gameplay", "ClassicArtsAndCrafters", false, "Classic Arts and Crafters: no spinning, instant teleport on touch.");
            NoPrincipalFacultyKnock = Config.Bind("Gameplay", "NoPrincipalFacultyKnock", false, "The Principal does not knock on faculty doors; he just opens them.");
            OldConveyorBelt = Config.Bind("Gameplay", "OldConveyorBelt", false, "Uses the old conveyor belt speed.");
            NametagForFieldTrip = Config.Bind("Gameplay", "NametagForFieldTrip", true, "You can use the Faculty Nametag to enter a field trip.");
            OnlyBaldiEveryFloor = Config.Bind("Gameplay", "OnlyBaldiEveryFloor", false, "Only Baldi spawns on every floor.");
            BootsSnapRope = Config.Bind("Gameplay", "BootsSnapRope", true, "Techno-Boots snap Playtime's jumprope.");
            BootsClassicDuration = Config.Bind("Gameplay", "BootsClassicDuration", false, "Techno-Boots duration is set to 15 seconds.");
            NotebookRestoreStamina = Config.Bind("Gameplay", "NotebookRestoreStamina", true, "Restores full stamina when collecting a notebook.");
            GottaSweepAcceleration = Config.Bind("Gameplay", "GottaSweepAcceleration", true, "Gotta Sweep starts slow and accelerates over time.");
            CustomInventorySlots = Config.Bind("Gameplay", "CustomInventorySlots", false, "Enables a custom inventory slot count.");
            InventorySlotCount = Config.Bind("Gameplay", "InventorySlotCount", 5, "Number of inventory slots (1-9).");
            InfiniteSodaMachine = Config.Bind("Gameplay", "InfiniteSodaMachine", false, "Vending machines never run out of goodness.");
            EnableSeedLetters = Config.Bind("Gameplay", "SeedTweaks", true, "Allows seeds to accept any characters with seeds of any length, as well as display \"Pre-Made\" on pre-made levels.");
            FirstPrizeBreakByBSODA = Config.Bind("Gameplay", "FirstPrizeBreakByBSODA", true, "BSODA can stun First Prize on hit.");
            InfiniteReach = Config.Bind("Gameplay", "InfiniteReach", false, "Allows picking up items from any distance.");
            ReachDistance = Config.Bind("Gameplay", "ReachDistance", 10000f, "Maximum reach distance for picking up items (10000 = infinite).");
            EnableDropItem = Config.Bind("Gameplay", "EnableDropItem", true, "Drop items using the R key.");
            ToggleableDoors = Config.Bind("Gameplay", "ToggleableDoors", false, "Allows doors to be toggled open and closed by clicking them (similar to DFACR).");

            GrapplingHookBreakWindows = Config.Bind("Gameplay", "GrapplingHookBreakWindows", true, "The grappling hook can break windows.");
            GrapplingHookOpenDoors = Config.Bind("Gameplay", "GrapplingHookOpenDoors", true, "The grappling hook can open doors with clickables.");
            GrapplingHookPushNPCs = Config.Bind("Gameplay", "GrapplingHookPushNPCs", true, "The grappling hook pushes NPCs on hit.");
            GrapplingHookHitGum = Config.Bind("Gameplay", "GrapplingHookHitGum", true, "The grappling hook can hit flying gum.");
            GrapplingHookBreakBalder = Config.Bind("Gameplay", "GrapplingHookBreakBalder", true, "The grappling hook can break Baldi on hit.");
            GrapplingHookUnlockDoors = Config.Bind("Gameplay", "GrapplingHookUnlockDoors", true, "The grappling hook can break locks and open locked doors.");

            FastModeEnabled = Config.Bind("FunSettings", "FastMode", false, "Makes everything move faster.");
            LethalTouchEnabled = Config.Bind("FunSettings", "LethalTouch", false, "Any NPC touching the player kills them instantly.");
            LightsOutEnabled = Config.Bind("FunSettings", "LightsOut", false, "Creates darkness everywhere.");
            AllKnowingPrincipalEnabled = Config.Bind("FunSettings", "AllKnowingPrincipal", false, "The Principal instantly knows where you are and chases you.");

            SchoolHouseEscape = Config.Bind("Visuals", "SchoolHouseEscape", true, "Plays the SchoolHouse Escape music when all notebooks are collected.");
            NoTransparentMap = Config.Bind("Visuals", "NoTransparentMap", true, "Removes transparency from the map.");
            ColoredActivities = Config.Bind("Visuals", "ColoredActivities", true, "Colored balloons in activities (Balloon Buster only).");
            EnableHUDShadows = Config.Bind("Visuals", "EnableHUDShadows", true, "Adds shadows to HUD text.");
            EnableStaminaNoLimit = Config.Bind("Visuals", "EnableStaminaNoLimit", false, "Makes your stamina infinite.");
            EnableStaminaText = Config.Bind("Visuals", "EnableStaminaText", true, "Shows stamina percentage text next to the staminometer.");
            EnableStaminaRestText = Config.Bind("Visuals", "EnableStaminaRestText", true, "Shows 'YOU NEED REST!' text when stamina is empty.");
            EnablePickupDissolve = Config.Bind("Visuals", "EnablePickupDissolve", true, "Smooth dissolve effect when picking up items.");
            PickupDissolveDuration = Config.Bind("Visuals", "PickupDissolveDuration", 0.5f, "Duration of the dissolve effect in seconds.");
            EnableNotebookSpin = Config.Bind("Visuals", "EnableNotebookSpin", true, "Enables the notebook spinning animation.");
            EnableMysteryRoomMap = Config.Bind("Visuals", "EnableMysteryRoomMap", true, "Shows the Mystery Room on the map with color changes (gray for inactive, dark green for active).");
            EnableCustomLightRadius = Config.Bind("Visuals", "EnableCustomLightRadius", true, "Enables lighting changes.");
            AmbientDarknessLevel = Config.Bind("Visuals", "AmbientDarknessLevel", 0.1f, "Base light level for unlit tiles (0 = pitch black, 1 = fully lit).");
            CustomLightRadiusMultiplier = Config.Bind("Visuals", "CustomLightRadiusMultiplier", 0.5f, "Multiplier for light source radius. Lower values create more dark spots.");
            TechnoBootsAnimation = Config.Bind("Visuals", "TechnoBootsAnimation", true, "Enables the Techno Boots HUD sprite animation.");
            EnableNametagHudAnimation = Config.Bind("Visuals", "EnableNametagHudAnimation", true, "Displays and animates the nametag sprites at the bottom center of the screen when active.");
            EnableDetentionMessage = Config.Bind("Visuals", "EnableDetentionMessage", true, "Displays a red detention message centered on the screen with remaining seconds when in detention.");
            BouncyYTPDisplay = Config.Bind("Visuals", "BouncyYTPDisplay", true, "Makes the points addition text bounce/animate when scoring points.");
            MapTileFade = Config.Bind("Visuals", "MapTileFade", true, "Fades tiles on the map in/out when they become visible/invisible.");
            StaminaOnPoints = Config.Bind("Stamina", "StaminaOnPoints", true, "Restores stamina when earning points.");
            EnableBaldiQuarterReward = Config.Bind("Gameplay", "EnableBaldiQuarterReward", false, "Baldi praise and give a Quarter after completing an activity (like classic versions)");

            RandomJumpsEnabled = Config.Bind("Gameplay", "RandomJumpsEnabled", false, "Enables a random jump count in the Playtime minigame.");
            MinJumps = Config.Bind("Gameplay", "MinJumps", 3, "Minimum number of jumps required.");
            MaxJumps = Config.Bind("Gameplay", "MaxJumps", 10, "Maximum number of jumps required.");
            FasterJumpropeEnabled = Config.Bind("Gameplay", "FasterJumpropeEnabled", false, "Makes jumprope 1.5x faster.");
            BaldiKillsNPCs = Config.Bind("Gameplay", "BaldiKillsNPCs", false, "Baldi can kill other NPCs when touching them.");
            FinalLevelPreEndingEnabled = Config.Bind("Gameplay", "FinalLevelPreEndingEnabled", true, "On the final level, when breaking the second-to-last elevator, despawn other NPCs and make Baldi accelerate faster over time.");
            AlwaysClosedValves = Config.Bind("Gameplay", "AlwaysClosedValves", true, "Steam valves always start closed.");
            LockdownDoorSpeedMultiplier = Config.Bind("Gameplay", "LockdownDoorSpeedMultiplier", 1f, "Multiplier for lockdown door movement speed (default is 1).");
            EnableBaldiPushBack = Config.Bind("Gameplay", "EnableBaldiPushBack", true, "Pushes Baldi back when he catches you.");
            BaldiPushForce = Config.Bind("Gameplay", "BaldiPushForce", 20f, "Force of the push.");
            BaldiPushCooldown = Config.Bind("Gameplay", "BaldiPushCooldown", 1.5f, "Cooldown duration between pushes.");
            BaldiMaxPushes = Config.Bind("Gameplay", "BaldiMaxPushes", 3, "Maximum number of pushes before Baldi catches you.");
            EnableOldTestBehavior = Config.Bind("Gameplay", "EnableOldTestBehavior", true, "Enables old behavior for The Test.");
            EnableNewTestFeatures = Config.Bind("Gameplay", "EnableNewTestFeatures", false, "Uses new test features (head bobbing, speed scaling).");
            OldTestTimeStop = Config.Bind("Gameplay", "OldTestTimeStop", true, "Stops or slows down time when looking at The Test.");
            OldTestFastForward = Config.Bind("Gameplay", "OldTestFastForward", false, "Fast-forwards time while looking at The Test.");
            OldTestDisappear = Config.Bind("Gameplay", "OldTestDisappear", false, "The Test disappears when not in sight.");
            OldTestMovingItems = Config.Bind("Gameplay", "OldTestMovingItems", true, "Items and entities can move while looking at The Test.");
            EnableMrsPompTimeControl = Config.Bind("Gameplay", "EnableMrsPompTimeControl", true, "Overrides Mrs. Pomp's class arrival time.");
            MrsPompClassTime = Config.Bind("Gameplay", "MrsPompClassTime", 300f, "Fixed class time in seconds (default 300 = 5 minutes).");
            MrsPompRandomizeTime = Config.Bind("Gameplay", "MrsPompRandomizeTime", true, "Randomizes class time between the minimum and maximum limits.");
            MrsPompMinTime = Config.Bind("Gameplay", "MrsPompMinTime", 60f, "Minimum random class time in seconds (default 60 = 1 minute).");
            MrsPompMaxTime = Config.Bind("Gameplay", "MrsPompMaxTime", 540f, "Maximum random class time in seconds (default 540 = 9 minutes).");
            EnableYTPSMultiplier = Config.Bind("Gameplay", "EnableYTPSMultiplier", false, "Multiplies points gained from the YTPS item.");
            YTPSMultiplier = Config.Bind("Gameplay", "YTPSMultiplier", 3, "Multiplier for YTPS points (default 3).");
            EnableCampingItemLimit = Config.Bind("Gameplay", "EnableCampingItemLimit", true, "Overrides the maximum pickup limit during camping (set to a large number to disable the limit).");
            CampingItemPickupLimit = Config.Bind("Gameplay", "CampingItemPickupLimit", 999f, "Maximum items you can collect during camping before others disappear.");
            EnableJohnnyBringCount = Config.Bind("Gameplay", "EnableJohnnyBringCount", true, "Overrides how many items Johnny brings to the lobby.");
            JohnnyBringItemCount = Config.Bind("Gameplay", "JohnnyBringItemCount", 999, "Number of items Johnny brings (default 3).");
            DisableBananasInCafeteria = Config.Bind("Gameplay", "DisableBananasInCafeteria", true, "Removes banana peels from the cafeteria.");

            EnableMathMachineMultiplication = Config.Bind("MathMachine", "EnableMultiplication", true, "Allows multiplication problems on math machines.");
            EnableMathMachineDivision = Config.Bind("MathMachine", "EnableDivision", true, "Allows division problems on math machines.");
            EnableMathMachineExponent = Config.Bind("MathMachine", "EnableExponent", true, "Allows exponentiation problems on math machines.");
            ReplaceMathMachineCompletely = Config.Bind("MathMachine", "ReplaceCompletely", false, "Replaces all problems with multiplication, division, or exponentiation (if true, removes addition and subtraction).");
            EnableQuarterSpawning = Config.Bind("Gameplay", "EnableQuarterSpawning", false, "Allow quarters to spawn randomly in hallways like classic versions.");
            QuarterSpawnChance = Config.Bind("Gameplay", "QuarterSpawnChance", 0.1f, "Chance (0-1) for each hallway tile to spawn a quarter.");
            QuarterMaxPerFloor = Config.Bind("Gameplay", "QuarterMaxPerFloor", 5, "Maximum quarters that can spawn per floor.");
            DisableGaugeVisuals = Config.Bind("Visuals", "DisableGaugeVisuals", false, "Hides the gauges.");
            EnableScissorsCutRuler = Config.Bind("Gameplay", "EnableScissorsCutRuler", false, "Scissors can cut Baldi's ruler for 15 seconds.");
            ZeroStaminaEnabled = Config.Bind("FunSettings", "ZeroStamina", false, "Stamina is always 0 and cannot be restored.");
            EnableMatchMachineGhosting = Config.Bind("Visuals", "EnableMatchMachineGhosting", false, "Make Match Mathines easier.");
            DisableStudentSpawns = Config.Bind("Gameplay", "DisableStudentSpawns", false, "Remove students from spawning and gives 50 ytps per student as compensation.");
            EnableTapePlayerReturnSprite = Config.Bind("Visuals", "EnableTapePlayerReturnSprite", true, "Return TapePlayer sprite to TapePlayerOpen after audio finishes.");
            EnableQuitCursorHide = Config.Bind("Visuals", "EnableQuitCursorHide", true, "Hide cursor when pressing Quit in main menu.");
            StartSpoopModeOnNotebooks = Config.Bind("Gameplay", "StartSpoopModeOnNotebooks", false, "Start spoop mode when the player collects two notebooks.");
            DoubleStorePrices = Config.Bind("FunSettings", "DoubleStorePrices", false, "Double the prices of all items in the store.");


            Harmony.CreateAndPatchAll(typeof(PatchedAndLatchedPlugin));

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
                Harmony.CreateAndPatchAll(typeof(BSODAReplacePatch));

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

            if (GrapplingHookBreakWindows.Value || GrapplingHookOpenDoors.Value || GrapplingHookUnlockDoors.Value)
            {
                Harmony.CreateAndPatchAll(typeof(GrapplingHookPatch));
            }

            if (GrapplingHookBreakBalder.Value)
                Harmony.CreateAndPatchAll(typeof(BalderGrapplingHookPatch));

            if (GrapplingHookHitGum.Value)
                Harmony.CreateAndPatchAll(typeof(GumHookPatch));

            if (GrapplingHookPushNPCs.Value)
                Harmony.CreateAndPatchAll(typeof(GrapplingHookNPCPushPatch));

            if (EnableSeedLetters.Value)
            {
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

            if (TechnoBootsAnimation.Value)
            {
                Harmony.CreateAndPatchAll(typeof(TechnoBootsAnimationPatch));
                Harmony.CreateAndPatchAll(typeof(TechnoBootsFloorSwitchPatch));
            }

            if (EnableNametagHudAnimation.Value)
            {
                Harmony.CreateAndPatchAll(typeof(NametagUsePatch));
                Harmony.CreateAndPatchAll(typeof(NametagHudFloorSwitchPatch));
            }

            if (EnableDetentionMessage.Value)
                Harmony.CreateAndPatchAll(typeof(DetentionMessagePatch));

            if (BouncyYTPDisplay.Value)
            {
                Harmony.CreateAndPatchAll(typeof(BouncyYTPPatch));
                Harmony.CreateAndPatchAll(typeof(BouncyYTPUpdatePatch));
            }

            if (MapTileFade.Value)
                Harmony.CreateAndPatchAll(typeof(MapTileFadeInPatch));

            if (EnableBaldiQuarterReward.Value)
                Harmony.CreateAndPatchAll(typeof(BaldiQuarterRewardPatch));

            if (EnableQuarterSpawning.Value)
                Harmony.CreateAndPatchAll(typeof(QuarterSpawnPatch));

            if (DisableGaugeVisuals.Value)
                Harmony.CreateAndPatchAll(typeof(GaugeHidePatch));

            if (EnableScissorsCutRuler.Value)
                Harmony.CreateAndPatchAll(typeof(ScissorsCutRulerPatch));

            if (ZeroStaminaEnabled.Value)
                Harmony.CreateAndPatchAll(typeof(ZeroStaminaPatch));

            if (EnableMatchMachineGhosting.Value)
                Harmony.CreateAndPatchAll(typeof(MatchMachineGhostingPatch));

            if (DisableStudentSpawns.Value)
                Harmony.CreateAndPatchAll(typeof(StudentSpawnerPatch));

            if (EnableTapePlayerReturnSprite.Value)
                Harmony.CreateAndPatchAll(typeof(TapePlayerPatch));

            if (EnableQuitCursorHide.Value)
                Harmony.CreateAndPatchAll(typeof(MainMenuQuitPatch));

            if (StartSpoopModeOnNotebooks.Value)
                Harmony.CreateAndPatchAll(typeof(NotebookSpoopModePatch));

            if (DoubleStorePrices.Value)
                Harmony.CreateAndPatchAll(typeof(DoubleStorePricesPatch));
        }

        [HarmonyPatch(typeof(NameManager), "Awake")]
        [HarmonyPrefix]
        private static void RegisterInputsOnGameStart()
        {
            if (inputsRegistered) return;
            inputsRegistered = true;

            if (IsRewiredCompatInstalled)
            {
                RegisterRewiredInputs();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void RegisterRewiredInputs()
        {
            BBPRewiredCompat.RewiredPlusManager.CreateNewInput(
                name: "DropItem",
                descriptionName: "Drop Item",
                behaviorID: BBPRewiredCompat.RewiredPlusManager.InputBehaviorID.Snap,
                categoryID: BBPRewiredCompat.RewiredPlusManager.InputMapCategory.Actions,
                key: KeyCode.R
            );
        }
    }
}
