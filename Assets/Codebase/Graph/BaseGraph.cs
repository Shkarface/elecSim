using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseGraph : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] protected int Resolution = 100;
    [SerializeField] float UpperLimit = 10;
    [SerializeField] float LowerLimit = 5;
    [SerializeField] string Unit;
    [SerializeField] string AxisFormat = "n0";
    [Range(1, 2000)]
    [SerializeField] protected int UpdateDelay = 100;
    [SerializeField] bool UpdateLimits = true;

    [Header("Components")]
    [SerializeField] protected GraphLine[] GraphLines;
    [SerializeField] TextMeshProUGUI UpperMaxText;
    [SerializeField] TextMeshProUGUI UpperHalfText;
    [SerializeField] TextMeshProUGUI LowerHalfText;
    [SerializeField] TextMeshProUGUI LowerMaxText;

    private float _LastGraphUpperLimitUpdateTime;
    private float _LastGraphLowerLimitUpdateTime;

    protected virtual void Awake()
    {
        foreach (var graphLine in GraphLines)
        {
            graphLine.Initialize(Resolution);
            for (int i = 0; i < Resolution; i++)
                graphLine.LinePoints[i] = new Vector3((i / (float)(Resolution - 1)) * 10f, 0);
            graphLine.LineRenderer.material.color = graphLine.Color;
            graphLine.LineRenderer.positionCount = Resolution;
            graphLine.LineRenderer.SetPositions(graphLine.LinePoints);
        }
        UpdateGraphProperties();
        UpdateLoop();
    }
    protected virtual void NewValueRequest() { }
    protected async void UpdateLoop()
    {
        while (true)
        {
            NewValueRequest();
            await Task.Delay(UpdateDelay);
        }
    }

    protected void NewPoint(float[] values)
    {
        Assert.AreEqual(GraphLines.Length, values.Length);

        if (UpdateLimits)
        {
            float maxValue = 0, minValue = 0;

            foreach (var graphLine in GraphLines)
                for (int i = 1; i < Resolution; i++)
                {
                    if (graphLine.LineValues[i] > maxValue)
                        maxValue = graphLine.LineValues[i];
                    else if (graphLine.LineValues[i] < minValue)
                        minValue = graphLine.LineValues[i];
                }

            if (LowerLimit > minValue)
            {
                LowerLimit = minValue * -1.2f;
                _LastGraphLowerLimitUpdateTime = Time.time;
            }
            if (UpperLimit < maxValue)
            {
                UpperLimit = maxValue * 1.2f;
                _LastGraphUpperLimitUpdateTime = Time.time;
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > UpperLimit)
                {
                    UpperLimit = values[i] * 1.2f;
                    _LastGraphUpperLimitUpdateTime = Time.time;
                }
                else if (values[i] < -LowerLimit)
                {
                    LowerLimit = values[i] * -1.2f;
                    _LastGraphLowerLimitUpdateTime = Time.time;
                }
            }

            if (_LastGraphUpperLimitUpdateTime + (Resolution / 15f) < Time.time)
            {
                UpperLimit = maxValue * 1.2f;
                _LastGraphUpperLimitUpdateTime = Time.time;
            }
            else if (_LastGraphLowerLimitUpdateTime + (Resolution / 15f) < Time.time)
            {
                LowerLimit = minValue * -1.2f;
                _LastGraphLowerLimitUpdateTime = Time.time;
            }

            UpdateGraphProperties();
        }

        for (int lineIndex = 0; lineIndex < GraphLines.Length; lineIndex++)
        {
            GraphLine graphLine = GraphLines[lineIndex];
            for (int i = 1; i < Resolution; i++)
            {
                graphLine.LineValues[i - 1] = graphLine.LineValues[i];
                graphLine.LinePoints[i - 1] = new Vector3(graphLine.LinePoints[i - 1].x, NormalizeValue(graphLine.LineValues[i]));
            }

            graphLine.LineValues[Resolution - 1] = values[lineIndex];
            graphLine.LinePoints[Resolution - 1] = new Vector3(graphLine.LinePoints[Resolution - 1].x, NormalizeValue(graphLine.LineValues[Resolution - 1]));

            graphLine.LineRenderer.SetPositions(graphLine.LinePoints);
        }
    }

    protected virtual void UpdateGraphProperties()
    {
        if (UpperMaxText != null)
            UpperMaxText.text = $"{UpperLimit.ToString(AxisFormat)} {Unit}";
        if (UpperHalfText != null)
            UpperHalfText.text = $"{(UpperLimit / 2f).ToString(AxisFormat)} {Unit}";

        string lowerMax = string.Empty;
        string lowerHalf = string.Empty;

        if (LowerLimit != 0)
        {
            lowerMax = $"-{LowerLimit.ToString(AxisFormat)} {Unit}";
            lowerHalf = $"-{(LowerLimit / 2f).ToString(AxisFormat)} {Unit}";
        }

        if (LowerMaxText != null)
            LowerMaxText.text = lowerMax;
        if (LowerHalfText != null)
            LowerHalfText.text = lowerHalf;
    }

    protected float NormalizeValue(float value)
    {
        if (value > 0)
        {
            if (UpperLimit > 0)
                value = (value / UpperLimit) * (4.5f - GraphLines[0].LineRenderer.transform.localPosition.y);
            else value = 0;
        }
        else
        {
            if (LowerLimit > 0)
                value = (value / LowerLimit) * (4.5f + GraphLines[0].LineRenderer.transform.localPosition.y);
            else value = 0;
        }
        return value;
    }
}

[System.Serializable]
public class GraphLine
{
    public LineRenderer LineRenderer;
    public Color Color = Color.white;

    internal Vector3[] LinePoints { get; private set; }
    internal float[] LineValues { get; private set; }

    public void Initialize(int resolution)
    {
        LinePoints = new Vector3[resolution];
        LineValues = new float[resolution];
    }
}
