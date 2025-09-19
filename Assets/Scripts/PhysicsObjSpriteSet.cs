using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PhysicsObjSpriteSet", menuName = "SpriteSet", order = 1)]
public class PhysicsObjSpriteSet : ScriptableObject
{
    public Sprite idleSprite;
    public Sprite glidingSprite;
    public Sprite movingSprite;
    public Sprite knockbackSprite;
    public Sprite hitStopSprite;

}
