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
        {
            if (!target.name.Equals("LevelUI"))
                levelui.name = "LevelUI";
        }
        else
        {
            string newname = levelui.level.name + "UI";
            if (!target.name.Equals(newname))
                target.name = newname;
        }
        
    }
}
