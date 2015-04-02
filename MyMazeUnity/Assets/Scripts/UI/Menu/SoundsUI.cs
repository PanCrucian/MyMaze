using UnityEngine;
using System.Collections;

public class SoundsUI : MonoBehaviour {

    public void EnableAllSounds()
    {
        MyMaze.Instance.Sounds.EnableAllSounds();
    }

    public void DisableAllSounds()
    {
        MyMaze.Instance.Sounds.DisableAllSounds();
    }

    public void SoundsOnly()
    {
        MyMaze.Instance.Sounds.SoundsOnly();
    }
}
