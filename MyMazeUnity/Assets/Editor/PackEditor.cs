using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Pack))]
public class PackEditor : Editor
{  

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Pack pack = (Pack)target;
        string newname = "Pack" + (pack.transform.GetSiblingIndex() + 1).ToString() + "_" + pack.packName;
        if (!target.name.Equals(newname))
            target.name = newname;
        
    }
}
