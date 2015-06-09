using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Sounds))]
[CanEditMultipleObjects]
public class SoundsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Sounds sounds = (Sounds)target;
        foreach(Sounds.SoundMap sMap in sounds.soundsMap) {
            string newName = sMap.nameEnum.ToString("g");
            if (!sMap.name.Equals(newName))
            {
                sMap.name = newName;
                EditorUtility.SetDirty(sounds);
            }
        }
    }
}
