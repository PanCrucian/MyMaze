using UnityEngine;
using System.Collections;

public class LockPosition : MonoBehaviour {

    public bool doLock;

    public Vector3 lockPosition;

    void Update()
    {
        if (doLock)
            Lock();
    }

    public void SetLockPosition()
    {
        lockPosition = transform.localPosition;
    }

    public void Lock()
    {
        transform.localPosition = lockPosition;
    }
}
