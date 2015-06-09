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
}
