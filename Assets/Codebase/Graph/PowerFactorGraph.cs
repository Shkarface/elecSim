using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class PowerFactorGraph : BaseGraph
{
    public GraphLine GraphLine => GraphLines[0];

    [Header("Power")]
    public ElectricNode Node;
    public TextMeshProUGUI StateText;
    public ThresholdColor GoodThreshold = new ThresholdColor(0.95f, Color.green);
    public ThresholdColor NormalThreshold = new ThresholdColor(0.85f, Color.yellow);
    public ThresholdColor BadThreshold = new ThresholdColor(0.6f, Color.red);

    private float[] _PowerFactorValues = new float[1];

    protected override void Awake()
    {
        Assert.AreEqual(1, GraphLines.Length);
        Assert.IsNotNull(StateText);
        base.Awake();
    }

    protected override void NewValueRequest()
    {
        _PowerFactorValues[0] = (float)Node.PowerFactor;
        NewPoint(_PowerFactorValues);
        if (_PowerFactorValues[0] <= 0)
            StateText.text = string.Empty;
        else if (_PowerFactorValues[0] > GoodThreshold.Threshold)
        {
            StateText.text = $"Good - {_PowerFactorValues[0].ToString("p0")}";
            StateText.color = GoodThreshold.Color;
        }
        else if (_PowerFactorValues[0] > NormalThreshold.Threshold)
        {
            StateText.text = $"Normal - {_PowerFactorValues[0].ToString("p0")}";
            StateText.color = NormalThreshold.Color;
        }
        else
        {
            StateText.text = $"Bad - {(int)(_PowerFactorValues[0] * 100)}%";
            StateText.color = BadThreshold.Color;
        }
    }

    [System.Serializable]
    public struct ThresholdColor
    {
        public float Threshold;
        public Color Color;

        public ThresholdColor(float threshold, Color color)
        {
            Threshold = threshold;
            Color = color;
        }
    }
}
