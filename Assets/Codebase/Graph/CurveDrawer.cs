using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CurveDrawer : MonoBehaviour
{
    [Header("Graph Properties")]
    public int Width = 512;
    public int Height = 256;
    public int ValueResolution = 100;
    public bool ShowVerticalLine;
    public bool ShowHorizontalLine;
    [Range(0, 0.9f)]
    public float GuidelineHorizontalSplit = 0.5f;
    [Range(1, 10)]
    public int GuidelineWidth = 2;
    public Color GuidelineColor = Color.white;
    public RawImage Image;

    [Header("Range Properties")]
    public float UpperLimit = 10f;
    public float LowerLimit = -10f;

    private Texture2D _Texture;
    Color[] _Colors;
    float[] _Values;

    protected void Awake()
    {
        CreateTexture();
        Image.texture = _Texture;
        UpdateLoop();
    }

    protected void OnValidate()
    {
        Width = Mathf.NextPowerOfTwo(Width);
        Height = Mathf.NextPowerOfTwo(Height);
        ValueResolution = Mathf.Min(Mathf.NextPowerOfTwo(ValueResolution), Width);
        UpperLimit = Mathf.Max(UpperLimit, 0);
        LowerLimit = Mathf.Min(LowerLimit, 0);
    }

    [ContextMenu("Create")]
    protected void CreateTexture()
    {
        _Texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);

        _Colors = new Color[Width * Height];
        _Values = new float[ValueResolution + 1];
        _Texture.SetPixels(_Colors);

        PostUpdate();
    }
    protected int NormalizeValue(float value)
    {
        int start = (int)(GuidelineHorizontalSplit * Height);
        if (value == 0)
            return start;
        else if (value > 0)
            return (int)(((value / UpperLimit) * ((1f - GuidelineHorizontalSplit) * Height)) + start);
        else
        {
            return start - (int)((value / LowerLimit) * start);
        }
    }

    protected async void UpdateLoop()
    {
        while (_Texture != null)
        {
            _Colors = new Color[Width * Height];
            for (int i = 1; i < _Values.Length; i++)
            {
                _Values[i - 1] = _Values[i];
                if (i == _Values.Length - 1)
                    _Values[i] = Mathf.Cos(Time.time * 10) * UpperLimit * 0.8f;
                UpperLimit = Mathf.Max(UpperLimit, _Values[i] * 1.1f);
                LowerLimit = Mathf.Min(LowerLimit, _Values[i] * 1.1f);
            }


            int step = Mathf.CeilToInt(Width / (float)(_Values.Length - 1));

            int currentX = 0;
            for (int i = 0; i < _Values.Length - 1; i++)
            {
                int nextX = Mathf.Clamp(currentX + step, 0, Width - 1);
                var startColor = (Mathf.Abs(_Values[i]) > UpperLimit / 2f) ? Color.green : Color.red;
                var endColor = (Mathf.Abs(_Values[i + 1]) > UpperLimit / 2f) ? Color.green : Color.red;

                var currentY = NormalizeValue(_Values[i]);
                var nextY = NormalizeValue(_Values[i + 1]);
                //Debug.Log($"v0 = {_Values[x]} - y0 = {startY},v1 = {_Values[x + 1]} - y1 = {nextY}");
                Line(currentX, currentY, nextX, nextY, startColor, endColor);
                currentX += step;
            }
            PostUpdate();
            await Task.Delay(100);
        }
    }
    protected void PostUpdate()
    {
        if (_Texture == null) return;
        _Texture.SetPixels(_Colors);
        if (ShowVerticalLine)
        {
            Color[] verticalLine = new Color[2 * Height];
            for (int row = 0; row < Height; row++)
                for (int column = 0; column < GuidelineWidth; column++)
                    verticalLine[(row * GuidelineWidth) + column] = GuidelineColor;
            _Texture.SetPixels(0, 0, GuidelineWidth, Height, verticalLine);
        }
        if (ShowHorizontalLine)
        {
            Color[] horizontalLine = new Color[2 * Width];
            for (int column = 0; column < Width; column++)
                for (int row = 0; row < GuidelineWidth; row++)
                    horizontalLine[(column * GuidelineWidth) + row] = GuidelineColor;

            _Texture.SetPixels(0, (int)(Height * GuidelineHorizontalSplit) - (GuidelineWidth / 2), Width, GuidelineWidth, horizontalLine);
        }

        _Texture.Apply();
    }
    private void Line(int startX, int startY, int endX, int endY, Color startColor, Color endColor)
    {
        var dy = endY - startY;
        var dx = endX - startX;
        int stepy = 1, stepx = 1;
        int currentX = startX, currentY = startY;
        int fraction;

        if (dy < 0)
        {
            dy = -dy;
            stepy = -1;
        }

        if (dx < 0)
        {
            dx = -dx;
            stepx = -1;
        }
        dy <<= 1;
        dx <<= 1;

        _Colors[(currentY * Width) + currentX] = startColor;
        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (currentX != endX)
            {
                if (fraction >= 0)
                {
                    currentY += stepy;
                    fraction -= dx;
                }
                currentX += stepx;
                fraction += dy;
                _Colors[(currentY * Width) + currentX] = Color.Lerp(startColor, endColor, (((float)currentX - startX) / (endX - startX)));
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (currentY != endY)
            {
                if (fraction >= 0)
                {
                    currentX += stepx;
                    fraction -= dy;
                }
                currentY += stepy;
                fraction += dx;
                _Colors[(currentY * Width) + currentX] = Color.Lerp(startColor, endColor, (((float)currentX - startX) / (endX - startX))); ;
            }
        }
    }
}
