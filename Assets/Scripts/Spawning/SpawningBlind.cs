using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawningBlind : MonoBehaviour {
    public Canvas canvas;
    public Text text;
    public Image image;

    private float fadeTime = 0f;
    private float targetFadeTime = 0f;
    private float startingAlpha = 0f;
    private float endingAlpha = 0f;

    public static SpawningBlind instance = null;

    void Awake() {
        if (instance != null) {
            DestroyImmediate(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
            enabled = false;
            Hide();
        }

    }

    /// <summary>
    /// get or set the text color
    /// </summary>
    public Color TextColor {
        get {
            return text.color;
        }
        set {
            text.color = value;
        }
    }

    /// <summary>
    /// get or set the background color
    /// </summary>
    public Color BackgroundColor {
        get {
            return image.color;
        }
        set {
            image.color = value;
        }
    }

    /// <summary>
    /// Get or set the transparency of the blind (0-1)
    /// </summary>
    public float Alpha {
        get {
            return BackgroundColor.a;
        }
        set {
            Color temp = BackgroundColor;
            temp.a = value;
            BackgroundColor = temp;
            temp = TextColor;
            temp.a = value;
            TextColor = temp;
        }
    }

    /// <summary>
    /// Get or set the text on the blind
    /// </summary>
    public string Text {
        get {
            return text.text;
        }
        set {
            text.text = value;
        }
    }

    /// <summary>
    /// Shorthand for FadeTo(time, 0.0f)
    /// </summary>
    /// <param name="time"></param>
    public void FadeOut(float time) {
        FadeTo(time, 0.0f);
    }

    /// <summary>
    /// Shorthand for FadeTo(time, 1.0f)
    /// </summary>
    /// <param name="time"></param>
    public void FadeIn(float time) {
        FadeTo(time, 1.0f);
    }

    /// <summary>
    /// Fades to the give alpha value over the given amount of time
    /// </summary>
    /// <param name="time"></param>
    /// <param name="alpha"></param>
    public void FadeTo(float time, float endAlpha) {
        canvas.enabled = true;
        targetFadeTime = time;
        this.enabled = true;
        startingAlpha = Alpha;
        endingAlpha = endAlpha;
        fadeTime = 0f;
    }

    /// <summary>
    /// Cancels the current fade and makes the blind fully opaque
    /// </summary>
    public void Show() {
        Alpha = 1.0f;
        this.enabled = false;
        canvas.enabled = true;
    }

    /// <summary>
    /// Cancels the current fade and hides the blind.  Use this instead of setting Alpha to zero
    /// </summary>
    public void Hide() {
        canvas.enabled = false;
        this.enabled = false;
    }

    void Update() {
        fadeTime += Time.deltaTime;
        if (fadeTime >= targetFadeTime) {
			Alpha = endingAlpha;
			this.enabled = false;;
        } else {
            float percent = fadeTime / targetFadeTime;
            Alpha = Mathf.Lerp(startingAlpha, endingAlpha, percent);
        }
    }


}
