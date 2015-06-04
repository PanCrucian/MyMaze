using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyMaze))]
public class SceneLoader : MonoBehaviour {

    public string sceneName;
    public bool loadOnStart;

    void Start()
    {
        if (loadOnStart)
            Load();
    }

    public void SetName(string levelName)
    {
        this.sceneName = levelName;
    }

    public void Load()
    {
        Application.LoadLevel(this.sceneName);
    }

    public void Load(string levelName)
    {
        this.sceneName = levelName;
        Load();
    }

    public void LoadMenu()
    {
        Load("Main");
    }
}
