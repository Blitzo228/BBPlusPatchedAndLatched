using HarmonyLib;
using PatchedAndLatched;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    internal static class CafeteriaRestorer
    {
        private static Transform? _fluorescentLightPrefab;
        private static Texture2D? _oldCeilingTexture;
        private static bool _assetsResolved;
        private static bool _assetsPatched;

        private static readonly string[] HangingLightNameParts = new string[5] { "CordedHanging", "HangingLight", "StandardHanging", "HangingLightFan", "CeilingFan" };
        private static readonly string[] HighCeilingNameParts = new string[3] { "SuspendedCeiling", "FactoryCeiling", "Factory_Ceiling" };

        internal static void EnsureAssetsResolved()
        {
            if (_assetsResolved) return;

            _fluorescentLightPrefab = FindPrefabByName("FluorescentLight");
            _oldCeilingTexture = FindTextureByName("CeilingNoLight") ?? FindTextureByName("ElCeiling") ?? FindTextureByName("Ceiling_Texture");

            _assetsResolved = true;
        }

        internal static void PatchRoomAssets()
        {
            if (!PatchedAndLatchedPlugin.EnableOldCafeteriaCeiling.Value) return;

            EnsureAssetsResolved();
            if (_assetsPatched) return;

            int count = 0;
            foreach (var asset in Resources.FindObjectsOfTypeAll<RoomAsset>())
            {
                if (IsCafeteriaAsset(asset) && PatchRoomAsset(asset))
                    count++;
            }
            _assetsPatched = true;
        }

        internal static bool IsCafeteriaRoom(RoomController room)
        {
            if (room == null) return false;
            return room.name.StartsWith("Cafeteria", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsCafeteriaAsset(RoomAsset asset)
        {
            if (asset == null) return false;
            return asset.name.StartsWith("Cafeteria", StringComparison.OrdinalIgnoreCase);
        }

        internal static void FixLoadedRoom(RoomController room)
        {
            if (!PatchedAndLatchedPlugin.EnableOldCafeteriaCeiling.Value) return;
            if (!IsCafeteriaRoom(room)) return;

            EnsureAssetsResolved();

            if (_fluorescentLightPrefab != null)
                room.lightPre = _fluorescentLightPrefab;

            if (_oldCeilingTexture != null && room.ec != null)
            {
                room.ceilTex = _oldCeilingTexture;
                room.GenerateTextureAtlas();
            }

            Transform root = room.objectObject != null ? room.objectObject.transform : room.transform;
            var toRemove = new List<Transform>();

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child == root) continue;
                string name = child.name;
                if (ShouldRemoveObject(name))
                    toRemove.Add(child);
                else if (ShouldLowerCeiling(name, child))
                    LowerCeilingPiece(child);
            }

            foreach (var t in toRemove)
            {
                if (t != null)
                    UnityEngine.Object.Destroy(t.gameObject);
            }

            ReplaceCellLights(room);
            RefreshRoomTileMaterials(room);
        }

        private static bool PatchRoomAsset(RoomAsset asset)
        {
            bool changed = false;

            if (_fluorescentLightPrefab != null && asset.lightPre != _fluorescentLightPrefab)
            {
                asset.lightPre = _fluorescentLightPrefab;
                changed = true;
            }

            if (_oldCeilingTexture != null && asset.ceilTex != _oldCeilingTexture)
            {
                asset.ceilTex = _oldCeilingTexture;
                changed = true;
            }

            if (asset.basicObjects.RemoveAll(obj => obj.prefab != null && ShouldRemoveObject(obj.prefab.name)) > 0)
                changed = true;

            return changed;
        }

        private static bool ShouldRemoveObject(string objectName)
        {
            foreach (var part in HangingLightNameParts)
                if (objectName.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }

        private static bool ShouldLowerCeiling(string objectName, Transform transform)
        {
            if (objectName.IndexOf("Fluorescent", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;

            foreach (var part in HighCeilingNameParts)
                if (objectName.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

            if (objectName.Equals("Ceiling", StringComparison.OrdinalIgnoreCase) && transform.position.y > 11f)
                return true;

            return false;
        }

        private static void LowerCeilingPiece(Transform ceiling)
        {
            Vector3 pos = ceiling.position;
            if (pos.y > 11f)
                ceiling.position = new Vector3(pos.x, 10f, pos.z);

            Vector3 scale = ceiling.localScale;
            if (scale.y > 1.25f)
                ceiling.localScale = new Vector3(scale.x, 1f, scale.z);
        }

        private static void ReplaceCellLights(RoomController room)
        {
            if (_fluorescentLightPrefab == null) return;

            foreach (Cell cell in room.cells)
            {
                if (cell != null && cell.hasLight)
                {
                    Transform light = FindLightTransform(cell);
                    if (light != null && light.name.IndexOf("Fluorescent", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        Vector3 pos = light.position;
                        Quaternion rot = light.rotation;
                        Transform parent = light.parent;
                        UnityEngine.Object.Destroy(light.gameObject);

                        Transform newLight = UnityEngine.Object.Instantiate(_fluorescentLightPrefab, pos, rot, parent);
                        newLight.name = _fluorescentLightPrefab.name;
                        cell.AssignLightController(newLight);
                    }
                }
            }
        }

        private static Transform FindLightTransform(Cell cell)
        {
            if (cell.TileTransform == null) return null;

            foreach (Transform child in cell.TileTransform.GetComponentsInChildren<Transform>(true))
            {
                if (ShouldRemoveObject(child.name) || child.name.IndexOf("Light", StringComparison.OrdinalIgnoreCase) >= 0)
                    return child;
            }
            return null;
        }

        private static void RefreshRoomTileMaterials(RoomController room)
        {
            if (room.baseMat == null) return;
            foreach (Cell cell in room.cells)
                if (cell != null)
                    cell.SetBase(room.baseMat);
        }

        private static Transform FindPrefabByName(string name)
        {
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
                if (go != null && go.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return go.transform;
            return null;
        }

        private static Texture2D FindTextureByName(string name)
        {
            return Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(tex => tex != null && tex.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    [HarmonyPatch(typeof(RoomFunctionContainer), "OnGenerationFinished")]
    internal static class RoomFunctionContainerPatch
    {
        private static void Postfix(RoomFunctionContainer __instance)
        {
            if (PatchedAndLatchedPlugin.EnableOldCafeteriaCeiling.Value)
            {
                var room = __instance.GetComponentInParent<RoomController>();
                if (room != null)
                    CafeteriaRestorer.FixLoadedRoom(room);
            }
        }
    }

    [HarmonyPatch(typeof(LevelBuilder), "LoadRoom", new Type[] { typeof(RoomAsset), typeof(IntVector2), typeof(IntVector2), typeof(Direction), typeof(bool), typeof(Texture2D), typeof(Texture2D), typeof(Texture2D) })]
    internal static class LevelBuilderLoadRoomOverridePatch
    {
        private static void Postfix(RoomController __result, RoomAsset asset)
        {
            if (PatchedAndLatchedPlugin.EnableOldCafeteriaCeiling.Value && __result != null && CafeteriaRestorer.IsCafeteriaAsset(asset))
                CafeteriaRestorer.FixLoadedRoom(__result);
        }
    }

    [HarmonyPatch(typeof(LevelBuilder), "LoadRoom", new Type[] { typeof(RoomAsset), typeof(IntVector2), typeof(IntVector2), typeof(Direction), typeof(bool) })]
    internal static class LevelBuilderLoadRoomPatch
    {
        private static void Postfix(RoomController __result, RoomAsset asset)
        {
            if (PatchedAndLatchedPlugin.EnableOldCafeteriaCeiling.Value && __result != null && CafeteriaRestorer.IsCafeteriaAsset(asset))
                CafeteriaRestorer.FixLoadedRoom(__result);
        }
    }

    [HarmonyPatch(typeof(LevelBuilder), "StartGenerate")]
    internal static class LevelBuilderStartGeneratePatch
    {
        private static void Prefix()
        {
            CafeteriaRestorer.PatchRoomAssets();
        }
    }
}