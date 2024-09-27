using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerObject
{
    public Vector2 Position;
    public Vector2 Direction;
    public float ChangeChance;

    public WalkerObject(Vector2 position, Vector2 direction, float changeChance)
    {
        Position = position;
        Direction = direction;
        ChangeChance = changeChance;
    }
}
