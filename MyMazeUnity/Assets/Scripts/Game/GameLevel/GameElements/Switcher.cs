using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Switcher : ButtonLevel
{
    /// <summary>
    /// Время работы в секундах, после входа в состояние SwitcherStates.IsWorking
    /// </summary>
    public float workTime = 3f;

    private SwitcherStates switchState;
    private float workingTime = 0f;

    private Dictionary<int, SwitcherRecordData> switcherHistory = new Dictionary<int, SwitcherRecordData>(); 

    public override void Update()
    {
        if (switchState != SwitcherStates.IsWorking)
            return;
        workingTime += Time.deltaTime;
        if (workingTime >= workTime)
            NowTurnOff();
        base.Update();
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
    /// Запускаем включение
    /// </summary>
    void TurnsOn()
    {
        switchState = SwitcherStates.TurnsOn;
        animator.SetTrigger("TurnsOn");
        if(GameLevel.Instance.state == GameLevelStates.Game)
            GetComponent<SoundsPlayer>().PlayOneShootSound(SoundNames.SwitcherOnOff);
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
        if (GameLevel.Instance.state == GameLevelStates.Game)
            GetComponent<SoundsPlayer>().PlayOneShootSound(SoundNames.SwitcherAlert);
    }

    /// <summary>
    /// Запускаем анимацию выключения
    /// </summary>
    public void TurnsOff()
    {
        switchState = SwitcherStates.TurnsOff;
        animator.SetTrigger("TurnsOff");
        if (GameLevel.Instance.state == GameLevelStates.Game)
            GetComponent<SoundsPlayer>().PlayOneShootSound(SoundNames.SwitcherOnOff);
        if (OnRelease != null)
            OnRelease(this);
    }

    /// <summary>
    /// Перевели переключатель в выключенное состояние
    /// </summary>
    public void Off()
    {
        switchState = SwitcherStates.Off;
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
            animator.SetTrigger("Reset");
        workingTime = 0f;
    }

    /// <summary>
    /// Уровень перезпустился
    /// </summary>
    public override void Restart()
    {
        Off();
        base.Restart();
    }

    /// <summary>
    /// Записывает состояние пирамиды на ход
    /// </summary>
    /// <param name="move">Номер хода</param>
    public override void Record(int move)
    {
        if (switcherHistory.ContainsKey(move))
            switcherHistory.Remove(move);
        SwitcherRecordData recordData = new SwitcherRecordData();
        recordData.switcherState = switchState;
        recordData.workingTime = workingTime;
        switcherHistory.Add(move, recordData);
        base.Record(move);
    }

    /// <summary>
    /// Сбрасывает записи о состоянии пирамиды
    /// </summary>
    public override void RecordsReset()
    {
        switcherHistory = new Dictionary<int, SwitcherRecordData>();
        base.RecordsReset();
    }

    /// <summary>
    /// Возвращаемся в состояние на конкретный ход
    /// </summary>
    /// <param name="move">Ход в который хотим вернуться</param>
    public override void ReturnToMove(int move)
    {
        SwitcherRecordData recordData;
        if (switcherHistory.TryGetValue(move, out recordData))
        {
            switch (recordData.switcherState)
            {
                case SwitcherStates.Off:
                    if (switchState != SwitcherStates.Off)
                    {
                        if (OnRelease != null)
                            OnRelease(this);
                        Off();
                    }
                    break;
                case SwitcherStates.TurnsOn:
                    if (OnRelease != null)
                        OnRelease(this);
                    Off();
                    TurnsOn();
                    break;
                case SwitcherStates.IsWorking:
                    if (switchState == SwitcherStates.IsWorking)
                        workingTime = recordData.workingTime;
                    else
                    {
                        if (OnRelease != null)
                            OnRelease(this);
                        Off();
                        TurnsOn();
                    }
                    break;
                case SwitcherStates.NowTurnOff:
                    if (switchState == SwitcherStates.IsWorking || switchState == SwitcherStates.TurnsOn)
                        Off();
                    if (OnRelease != null)
                        OnRelease(this);
                    break;
                case SwitcherStates.TurnsOff:
                    if (switchState == SwitcherStates.IsWorking || switchState == SwitcherStates.TurnsOn)
                        Off();
                    if (OnRelease != null)
                        OnRelease(this);
                    break;
            }
        }
        base.ReturnToMove(move);
    }
}
