using HarmonyLib;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public static class SeedHelper
    {
        public static string CurrentSeed = "";
        public static bool SeedIsUsed = false;
        public static string Symbols = "";

        public static void InitializeSymbols(TMP_FontAsset font)
        {
            if (font == null || font.characterTable == null) return;

            foreach (TMP_Character item in font.characterTable)
            {
                char c = (char)item.unicode;
                if (!char.IsControl(c) && !Symbols.Contains(c.ToString()))
                {
                    Symbols += c;
                }
            }
        }

        public static string GenerateRandomSeed()
        {
            System.Random random = new System.Random();
            string text = "";
            int num = UnityEngine.Random.Range(1, 50);
            for (int i = 0; i <= num; i++)
            {
                text += Symbols[random.Next(0, Symbols.Length)];
            }
            if (UnityEngine.Random.Range(1, 3) == 2)
            {
                text = "-" + text;
            }
            CurrentSeed = text;
            SeedIsUsed = true;
            return CurrentSeed;
        }

        public static int StringToSeed(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            unchecked
            {
                int hash = 23;
                foreach (char c in str)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        public static void UpdateSeedText(SeedInput input)
        {
            bool useSeed = Traverse.Create(input).Field("useSeed").GetValue<bool>();
            TMP_Text tmp = Traverse.Create(input).Field("tmp").GetValue<TMP_Text>();

            if (tmp != null && tmp.font != null && string.IsNullOrEmpty(Symbols))
            {
                InitializeSymbols(tmp.font);
            }

            if (useSeed && !string.IsNullOrEmpty(CurrentSeed))
                tmp!.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("But_Seed") + CurrentSeed;
            else
                tmp!.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("But_Seed") + Singleton<LocalizationManager>.Instance.GetLocalizedText("But_SeedRandom");
        }
    }

    [HarmonyPatch(typeof(SeedInput))]
    internal static class SeedInputPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        private static void ClearSeed(TMP_Text ___tmp)
        {
            SeedHelper.CurrentSeed = "";
            if (___tmp != null && ___tmp.font != null)
            {
                SeedHelper.InitializeSymbols(___tmp.font);
            }
        }

        [HarmonyPatch("UpdateText")]
        [HarmonyPrefix]
        private static bool UpdateTextPatch(SeedInput __instance)
        {
            SeedHelper.UpdateSeedText(__instance);
            return false;
        }

        [HarmonyPatch("ChangeMode")]
        [HarmonyPrefix]
        private static bool ChangeModePatch(SeedInput __instance)
        {
            bool useSeed = Traverse.Create(__instance).Field("useSeed").GetValue<bool>();
            useSeed = !useSeed;
            Traverse.Create(__instance).Field("useSeed").SetValue(useSeed);
            SeedHelper.UpdateSeedText(__instance);
            return false;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool UpdatePatch(SeedInput __instance)
        {
            TMP_Text tmp = Traverse.Create(__instance).Field("tmp").GetValue<TMP_Text>();
            bool useSeed = Traverse.Create(__instance).Field("useSeed").GetValue<bool>();
            SeedHelper.SeedIsUsed = useSeed;

            SeedHelper.UpdateSeedText(__instance);

            tmp.autoSizeTextContainer = false;
            tmp.autoSizeTextContainer = true;

            if (!Input.anyKeyDown || !useSeed)
                return false;

            if (Input.GetKeyDown(KeyCode.Backspace) && SeedHelper.CurrentSeed.Length > 0)
            {
                SeedHelper.CurrentSeed = SeedHelper.CurrentSeed.Substring(0, SeedHelper.CurrentSeed.Length - 1);
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                SeedHelper.CurrentSeed = "";
            }
            else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.V))
            {
                string clipboard = GUIUtility.systemCopyBuffer;
                if (!string.IsNullOrEmpty(clipboard))
                {
                    SeedHelper.CurrentSeed += new string(clipboard.Where(c => !char.IsControl(c)).ToArray());
                }
            }
            else if (Input.inputString.Length > 0)
            {
                char c = Input.inputString[0];
                if (!char.IsControl(c))
                {
                    SeedHelper.CurrentSeed += c;
                }
            }

            SeedHelper.UpdateSeedText(__instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(ElevatorScreen))]
    internal static class ElevatorScreenSeedPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StyleSeedInElevator(ElevatorScreen __instance, TMP_Text ___seedText)
        {
            if (!SeedHelper.SeedIsUsed)
                SeedHelper.GenerateRandomSeed();

            if (___seedText != null)
            {
                ___seedText.enableWordWrapping = true;
                ___seedText.overflowMode = TextOverflowModes.Overflow;
                ___seedText.alignment = TextAlignmentOptions.MidlineLeft;
                ___seedText.transform.localPosition = new Vector3(67.45f, 94.25f, 0f);
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdateSeedTextInElevator(ElevatorScreen __instance, TMP_Text ___seedText)
        {
            if (___seedText == null) return;

            CoreGameManager instance = Singleton<CoreGameManager>.Instance;
            if (instance != null && instance.sceneObject != null && instance.sceneObject.levelAsset != null && !instance.sceneObject.levelTitle.ToUpper().Contains("PIT"))
            {
                ___seedText.text = "Pre-Made";
            }
            else
            {
                ___seedText.text = SeedHelper.CurrentSeed;
            }
        }
    }

    [HarmonyPatch(typeof(LevelGenerator))]
    internal static class UseSeedPatch
    {
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void UseCustomSeed(LevelGenerator __instance)
        {
            if (!SeedHelper.SeedIsUsed)
                SeedHelper.GenerateRandomSeed();

            int seedNum = SeedHelper.StringToSeed(SeedHelper.CurrentSeed);
            Singleton<CoreGameManager>.Instance.SetSeed(seedNum);
        }
    }

    [HarmonyPatch(typeof(PauseReset))]
    internal static class PauseMenuSeedPatch
    {
        private static void SetSeed()
        {
            if (SeedHelper.SeedIsUsed && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.C))
            {
                GUIUtility.systemCopyBuffer = SeedHelper.CurrentSeed;
            }
        }

        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        private static void ShowSeedInPauseMenu(PauseReset __instance, TMP_Text ___seedText)
        {
            if (!SeedHelper.SeedIsUsed || ___seedText == null) return;

            CoreGameManager instance = Singleton<CoreGameManager>.Instance;
            bool isPreMadeLevel = instance != null && instance.sceneObject != null && instance.sceneObject.levelAsset != null && !instance.sceneObject.levelTitle.ToUpper().Contains("PIT");

            if (isPreMadeLevel)
            {
                ___seedText.gameObject.SetActive(false);
                if (___seedText.transform.parent != null && ___seedText.transform.parent.childCount > 0)
                {
                    ___seedText.transform.parent.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
            {
                ___seedText.gameObject.SetActive(true);
                if (___seedText.transform.parent != null && ___seedText.transform.parent.childCount > 0)
                {
                    ___seedText.transform.parent.GetChild(0).gameObject.SetActive(true);
                }

                ___seedText.text = SeedHelper.CurrentSeed;
                ___seedText.enableWordWrapping = true;
                ___seedText.overflowMode = TextOverflowModes.Overflow;

                if (__instance.gameObject.GetComponent<SeedPauseUpdater>() == null)
                {
                    SeedPauseUpdater updater = __instance.gameObject.AddComponent<SeedPauseUpdater>();
                    updater.action = SetSeed;
                }
            }
        }
    }

    [HarmonyPatch(typeof(CoreGameManager))]
    internal static class FixSaves
    {
        [HarmonyPatch("SaveAndQuit")]
        [HarmonyPostfix]
        private static void SaveSeed()
        {
            if (SeedHelper.SeedIsUsed && !string.IsNullOrEmpty(SeedHelper.CurrentSeed))
            {
                if (Singleton<PlayerFileManager>.Instance != null && Singleton<PlayerFileManager>.Instance.savedGameData != null)
                {
                    Singleton<PlayerFileManager>.Instance.savedGameData.seed = SeedHelper.StringToSeed(SeedHelper.CurrentSeed);
                    Singleton<PlayerFileManager>.Instance.Save();
                }
            }
        }
    }

    internal class SeedPauseUpdater : MonoBehaviour
    {
        public Action? action;
        private void Update()
        {
            action?.Invoke();
        }
    }
}
