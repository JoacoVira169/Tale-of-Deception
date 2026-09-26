#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class OrcAttackAnimationEventInstaller
{
    private static readonly string[] ClipPaths =
    {
        "Assets/Animations/Base_EnemyAnims/Orc_attack/Standing Melee Attack Backhand.fbx",
        "Assets/Animations/Base_EnemyAnims/Orc_attack/Standing Melee Attack Downward.fbx",
        "Assets/Animations/Base_EnemyAnims/Orc_attack/Standing Melee Combo Attack Ver. 3.fbx"
    };

    static OrcAttackAnimationEventInstaller()
    {
        EditorApplication.delayCall += InstallEvents;
    }

    [MenuItem("Tools/Tale of Deception/Install Orc Axe Animation Events")]
    private static void InstallEventsFromMenu()
    {
        InstallEvents();
    }

    private static void InstallEvents()
    {
        foreach (string path in ClipPaths)
        {
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
            {
                continue;
            }

            ModelImporterClipAnimation[] clips = importer.clipAnimations.Length > 0
                ? importer.clipAnimations
                : importer.defaultClipAnimations;
            AnimationClip[] importedClips = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<AnimationClip>()
                .ToArray();
            bool changed = false;

            foreach (ModelImporterClipAnimation clip in clips)
            {
                AnimationClip importedClip = importedClips.FirstOrDefault(item => item.name == clip.name)
                    ?? importedClips.FirstOrDefault();
                if (importedClip == null || importedClip.length <= 0f)
                {
                    continue;
                }

                AnimationEvent[] events = clip.events ?? new AnimationEvent[0];
                if (!events.Any(item => item.functionName == "AbrirVentanaDeDaño"))
                {
                    events = events.Concat(new[]
                    {
                        new AnimationEvent
                        {
                            time = importedClip.length * 0.4f,
                            functionName = "AbrirVentanaDeDaño"
                        }
                    }).ToArray();
                    changed = true;
                }

                if (!events.Any(item => item.functionName == "CerrarVentanaDeDaño"))
                {
                    events = events.Concat(new[]
                    {
                        new AnimationEvent
                        {
                            time = importedClip.length * 0.62f,
                            functionName = "CerrarVentanaDeDaño"
                        }
                    }).ToArray();
                    changed = true;
                }

                clip.events = events;
            }

            if (changed)
            {
                importer.clipAnimations = clips;
                importer.SaveAndReimport();
                Debug.Log($"Animation Events del hacha instalados en {path}.");
            }
        }
    }
}
#endif