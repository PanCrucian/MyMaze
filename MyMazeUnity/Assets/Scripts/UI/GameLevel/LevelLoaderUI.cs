using UnityEngine;
using System.Collections;

public class LevelLoaderUI : MonoBehaviour {

    public void LoadMenu()
    {
        MyMaze.Instance.LevelLoader.levelName = "Main";
        MyMaze.Instance.LevelLoader.Load();
    }
}
