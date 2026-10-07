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

                bool primerOSegundoAtaque = path == ClipPaths[0] || path == ClipPaths[1];
                float porcentajeApertura = primerOSegundoAtaque ? 0.3f : 0.4f;
                float porcentajeCierre = primerOSegundoAtaque ? 0.7f : 0.62f;
                float tiempoApertura = importedClip.length * porcentajeApertura;
                float tiempoCierre = importedClip.length * porcentajeCierre;
                AnimationEvent[] events = clip.events ?? new AnimationEvent[0];
                AnimationEvent eventoApertura = events.FirstOrDefault(item => item.functionName == "AbrirVentanaDeDaño");
                if (eventoApertura == null)
                {
                    events = events.Concat(new[]
                    {
                        new AnimationEvent
                        {
                            time = tiempoApertura,
                            functionName = "AbrirVentanaDeDaño"
                        }
                    }).ToArray();
                    changed = true;
                }
                else if (Mathf.Abs(eventoApertura.time - tiempoApertura) > 0.001f)
                {
                    eventoApertura.time = tiempoApertura;
                    changed = true;
                }

                AnimationEvent eventoCierre = events.FirstOrDefault(item => item.functionName == "CerrarVentanaDeDaño");
                if (eventoCierre == null)
                {
                    events = events.Concat(new[]
                    {
                        new AnimationEvent
                        {
                            time = tiempoCierre,
                            functionName = "CerrarVentanaDeDaño"
                        }
                    }).ToArray();
                    changed = true;
                }
                else if (Mathf.Abs(eventoCierre.time - tiempoCierre) > 0.001f)
                {
                    eventoCierre.time = tiempoCierre;
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