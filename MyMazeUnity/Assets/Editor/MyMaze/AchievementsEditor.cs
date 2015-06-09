using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Achievements))]
[CanEditMultipleObjects]
public class AchievementsEditor : Editor
{

    public bool stateFlag;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Achievements achievements = (Achievements)target;

        if (!Application.isPlaying)
        {
            foreach (Achievement achievement in achievements.elements)
            {
                string newname = achievement.type.ToString("g");
                if (!newname.Equals(achievement.name))
                    achievement.name = newname;
            }
        }

        stateFlag = EditorGUILayout.Foldout(stateFlag, "Состояние флагов");
        if (stateFlag)
            foreach (Achievement achievement in achievements.elements)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(achievement.name + " " + achievement.IsAchieved.ToString());
                if(Application.isPlaying)
                    if (!achievement.IsAchieved)
                    {
                        if (GUILayout.Button("Получить", GUILayout.Width(150)))
                        {
                            achievements.Achieve(achievement.type);
                            EditorUtility.SetDirty(achievements);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Убрать", GUILayout.Width(150)))
                        {
                            achievement.Lost();
                            EditorUtility.SetDirty(achievements);
                        }
                    }
                EditorGUILayout.EndHorizontal();
            }
    }
}
