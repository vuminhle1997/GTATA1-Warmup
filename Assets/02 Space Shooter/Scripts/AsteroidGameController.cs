using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace Scripts
{
    /// <summary>
    /// Game controller handling asteroids and intersection of components.
    /// </summary>
    public class AsteroidGameController : MonoBehaviour
    {
        public Asteroid[] bigAsteroids;
        public Asteroid[] mediumAsteroids;
        public Asteroid[] smallAsteroids;

        // contains the registered powerups
        public PowerUp[] powerUps;

        [SerializeField] private Vector3 maximumSpeed, maximumSpin;
        [SerializeField] private PlayerShip playerShip;
        [SerializeField] private Transform spawnAnchor;
        // for changing the laser parameters, if taking the red pill
        [SerializeField] private Laser laser;

        private List<Asteroid> activeAsteroids;
        private List<PowerUp> activePowerUps;
        private Random random;

        // ship health
        private float shipHP = 100f;
        // if ship is invincible
        private Boolean invincible = false;
        private void Start()
        {
            activeAsteroids = new List<Asteroid>();
            activePowerUps = new List<PowerUp>();
            random = new Random();
            // spawn some initial asteroids
            for (var i = 0; i < 5; i++)
            {
                SpawnAsteroid(bigAsteroids, Camera.main.OrthographicBounds());
            }

            // spawn initial powerups
            for (var i = 0; i < 3; i++)
            {
                SpawnPowerUps(powerUps, Camera.main.OrthographicBounds());
            }
        }

        private void Update()
        {
            ShipIntersection(playerShip.shipSprite);
            if (activeAsteroids.Count == 0)
            {
                SceneManager.LoadScene("02 Won", LoadSceneMode.Single);
            }
        }

        /// <summary>
        /// Behaviour to spawn an asteroid within the screen
        /// If there is a parent given, the velocity of that parent is put into consideration
        /// </summary>
        private void SpawnAsteroid(Asteroid[] prefabs, Bounds inLocation, Asteroid parent = null)
        {
            // get a random prefab from the list
            var prefab = prefabs[random.Next(prefabs.Length)];
            // create an instance of the prefab
            var newObject = Instantiate(prefab, spawnAnchor);
            // position it randomly within the box given (either the parent asteroid or the camera)
            newObject.transform.position = RandomPointInBounds(inLocation);
            // we can randomly invert the x/y scale to mirror the sprite. This creates overall more variety
            newObject.transform.localScale = new Vector3(UnityEngine.Random.value > 0.5f ? -1 : 1,
                UnityEngine.Random.value > 0.5f ? -1 : 1, 1);
            // renaming, I'm also sometimes lazy typing
            var asteroidSprite = newObject.spriteRenderer;

            // try to position the asteroid somewhere where it doesn't hit the player or another active asteroid
            for (var i = 0;
                playerShip.shipSprite.bounds.Intersects(asteroidSprite.bounds) ||
                activeAsteroids.Any(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(asteroidSprite.bounds));
                i++)
            {
                // give up after 15 tries.
                if (i > 15)
                {
                    DestroyImmediate(newObject.gameObject);
                    return;
                }

                newObject.transform.position = RandomPointInBounds(inLocation);
            }
            
            // take parent velocity into consideration
            if (parent != null)
            {
                var offset = parent.transform.position - newObject.transform.position;
                var parentVelocity = parent.movementObject.CurrentVelocity.magnitude *
                                     (UnityEngine.Random.value * 0.4f + 0.8f);
                newObject.movementObject.Impulse(offset.normalized * parentVelocity, RandomizeVector(maximumSpeed));
            }
            // otherwise randomize just some velocity
            else
            {
                newObject.movementObject.Impulse(RandomizeVector(maximumSpeed), RandomizeVector(maximumSpin));
            }

            activeAsteroids.Add(newObject);
        }
        
        /// <summary>
        /// taken from above
        /// </summary>
        /// <param name="prefabs"></param>
        /// <param name="inLocation"></param>
        /// <param name="parent"></param>
        private void SpawnPowerUps(PowerUp[] prefabs, Bounds inLocation, Asteroid parent = null)
        {
            // get a random prefab from the list
            var prefab = prefabs[random.Next(prefabs.Length)];
            // create an instance of the prefab
            var newObject = Instantiate(prefab, spawnAnchor);
            // position it randomly within the box given (either the parent asteroid or the camera)
            newObject.transform.position = RandomPointInBounds(inLocation);
            // we can randomly invert the x/y scale to mirror the sprite. This creates overall more variety
            newObject.transform.localScale = new Vector3(UnityEngine.Random.value > 0.5f ? -1 : 1,
                UnityEngine.Random.value > 0.5f ? -1 : 1, 1);
            // renaming, I'm also sometimes lazy typing
            var powerUpSprite = newObject.spriteRenderer;

            // try to position the asteroid somewhere where it doesn't hit the player or another active asteroid
            for (var i = 0;
                playerShip.shipSprite.bounds.Intersects(powerUpSprite.bounds) ||
                activeAsteroids.Any(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(powerUpSprite.bounds));
                i++)
            {
                // give up after 15 tries.
                if (i > 15)
                {
                    DestroyImmediate(newObject.gameObject);
                    return;
                }

                newObject.transform.position = RandomPointInBounds(inLocation);
            }
            
            // take parent velocity into consideration
            if (parent != null)
            {
                var offset = parent.transform.position - newObject.transform.position;
                var parentVelocity = parent.movementObject.CurrentVelocity.magnitude *
                                     (UnityEngine.Random.value * 0.4f + 0.8f);
                newObject.movementObject.Impulse(offset.normalized * parentVelocity, RandomizeVector(maximumSpeed));
            }
            // otherwise randomize just some velocity
            else
            {
                newObject.movementObject.Impulse(RandomizeVector(maximumSpeed), RandomizeVector(maximumSpin));
            }

            activePowerUps.Add(newObject);
        }


        /// <summary>
        /// Checks if a laser is intersecting with an asteroid and executes gameplay behaviour on that
        /// </summary>
        public void LaserIntersection(SpriteRenderer laser)
        {
            // go through all asteroids, check if they intersect with a laser and stop after the first
            var asteroid = activeAsteroids
                .FirstOrDefault(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(laser.bounds));

            // premature exit: this laser hasn't hit anything
            if (asteroid == null)
            {
                return;
            }
            
            // otherwise remove the asteroid from the tracked asteroid
            activeAsteroids.Remove(asteroid);
            var bounds = asteroid.spriteRenderer.bounds;
            // get the correct set of prefabs to spawn asteroids in place of the asteroid that now explodes
            var prefabs = asteroid.asteroidSize switch
            {
                AsteroidSize.Large => mediumAsteroids,
                AsteroidSize.Medium => smallAsteroids,
                _ => null
            };
            // remote the asteroid gameobject with all its components
            Destroy(asteroid.gameObject);
            // premature exit: we have no prefabs (ie: small asteroids exploding)
            if (prefabs == null)
            {
                return;
            }

            // randomize two to six random asteroids
            var objectCountToSpawn = (int) (UnityEngine.Random.value * 4 + 2);
            for (var i = 0; i < objectCountToSpawn; i++)
            {
                SpawnAsteroid(prefabs, bounds);
            }
        
            // oh, also get rid of the laser now
            Destroy(laser.gameObject);
        }

        
        /// <summary>
        /// Checks, if ship intersects with powerup or asteroid
        /// </summary>
        /// <param name="ship"></param>
        private void ShipIntersection(SpriteRenderer ship)
        {
            // :thinking: this could be solved very similarly to a laser intersection
            var powerUp =
                activePowerUps.FirstOrDefault(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(ship.bounds));
            var asteroid =
                activeAsteroids.FirstOrDefault(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(ship.bounds));

            if (powerUp == null && asteroid == null)
            {
                return;
            }

            // is powerup is hitted
            if (powerUp)
            {
                switch (powerUp.powerUp)
                {
                    case PowerUpEnum.SHOTS:
                        laser.EnhanceLaserSpeed(100f);
                        break;
                    case PowerUpEnum.INVINCIBLE:
                        StartCoroutine(MakeShipInvincible());
                        break;
                    default:
                        Debug.Log("Nothing");
                        break;
                }
                activePowerUps.Remove(powerUp);
                Destroy(powerUp.gameObject);
            }

            // asteroid is hitted and ship haven't took the red pill
            // take damage and show game over screen, if threshold is reached
            if (asteroid)
            {
                if (!invincible)
                {
                    switch (asteroid.asteroidSize)
                    {
                        case AsteroidSize.Large:
                            shipHP -= 50f;
                            break;
                        case AsteroidSize.Medium:
                            shipHP -= 12.25f;
                            break;
                        case AsteroidSize.Small:
                            shipHP -= 7.25f;
                            break;
                    }
                    activeAsteroids.Remove(asteroid);
                    Destroy(asteroid.gameObject);
                    if (shipHP <= 0f)
                    {
                        Destroy(playerShip);
                        Destroy(ship);
                        SceneManager.LoadScene("00 Game Over", LoadSceneMode.Single);
                    }
                }
            }
        }

        private static float RandomPointOnLine(float min, float max)
        {
            return UnityEngine.Random.value * (max - min) + min;
        }

        private static Vector2 RandomPointInBox(Vector2 min, Vector2 max)
        {
            return new Vector2(RandomPointOnLine(min.x, max.x), RandomPointOnLine(min.y, max.y));
        }

        private static Vector2 RandomPointInBounds(Bounds bounds)
        {
            return RandomPointInBox(bounds.min, bounds.max);
        }

        private static Vector3 RandomizeVector(Vector3 maximum)
        {
            // that is an inline method - it's good enough to just get a float [-1...+1]
            float RandomValue()
            {
                return UnityEngine.Random.value - 0.5f * 2;
            }

            maximum.Scale(new Vector3(RandomValue(), RandomValue(), RandomValue()));
            return maximum;
        }

        /// <summary>
        /// makes the ship invincible for a certain (default: 3.25s) time, by decreasing
        /// the opacity, so the player can see the invincible effect
        /// </summary>
        /// <returns></returns>
        private IEnumerator MakeShipInvincible()
        {
            invincible = true;
            playerShip.shipSprite.color = new Color(255, 255, 255, 0.35f);
            yield return new WaitForSeconds(3.25f);
            playerShip.shipSprite.color = new Color(255, 255, 255, 255);
            invincible = false;
        }
    }
}