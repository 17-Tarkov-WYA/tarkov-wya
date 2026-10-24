# Tarkov - Where You At - Protocol

[![status](https://img.shields.io/badge/status-%E9%96%8B%E7%99%BC%E4%B8%AD-orange)](../../docs/roadmap.md)
[![JSON Schema](https://img.shields.io/badge/JSON%20Schema-%E9%A0%90%E5%AE%9A-000000)](https://json-schema.org)

事件與資料格式規格：所有 app 之間溝通的唯一規格來源。

> 尚未定義任何事件。

## 運作方式 (How it works)

規格以 JSON Schema 寫在 [`schema/`](schema)，再產生各語言的型別：

- C#：client、planner
- TypeScript：map、server（恢復開發後）

修改事件名稱或資料結構時，只改這裡，再重新產生型別。

## 技術棧 (Tech Stack)

- **JSON Schema** ([官網](https://json-schema.org) | [GitHub](https://github.com/json-schema-org/json-schema-spec))：與語言無關的資料格式規格，可產生 C# 與 TypeScript 型別。
