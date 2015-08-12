using UnityEngine;
using System.Collections;

public class LevelsMenuUI : MonoBehaviour {

    private AdsLifeUI adsCG;
    
    void OnEnable()
    {
        if (MyMaze.Instance.Life.Units <= 0)
        {
            if (adsCG == null)
                adsCG = GameObject.FindObjectOfType<AdsLifeUI>();
            adsCG.Show();
        }
    }
}
