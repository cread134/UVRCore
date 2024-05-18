using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Service.AudioManagement {
    public class AudioObjectCreatorTool
    {
        [MenuItem("Assets/Create/GameSound_Selected", priority = 1)]
        public static void CreateGameSound()
        {
            var clips = GetSelectedClips();
            if (clips.Count == 0)
            {
                return;
            }
            var path = AssetDatabase.GetAssetPath(clips.First());
            var name = Path.GetFileNameWithoutExtension(path);
            var directory = Path.GetDirectoryName(path);
            var usePath = Path.Combine(directory, $"{name}_object.asset");
            Debug.Log($"Path: {usePath}");

            var createdInstance = ScriptableObject.CreateInstance<GameSound>();
            AssetDatabase.CreateAsset(createdInstance, usePath);

            if (clips.Count > 0)
            {
                createdInstance.AudioClips = clips.ToArray();
            }

            EditorUtility.SetDirty(createdInstance);
            AssetDatabase.SaveAssets();
            Selection.activeObject = createdInstance;
            EditorUtility.FocusProjectWindow();
        }

        public static List<AudioClip> GetSelectedClips()
        {
            var selected = Selection.GetFiltered(typeof(AudioClip), SelectionMode.Assets);
            return selected.Cast<AudioClip>().ToList();
        }
    }
}
