using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

    public string levelName;
    public bool loadOnStart;

    void Start()
    {
        if (loadOnStart)
            Load();
    }

    public void SetName(string levelName)
    {
        this.levelName = levelName;
    }

    public void Load()
    {
        Application.LoadLevel(this.levelName);
    }

    public void Load(string levelName)
    {
        Application.LoadLevel(levelName);
    }
}
