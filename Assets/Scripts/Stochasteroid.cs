using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stochasteroid is a script for creating randomized asteroid fields. Simply place
/// this script on a cube GameObject named "StochasteroidPen," set the asteroid scale,
/// rotation, velocity, count, select desired trajectories, and assign asteroid prefabs
/// and the class will automatically fill the cube with randomly placed asteroids. Each
/// asteroid requires a rigid body to realistically simulate motion. Each asteroid
/// is generated randomly by selecting values within the given ranges. When an asteroid
/// reaches the edge of the cube it is automatically respawned on a new trajectory.
/// </summary>
public class Stochasteroid : MonoBehaviour
{
    /// <summary>
    /// The minimum scale factor of each asteroid. 1.0 is the model's initial size
    /// as imported by Unity. Actual value is randomly selected.
    /// </summary>
    public int asteroidMinScale = 1;

    /// <summary>
    /// The maximum scale factor of each asteroid. Actual value is randomly selected.
    /// </summary>
    public int asteroidMaxScale = 10;

    /// <summary>
    /// The minimum amount of rotation for a particular asteroid. This is a lower bound
    /// and is unitless. The actual value will be selected at random.
    /// </summary>
    public int asteroidMinRotation = 1;

    /// <summary>
    /// The maximum rotation that a particular asteroid may exhibit. This is an upper
    /// bound and is unitless. The actual value will be selected at random.
    /// </summary>
    public int asteroidMaxRotation = 100;

    /// <summary>
    /// The minimum velocity for an asteroid. This velocity is simply a multiplier
    /// and is unitless.
    /// </summary>
    public int asteroidMinVelocity = 1;

    /// <summary>
    /// The max velocity for an asteroid. This velocity is a multiplier and is
    /// unitless.
    /// </summary>
    public int asteroidMaxVelocity = 100;

    /// <summary>
    /// The number of asteroids to spawn.
    /// </summary>
    public int asteroidCount = 5;

    /// <summary>
    /// A list of GameObjects that will act as randomly chosen templates for the asteroids.
    /// </summary>
    public List<GameObject> asteroidTemplates;

    /// <summary>
    /// Enables asteroids with trajectories beginning or ending on the
    /// negative Z pane of the cube.
    /// </summary>
    public bool negativeZTrajectory = true;

    /// <summary>
    /// Enables asteroids with trajectories beginning or ending on the
    /// positive Z pane of the cube.
    /// </summary>
    public bool positiveZTrajectory = true;

    /// <summary>
    /// Enables asteroids with trajectories beginning or ending on the
    /// positive Y pane of the cube.
    /// </summary>
    public bool positiveYTrajectory = true;

    /// <summary>
    /// Enables asteroids with trajectories beginning or ending on the
    /// negative Y pane of the cube.
    /// </summary>
    public bool negativeYTrajectory = true;

    /// <summary>
    /// Enables asteroids with trajectories beginning or ending on the
    /// negative X pane of the cube.
    /// </summary>
    public bool negativeXTrajectory = true;

    /// <summary>
    /// Enables asteroids with trajectories beginning or ending on the
    /// positive X pane of the cube.
    /// </summary>
    public bool positiveXTrajectory = true;

    /// <summary>
    /// The list of spawned asteroids.
    /// </summary>
    private readonly List<GameObject> asteroidList = new List<GameObject>();

    /// <summary>
    /// The random number generator.
    /// </summary>
    private readonly System.Random random = new System.Random(DateTime.Now.Millisecond);

    /// <summary>
    /// Indicates critical failure.
    /// </summary>
    private bool dead = false;

	/// <summary>
    /// Initialize script.
    /// </summary>
	void Start()
    {
        if (this.name != "StochasteroidPen")
        {
            Debug.LogWarning("Stochasteroid must be on a dedicated object named StochasteroidPen");
        }

        if (asteroidCount < 1)
        {
            Debug.LogError("Stochasteroid minimum count cannot be less than 1.");
            dead = true;
        }

        if (this.asteroidTemplates.Count < 1)
        {
            Debug.LogError("Stochasteroid requires at least one asteroid template.");
            dead = true;
        }

        if (this.asteroidMinScale <= 0 || this.asteroidMinScale >= this.asteroidMaxScale)
        {
            Debug.LogError("Stochasteroid invalid min or max asteroid scales.");
            dead = true;
        }

        if (this.asteroidMinVelocity <= 0 || this.asteroidMinVelocity >= this.asteroidMaxVelocity)
        {
            Debug.LogError("Stochasteroid invalid min or max asteroid velocities.");
            dead = true;
        }

        if (this.asteroidMinRotation < 0 || this.asteroidMinRotation > this.asteroidMaxRotation)
        {
            Debug.LogError("Stochasteroid invalid min or max asteroid rotation.");
            dead = true;
        }

        // Disable scene gravity.
        Physics.gravity = Vector3.zero;

        // Create asteroid objects and give them initial positions.
        for (int i = asteroidCount; i > 0; i--)
        {
            var asteroid = this.CreateAsteroid();
            this.PlaceAsteroid(asteroid);
            this.asteroidList.Add(asteroid);
        }
	}
	
	/// <summary>
    /// Per frame update. Performs asteroid update.
    /// </summary>
	void Update()
    {
        if (!dead)
        {
            foreach (var asteroid in this.asteroidList)
            {
                this.UpdateAsteroid(asteroid);
            }
        }
	}

