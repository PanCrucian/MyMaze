using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ButtonLevel : GameLevelObject {
    public Deligates.ButtonEvent OnPress;
    public Deligates.ButtonEvent OnRelease;
    private ButtonStates state;

    public virtual void OnTriggerEnter2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            ToggleButton();
        }
    }

    /// <summary>
    /// Вызываем события нажатия кнопки
    /// </summary>
    void ToggleButton()
    {
        if (state == ButtonStates.Off)
            PressButton();
        else
            ReleaseButton();
    }

    /// <summary>
    /// Нажать кнопку
    /// </summary>
    void PressButton()
    {
        state = ButtonStates.On;
        if (OnPress != null)
            OnPress(this);
    }

    /// <summary>
    /// Отжать кнопку
    /// </summary>
    public void ReleaseButton()
    {
        state = ButtonStates.Off;
        if (OnRelease != null)
            OnRelease(this);
    }

    /// <summary>
    /// Уровень перезапустился
    /// </summary>
    public override void Restart()
    {
        base.Restart();
        ReleaseButton();
    }
}
