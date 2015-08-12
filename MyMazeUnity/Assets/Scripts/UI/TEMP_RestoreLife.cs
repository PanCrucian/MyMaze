using UnityEngine;
using System.Collections;

public class TEMP_RestoreLife : MonoBehaviour {

    public void Restore()
    {
        MyMaze.Instance.Life.RestoreOneUnit();
        if (GameLevel.Instance != null)
            GameLevel.Instance.OnRestartRequest();
    }
}
