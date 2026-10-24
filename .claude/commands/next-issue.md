---
description: 列出目前可以交給 Claude 開發的子 issue，並建議下一個要做的
argument-hint: "[目標 issue 編號（選填）]"
---

列出目前可以接手的 issue。只查詢與整理，**不開始開發、不修改任何 issue**。規則見 `docs/workflow.md` 第 2 節。

以下指令中的 `<repo>` 一律是 `17-Tarkov-WYA/tarkov-wya`。

## 1. 取得候選

```
gh issue list --repo <repo> --state open --label status:ready --json number,title,labels,milestone,body
gh issue list --repo <repo> --state open --label status:changes-requested --json number,title,labels,milestone,body
```

有指定目標 issue（$ARGUMENTS）時，只保留「上層目標」為該編號的 issue。

## 2. 篩選

排除：

- 帶有 `type:goal` 的 issue
- 沒有「上層目標」的 issue
- 目標 issue 沒有「開發分支」，或該分支不存在於 `origin` 的（先 `git fetch origin`，再用 `git branch -r --list origin/<dev 分支>` 確認）
- 「依賴」中有任何 issue 尚未關閉的（用 `gh issue view <編號> --repo <repo> --json state` 確認）

## 3. 顯示結果

| 編號 | 標題 | 狀態 | 上層目標 | dev 分支 | milestone | 依賴 |
|---|---|---|---|---|---|---|

另外列出被排除的 issue 與原因（例如「依賴 #11 尚未完成」「目標 #1 的 dev 分支尚未建立」）。

## 4. 建議

依以下順序建議下一個：

1. `status:changes-requested`（已退回的優先處理）
2. milestone 編號較小的
3. issue 編號較小的

最後提示使用者：確認後執行 `/develop-issue <編號>`。
