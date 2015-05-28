using UnityEngine;
using UnityEngine.UI;

public class SoundsOnClickPlayer : SoundsPlayer
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("Кнопка не найдена, звук не будет воспроизводиться");
            return;
        }
        button.onClick.AddListener(() => { PlayOneShootSound(); });
    }
}
