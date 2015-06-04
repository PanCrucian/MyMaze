using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PackButtonUI : MonoBehaviour {

    private ColorBlock normalColorBlock;
    private ColorBlock closedColorBlock;
    private Button button;
    private PackUI packUI;

	void Start () {
        button = GetComponent<Button>();
        packUI = GetComponentInParent<PackUI>();
        normalColorBlock = button.colors;
        closedColorBlock = normalColorBlock;
        closedColorBlock.pressedColor = normalColorBlock.normalColor;
	}

    void Update()
    {
        if (packUI.pack.IsClosed)
        {
            button.colors = closedColorBlock;
        }
        else
        {
            button.colors = normalColorBlock;
        }
    }
}
