using UnityEngine;
using UnityEngine.UI;

public class GraphicSettings : MonoBehaviour
{
    public int targetFrameRate = 60;
    public int vSyncCount = 1;
    public Toggle vsyncCheckbox;
    public Slider frameRateSlider;
    public Text frameRateLabel;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = vSyncCount;
        vsyncCheckbox.isOn = vSyncCount > 0;
        frameRateSlider.value = targetFrameRate;
        frameRateLabel.text = "Max Framerate: " + targetFrameRate.ToString();
    }

    private void OnEnable()
    {
        vsyncCheckbox.onValueChanged.AddListener(OnVSyncChange);
        frameRateSlider.onValueChanged.AddListener(OnFrameRateChange);
        frameRateSlider.enabled = vSyncCount == 0;
    }

    private void OnDisable()
    {
        vsyncCheckbox.onValueChanged.RemoveListener(OnVSyncChange);
        frameRateSlider.onValueChanged.RemoveListener(OnFrameRateChange);
    }

    private void OnVSyncChange(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
        frameRateSlider.enabled = !value;
    }

    private void OnFrameRateChange(float value)
    {
        Application.targetFrameRate = (int)value;
        frameRateLabel.text = "Max Framerate: " + value.ToString();
    }

    // Update is called once per frame
    void Update() { }
}
