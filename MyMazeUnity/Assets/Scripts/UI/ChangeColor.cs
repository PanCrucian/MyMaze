using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeColor : MonoBehaviour {

    public void Change(string hex)
    {
        Image img = GetComponent<Image>();
        img.color = HexToColor(hex);
    }

    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        if (hex.Length == 6)
        {            
            return new Color32(r, g, b, 255);
        }

        byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, a);
    }
}
