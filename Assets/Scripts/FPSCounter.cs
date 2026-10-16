using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    // Cứ sau bao nhiêu giây thì cập nhật số hiển thị một lần
    [SerializeField] private float updateInterval = 0.5f;

    private float timer;        // Thời gian đã trôi qua từ lần cập nhật trước
    private int frameCount;     // Số Frame đã vẽ trong 1 khoảng TG
    private float fps;          // FPS trung bình trong 1 khoảng TG
    private float frameMs;      // Thời gian trung bình mỗi frame (ms)
    private GUIStyle style;
    private string label = "...";    // Chuỗi hiển thị, chỉ tạo lại khi số liệu đổi
    private Texture2D background;    // Nền đen mờ

    void Awake()
    {
        // Tắt VSync và gỡ giới hạn FPS để đo hiệu năng thật 
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        frameCount ++;

        if (timer >= updateInterval)
        {
            fps = frameCount / timer;
            frameMs = 1000f * timer / frameCount;
            timer = 0f;
            frameCount = 0;

            label = $"{fps:F0} FPS  |  {frameMs:F1} ms";
        }
    }

    void OnGUI()
    {
        if (style == null)
        {
            // Nền: texture 1x1 màu đen, alpha 0.5
            background = new Texture2D(1, 1);
            background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
            background.Apply();

            style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.background = background;
        }

        // Từ 90 FPS trở lên: xanh; dưới 90: đỏ nhạt
        style.normal.textColor = fps >= 90f ? Color.green : new Color(1f, 0.4f, 0.4f);

        GUI.Label(new Rect(10, 10, 210, 34), label, style);
    }

}