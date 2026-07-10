using HarmonyLib;
using UnityEngine;
using System.Reflection;

[HarmonyPatch(typeof(ITM_Scissors))]
public static class GrapplingHookCutPatch
{
    private static FieldInfo? _audSnipField;
    private static FieldInfo? _hookPmField;
    private static FieldInfo? _hookMoveModField;

    private static SoundObject? GetAudSnip(ITM_Scissors scissors)
    {
        if (_audSnipField == null)
            _audSnipField = typeof(ITM_Scissors).GetField("audSnip", BindingFlags.NonPublic | BindingFlags.Instance);
        return _audSnipField?.GetValue(scissors) as SoundObject;
    }

    private static PlayerManager? GetHookOwner(ITM_GrapplingHook hook)
    {
        if (_hookPmField == null)
            _hookPmField = typeof(ITM_GrapplingHook).GetField("pm", BindingFlags.NonPublic | BindingFlags.Instance);
        return _hookPmField?.GetValue(hook) as PlayerManager;
    }

    private static MovementModifier? GetMoveMod(ITM_GrapplingHook hook)
    {
        if (_hookMoveModField == null)
            _hookMoveModField = typeof(ITM_GrapplingHook).GetField("moveMod", BindingFlags.NonPublic | BindingFlags.Instance);
        return _hookMoveModField?.GetValue(hook) as MovementModifier;
    }

    [HarmonyPostfix]
    [HarmonyPatch("Use")]
    public static void Use_Postfix(ITM_Scissors __instance, PlayerManager pm, ref bool __result)
    {
        if (__result) return;

        foreach (ITM_GrapplingHook hook in Object.FindObjectsOfType<ITM_GrapplingHook>())
        {
            if (hook == null || GetHookOwner(hook) != pm) continue;

            MovementModifier? moveMod = GetMoveMod(hook);
            if (moveMod != null && pm.Am != null)
            {
                pm.Am.moveMods.Remove(moveMod);
            }

            Object.Destroy(hook.gameObject);

            SoundObject? audSnip = GetAudSnip(__instance);
            if (audSnip != null)
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(audSnip);

            __result = true;
            break;
        }
    }
}