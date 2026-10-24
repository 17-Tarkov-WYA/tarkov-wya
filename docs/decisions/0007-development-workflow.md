# 0007. 開發流程：討論 → issue → agent team → 使用者審核

- 狀態：已採用
- 日期：2026-10-24

## 背景

使用者希望開發與 issue 高度結合，並由不同的 Claude 分別負責討論與實作，最後由使用者審核。

## 決策

1. **討論**：使用者與 Claude 討論開發方向，結論寫成決策紀錄或更新 `docs/`
2. **建 issue**：使用者依討論結論建立 issue
3. **開發**：另一個 Claude 開 agent team，依 issue 在分支上開發
4. **審核**：Claude 開發完成後開 PR（內容為開發回報）；使用者在 PR 上審核，**只有使用者能合併**

## 理由

- issue 的建立與 PR 是否合併由使用者掌控
- 由 Claude 開 PR，使用者可以在同一頁看到回報、差異與 CI 結果，審核較方便；考慮過由使用者自行開 PR，但審核資訊分散在 issue 留言與分支比較頁
- 開發的 Claude 看不到事前討論，所以 issue 與 repo 文件必須足以獨立閱讀

## 影響

- issue 範本要求填寫「不做的範圍」「相關文件」「驗證方式」
- 開發規則寫在 [`CLAUDE.md`](../../CLAUDE.md)：可以開 PR；禁止 push 到 main、merge、合併／關閉／核准 PR
- 流程細節見 [`workflow.md`](../workflow.md)
