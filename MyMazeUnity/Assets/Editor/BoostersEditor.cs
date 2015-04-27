using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BoostersController))]
public class BoostersEditor : Editor
{
	public override void OnInspectorGUI()
    {
 	    DrawDefaultInspector();
        BoostersController boostersController = (BoostersController)target;
        if(Application.isPlaying)
            return;
        foreach (Booster booster in boostersController.boosters)
            if (!booster.name.Equals(booster.type.ToString("g")))
            {
                booster.name = booster.type.ToString("g");
                EditorUtility.SetDirty(boostersController);
            }
    }
}
