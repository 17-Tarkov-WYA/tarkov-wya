# Tarkov - Where You At - Client

[![status](https://img.shields.io/badge/status-%E9%96%8B%E7%99%BC%E4%B8%AD-orange)](../../docs/roadmap.md)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-14-239120)](https://learn.microsoft.com/dotnet/csharp/)
[![platform](https://img.shields.io/badge/platform-Windows-0078D4)](#如何開始-how-to-start)

你電腦上的本機中樞：在玩家的 Windows 電腦上常駐執行，負責收集資料並提供給 [planner](../planner)（之後也會提供給 map 與 server）。

> 目前只有設計，尚未建立程式。整體設計見 [`docs/architecture.md`](../../docs/architecture.md)。

## 規劃功能 (Features)

- 📜 讀取遊戲紀錄檔（log），偵測任務等狀態變化
- 🎮 觸發截圖並從檔名讀取座標（map 恢復開發後使用；會產生截圖音效）
- 🔌 本機中樞：讓 planner、map 取得資料
- 🔒 分享設定：決定哪些主題要公開給隊友

## 設計原則 (Design Principles)

- **效能優先**：監聽資料夾變化（事件驅動），不定時輪詢
- **貼合 Windows**：系統匣常駐、單一執行檔發佈
- **不碰遊戲本體**：不讀取記憶體、不修改遊戲檔案

## 技術棧 (Tech Stack)

- **.NET 10** ([官網](https://dotnet.microsoft.com) | [GitHub](https://github.com/dotnet/runtime))：執行環境，長期支援（LTS）版本，可發佈成單一執行檔。
- **C#** ([官網](https://learn.microsoft.com/dotnet/csharp/) | [GitHub](https://github.com/dotnet/csharplang))：開發語言，可直接使用 Windows 原生 API（檔案監聽、按鍵輸入、未來的 OCR）。

## 如何開始 (How to start)

需求（預定）：Windows 10／11、[.NET SDK 10](https://dotnet.microsoft.com/download)

尚未建立程式，執行方式會在建立骨架後補上。
