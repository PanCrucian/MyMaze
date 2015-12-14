using System.Collections;
using UnityEngine;

public static class Deligates
{
    public delegate void SimpleEvent();
    public delegate void DirectionEvent(Directions direction);
    public delegate void PyramidEvent(Pyramid pyramid);
    public delegate void ButtonEvent(ButtonLevel button);
    public delegate void IntegerEvent(int value);
    public delegate void Vector2Event(Vector2 vector2);
    public delegate void LevelEvent(Level level);
    public delegate void PackEvent(Pack pack);
    public delegate void StarEvent(Star star);
    public delegate void PackGroupEvent(PackGroupTypes group);
    public delegate void TransactionEvent(ProductTypes type);
    public delegate void RecieptTransactionEvent(ProductTypes type, string reciept, string transId, string signature);
    public delegate void BoolEvent(bool flag);
    public delegate void NotificationEvent(int id, System.DateTime launchTime, string name);
}
