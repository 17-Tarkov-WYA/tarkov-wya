# 0012. 可追蹤的開發紀錄：commit 拆分、合併方式、CI 回報與完成條件確認

- 狀態：已採用
- 日期：2026-10-24

## 背景

第一次完整跑完流程（目標 #1 → 子 issue #2 → PR #3）後，發現：

- PR #3 的變更集中在少數 commit，git 歷史看不出每一部分改了什麼、為什麼改
- 決策 0011 規定子 issue 合併時把所有 commit 壓成一個，合併後一個子 issue 只剩一個 commit，拆得再細也會被合成一個
- `/develop-issue` 開 PR 後馬上檢查 CI，CI 還在執行，回報寫「結果會另外留言」，之後卻沒有留言
- 子 issue 完成後，issue 上看不出哪些完成條件已經驗證

## 決策

### commit 依內容性質拆分

一個 commit 只包含一種性質：

| 性質 | 內容 | 類型 |
|---|---|---|
| 設定與建置 | 專案檔、相依套件、`.editorconfig`、CI、`.gitignore` 等 | `chore` |
| 程式邏輯 | 程式碼 | `feat`／`fix`／`refactor` |
| UI | XAML、版面、UI 文字 | 依情況用 `feat`／`fix`，摘要中寫出是 UI |
| 測試 | 測試程式與範例資料 | `test` |
| 文件 | README、`docs/`、`CLAUDE.md`、`.claude/` 中的說明 | `docs` |

- 只有一種性質時，一個 commit 即可；同一種性質但目的不同時，也要拆開
- 依「被依賴的先 commit」排序（例如設定 → 程式 → UI → 測試 → 文件），讓每個 commit 都盡量可以建置

### commit 訊息內文必填

標題格式不變（決策 [0009](0009-development-conventions.md)），但**內文為必填**：

```
<類型>(<範圍>): <繁體中文摘要> (#<issue 編號>)

改了什麼：
- <檔案或模組>：<變更>

為什麼：
- <理由；對應 issue 的哪一個完成條件或 PR 上的哪一條意見>
```

- 退回修改時，每一條修改意見各自一個（或一組）commit，「為什麼」寫出對應的意見
- 不改寫已 push 的 commit

### 所有 PR 一律 Create a merge commit

子 issue 分支 → dev 分支、dev 分支 → `main`，都用 **Create a merge commit** 合併。

### 等 CI 結束再回報

- 開 PR（或退回修改後 push）之後，Claude 執行 `gh pr checks <PR 編號> --watch`，等所有檢查結束
- 通過：在 PR 留言「🤖 CI 結果（Claude）」，列出每項檢查與結果
- 失敗：在 PR 留言失敗原因與修正方式，修正、commit、push 後重新等待，再留言新的結果；無法修正時改為 `status:blocked`
- 開發回報中的 CI 一列寫「見 PR 留言『🤖 CI 結果』」，不寫「之後另外留言」

### 在子 issue 留言確認完成條件

- issue 內容是使用者寫的原始需求，Claude **不修改**（維持決策 [0011](0011-goals-and-status.md)「不能修改 issue 的其他內容」），改為在子 issue 下方留言「🤖 完成條件確認（Claude）」
- 時機：CI 結束之後、改為 `status:review` 之前；退回修改後完成時，**另外再留一則新的**，不編輯舊留言
- 內容：逐條**原文引用**子 issue「完成條件」的每一項，順序與原文相同，不可省略、改寫或合併；每一條標示：
    - ✅ 已完成：寫出如何確認（執行的指令與結果、檔案位置）
    - ❌ 未完成：寫出原因
    - 👤 需要使用者確認：Claude 無法驗證的項目（例如需要看畫面），寫出確認方法
- 格式見 [`CLAUDE.md`](../../CLAUDE.md)「回報與 PR」
- **不修改目標 issue**，也不在目標 issue 留言；目標的完成條件由使用者最後驗收
- 這則留言代表「Claude 已自行驗證」，不代表使用者已驗收；**合併 PR 才是使用者的審核結果**

### 目標完成後保留 dev 分支

dev 分支合併到 `main` 後**不刪除**：ruleset 禁止刪除 `dev/**`，用來防止誤刪；保留的 dev 分支也是目標的紀錄。

## 理由

- 使用者要能從 git 歷史追蹤每一部分改了什麼、為什麼改；依性質拆分後，看 commit 就知道設定、程式、文件各自的變更
- 壓成一個 commit 的合併方式會把拆好的 commit 合成一個，所以子 issue 也改用 merge commit；`git log --first-parent` 仍能看到一個子 issue 一個合併點，不會失去「一個子 issue 一個單位」的好處
- 「被依賴的先 commit」讓每個 commit 盡量可以建置，之後要找出問題出在哪個 commit 比較容易
- CI 通常要幾分鐘，開 PR 後馬上檢查只會看到「執行中」；等結束再回報，使用者看到 `status:review` 時就能直接審核
- 考慮過讓 Claude 在 issue 內容的完成條件上打勾：使用者寫的原始需求會被修改，也分不出是誰確認的，所以改為另外留言；留言保留每一次的確認紀錄，退回修改後也看得出前後差異

## 影響

- 決策 [0011](0011-goals-and-status.md) 的「合併方式」被本決策取代，0011 的狀態改為「部分被 0012 取代（合併方式）」
- `docs/workflow.md`、`docs/claude-code.md`、`CLAUDE.md`、`.claude/commands/develop-issue.md`、`.claude/agents/implementer.md` 依本決策更新
- 開發回報（PR 內容）加上「## Commits」，逐一列出 commit 的 hash、標題與為什麼
- implementer 完成時依性質分組回報修改的檔案，主要的 Claude 據此拆 commit
- `status:review` 改為在 CI 結果與完成條件確認都留言之後才加上
- `docs/workflow.md`、`docs/claude-code.md` 的「完成目標」移除「刪除 dev 分支」的步驟
- GitHub 上的合併選項由使用者手動調整
