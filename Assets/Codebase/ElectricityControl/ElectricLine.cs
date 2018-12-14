using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(ElectricLine))]
public class ElectricLineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (targets.Length == 1)
        {
            ElectricLine obj = target as ElectricLine;
            EditorGUILayout.HelpBox($"Voltage:\t{obj.StartingVoltage} V{Environment.NewLine}Current:\t{obj.Current.Magnitude} A{Environment.NewLine}Phase:\t{obj.Current.Phase * (Math.PI / 180.0)}", MessageType.Info);
        }
    }
}
#endif

[RequireComponent(typeof(LineRenderer))]
public class ElectricLine : MonoBehaviour
{
    public static readonly Color LiveColor = Color.red;
    public static readonly Color OffColor = Color.gray;

    /// <summary>
    /// OldValue, NewValue
    /// </summary>
    public event UnityAction<Complex, Complex> OnCurrentChanged;

    protected LineRenderer LineRenderer;
    public LineData WireType;
    public ElectricNode Destination;
    public Subline[] Sublines;

    private float[] _Distances;

    private float _StartingVoltage;
    public float StartingVoltage
    {
        get
        {
            return _StartingVoltage;
        }
        set
        {
            Assert.IsNotNull(LineRenderer);
            _StartingVoltage = value;
            var absVoltage = Math.Abs(_StartingVoltage);
            var sign = (_StartingVoltage > 0) ? 1 : -1;
            LineRenderer.material.color = (absVoltage > 0 ? LiveColor : OffColor);

            if (Destination != null)
                Destination.Voltage = (absVoltage > 0) ? _StartingVoltage - (VoltageDrop * sign) : 0;
            if (Sublines.Length > 0)
            {
                foreach (var subline in Sublines)
                    subline.Line.StartingVoltage = (absVoltage > 0) ? _StartingVoltage - (GetVoltageDropAtPoint(subline.PointIndex) * sign) : 0;
            }
        }
    }

    public float EndVoltage => StartingVoltage - VoltageDrop;

    private Complex _Current;
    public Complex Current
    {
        get { return _Current; }
        set
        {
            if (_Current != value)
            {
                Complex oldValue = _Current;
                _Current = value;
                OnCurrentChanged?.Invoke(oldValue, _Current);
            }
        }
    }

    public float LocalLength
    {
        get;
        private set;
    }

    public float VoltageDrop
    {
        get
        {
            return WireType.ResistancePerMeter * LocalLength;
        }
    }

    protected void Awake()
    {
        LineRenderer = GetComponent<LineRenderer>();
        int pointsCount = LineRenderer.positionCount;
        _Distances = new float[pointsCount - 1];
        Vector3[] points = new Vector3[pointsCount];
        LineRenderer.GetPositions(points);
        for (int i = 0; i < pointsCount - 1; i++)
        {
            _Distances[i] = Vector3.Distance(points[i], points[i + 1]);
            LocalLength += _Distances[i];
        }

        if (Destination != null)
            Destination.OnCurrentChanged += CurrentChanged;
        foreach (var subLine in Sublines)
            subLine.Line.OnCurrentChanged += CurrentChanged;
    }

    private void CurrentChanged(Complex oldValue, Complex newValue)
    {
        Complex delta = newValue - oldValue;
        Current += delta;
    }

    public float GetVoltageDropAtPoint(int pointIndex)
    {
        float length = 0;
        for (int i = 0; i < pointIndex; i++)
            length += _Distances[i];
        return WireType.ResistancePerMeter * length;
    }

    [System.Serializable]
    public class Subline
    {
        public ElectricLine Line;
        public int PointIndex;
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        LineRenderer = GetComponent<LineRenderer>();
    }
#endif
}
