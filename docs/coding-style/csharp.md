# C# 開發風格 (C# Coding Style)

適用於 `apps/planner/`、`apps/client/`、`tests/` 中的 C# 程式碼。請先讀過 [通用規則](README.md)。

可以自動檢查的規則寫在 [`.editorconfig`](../../.editorconfig) 的 `[*.cs]` 區段，建置時由程式風格檢查（`EnforceCodeStyleInBuild`，骨架階段啟用）執行。

## 1. 環境

- .NET 10，啟用 nullable
- 編碼與換行：CRLF、UTF-8 BOM、縮排 4 格

## 2. 命名

| 對象 | 規則 | 範例 |
|---|---|---|
| 類別、結構、列舉、屬性、方法、事件、命名空間 | PascalCase | `QuestProgress`、`LoadAsync` |
| 介面 | `I` 開頭的 PascalCase | `ILogWatcher` |
| 非同步方法 | 結尾加 `Async` | `ParseLineAsync` |
| 私有欄位 | `_` 開頭的 camelCase | `_lastReadPosition` |
| 區域變數、參數 | camelCase | `questId` |
| 常數 | PascalCase | `DefaultPort` |
| 命名空間 | `TarkovWYA.<App>.<資料夾>` | `TarkovWYA.Client.Logs` |

## 3. 語法

- 使用**檔案範圍命名空間**（`namespace TarkovWYA.Client;`）
- `using` 放在命名空間之外，`System` 開頭的排在最前面
- `if`、`for` 等一律加大括號；大括號獨立一行
- **不使用** `!` 壓掉 nullable 警告，除非附註解說明為什麼一定不是 null
- 型別明確時使用 `var`
- 一個檔案一個公開型別，檔名與型別名稱相同
- 優先使用 `record` 表示不可變的資料（例如 protocol 的事件）
- 公開 API 使用 XML 文件註解（`///`）

## 4. 非同步與資源

- I/O 一律使用非同步 API，並傳遞 `CancellationToken`
- 不使用 `async void`（WPF 事件處理常式除外）
- 實作 `IDisposable`／`IAsyncDisposable` 的物件使用 `using` 釋放

## 5. 錯誤處理與日誌

- 不捕捉 `Exception` 後什麼都不做；至少記錄日誌
- 使用 `Microsoft.Extensions.Logging` 的 `ILogger<T>`
- 層級：`Debug` 開發細節、`Information` 狀態變化、`Warning` 可恢復的問題、`Error` 功能失敗

## 6. WPF（planner）

- 採用 **MVVM**：`View`（XAML）只負責呈現，邏輯放在 `ViewModel`，資料放在 `Model`
- code-behind（`*.xaml.cs`）只放純畫面相關的程式碼
- UI 文字集中管理，不散落在 XAML 與程式碼中
- 長時間的工作不阻塞 UI 執行緒

## 7. 資料夾結構

依功能分資料夾，不依類型分（例如用 `Quests/`，而不是 `Models/`、`Services/` 各放一部分）：

```
apps/<app>/
├── README.md
├── TarkovWYA.<App>.csproj
├── <功能>/                 # 例如 Logs/、Quests/、Hub/
└── ...
tests/
├── fixtures/               # 去除個人資訊的範例資料
└── TarkovWYA.<App>.Tests/  # 對應的測試專案
```

## 8. 測試

- 測試框架：**xUnit**（於骨架階段建立）
- 測試專案放在 `tests/TarkovWYA.<App>.Tests/`
- 測試方法命名：`<方法>_<情境>_<預期結果>`，例如 `ParseLine_QuestCompleted_ReturnsCompletedEvent`

## 9. 驗證指令

```bash
dotnet build TarkovWYA.slnx
dotnet test TarkovWYA.slnx
```
