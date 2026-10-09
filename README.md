<div align="center">

# 🥽 Stereo Panorama Viewer

### Trình xem Stereo Panorama Photosphere / Moviesphere với **thị sai độ sâu (Depth Parallax)**

![Unity](https://img.shields.io/badge/Unity-6000.3.25f1-000000?style=for-the-badge&logo=unity&logoColor=white)
![URP](https://img.shields.io/badge/Pipeline-URP-1E88E5?style=for-the-badge)
![Shader Graph](https://img.shields.io/badge/Shader%20Graph-Displacement-8E24AA?style=for-the-badge)
![C#](https://img.shields.io/badge/C%23-Visual%20Studio-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Blender](https://img.shields.io/badge/Blender-Panorama%20%2B%20Depth-F5792A?style=for-the-badge&logo=blender&logoColor=white)
![Git](https://img.shields.io/badge/Git-GitHub-181717?style=for-the-badge&logo=github&logoColor=white)
![Target](https://img.shields.io/badge/Target-90%2B%20FPS-E53935?style=for-the-badge)

**Bài tập lớn cuối kỳ · Môn _Xây dựng ứng dụng thực tế ảo_ · Đề tài 29**

</div>

---

## 📚 Thông tin học phần

| Mục                         | Nội dung                     |
| --------------------------- | ---------------------------- |
| 📖 **Môn học**              | Xây dựng ứng dụng thực tế ảo |
| 👨‍🏫 **Giảng viên hướng dẫn** | ThS. Đỗ Trí Nhựt             |
| 👨‍🎓 **Sinh viên thực hiện**  | Nguyễn Tuấn Anh              |
| 🪪 **MSSV**                 | 23050118                     |

---

## 🌌 Giới thiệu

Ảnh panorama 360° thông thường chỉ cho phép xoay đầu (**3 bậc tự do – 3DoF**). Khi người xem dịch chuyển đầu, cảnh không thay đổi nên thiếu cảm giác chiều sâu thật.

Dự án xây dựng một trình xem panorama 360° có:

|     | Tính năng                            | Mô tả                                                                                                                                     |
| --- | ------------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------- |
| 👀  | **Stereo**                           | Hiển thị cặp ảnh trái/phải lên hai mắt để tạo cảm giác 3D.                                                                                |
| 🌊  | **Thị sai độ sâu (Motion Parallax)** | Dùng bản đồ độ sâu (depth map) để dịch chuyển các đỉnh (vertex) của mặt cầu, giúp vật gần dịch chuyển nhiều hơn vật xa khi đầu tịnh tiến. |

> 📘 Cơ sở lý thuyết tham khảo từ giáo trình _Virtual Reality_ của **Steven M. LaValle** (các chương về thị giác, quang học và tạo cảm giác chiều sâu).

---

## 🛠️ Công nghệ sử dụng

| Công nghệ                                    | Vai trò                                                 |
| -------------------------------------------- | ------------------------------------------------------- |
| 🎮 **Unity 6 LTS** (`6000.3.25f1`) + **URP** | Nền tảng và pipeline render (Universal Render Pipeline) |
| 🧩 **Shader Graph**                          | Shader displacement theo depth map                      |
| 💻 **C#** (Visual Studio)                    | Mô hình hóa mặt cầu, điều khiển camera và giao diện     |
| 🎨 **Blender**                               | Tạo ảnh panorama màu và depth map đầu vào               |
| 🔀 **Git / GitHub**                          | Quản lý phiên bản                                       |

---

## 📁 Cấu trúc thư mục

```
StereoPanoramaViewer/
├── 📂 Assets/
│   ├── 🎬 Scenes/        Scene của ứng dụng
│   ├── 📜 Scripts/       Mã nguồn C# (tạo mesh cầu, camera, UI, ...)
│   ├── ✨ Shaders/       Shader Graph (displacement theo depth map)
│   ├── 🎨 Materials/     Material dùng shader trên
│   ├── 🖼️ Textures/      Ảnh panorama màu (trái/phải) và depth map
│   ├── 🧱 Prefabs/       Đối tượng dựng sẵn để tái sử dụng (mặt cầu, rig camera, ...)
│   └── ⚙️ Settings/      Cấu hình URP do Unity tạo
├── 📦 Packages/          Danh sách package của project
├── 🔧 ProjectSettings/   Cài đặt project Unity
├── 🚫 .gitignore         Bỏ qua Library, Temp, Logs, ... do Unity tự sinh
└── 📝 README.md
```

---

## 🚀 Hướng dẫn chạy

> 🚧 `Đang cập nhật .....`

---

## 🗺️ Tiến độ

📄 [**File kế hoạch chi tiết**](./Docs/chi-tiet-ke-hoach.md)

| Chặng | Nội dung                                                        |  Trạng thái   |
| :---: | --------------------------------------------------------------- | :-----------: |
| **0** | Thiết lập môi trường (Unity URP, Visual Studio, Git/GitHub)     | ✅ Hoàn thành |
| **1** | Dữ liệu đầu vào từ Blender (panorama, cặp trái/phải, depth map) |  ⬜ Chưa làm  |
| **2** | Trình xem 360° cơ bản (mặt cầu, điều khiển nhìn quanh)          |  ⬜ Chưa làm  |
| **3** | Stereo (hai camera, chia viewport)                              |  ⬜ Chưa làm  |
| **4** | Shader displacement theo độ sâu (Shader Graph)                  |  ⬜ Chưa làm  |
| **5** | Tịnh tiến đầu, giao diện cài đặt, đo FPS                        |  ⬜ Chưa làm  |
| **6** | Báo cáo                                                         |  ⬜ Chưa làm  |

---

## 🎯 Tiêu chí đánh giá (tham khảo)

|  Tỷ trọng  | Tiêu chí                                               |
| :--------: | ------------------------------------------------------ |
| 🟦 **40%** | Độ chính xác kỹ thuật / cảm thụ                        |
| 🟩 **40%** | Kiến trúc phần mềm và hiệu năng (mục tiêu **90+ FPS**) |
| 🟨 **20%** | Báo cáo kỹ thuật                                       |

---

<div align="center">

⭐ _Nếu thấy dự án thú vị, hãy để lại một ngôi sao nhé!_ ⭐

**Made with ❤️ và Unity bởi Nguyễn Tuấn Anh · 23050118**

**File markdown được viết bởi Claude**

</div>
