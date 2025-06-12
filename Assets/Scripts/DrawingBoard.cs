using UnityEngine;
using UnityEngine.UI;

public enum PlayerRole { Artist, Guesser }

public class DrawingBoard : MonoBehaviour
{
    [Header("Settings")]
    public int textureWidth = 1024;
    public int textureHeight = 1024;
    public int brushSize = 8;
    public Color brushColor = Color.black;
    public RawImage drawArea;

    [Header("Player Role")]
    public PlayerRole role = PlayerRole.Guesser; // Default to guesser

    private Texture2D drawTexture;
    private Vector2 lastPos;
    private bool isDrawing = false;

    void Start()
    {
        drawTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;
        ClearTexture();
        drawArea.texture = drawTexture;
    }

    void Update()
    {
        if (role != PlayerRole.Artist)
            return;

        if (Input.GetMouseButtonDown(0) && IsMouseOverDrawArea())
        {
            isDrawing = true;
            lastPos = GetMouseLocalPosition(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }

        if (isDrawing && IsMouseOverDrawArea())
        {
            Vector2 currentPos = GetMouseLocalPosition(Input.mousePosition);
            DrawLine((int)lastPos.x, (int)lastPos.y, (int)currentPos.x, (int)currentPos.y, brushColor, brushSize);

            // 🔌 Send this stroke to the server
            _ = NetworkingClient.Instance.SendDrawData(lastPos, currentPos, brushColor, brushSize);

            lastPos = currentPos;
            drawTexture.Apply();
        }
    }

    bool IsMouseOverDrawArea()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(drawArea.rectTransform, Input.mousePosition, null);
    }

    Vector2 GetMouseLocalPosition(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawArea.rectTransform, screenPos, null, out Vector2 localPoint);
        Vector2 rectSize = drawArea.rectTransform.rect.size;
        localPoint += rectSize * 0.5f;

        float x = localPoint.x / rectSize.x * textureWidth;
        float y = localPoint.y / rectSize.y * textureHeight;

        return new Vector2(
            Mathf.Clamp(x, 0, textureWidth - 1),
            Mathf.Clamp(y, 0, textureHeight - 1)
        );
    }

    public void DrawLine(int x0, int y0, int x1, int y1, Color color, int size)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            DrawBrush(x0, y0, color, size);
            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }

    void DrawBrush(int cx, int cy, Color color, int size)
    {
        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                int px = cx + x;
                int py = cy + y;

                if (px >= 0 && py >= 0 && px < textureWidth && py < textureHeight)
                {
                    drawTexture.SetPixel(px, py, color);
                }
            }
        }
    }

    public void ClearTexture()
    {
        Color32[] pixels = new Color32[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;

        drawTexture.SetPixels32(pixels);
        drawTexture.Apply();
    }

    /// <summary>
    /// Used by remote users to draw incoming strokes from the network.
    /// </summary>
    public void DrawRemoteStroke(Vector2 start, Vector2 end, Color color, int size)
    {
        DrawLine((int)start.x, (int)start.y, (int)end.x, (int)end.y, color, size);
        drawTexture.Apply();
    }
}
