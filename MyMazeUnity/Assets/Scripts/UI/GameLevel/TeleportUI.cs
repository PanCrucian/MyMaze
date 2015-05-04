using UnityEngine;
using System.Collections;

public class TeleportUI : MonoBehaviour {

    public void InitTeleportAction()
    {
        Player.Instance.PrepareForTeleport();
    }
}
