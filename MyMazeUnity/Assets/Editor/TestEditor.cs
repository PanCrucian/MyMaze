using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Test))]
[CanEditMultipleObjects]
public class TestEditor : Editor {

    public Test test;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        test = (Test)target;
    }

    void OnSceneGUI()
    {
        Event current = Event.current;
        switch (current.type)
        {
            case EventType.mouseUp:
               test.transform.localPosition = new Vector3(
                    Mathf.Round(test.transform.localPosition.x / 0.62f) * 0.62f,
                    Mathf.Round(test.transform.localPosition.y / 0.62f) * 0.62f,
                    test.transform.localPosition.z
                    );
                break;
            case EventType.mouseDrag:                
                break;
            default:
                break;
        }
    }
}
