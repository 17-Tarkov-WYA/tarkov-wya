# client 開發規則

- 語言：**C# · .NET 10**
- 開發風格：[通用規則](../../docs/coding-style/README.md) ＋ [C# 開發風格](../../docs/coding-style/csharp.md)；**不需要**閱讀 TypeScript 的規則
- 效能：以事件驅動監聽檔案變化，不定時輪詢
- 只讀取遊戲寫在本機的紀錄檔與截圖檔名；**不讀取遊戲記憶體、不修改任何遊戲檔案**
- 解析遊戲 log 的邏輯一律以 `tests/fixtures/` 中去除個人資訊的範例資料測試
- 驗證：

  ```bash
  dotnet build TarkovWYA.slnx
  dotnet test TarkovWYA.slnx
  ```
