# 0002. 技術選擇

- 狀態：已採用
- 日期：2026-10-24

## 背景

從零重新開發，需要為每個 app 選擇技術。需求：client 與 Windows 的整合度要最高、效能要好、方便使用者開始使用，未來要導入圖像辨識（英文、繁中、簡中）；planner 是電腦桌面程式，不需要支援手機；map 維持 Vue；server 要能部署在 Render、Google Cloud Run 這類註冊即可使用的平台。

## 決策

| 元件 | 技術 |
|---|---|
| client | C# · .NET 10 |
| planner | C# · WPF（.NET 10） |
| map | Vue 3 · Vite · TypeScript |
| server | TypeScript · Node.js（附 Dockerfile） |
| protocol | JSON Schema，產生 C# 與 TypeScript 型別 |

## 理由

- **client 用 C#**：可直接使用 Windows 原生 API（檔案監聽、按鍵輸入、內建 OCR、畫面擷取、系統匣）；以事件驅動監聽，常駐時幾乎不耗資源；可發佈成單一執行檔。考慮過 Python（舊版）、TypeScript、Go／Rust
- **.NET 10**：長期支援（LTS）版本。.NET 8 的支援將於 2026 年 11 月結束
- **planner 用 WPF**：完整的桌面體驗，與 client 共用語言；比 WinUI 3 成熟、打包簡單；考慮過 WinUI 3、Tauri + Vue
- **server 用 TypeScript**：各大雲端平台原生支援 Node.js，記憶體用量小；.NET 在部分平台需要自行準備 Docker。考慮過 C#
- **protocol 用 JSON Schema**：與語言無關，C# 與 TypeScript 共用同一份規格

## 影響

- 開發環境需要 Windows 與 .NET SDK 10
- 跨語言的資料格式一律由 protocol 產生型別，不手寫
