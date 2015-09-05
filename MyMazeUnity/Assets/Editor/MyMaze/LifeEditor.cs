using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Life))]
[CanEditMultipleObjects]
public class LifeEditor : Editor {

    public bool livesFold;

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        if (!Application.isPlaying)
            return;
        Life life = (Life)target;
        GUILayout.Label("Количество жизней: " + life.Units.ToString());
        GUILayout.Label("Регенерация в: " + life.GetNextBlockRegenerationTime().ToString());

        Timers.Instance.unixTimeOffset = EditorGUILayout.IntField("Коррекция времени", Timers.Instance.unixTimeOffset);
        EditorGUILayout.Space();

        if (GUILayout.Button("Восстановить все"))
            life.RestoreAllUnits();

        if (GUILayout.Button("Потратить 1 жизнь"))
            life.Use();

        if (GUILayout.Button("Восстановить 1 жизнь"))
            life.RestoreOneUnit();

        livesFold = EditorGUILayout.Foldout(livesFold,"Данные о жизнях");
        if (livesFold)
        {
            foreach (LifeBlock lb in life.Blocks)
            {
                GUILayout.Label("Life " + lb.index.ToString() + ": " + GetTimerString(lb.regenerationTime));
            }
        }

        GUILayout.Button("Refresh");
    }

    /// <summary>
    /// Пересчитываем время до восстановления жизни в человеческий вид
    /// </summary>
    /// <returns></returns>
    string GetTimerString(int regenerationTime)
    {
        int currentUnixTime = Timers.Instance.UnixTimestamp;

        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(Mathf.Abs(regenerationTime - currentUnixTime));

        if (regenerationTime > 0)
            return System.String.Format("{0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        else
            return "0:00";
    }
}
