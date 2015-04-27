using UnityEngine;
using System.Collections;

public class MoveBackUI : MonoBehaviour {

    public void ReturnToMove()
    {
        GameLevel.Instance.ReturnToMoveRequest(Player.Instance.MovesCount - 1);
    }
}
