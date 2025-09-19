using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerSpriteSet", menuName = "SpriteSet", order = 1)]
public class PlayerSpriteSet : PhysicsObjSpriteSet
{
    public Sprite chargingSprite;
    public Sprite specialChargingSprite;
    public Sprite parrySprite;
    public Sprite wallTechSprite;

}
