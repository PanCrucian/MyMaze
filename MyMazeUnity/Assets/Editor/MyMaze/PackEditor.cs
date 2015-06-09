using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Pack))]
[CanEditMultipleObjects]
public class PackEditor : Editor
{  

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Pack pack = (Pack)target;
        pack.levels = pack.GetComponentsInChildren<Level>();
        string newname = "Pack" + (pack.transform.GetSiblingIndex() + 1).ToString() + "_" + pack.packName;
        if (!target.name.Equals(newname))
        {
            target.name = newname;
        }
        if (GUILayout.Button("Закрыть все уровни, кроме первого"))
        {
            for (int i = 0; i < pack.levels.Length; i++)
            {
                Level level = pack.levels[i];
                if (i == 0)
                    level.Open();
                else
                    level.Close();
                EditorUtility.SetDirty(level);
            }
        }
        if (GUILayout.Button("Открыть все уровни"))
        {
            for (int i = 0; i < pack.levels.Length; i++)
            {
                Level level = pack.levels[i];
                level.Open();
                EditorUtility.SetDirty(level);
            }
        }
        if (pack.StarsRequired < 0)
            pack.StarsRequired = 0;
        if (GUILayout.Button("Подобрать все звезды"))
        {
            foreach (Level level in pack.levels)
            {
                foreach (Star star in level.stars)
                    star.Collect();

                EditorUtility.SetDirty(level);
            }
        }
        if (!Application.isPlaying)
            EditorUtility.SetDirty(pack);
    }
}