    /// <summary>
    /// Creates a new Asteroid object randomly from the avaiable templates.
    /// </summary>
    /// <returns>A new asteroid.</returns>
    private GameObject CreateAsteroid()
    {
        var asteroid = Instantiate(this.asteroidTemplates[this.random.Next(0, this.asteroidTemplates.Count)]);

        // Randomly pick asteroid scale from allowed sizes.
        var scale = this.random.Next(this.asteroidMinScale, this.asteroidMaxScale);
        asteroid.transform.localScale = new Vector3(scale, scale, scale);

        return asteroid;
    }

    /// <summary>
    /// Randomly selects a point from one of the six cube faces. If a selected face is one that
    /// is deactivated by the trajectory params for the script then selects again.
    /// </summary>
    /// <returns>A point on one of the cube faces.</returns>
    private Vector3 RandomFacePoint()
    {
        // TODO: using scaling factor is a bit imprecise.
        var penExtents = this.transform.lossyScale / 2;
        var penLocation = this.transform.position;

        ChooseFace:
        switch (this.random.Next(0, 5))
        {
            case 0:
                if (!this.negativeZTrajectory)
                {
                    goto ChooseFace;
                }

                return new Vector3(
                    penLocation.x + this.random.Next((int)-penExtents.x, (int)penExtents.x),
                    penLocation.y + this.random.Next((int)-penExtents.y, (int)penExtents.y),
                    penLocation.z - penExtents.z);

            case 1:
                if (!this.positiveZTrajectory)
                {
                    goto ChooseFace;
                }

                return new Vector3(
                    penLocation.x + this.random.Next((int)-penExtents.x, (int)penExtents.x),
                    penLocation.y + this.random.Next((int)-penExtents.y, (int)penExtents.y),
                    penLocation.z + penExtents.z);
            case 2:
                if (!this.positiveYTrajectory)
                {
                    goto ChooseFace;
                }

                return new Vector3(
                    penLocation.x + this.random.Next((int)-penExtents.x, (int)penExtents.x),
                    penLocation.y - penExtents.y,
                    penLocation.z + this.random.Next((int)-penExtents.z, (int)penExtents.z));
            case 3:
                if (!this.negativeYTrajectory)
                {
                    goto ChooseFace;
                }

                return new Vector3(
                    penLocation.x + this.random.Next((int)-penExtents.x, (int)penExtents.x),
                    penLocation.y + penExtents.y,
                    penLocation.z + this.random.Next((int)-penExtents.z, (int)penExtents.z));
            case 4:
                if (!this.negativeXTrajectory)
                {
                    goto ChooseFace;
                }

                return new Vector3(
                    penLocation.x - penExtents.x,
                    penLocation.y + this.random.Next((int)-penExtents.x, (int)penExtents.x),
                    penLocation.z + this.random.Next((int)-penExtents.z, (int)penExtents.z));
            case 5:
                if (!this.positiveXTrajectory)
                {
                    goto ChooseFace;
                }

                return new Vector3(
                    penLocation.x + penExtents.x,
                    penLocation.y + this.random.Next((int)-penExtents.x, (int)penExtents.x),
                    penLocation.z + this.random.Next((int)-penExtents.z, (int)penExtents.z));
        }

        // Not possible but required for compilation.
        return Vector3.zero;
    }

    /// <summary>
    /// Checks if asteroid is in cube.
    /// </summary>
    /// <param name="asteroid">The asteroid.</param>
    /// <returns>True if the asteroid is in the cube.</returns>
    private bool IsAsteroidInCube(GameObject asteroid)
    {
        var penExtents = this.transform.lossyScale;
        var penLocation = this.transform.position;

        var penUpperBounds = penLocation + penExtents;
        var penLowerBounds = penLocation - penExtents;

        var asteroidLocation = asteroid.transform.position;
        if ((asteroidLocation.x > penUpperBounds.x || asteroidLocation.x < penLowerBounds.x) ||
            (asteroidLocation.y > penUpperBounds.y || asteroidLocation.y < penLowerBounds.y) ||
            (asteroidLocation.z > penUpperBounds.z || asteroidLocation.z < penLowerBounds.z))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Performs asteroid frame update. We use physics to move asteroids so this function
    /// merely picks a new trajectory for the asteroid if we reach the edge of the cube.
    /// </summary>
    /// <param name="asteroid"></param>
    private void UpdateAsteroid(GameObject asteroid)
    {
        if (!this.IsAsteroidInCube(asteroid))
        {
            this.PlaceAsteroid(asteroid);
        }
    }

    /// <summary>
    /// Picks a random trajectory through the cube of those allowed by the script
    /// parameters and applies a random force and rotation on the asteroid in that
    /// direction.
    /// </summary>
    /// <param name="asteroid">The asteroid object.</param>
    private void PlaceAsteroid(GameObject asteroid)
    {
        var pointA = this.RandomFacePoint();
        var pointB = this.RandomFacePoint();

        asteroid.transform.position = pointA;
        asteroid.transform.LookAt(pointB);

        var rigidBody = asteroid.GetComponent<Rigidbody>();

        if (rigidBody == null)
        {
            Debug.LogError("Stochasteroid asteroid template does not have rigid body.");
            this.dead = true;
            return;
        }

        rigidBody.velocity = Vector3.zero;
        rigidBody.AddForce((pointB - pointA) * this.random.Next(this.asteroidMinVelocity, this.asteroidMaxVelocity));
        rigidBody.AddTorque((UnityEngine.Random.rotation.eulerAngles 
            * this.random.Next(this.asteroidMinRotation, this.asteroidMaxRotation)) / 100);
    }
}
