using HarmonyLib;
using PatchedAndLatched;
using PatchedAndLatched.Patches.OldTheTest;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches.OldTheTest
{

    [HarmonyPatch(typeof(LookAtGuy), "Activate")]
    internal static class TheTestActivatePatch
    {
        private static void Prefix(LookAtGuy __instance, Transform ___headTransform)
        {
            __instance.StartCoroutine(HeadRise(___headTransform));
        }

        private static System.Collections.IEnumerator HeadRise(Transform head)
        {
            float y = head.localPosition.y;
            while (y < 4.2f)
            {
                y += Time.deltaTime * 0.35f;
                head.localPosition = new Vector3(head.localPosition.x, y, head.localPosition.z);
                yield return null;
            }
        }
    }


    [HarmonyPatch(typeof(LookAtGuy), "Blind")]
    internal static class TheTestBlindPatch
    {
        private static bool Prefix(LookAtGuy __instance, TimeScaleModifier ___timeScale, bool ___fleeing, QuickExplosion ___explosionPrefab, Sprite ___crumbledSprite, Transform ___billboardedTransform, Transform ___headTransform, AnimatedSpriteRotator ___spriteRotator, SpriteRenderer ___sprite, AudioManager ___blindAudMan, SoundObject ___audBlindStart, SoundObject ___audBlindLoop, HudGauge ___gauge, Sprite ___gaugeSprite, float ___fogTime)
        {
            if (!PatchedAndLatchedPlugin.EnableOldTestBehavior.Value)
                return true; 

            __instance.FreezeNPCs(false);
            __instance.Navigator.maxSpeed = 0f;

            var ogLayerField = AccessTools.Field(typeof(LookAtGuy), "ogLayer");
            if (ogLayerField != null)
                ogLayerField.SetValue(__instance, __instance.gameObject.layer);

            __instance.gameObject.layer = 20;

            ___blindAudMan.QueueAudio(___audBlindStart);
            ___blindAudMan.QueueAudio(___audBlindLoop);
            ___blindAudMan.SetLoop(true);

            ___gauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager.ActivateNewGauge(___gaugeSprite, ___fogTime);

            __instance.behaviorStateMachine.ChangeState(new LookAt_OldBlinding(__instance, ___headTransform, ___gauge, ___fogTime));

            var fleeSpeedField = AccessTools.Field(typeof(LookAtGuy), "fleeSpeed");
            if (fleeSpeedField != null)
                fleeSpeedField.SetValue(__instance, 0f);

            if (!___fleeing)
            {
                __instance.ec.AddFog(__instance.fog);
                ___spriteRotator.enabled = false;
                ___sprite.enabled = false;
            }
            else
            {
                var exp = Object.Instantiate(___explosionPrefab, ___billboardedTransform);
                exp.transform.localPosition += Vector3.forward * 0.015f;
                ___spriteRotator.enabled = false;
                ___sprite.sprite = ___crumbledSprite;
                ___headTransform.gameObject.SetActive(false);

                ___timeScale.npcTimeScale = 2.25f;
                ___timeScale.environmentTimeScale = 2.25f;
                __instance.ec.AddTimeScale(___timeScale);
            }

            return false;
        }
    }


    [HarmonyPatch(typeof(LookAtGuy), "FleePlayer")]
    internal static class TheTestFleePlayerPatch
    {
        private static bool Prefix(LookAtGuy __instance, ref PlayerManager player, int ___currentSpeedLevel, float[] ___speedLevels, float ___moveSpeed)
        {
            if (!PatchedAndLatchedPlugin.EnableOldTestBehavior.Value)
                return true;

            __instance.behaviorStateMachine.CurrentNavigationState.priority = 0;
            __instance.behaviorStateMachine.ChangeNavigationState(new NavigationState_TargetPlayer(__instance, 127, player.transform.position, true));

            bool useNew = PatchedAndLatchedPlugin.EnableNewTestFeatures.Value;
            float targetSpeed = useNew ? ___speedLevels[2] : ___speedLevels[___speedLevels.Length - 1];

            if (___currentSpeedLevel >= (useNew ? 2 : ___speedLevels.Length) && !useNew)
            {
                __instance.transform.position = player.transform.position;
            }

            if (___currentSpeedLevel < (useNew ? 2 : ___speedLevels.Length))
            {
                __instance.Navigator.SetSpeed(___speedLevels[___currentSpeedLevel]);
                var moveSpeedField = AccessTools.Field(typeof(LookAtGuy), "moveSpeed");
                if (moveSpeedField != null)
                    moveSpeedField.SetValue(__instance, ___speedLevels[___currentSpeedLevel]);
            }

            return false;
        }
    }


    [HarmonyPatch(typeof(LookAtGuy), "UpdateHeadPosition")]
    internal static class TheTestFleeUpdateHeadPositionPatch
    {
        private static bool Prefix(LookAtGuy __instance, int ___currentSpeedLevel)
        {
            if (!PatchedAndLatchedPlugin.EnableOldTestBehavior.Value)
                return true;

            __instance.behaviorStateMachine.CurrentNavigationState.priority = 0;
            __instance.behaviorStateMachine.ChangeNavigationState(new NavigationState_DoNothing(__instance, 127, true));

            var speedLevelField = AccessTools.Field(typeof(LookAtGuy), "currentSpeedLevel");
            if (speedLevelField != null)
                speedLevelField.SetValue(__instance, ___currentSpeedLevel + 1);

            __instance.Navigator.maxSpeed = 0f;
            return false;
        }
    }


    [HarmonyPatch(typeof(LookAtGuy), "FreezeNPCs")]
    internal static class TheTestFreezePatch
    {
        private static bool Prefix(ref bool freeze, LookAtGuy __instance, AudioManager ___audMan, AudioManager ___rumbleAudMan, TimeScaleModifier ___timeScale, SoundObject ___audLoop, SoundObject ___audSighted)
        {
            if (!PatchedAndLatchedPlugin.EnableOldTestBehavior.Value)
                return true;

            var freezingField = AccessTools.Field(typeof(LookAtGuy), "freezing");

            if (freeze)
            {
                if (freezingField != null && !(bool)freezingField.GetValue(__instance))
                {
                    __instance.ec.AddTimeScale(___timeScale);
                    if (!PatchedAndLatchedPlugin.OldTestMovingItems.Value)
                    {
                        foreach (var bob in Object.FindObjectsOfType<PickupBobValue>())
                            if (bob.speed != 0f)
                                bob.speed = 0f;
                    }
                    freezingField.SetValue(__instance, true);
                    ___audMan.SetLoop(true);
                    ___audMan.maintainLoop = true;
                    ___audMan.QueueAudio(___audLoop);
                    ___rumbleAudMan.PlaySingle(___audSighted);
                    __instance.ec.FlickerLights(true);
                }
            }
            else
            {
                if (freezingField != null && (bool)freezingField.GetValue(__instance))
                {
                    __instance.ec.RemoveTimeScale(___timeScale);
                    if (!PatchedAndLatchedPlugin.OldTestMovingItems.Value)
                    {
                        foreach (var bob in Object.FindObjectsOfType<PickupBobValue>())
                            if (bob.speed != 5f)
                                bob.speed = 5f;
                    }
                    freezingField.SetValue(__instance, false);
                    ___audMan.FlushQueue(true);
                    __instance.ec.FlickerLights(false);
                }
            }
            return false;
        }
    }


    [HarmonyPatch(typeof(LookAtGuy), "Initialize")]
    internal static class TheTestInitializePatch
    {
        private static void Postfix(LookAtGuy __instance, Transform ___headTransform, TimeScaleModifier ___timeScale)
        {
            if (!PatchedAndLatchedPlugin.EnableOldTestBehavior.Value)
                return;

            __instance.Navigator.Initialize(__instance.ec);
            __instance.behaviorStateMachine.ChangeState(new LookAt_OldInactive(__instance, ___headTransform));

            var fleeSpeedField = AccessTools.Field(typeof(LookAtGuy), "fleeSpeed");
            if (fleeSpeedField != null)
                fleeSpeedField.SetValue(__instance, 0f);

            __instance.ec.RemoveTimeScale(___timeScale);

            var speedLevelField = AccessTools.Field(typeof(LookAtGuy), "currentSpeedLevel");
            if (speedLevelField != null)
                speedLevelField.SetValue(__instance, 0);

            ___timeScale.npcTimeScale = PatchedAndLatchedPlugin.OldTestTimeStop.Value ? 0f : 0.35f;
            ___timeScale.environmentTimeScale = PatchedAndLatchedPlugin.OldTestMovingItems.Value ? 1f : 0f;
        }
    }


    [HarmonyPatch(typeof(LookAtGuy), "Respawn")]
    internal static class TheTestRespawnPatch
    {
        private static bool Prefix(LookAtGuy __instance, TimeScaleModifier ___timeScale, AnimatedSpriteRotator ___spriteRotator, SpriteRenderer ___sprite, Transform ___headTransform, AudioManager ___blindAudMan)
        {
            if (!PatchedAndLatchedPlugin.EnableOldTestBehavior.Value)
                return true;

            var hall = __instance.ec.mainHall;
            var cell = hall.cells[Random.Range(0, hall.cells.Count)];
            __instance.transform.position = cell.FloorWorldPosition + Vector3.up * 5f;

            ___spriteRotator.enabled = true;
            ___sprite.enabled = true;
            ___headTransform.localPosition = new Vector3(___headTransform.localPosition.x, 3.3f, ___headTransform.localPosition.z);

            var speedLevelField = AccessTools.Field(typeof(LookAtGuy), "currentSpeedLevel");
            if (speedLevelField != null)
                speedLevelField.SetValue(__instance, 2);

            __instance.ec.RemoveFog(__instance.fog);
            ___blindAudMan.FlushQueue(true);

            var ogLayerField = AccessTools.Field(typeof(LookAtGuy), "ogLayer");
            if (ogLayerField != null)
                __instance.gameObject.layer = (int)ogLayerField.GetValue(__instance);

            __instance.behaviorStateMachine.ChangeState(new LookAt_OldInactive(__instance, ___headTransform));
            __instance.ec.RemoveTimeScale(___timeScale);

            ___timeScale.npcTimeScale = PatchedAndLatchedPlugin.OldTestTimeStop.Value ? 0f : 0.35f;
            ___timeScale.environmentTimeScale = 1f;
            ___headTransform.gameObject.SetActive(true);

            return false;
        }
    }


    [HarmonyPatch(typeof(LookAtGuy), "VirtualUpdate")]
    internal static class TheTestVirtualUpdatePatch
    {
        private static bool Prefix()
        {
            return !PatchedAndLatchedPlugin.EnableOldTestBehavior.Value;
        }
    }
}