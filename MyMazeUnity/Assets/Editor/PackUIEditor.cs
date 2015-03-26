using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PackUI))]
public class PackUIEditor : Editor
{  

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PackUI packui = (PackUI)target;
        if (packui.pack == null)
            return;
        string newname = packui.pack.name + "UI";
        if (!target.name.Equals(newname))
            target.name = newname;
        
    }
}
