using UnityEngine;
using System.Collections;

public class WebLinks : MonoBehaviour {
    public string facebook;
    public string vk;
    public string itunes;

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
}
