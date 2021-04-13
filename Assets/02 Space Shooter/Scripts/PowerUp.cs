using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;

namespace scripts
{
    public class PowerUp : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public MovementObject movementObject;
        public PowerUpEnum powerUp;
    }

    /// <summary>
    /// powerup types
    /// </summary>
    public enum PowerUpEnum
    {
        LASER,
        SHOTS,
        HEALTH,
        INVINCIBLE
    }
}