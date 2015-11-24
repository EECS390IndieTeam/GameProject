using UnityEngine;

/// <summary>
/// Effects for making menus and other game objects rotate continuously at a constant
/// rate and wiggle in perspective as the mouse is moved.
/// </summary>
public class MenuEffects : MonoBehaviour
{
    /// <summary>
    /// Enables perspective wiggling.
    /// </summary>
    public bool enableWiggle;

    /// <summary>
    /// Enables continous rotate.
    /// </summary>
    public bool enableRotate;

    /// <summary>
    /// Vector for continous rotation direction.
    /// </summary>
    public Vector3 rotationVector;

    /// <summary>
    /// Rate at which rotation occurs.
    /// </summary>
    public float degreesPerSecond;

    /// <summary>
    /// Max travel for perspective wiggle.
    /// </summary>
    public float maxWiggleDegrees = 25;

    /// <summary>
    /// Stores the original orientation for the perspective wiggle.
    /// </summary>
    private Vector3 originalRotation;

    /// <summary>
    /// Initialization.
    /// </summary>
    void Start()
    {
        Cursor.visible = true;
		this.originalRotation = gameObject.transform.rotation.eulerAngles;
    }

    /// <summary>
    /// Frame update.
    /// </summary>
    void Update()
    {
        // Rotate object to original orientation.
        transform.rotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, originalRotation);

        if (this.enableRotate)
        {
            gameObject.transform.Rotate(rotationVector * Time.timeSinceLevelLoad * degreesPerSecond);
        }

        if (this.enableWiggle)
        {
            // Float from -1.0f to 1.0f of mouse percentage distances from center.
            float xFromCenter = -((Input.mousePosition.x - (Screen.width / 2)) / (Screen.width / 2));
            float yFromCenter = (Input.mousePosition.y - (Screen.height / 2)) / (Screen.height / 2);

            gameObject.transform.Rotate(
                new Vector3(maxWiggleDegrees * yFromCenter,
                maxWiggleDegrees * xFromCenter,
                0));
        }
    }
}
