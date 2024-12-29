using BepInEx;
using BepInEx.Logging;
using LethalLib.Extras;
using LethalLib.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LethalChungus.Patches
{
    internal static class CustomOutsideObjects
    {
        private const string AssetName = "chungus";

        public static void Init(ManualLogSource logger, PluginInfo info)
        {
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(info.Location), AssetName);

                logger.LogInfo("Loading Asset: " + path);
                AssetBundle assetBundle = AssetBundle.LoadFromFile(path);

                foreach (SpawnableOutsideObjectDef outsideObj in CreateOutsideObjectDefinitions(assetBundle))
                {
                    logger.LogInfo("Loading object: " + outsideObj.spawnableMapObject.spawnableObject.prefabToSpawn.name);
                    MapObjects.RegisterOutsideObject(outsideObj, Levels.LevelTypes.All, (level) => AnimationCurve.Linear(0, 8, 1, 30));
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private static List<SpawnableOutsideObjectDef> CreateOutsideObjectDefinitions(AssetBundle assetBundle)
        {
            return new List<SpawnableOutsideObjectDef>()
            {
                CreateOutsideObjectWithRarityDef(assetBundle.LoadAsset<GameObject>("chungusOutside"))
            };
        }

        private static SpawnableOutsideObjectDef CreateOutsideObjectWithRarityDef(GameObject prefab)
        {
            SpawnableOutsideObjectDef outsideObj = ScriptableObject.CreateInstance<SpawnableOutsideObjectDef>();
            outsideObj.spawnableMapObject = CreateOutsideObjectWithRarity(prefab);
            outsideObj.name = outsideObj.spawnableMapObject.spawnableObject.name;
            return outsideObj;
        }

        private static SpawnableOutsideObjectWithRarity CreateOutsideObjectWithRarity(GameObject prefab)
        {
            return new SpawnableOutsideObjectWithRarity
            {
                spawnableObject = CreateOutsideObject(prefab)
            };
        }

        private static SpawnableOutsideObject CreateOutsideObject(GameObject prefab)
        {
            SpawnableOutsideObject obj = ScriptableObject.CreateInstance<SpawnableOutsideObject>();
            obj.name = prefab.name + "_so";
            obj.objectWidth = 3;
            obj.spawnFacingAwayFromWall = true;
            obj.prefabToSpawn = prefab;
            obj.spawnableFloorTags = new string[] {
                "Grass",
                "Gravel",
                "Snow"
            };
            return obj;
        }

    }
}
