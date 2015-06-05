using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Pyramid : GameLevelObject
{
    public float zDepth = -9f;
    public Deligates.PyramidEvent OnPyramidPickUp;

    /// <summary>
    /// Подобрана ли пирамида
    /// </summary>
    public bool IsUsed
    {
        get
        {
            return _isUsed;
        }
    }

    private bool _isUsed;
    private BoxCollider2D boxCollider;

    private Dictionary<int, PyramidRecordData> pyramidHistory = new Dictionary<int, PyramidRecordData>();

    public override void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zDepth);
        base.Start();
    }

    /// <summary>
    /// Подобрать пирамиду
    /// </summary>
    public void PickUp()
    {
        _isUsed = true;
        boxCollider.enabled = false;
        animator.SetBool("PickUp", true);
        PlayPickUpSound();

        if (OnPyramidPickUp != null)
            OnPyramidPickUp(this);
    }

    /// <summary>
    /// Играет звук подбора
    /// </summary>
    void PlayPickUpSound()
    {
        SoundNames playableSoundName;
        int random = Random.Range(1, 3);
        if (random == 1)
            playableSoundName = SoundNames.Pyramid01;
        else if (random == 2)
            playableSoundName = SoundNames.Pyramid02;
        else
            playableSoundName = SoundNames.Pyramid03;

        SoundsPlayer soundsPlayer = GetComponent<SoundsPlayer>();
        soundsPlayer.PlayOneShootSound(playableSoundName);
    }

    /// <summary>
    /// Что-то вошло в зону тригера
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            PickUp();
        }
    }

    /// <summary>
    /// Уровень перезапустился
    /// </summary>
    public override void Restart()
    {
        ToDefaultState();
        base.Restart();
    }

    /// <summary>
    /// В стартовое состояние
    /// </summary>
    void ToDefaultState()
    {
        animator.SetBool("PickUp", false);
        boxCollider.enabled = true;
        _isUsed = false;
    }

    /// <summary>
    /// Записывает состояние пирамиды на ход
    /// </summary>
    /// <param name="move">Номер хода</param>
    public override void Record(int move)
    {
        if (pyramidHistory.ContainsKey(move))
            pyramidHistory.Remove(move);
        PyramidRecordData recordData = new PyramidRecordData();
        recordData.IsUsed = this.IsUsed;
        pyramidHistory.Add(move, recordData);
        base.Record(move);
    }

    /// <summary>
    /// Сбрасывает записи о состоянии пирамиды
    /// </summary>
    public override void RecordsReset()
    {
        pyramidHistory = new Dictionary<int, PyramidRecordData>();
        base.RecordsReset();
    }

    /// <summary>
    /// Возвращаемся в состояние на конкретный ход
    /// </summary>
    /// <param name="move">Ход в который хотим вернуться</param>
    public override void ReturnToMove(int move)
    {
        PyramidRecordData recordData;
        if (pyramidHistory.TryGetValue(move, out recordData))
        {
            if (!recordData.IsUsed)
                ToDefaultState();
        }
        base.ReturnToMove(move);
    }
}
