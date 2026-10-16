using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private SphereMeshGenerator leftSphere;
    [SerializeField] private SphereMeshGenerator rightSphere;
    [SerializeField] private DisplacementControl leftDisp;
    [SerializeField] private DisplacementControl rightDisp;

    // Tham chiếu để reset và đọc vị trí đầu
    [SerializeField] private HeadMover headMover;

    // Số cột (kinh độ) của từng mức. Số hàng = 1/2 số cột
    private readonly int[] columns = { 64, 128, 256, 512, 1024, 2048, 4096 };
    private bool displacementOn = true;

    // Bảng màu
    private static readonly Color accent = new Color(0.12f, 0.55f, 0.95f);         // xanh dương chủ đạo
    private static readonly Color panelBg = new Color(0.06f, 0.07f, 0.09f, 0.80f); // nền panel
    private static readonly Color btnBg = new Color(0.18f, 0.20f, 0.24f, 0.90f);   // nút thường
    private static readonly Color btnHover = new Color(0.27f, 0.30f, 0.36f, 0.95f);// nút khi rê chuột
    private static readonly Color onBg = new Color(0.16f, 0.62f, 0.38f, 1f);       // Displacement ON
    private static readonly Color offBg = new Color(0.65f, 0.25f, 0.25f, 1f);      // Displacement OFF

    // Các style, tạo một lần ở lần OnGUI đầu tiên
    private GUIStyle panelStyle, buttonStyle, selectedStyle, infoStyle, toggleOnStyle, toggleOffStyle;

    // Tạo texture 1x1 một màu, dùng làm nền cho style
    private Texture2D MakeTex(Color c)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, c);
        tex.Apply();
        return tex;
    }

    // Tạo style nút: màu thường + màu khi rê chuột/bấm
    private GUIStyle MakeButtonStyle(Color normal, Color hover)
    {
        GUIStyle s = new GUIStyle(GUI.skin.button);
        s.normal.background = MakeTex(normal);
        s.hover.background = MakeTex(hover);
        s.active.background = MakeTex(hover);
        s.normal.textColor = Color.white;
        s.hover.textColor = Color.white;
        s.active.textColor = Color.white;
        s.border = new RectOffset(0, 0, 0, 0);   // bỏ viền 9-slice mặc định
        s.fontSize = 16;
        s.fontStyle = FontStyle.Bold;
        s.alignment = TextAnchor.MiddleCenter;
        s.fixedHeight = 30;
        return s;
    }

    private void InitStyles()
    {
        panelStyle = new GUIStyle(GUI.skin.box);
        panelStyle.normal.background = MakeTex(panelBg);

        buttonStyle = MakeButtonStyle(btnBg, btnHover);
        selectedStyle = MakeButtonStyle(accent, accent * 1.15f);   // nút mức đang dùng
        toggleOnStyle = MakeButtonStyle(onBg, onBg * 1.15f);
        toggleOffStyle = MakeButtonStyle(offBg, offBg * 1.15f);

        infoStyle = new GUIStyle(GUI.skin.label);
        infoStyle.fontSize = 18;
        infoStyle.alignment = TextAnchor.MiddleCenter;
        infoStyle.normal.textColor = new Color(0.8f, 0.85f, 0.9f);
        infoStyle.fixedHeight = 30;
    }

    void OnGUI()
    {
        if (panelStyle == null) InitStyles();

        // Panel căn giữa, sát mép trên (cao 120 để đủ 3 hàng)
        float w = Mathf.Max(Screen.width * 0.5f, 600f);
        Rect panel = new Rect((Screen.width - w) / 2f, 10, w, 120);

        // Nền + vạch accent ở mép trên
        GUI.Box(panel, GUIContent.none, panelStyle);
        GUI.color = accent;
        GUI.DrawTexture(new Rect(panel.x, panel.y, panel.width, 3), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Vùng nội dung, thụt vào so với mép panel
        GUILayout.BeginArea(new Rect(panel.x + 12, panel.y + 10, panel.width - 24, panel.height - 18));

        // Hàng 1: các nút mức mesh
        GUILayout.BeginHorizontal();
        for (int i = 0; i < columns.Length; i++)
        {
            int lo = columns[i];
            int la = lo / 2;
            bool isCurrent = leftSphere.loSegments == lo;

            if (GUILayout.Button($"{lo}×{la}", isCurrent ? selectedStyle : buttonStyle))
            {
                leftSphere.Rebuild(lo, la);
                rightSphere.Rebuild(lo, la);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        // Hàng 2: thông tin mesh bên trái; nút Reset và nút Displacement bên phải
        GUILayout.BeginHorizontal();
        int vertices = (leftSphere.loSegments + 1) * (leftSphere.laSegments + 1);
        GUILayout.Label($"Mesh {leftSphere.loSegments} × {leftSphere.laSegments}  ·  {vertices:N0} đỉnh / sphere", infoStyle);
        GUILayout.FlexibleSpace();

        // Nút Reset vị trí quan sát
        if (GUILayout.Button("Reset vị trí", buttonStyle, GUILayout.Width(120)))
        {
            if (headMover != null) headMover.ResetPosition();
        }
        GUILayout.Space(8);

        string text = displacementOn ? "Displacement: ON" : "Displacement: OFF";
        if (GUILayout.Button(text, displacementOn ? toggleOnStyle : toggleOffStyle, GUILayout.Width(170)))
        {
            displacementOn = !displacementOn;
            leftDisp.SetDisplacement(displacementOn);
            rightDisp.SetDisplacement(displacementOn);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        // Hàng 3: vị trí đầu so với gốc
        if (headMover != null)
        {
            Vector3 o = headMover.OffsetFromOrigin;
            GUILayout.Label($"Vị trí đầu (so với gốc):  X: [{o.x:F3}]   Y: [{o.y:F3}]   Z: [{o.z:F3}]  m", infoStyle);
        }

        GUILayout.EndArea();
    }
}