using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LevelUI))]
[CanEditMultipleObjects]
public class LevelUIEditor : Editor
{  

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelUI levelui = (LevelUI)target;
        if (levelui.level == null)
            return;
        string newname = levelui.level.name + "UI";
        if (!target.name.Equals(newname))
            target.name = newname;
        
    }
}
