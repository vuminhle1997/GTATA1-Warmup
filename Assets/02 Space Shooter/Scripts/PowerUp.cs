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

    public enum PowerUpEnum
    {
        LASER
    }
}