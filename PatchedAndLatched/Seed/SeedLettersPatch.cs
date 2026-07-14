using HarmonyLib;
using PatchedAndLatched;
using System;
using TMPro;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public static class SeedHelper
    {
        public static string CurrentSeed = "";
        public static bool SeedIsUsed = false;
        public static string AllowedSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-";

        public static string GenerateRandomSeed()
        {
            int length = UnityEngine.Random.Range(4, 11);
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
                result[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
            CurrentSeed = new string(result);
            SeedIsUsed = true;
            return CurrentSeed;
        }

        public static int FromBase36(string base36)
        {
            long value = 0;
            base36 = base36.ToUpper();
            int len = base36.Length - 1;
            for (int i = len; i >= 0; i--)
            {
                char c = base36[i];
                int digit = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(c);
                if (digit == -1) digit = 0;
                value += digit * (long)Math.Pow(36, len - i);
            }
            if (value > int.MaxValue) value %= int.MaxValue;
            return (int)value;
        }

        public static void UpdateSeedText(SeedInput input)
        {
            bool useSeed = Traverse.Create(input).Field("useSeed").GetValue<bool>();
            TMP_Text tmp = Traverse.Create(input).Field("tmp").GetValue<TMP_Text>();
            if (useSeed && !string.IsNullOrEmpty(CurrentSeed))
                tmp.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("But_Seed") + CurrentSeed;
            else
                tmp.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("But_Seed") + Singleton<LocalizationManager>.Instance.GetLocalizedText("But_SeedRandom");
        }
    }

    [HarmonyPatch(typeof(SeedInput))]
    internal static class SeedInputPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        private static void ClearSeed(TMP_Text ___tmp)
        {
            if (!PatchedAndLatchedPlugin.EnableSeedLetters.Value) return;
            SeedHelper.CurrentSeed = "";
        }

        [HarmonyPatch("UpdateText")]
        [HarmonyPrefix]
        private static bool UpdateTextPatch(SeedInput __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableSeedLetters.Value) return true;
            SeedHelper.UpdateSeedText(__instance);
            return false;
        }

        [HarmonyPatch("ChangeMode")]
        [HarmonyPrefix]
        private static bool ChangeModePatch(SeedInput __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableSeedLetters.Value) return true;
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
            if (!PatchedAndLatchedPlugin.EnableSeedLetters.Value) return true;

            TMP_Text tmp = Traverse.Create(__instance).Field("tmp").GetValue<TMP_Text>();
            bool useSeed = Traverse.Create(__instance).Field("useSeed").GetValue<bool>();
            SeedHelper.SeedIsUsed = useSeed;

            SeedHelper.UpdateSeedText(__instance);
            tmp.autoSizeTextContainer = false;
            tmp.autoSizeTextContainer = true;

            if (!Input.anyKeyDown || !useSeed)
                return false;

            // Обработка ввода
            if (Input.GetKeyDown(KeyCode.Backspace) && SeedHelper.CurrentSeed.Length > 0)
            {
                SeedHelper.CurrentSeed = SeedHelper.CurrentSeed.Substring(0, SeedHelper.CurrentSeed.Length - 1);
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                SeedHelper.CurrentSeed = "";
            }
            else if (Input.inputString.Length > 0)
            {
                char c = Input.inputString[0];
                if (SeedHelper.AllowedSymbols.Contains(c.ToString()))
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
        private static void ShowSeedInElevator(Elevator __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableSeedLetters.Value) return;
            if (!SeedHelper.SeedIsUsed)
                SeedHelper.GenerateRandomSeed();

            TMP_Text seedText = Traverse.Create(__instance).Field("seedText").GetValue<TMP_Text>();
            if (seedText != null)
            {
                seedText.autoSizeTextContainer = true;
                seedText.alignment = TextAlignmentOptions.MidlineLeft;
                seedText.transform.localPosition = new Vector3(67.45f, 94.25f, 0f);
                seedText.text = SeedHelper.CurrentSeed;
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
            if (!PatchedAndLatchedPlugin.EnableSeedLetters.Value) return;
            if (!SeedHelper.SeedIsUsed)
                SeedHelper.GenerateRandomSeed();

            int seedNum = SeedHelper.FromBase36(SeedHelper.CurrentSeed);
            Singleton<CoreGameManager>.Instance.SetSeed(seedNum);
        }
    }
}