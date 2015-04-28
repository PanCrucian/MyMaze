using UnityEngine;
using System.Collections;

public class Prepare : MonoBehaviour {

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Application.LoadLevel("Main");
    }
}
