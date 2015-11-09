using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawningBlind : MonoBehaviour {
    public Canvas canvas;
    public Text text;
    public Image image;

    //private bool fading = false;
    private float fadeTime = 0f;
    private float targetFadeTime = 0f;
    private float startingAlpha = 0f;

    void Awake() {
        transform.SetParent(null, false);
        transform.position = Vector3.zero;
        enabled = false;
    }

    public Color TextColor {
        get {
            return text.color;
        }
        set {
            text.color = value;
        }
    }

    public Color BackgroundColor {
        get {
            return image.color;
        }
        set {
            image.color = value;
        }
    }

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

    public string Text {
        get {
            return text.text;
        }
        set {
            text.text = value;
        }
    }

    public void FadeOut(float time) {
        targetFadeTime = time;
        this.enabled = true;
        //fading = true;
        startingAlpha = Alpha;
        fadeTime = 0f;
    }

    public void Show() {
        Alpha = 1.0f;
        this.enabled = false;
        canvas.enabled = true;
    }

    public void Hide() {
        canvas.enabled = false;
        this.enabled = false;
    }

    void Update() {
        //if (!fading) return;
        fadeTime += Time.deltaTime;
        if (fadeTime >= targetFadeTime) {
            Hide();
        } else {
            float percent = fadeTime / targetFadeTime;
            Alpha = Mathf.Lerp(startingAlpha, 0.0f, percent);
        }
    }
}
