using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyMaze))]
public class WebLinks : MonoBehaviour {
    public string facebook;
    public string vk;
    public string itunes;
    public string appleStore = "https://itunes.apple.com/app/id1018888691";
    public string playMarket = "https://play.google.com/store/apps/details?id=com.thumbspire.mymaze";

    public void OpenFacebook()
    {
        Application.OpenURL(facebook);
    }

    public void OpenVk() {
        Application.OpenURL(vk);
    }

    public void OpenItunes()
    {
        Application.OpenURL(itunes);
    }

    public void OpenAppleStore()
    {
        Application.OpenURL(appleStore);
    }

    public void OpenPlayMarket()
    {
        Application.OpenURL(playMarket);
    }
}
