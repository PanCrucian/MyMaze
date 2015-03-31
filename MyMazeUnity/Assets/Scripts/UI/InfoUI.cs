using UnityEngine;
using System.Collections;

public class InfoUI : MonoBehaviour {

    public void OpenFacebookWebPage()
    {
        MyMaze.Instance.WebLinks.OpenFacebook();
    }

    public void OpenVkWebPage()
    {
        MyMaze.Instance.WebLinks.OpenVk();
    }
}
