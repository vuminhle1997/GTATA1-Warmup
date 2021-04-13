using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Scripts
{
    /// <summary>
    /// An Upgrade is a down-moving block, that collides only with the pedal
    /// </summary>
    public class Upgrade : MonoBehaviour
    {
        [SerializeField] private UpgradeType upgradeType;
        [SerializeField] private SpriteAssignment[] spriteAssignment;
        public UpgradeType Type => upgradeType;

        private void OnEnable()
        {   // dice the type, set the appropriate sprite
            RandomizeType();
            GetComponent<Image>().sprite = spriteAssignment.FirstOrDefault(x => x.type == upgradeType)?.sprite;
        }

        private void RandomizeType()
        {
            var number = Random.value;

            if (number < 0.25f)
            {
                upgradeType = UpgradeType.BiggerPedal;
                return;
            }

            if (number < 0.55f)
            {
                upgradeType = UpgradeType.SmallerPedal;
                return;
            }

            if (number < 0.75f)
            {
                upgradeType = UpgradeType.FasterBall;
                return;
            }

            upgradeType = UpgradeType.ExtraBall;
        }

        [Serializable]
        private class SpriteAssignment
        {
            public UpgradeType type;
            public Sprite sprite;
        }
    }
}