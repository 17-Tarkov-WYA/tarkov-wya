# Tarkov - Where You At - Planner

[![status](https://img.shields.io/badge/status-%E9%96%8B%E7%99%BC%E4%B8%AD-orange)](../../docs/roadmap.md)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-14-239120)](https://learn.microsoft.com/dotnet/csharp/)
[![WPF](https://img.shields.io/badge/WPF-.NET%2010-512BD4)](https://learn.microsoft.com/dotnet/desktop/wpf/)
[![platform](https://img.shields.io/badge/platform-Windows-0078D4)](#如何開始-how-to-start)

你進行到哪裡：Windows 桌面程式，回答「我的任務做到哪？這場該帶什麼？」。資料由 [client](../client) 提供。

> 目前只有設計，尚未建立程式。整體設計見 [`docs/architecture.md`](../../docs/architecture.md)。

## 規劃功能 (Features)

- 📜 任務進度：由 client 讀取遊戲紀錄檔自動更新，也可以手動修正
- 🎒 出發前清單：選好地圖後，彙整這場該帶的鑰匙、物品與裝備
- 🔑 鑰匙管理：記錄擁有的鑰匙，以及哪張地圖、哪個任務需要
- 🏠 藏身處需求：列出升級需要的物品

## 技術棧 (Tech Stack)

- **.NET 10** ([官網](https://dotnet.microsoft.com) | [GitHub](https://github.com/dotnet/runtime))：執行環境，長期支援（LTS）版本。
- **C#** ([官網](https://learn.microsoft.com/dotnet/csharp/) | [GitHub](https://github.com/dotnet/csharplang))：開發語言。
- **WPF** ([官網](https://learn.microsoft.com/dotnet/desktop/wpf/) | [GitHub](https://github.com/dotnet/wpf))：Windows 桌面 UI 框架，支援置頂、多螢幕等桌面操作。

## 如何開始 (How to start)

需求（預定）：Windows 10／11、[.NET SDK 10](https://dotnet.microsoft.com/download)

尚未建立程式，執行方式會在建立骨架後補上。
