# Tarkov - Where You At - Server

[![status](https://img.shields.io/badge/status-%E6%9A%AB%E5%81%9C-lightgrey)](../../docs/roadmap.md)
[![Node.js](https://img.shields.io/badge/Node.js-%E9%A0%90%E5%AE%9A-5FA04E?logo=nodedotjs&logoColor=white)](https://nodejs.org)
[![TypeScript](https://img.shields.io/badge/TypeScript-%E9%A0%90%E5%AE%9A-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org)
[![Docker](https://img.shields.io/badge/Docker-%E9%A0%90%E5%AE%9A-2496ED?logo=docker&logoColor=white)](https://www.docker.com)

小隊中繼站：讓同一個房間的隊友即時共享各自選擇公開的資料。

> 目前**暫停**，優先開發 planner 與 client，見 [`docs/roadmap.md`](../../docs/roadmap.md)。

## 部署目標 (Deployment)

- 自己的電腦，或 Render、Google Cloud Run 這類註冊即可使用的平台
- 不依賴永久儲存（房間狀態只放在記憶體）
- 要能承受平台休眠後的冷啟動

## 技術棧 (Tech Stack)（預定）

- **Node.js** ([官網](https://nodejs.org) | [GitHub](https://github.com/nodejs/node))：執行環境，各大雲端平台原生支援。
- **TypeScript** ([官網](https://www.typescriptlang.org) | [GitHub](https://github.com/microsoft/TypeScript))：開發語言，與 protocol 產生的型別共用。
- **Docker** ([官網](https://www.docker.com) | [GitHub](https://github.com/docker))：容器化部署，讓 server 能部署到任何平台。
