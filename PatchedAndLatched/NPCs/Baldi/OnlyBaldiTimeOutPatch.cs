using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(BaseGameManager))]
    public static class OnlyBaldiTimeOutPatch
    {
        private static bool _timeOutStarted = false;
        private static FieldInfo? _eventsField;
        private static FieldInfo? _ecField;

        [HarmonyPostfix]
        [HarmonyPatch("AllNotebooks")]
        public static void AllNotebooks_Postfix(BaseGameManager __instance)
        {
            if (!PatchedAndLatchedPlugin.OnlyBaldiEveryFloor.Value) return;
            if (_timeOutStarted) return;

            if (_ecField == null)
                _ecField = typeof(BaseGameManager).GetField("ec", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_ecField == null) return;

            EnvironmentController? ec = _ecField.GetValue(__instance) as EnvironmentController;
            if (ec == null) return;

            if (_eventsField == null)
                _eventsField = typeof(EnvironmentController).GetField("events", BindingFlags.NonPublic | BindingFlags.Instance);

            TimeOut? timeOut = null;
            if (_eventsField != null)
            {
                var events = _eventsField.GetValue(ec) as List<RandomEvent>;
                if (events != null)
                {
                    foreach (var randomEvent in events)
                    {
                        if (randomEvent is TimeOut)
                        {
                            timeOut = randomEvent as TimeOut;
                            break;
                        }
                    }
                }
            }

            if (timeOut != null)
            {
                timeOut.Begin();
                _timeOutStarted = true;
            }
            else
            {
                Singleton<MusicManager>.Instance.PlayMidi("TimeOut_MMP_Corrected", loop: true);
                _timeOutStarted = true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Initialize")]
        public static void Initialize_Prefix(BaseGameManager __instance)
        {
            if (!PatchedAndLatchedPlugin.OnlyBaldiEveryFloor.Value) return;
            _timeOutStarted = false;
        }
    }
}