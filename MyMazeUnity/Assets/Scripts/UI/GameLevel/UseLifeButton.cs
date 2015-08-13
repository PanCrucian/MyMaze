using UnityEngine;
using System.Collections;

public class UseLifeButton : MonoBehaviour {

    public void UseLife()
    {
        MyMaze.Instance.Life.Use();
    }
}
