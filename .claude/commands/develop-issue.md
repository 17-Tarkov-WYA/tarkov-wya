---
description: 依照 GitHub 子 issue 開發，由 implementer、tester、reviewer 三個角色完成，開 PR 到目標的 dev 分支並更新狀態標籤
argument-hint: <子 issue 編號>
---

依照 issue #$ARGUMENTS 開發。全程遵守 `CLAUDE.md`，狀態與分支規則見 `docs/workflow.md` 第 2、6 節。

以下指令中的 `<repo>` 一律是 `17-Tarkov-WYA/tarkov-wya`。

## 1. 確認可以接手

1. 讀取子 issue：`gh issue view $ARGUMENTS --repo <repo> --comments`
2. 讀取「上層目標」的目標 issue：`gh issue view <目標編號> --repo <repo>`，取得「**開發分支**」欄位的值（以下稱 `<dev 分支>`，例如 `dev/1-skeleton`）
3. 確認 dev 分支存在：`git fetch origin`，再用 `git branch -r --list origin/<dev 分支>` 確認有結果
4. 逐項確認，**任何一項不符合就停下來**：
    - 子 issue 是開啟中的，且不是目標 issue（沒有 `type:goal`）
    - 帶有 `status:ready` 或 `status:changes-requested`
    - 有「上層目標」，目標 issue 有「開發分支」，且該分支存在於 `origin`
    - 「依賴」欄位列出的每個 issue 都已關閉：`gh issue view <依賴編號> --repo <repo> --json state`
5. 不符合時：
    - 狀態標籤不對、是目標 issue：只告訴使用者原因，不修改 issue
    - 沒有上層目標、dev 分支不存在、依賴未關閉：加上 `status:blocked`、移除原本的狀態標籤，並在子 issue 留言說明原因，然後停止

## 2. 了解任務

1. 從目標 issue 了解這個子 issue 在整體中的角色（**只做子 issue 的範圍**）
2. 讀取 `CLAUDE.md`「開始前必讀」中，對應這個 issue 類型的所有檔案
3. 讀取 issue「相關文件」列出的每一個檔案
4. 完成條件、不做的範圍或驗證方式不清楚或互相矛盾時，**先停下來詢問使用者**

## 3. 決定分支

- **`status:ready`（第一次開發）**：依命名規則從最新的 `origin/<dev 分支>` 開新分支

    ```
    git switch -c <類型>/$ARGUMENTS-<簡短英文說明> origin/<dev 分支>
    ```

- **`status:changes-requested`（退回修改）**：找出子 issue 留言中最新一則「🤖 開始開發（Claude）」記錄的分支，沿用它，**不開新分支**

    ```
    git switch <分支名稱>
    git pull
    ```

    本機沒有這個分支時：`git switch -c <分支名稱> origin/<分支名稱>`

    讀取這個分支的 PR 與使用者的修改意見：`gh pr view <分支名稱> --repo <repo> --comments`。逐條列出要修改的項目；意見不清楚時先詢問使用者

- 不從 `main` 或其他子 issue 的分支開出；不自行 merge 或 rebase

## 4. 標示開始

1. 留言到子 issue（內容存成暫存檔，不要放在 repo 中）：

    ```markdown
    🤖 開始開發（Claude）

    - 分支：`<分支名稱>`
    - 基準：`origin/<dev 分支>` @ `<commit hash>`（沿用分支時寫「沿用既有分支」）
    - PR 目標：`<dev 分支>`
    - 上層目標：#<目標編號>
    ```

    `gh issue comment $ARGUMENTS --repo <repo> --body-file <暫存檔>`

2. 更新狀態：`gh issue edit $ARGUMENTS --repo <repo> --add-label status:in-progress --remove-label <原本的 status 標籤>`

## 5. 開發（agent team）

依序交給三個角色，每一步都把 issue 編號、上層目標、dev 分支、完成條件與相關文件交代清楚：

1. **implementer**：實作（退回修改時，只依 PR 上的修改意見修改）
2. **tester**：依完成條件與驗證方式實際執行並記錄結果
3. **reviewer**：審查變更（比較基準是 `origin/<dev 分支>`）

reviewer 提出「必須修正」時，交回 implementer 修正，再重新經過 tester 與 reviewer，直到沒有必須修正的問題。

## 6. commit 與 push

- 依 `docs/workflow.md` 的格式 commit，結尾引用 `(#$ARGUMENTS)`
- push 自己的分支：`git push -u origin <分支名稱>`
- **不** push 到 `main` 或任何 `dev/*` 分支、**不** merge、**不**關閉 issue

## 7. 開 PR 與回報

依 `CLAUDE.md` 的「回報與 PR」撰寫，內容存成暫存檔：

- **第一次完成**：開 PR **到 dev 分支**，內容為「開發回報」（第一行 `Closes #$ARGUMENTS`）

    ```
    gh pr create --repo <repo> --base <dev 分支> --head <分支名稱> --title "<commit 格式的標題>" --body-file <暫存檔>
    ```

    開完後用 `gh pr view <PR 編號> --repo <repo> --json baseRefName` 確認目標分支是 `<dev 分支>`，**不是 `main`**

- **退回修改後完成**：push 後 PR 會自動更新；在 PR 留言「修改回報」

    ```
    gh pr comment <PR 編號> --repo <repo> --body-file <暫存檔>
    ```

- 確認 CI：`gh pr checks <PR 編號> --repo <repo>`；失敗時修正後再 push，並在回報中說明
- 更新 issue 狀態：`gh issue edit $ARGUMENTS --repo <repo> --add-label status:review --remove-label status:in-progress`
- **無法完成**（需要使用者決定、資訊不足）：不開 PR；在子 issue 留言說明原因，改為 `status:blocked`
- **絕對不合併、關閉或核准 PR**
- 在對話中顯示同樣的回報，並告訴使用者 PR 連結、目標分支與目前的狀態標籤
