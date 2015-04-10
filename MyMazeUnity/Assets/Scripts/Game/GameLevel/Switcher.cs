using UnityEngine;
using System.Collections;

public class Switcher : ButtonLevel
{
    /// <summary>
    /// Время работы в секундах, после входа в состояние SwitcherStates.IsWorking
    /// </summary>
    public float workTime = 3f;

    private SwitcherStates switchState;
    private float workingTime = 0f;

    public override void Update()
    {
        base.Update();
        if (switchState != SwitcherStates.IsWorking)
            return;
        workingTime += Time.deltaTime;
        if (workingTime >= workTime)
            NowTurnOff();
    }

    /// <summary>
    /// Что-то стоит в зоне переключателя
    /// </summary>
    /// <param name="coll"></param>
	public override void OnTriggerEnter2D(Collider2D coll) {
        if (CheckForPlayer(coll.gameObject))
        {
            if (switchState == SwitcherStates.Off)
                TurnsOn();
        }
    }

    /// <summary>
    /// Запускаем велючение
    /// </summary>
    void TurnsOn()
    {
        switchState = SwitcherStates.TurnsOff;
        animator.SetTrigger("TurnsOn");
        if (OnPress != null)
            OnPress(this);
    }

    /// <summary>
    /// Включаем работу переключателя. Анимация включения закончилась
    /// </summary>
    public void On()
    {
        switchState = SwitcherStates.IsWorking;
        animator.SetTrigger("Working");
    }

    /// <summary>
    /// Мигаем лампочкой, скоро выключимся
    /// </summary>
    void NowTurnOff()
    {
        switchState = SwitcherStates.NowTurnOff;
        animator.SetTrigger("NowTurnOff");
    }

    /// <summary>
    /// Запускаем анимацию выключения
    /// </summary>
    public void TurnsOff()
    {
        switchState = SwitcherStates.TurnsOff;
        animator.SetTrigger("TurnsOff");
        if (OnRelease != null)
            OnRelease(this);
    }

    /// <summary>
    /// Перевели переключатель в выключенное состояние
    /// </summary>
    public void Off()
    {
        switchState = SwitcherStates.Off;
        animator.SetTrigger("Reset");
        workingTime = 0f;
    }

    /// <summary>
    /// Уровень перезпустился
    /// </summary>
    public override void Restart()
    {
        base.Restart();
        Off();
    }
}
