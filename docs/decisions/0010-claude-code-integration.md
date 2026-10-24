# 0010. Claude Code 整合與權限

- 狀態：已採用
- 日期：2026-10-24

## 背景

開發由 Claude Code 的 agent team 進行（見 [0007](0007-development-workflow.md)）。開發的 Claude 需要讀取 issue、更新狀態、開 PR 交給使用者審核，但不能合併。

## 決策

- **Claude Code 設定放在 repo 中**：
  - [`CLAUDE.md`](../../CLAUDE.md)：開發規則，每次啟動自動讀取
  - [`.claude/agents/`](../../.claude/agents)：agent team 的三個角色（implementer、tester、reviewer）
  - [`.claude/commands/`](../../.claude/commands)：`/next-issue`、`/develop-issue <編號>`
  - [`.claude/settings.json`](../../.claude/settings.json)：允許與禁止的指令
- **GitHub 權限**：使用只限本 repo 的 fine-grained personal access token，透過環境變數 `GH_TOKEN` 交給 GitHub CLI
  - Issues：讀寫（讀取 issue、留言、更新狀態標籤）
  - Pull requests：讀寫（開 PR、讀取審核意見、留言）
  - Metadata：唯讀（GitHub 規定）
  - Contents 及其他：**不給**
- 分支以既有的 SSH 金鑰 push；建議以 ruleset 規定 `main` 必須透過 PR 合併並通過 CI，**不**要求核准
- 開發回報寫在 PR 內容，開頭標示「🤖 開發回報（Claude）」；退回修改後在 PR 留言「🤖 修改回報（Claude）」

## 理由

- **多層防護**：`CLAUDE.md` 的規則、`settings.json` 的禁止清單、token 的權限範圍、`main` 的 ruleset，任一層失效時其他層仍有效
- 根據 GitHub 文件，合併 PR 需要 Contents 的寫入權限；token 不給 Contents，Claude 能開 PR 但**技術上無法合併**
- token 屬於使用者，PR 作者會顯示為使用者，GitHub 不允許作者核准自己的 PR，所以 ruleset 不要求核准，由使用者按下合併作為審核
- 回報留在 PR 與 issue 上，之後可以追溯每個決定的來由

## 影響

- 設定步驟見 [`claude-code.md`](../claude-code.md)
- token 不得出現在對話、log、commit 或任何檔案中
- token 到期或外流時立即撤銷並重新建立
