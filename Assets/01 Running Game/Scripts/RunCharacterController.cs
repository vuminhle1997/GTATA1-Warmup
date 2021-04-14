using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Controls the movement of the Character
    /// </summary>
    public class RunCharacterController : MonoBehaviour
    {
        public Transform Transform => character;
        public SpriteRenderer CharacterSprite => characterSprite;
        /// <summary>
        /// Since the Character controller takes responsibility for triggering Input events, it also emits an
        /// event when it does so
        /// </summary>
        public Action onJump;
        
        [SerializeField] private float jumpHeight;
        [SerializeField] private float jumpDuration;
        /// <summary>
        /// Unity handles Arrays and Lists in the inspector correctly (but not Maps, Dictionaries or other Collections)
        /// </summary>
        [SerializeField] private KeyCode[] jumpKeys;
        /// <summary>
        /// We don't require anything else from the Character than its transform
        /// </summary>
        [SerializeField] private Transform character;
        [SerializeField] private SpriteRenderer characterSprite;
        [SerializeField] private AnimationCurve jumpPosition;
        
        [SerializeField] private Sprite[] jumpSprites;
        [SerializeField] private Sprite[] idleSprites;
        [SerializeField] private Sprite[] walkingSprites1;
        [SerializeField] private Sprite[] walkingSprites2;
        [SerializeField] private CharacterSelectContoller characterSelectController;

        private AudioSource jumpSound;
        
        private bool canJump = true;
        private int currPos = 0;

        public void Start()
        {
            jumpSound = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Update is a Unity runtime function called *every rendered* frame before Rendering happens
        /// see: https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void Update()
        {
            if (!canJump)
            {
                return;
            }

            // here the input event counts - if there is any button pressed that were defined as jump keys, trigger a jump
            if (jumpKeys.Any(x => Input.GetKeyDown(x)))
            {   // first we disable the jump, then start the Coroutine that handles the jump and invoke the event
                canJump = false;
                jumpSound.Play();
                StartCoroutine(JumpRoutine());
                onJump?.Invoke();
            }
            else
            {
                StartCoroutine(WalkingAnimation());
            }
        }

        private void LateUpdate()
        {
            currPos = characterSelectController.GetPos();
        }

        /// <summary>
        /// OnDrawGizmosSelected is a Unity editor function called when the attached GameObject is selected and used to
        /// display debugging information in the Scene view
        /// see: https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            var upScale = transform.lossyScale;
            upScale.Scale(transform.up);
            Gizmos.DrawLine(transform.position, Vector3.up * jumpHeight * upScale.magnitude);
        }

        /// <summary>
        /// Handles the jump of a character
        /// 
        /// To be used in an Coroutine, this function is a generator (return IEnumerator) and has special syntactic
        /// sugar with "yield return"
        /// </summary>
        private IEnumerator JumpRoutine()
        {
            // the time this coroutine runs
            var totalTime = 0f;
            // low position is assumed to be a (0, 0, 0)
            var highPosition = character.up * jumpHeight;
            while (totalTime < jumpDuration)
            {
                characterSprite.sprite = jumpSprites[currPos];
                totalTime += Time.deltaTime;
                // what's the normalized time [0...1] this coroutine runs at
                var sampleTime = totalTime / jumpDuration;
                // Lerp is a Linear Interpolation between a...b based on a value between 0...1
                character.localPosition = Vector3.Lerp(Vector3.zero, highPosition, jumpPosition.Evaluate(sampleTime));
                // we enable jumping again after we're almost done to remove some "stuck" behaviour when landing down
                if (sampleTime > 0.95f)
                {
                    canJump = true;
                }
                // yield return null waits a single frame
                yield return null;
            }

            characterSprite.sprite = idleSprites[currPos];
        }

        // source: https://stackoverflow.com/questions/26964308/how-to-change-a-sprite-to-another-and-then-back-after-1-second
        // first sets sprite to first walking sprite
        // waits for .175 second
        // and proceeds to change to another sprite
        private IEnumerator WalkingAnimation()
        {
            characterSprite.sprite = walkingSprites1[currPos];
            yield return new WaitForSeconds(0.175f);
            characterSprite.sprite = walkingSprites2[currPos];
        }
    }
}