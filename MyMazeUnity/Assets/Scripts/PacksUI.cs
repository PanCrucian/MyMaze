using UnityEngine;
using System.Collections;

public class PacksUI : MonoBehaviour {

    public PackUI[] packsui;

    void Start()
    {
        packsui = GetComponentsInChildren<PackUI>();
    }
}
