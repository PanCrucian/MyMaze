using UnityEngine;
using System.Collections;

public class InfoUI : MonoBehaviour {

    public void OpenFacebookWebPage()
    {
        Game.Instance.WebLinks.OpenFacebook();
    }

    public void OpenVkWebPage()
    {
        Game.Instance.WebLinks.OpenVk();
    }
}
