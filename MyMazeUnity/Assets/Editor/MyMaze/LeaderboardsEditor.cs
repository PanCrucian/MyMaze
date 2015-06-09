using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Leaderboards))]
[CanEditMultipleObjects]
public class LeaderboardsEditor : Editor
{

    public bool stateFlag;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Leaderboards leaderboards = (Leaderboards)target;

        if (!Application.isPlaying)
        {
            foreach (Leaderboard leaderboard in leaderboards.elements)
            {
                string newname = leaderboard.type.ToString("g");
                if (!newname.Equals(leaderboard.name))
                    leaderboard.name = newname;
            }
        }
    }
}
