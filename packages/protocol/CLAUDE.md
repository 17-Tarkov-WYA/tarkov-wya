# protocol 開發規則

- 規格格式：**JSON Schema**，寫在 [`schema/`](schema)
- 開發風格：[通用規則](../../docs/coding-style/README.md) ＋ **產生型別的目標語言**那一份：
  - 產生 C# 型別（client、planner）→ [C# 開發風格](../../docs/coding-style/csharp.md)
  - 產生 TypeScript 型別（map、server）→ [TypeScript 開發風格](../../docs/coding-style/typescript.md)
  - 只讀與這次工作有關的那一份
- 事件與資料格式只改 `schema/`，不手寫各語言的型別
- 每筆可分享的資料都要標明所屬主題（[`decisions/0004`](../../docs/decisions/0004-topic-based-sharing.md)）
