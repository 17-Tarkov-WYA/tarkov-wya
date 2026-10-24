# Tarkov - Where You At

[![license](https://img.shields.io/badge/license-PolyForm%20NC%201.0.0-blue)](LICENSE)
[![status](https://img.shields.io/badge/status-%E9%96%8B%E7%99%BC%E4%B8%AD-orange)](docs/roadmap.md)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-14-239120)](https://learn.microsoft.com/dotnet/csharp/)
[![WPF](https://img.shields.io/badge/WPF-.NET%2010-512BD4)](https://learn.microsoft.com/dotnet/desktop/wpf/)
[![platform](https://img.shields.io/badge/platform-Windows%2010%20%7C%2011-0078D4)](#如何開始-how-to-start)

Tarkov-WYA 的主程式庫：從「你」出發，即時掌握你在《逃離塔科夫》裡進行到哪裡、身在哪裡。專案介紹請見 [Tarkov-WYA 組織首頁](https://github.com/17-Tarkov-WYA)。

> [!CAUTION]
> **本專案僅供研究與學習用途，禁止任何形式的付費或商業使用。**
> 授權條款見 [LICENSE](LICENSE)（PolyForm Noncommercial 1.0.0）。

## 專案結構 (Structure)

| 路徑 | 說明 | 狀態 |
|---|---|---|
| [`apps/planner`](apps/planner) | 進度規劃：任務進度、出發前清單、鑰匙、藏身處 | 開發中 |
| [`apps/client`](apps/client) | 本機中樞：讀取遊戲紀錄檔與截圖座標 | 開發中 |
| [`apps/map`](apps/map) | 互動地圖：位置、任務點、路線與標記 | 暫停 |
| [`apps/server`](apps/server) | 小隊中繼站 | 暫停 |
| [`packages/protocol`](packages/protocol) | 各 app 之間的事件與資料格式規格 | 開發中 |
| [`packages/game-data`](packages/game-data) | 地圖、任務、物品等遊戲資料 | 開發中 |
| [`docs`](docs) | 開發規劃與說明文件 | — |

## 技術棧 (Tech Stack)

開發中：

- **.NET 10** ([官網](https://dotnet.microsoft.com) | [GitHub](https://github.com/dotnet/runtime))：client 與 planner 的執行環境，長期支援（LTS）版本。
- **C#** ([官網](https://learn.microsoft.com/dotnet/csharp/) | [GitHub](https://github.com/dotnet/csharplang))：client 與 planner 的開發語言，與 Windows 整合度最高。
- **WPF** ([官網](https://learn.microsoft.com/dotnet/desktop/wpf/) | [GitHub](https://github.com/dotnet/wpf))：planner 的 Windows 桌面 UI 框架。
- **JSON Schema** ([官網](https://json-schema.org) | [GitHub](https://github.com/json-schema-org/json-schema-spec))：protocol 的規格格式，用來產生各語言的型別。

預定（暫停中）：

- **Vue 3** ([官網](https://vuejs.org) | [GitHub](https://github.com/vuejs/core))：map 的前端框架。
- **Vite** ([官網](https://vite.dev) | [GitHub](https://github.com/vitejs/vite))：map 的建置工具。
- **TypeScript** ([官網](https://www.typescriptlang.org) | [GitHub](https://github.com/microsoft/TypeScript))：server 與 map 的開發語言。
- **Node.js** ([官網](https://nodejs.org) | [GitHub](https://github.com/nodejs/node))：server 的執行環境。

## 如何開始 (How to start)

> 目前只有說明與設計文件，尚無可執行的程式。

預定的開發環境：

- Windows 10／11
- [.NET SDK 10](https://dotnet.microsoft.com/download)

建議閱讀順序：

1. [`docs/architecture.md`](docs/architecture.md)：整體架構與設計決策
2. [`docs/roadmap.md`](docs/roadmap.md)：開發順序與目前進度
3. 各 app 與套件的 README
