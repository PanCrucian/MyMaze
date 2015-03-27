using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarsUI : MonoBehaviour {

    public List<StarUI> stars = new List<StarUI>();
    public StarHiddenUI starHidden;

    void Awake()
    {
        foreach (Transform t in transform)
        {
            StarUI star = t.GetComponent<StarUI>();
            if (star == null)
                continue;

            StarHiddenUI starHidden = star.GetComponent<StarHiddenUI>();
            if (starHidden != null)
            {
                this.starHidden = starHidden;                
            }
            else
            {
                stars.Add(star);
            }
        }
    }
}
