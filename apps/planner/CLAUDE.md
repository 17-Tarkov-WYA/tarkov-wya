# planner 開發規則

- 語言：**C# · WPF（.NET 10）**
- 開發風格：[通用規則](../../docs/coding-style/README.md) ＋ [C# 開發風格](../../docs/coding-style/csharp.md)；**不需要**閱讀 TypeScript 的規則
- 架構：採 MVVM；planner 不顯示任何地圖或位置資訊（[`decisions/0003`](../../docs/decisions/0003-planner-map-separation.md)）
- 資料來源：只透過 client 取得遊戲狀態，不直接讀取遊戲檔案
- 資料模型要區分「自動偵測」與「手動設定」的值（[`decisions/0005`](../../docs/decisions/0005-progress-source.md)）
- 驗證：

  ```bash
  dotnet build TarkovWYA.slnx
  dotnet test TarkovWYA.slnx
  ```
