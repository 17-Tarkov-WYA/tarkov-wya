# 0011. 目標與子 issue、狀態標籤與分支規則

- 狀態：部分被 [0012](0012-traceable-development.md) 取代（合併方式）
- 日期：2026-10-24

## 背景

使用者與 Claude 討論出開發方向後，會建立一個目標，再拆成多個子 issue 交給另一個 Claude 開發，最後由使用者審核並完成目標。需要讓開發的 Claude 明確知道「可以接手哪些 issue、要用哪個分支」，也讓使用者隨時看得到開發狀態。

## 決策

- **兩層 issue**：「🎯 目標」issue（`type:goal`）＋ GitHub 原生的子 issue；目標不交給 Claude
- **每個目標一個 dev 分支** `dev/<目標編號>-<說明>`：使用者建立目標時從 `main` 開出，寫在目標範本的「開發分支」欄位；子 issue 的 PR 合併到 dev 分支，**不直接合併到 `main`**；子 issue 全部完成後，使用者把 dev 分支合併到 `main`，目標隨之關閉
- 子 issue 範本加上「上層目標」與「依賴」欄位；要交給 Claude 的子 issue 必須有上層目標
- **狀態標籤**：`status:ready`、`status:in-progress`、`status:review`、`status:changes-requested`、`status:blocked`（另有既有的 `status:paused`），每個子 issue 同時只有一個
- Claude 只接手開啟中、帶有 `status:ready` 或 `status:changes-requested`、有上層目標且其 dev 分支存在、依賴都已關閉的子 issue
- Claude 開始時在 issue 留言「🤖 開始開發（Claude）」，記錄分支名稱、基準 commit 與 PR 目標分支；這則留言同時表示 issue 已被取走
- **分支**：第一次開發從最新的 `origin/<dev 分支>` 開出；退回修改時沿用留言中記錄的分支；不從 `main` 或其他子 issue 的分支開出
- **合併方式**：子 issue → dev 分支用 Squash and merge；dev 分支 → `main` 用 Create a merge commit
- GitHub 只在合併到預設分支時自動關閉 `Closes #N` 的 issue，所以加上工作流程 `close-sub-issues.yml`，在 PR 合併到 `dev/**` 時關閉對應的子 issue
- Claude 可以用 `gh issue edit` 增減 `status:*` 標籤，但不能修改 issue 的其他內容

## 理由

- 狀態標籤只需要現有 token 的 Issues 權限，Claude 能自動更新；在 issue 列表上可以直接篩選，作為「待審核清單」
- 考慮過 GitHub Projects 看板：視覺化較好，但 Claude 要更新看板需要 organization 的 Projects 權限，設定較複雜；之後需要時可以讓看板依標籤同步
- **dev 分支**讓一個目標的所有工作先在獨立的分支上整合，使用者確認整體完成後才進入 `main`，`main` 隨時保持穩定；考慮過子 issue 直接合併到 `main`，但未完成的目標會讓 `main` 處於半完成狀態
- 依賴必須先合併到 dev 分支，所以子分支一律從 dev 分支開出，避免子分支互相堆疊造成審核與合併困難
- dev 分支 → `main` 用 merge commit，`main` 上保留每個子 issue 的 commit，看得出目標由哪些工作組成

## 影響

- 新增「🎯 目標」範本（含必填的「開發分支」）與 `type:goal`、四個狀態標籤
- 新增工作流程 `close-sub-issues.yml`；格式檢查在 push 到 `dev/**` 時也會執行
- `.claude/settings.json` 禁止 push 到 `dev/*`、禁止開 PR 到 `main`
- 建議以 ruleset 同時保護 `main` 與 `dev/**`
- `.claude/settings.json` 允許 `gh issue edit`（`CLAUDE.md` 限制只能改 `status:*` 標籤）
- 新增 `/next-issue` 指令；`/develop-issue` 加上狀態檢查、依賴檢查、開始留言與標籤更新
