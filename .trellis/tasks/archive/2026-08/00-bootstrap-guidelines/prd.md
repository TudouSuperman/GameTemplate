# Bootstrap GameTemplate Unity Client Specs

## Goal

为 GameTemplate 建立基于真实代码的 Trellis 项目规范，使后续 Codex 会话按当前 Unity 客户端架构开发，而不是套用 Web 全栈或参考项目的 ET/服务端规则。

## Inputs

- 参考说明：`E:/Other/GameDevelopmentKit/AGENTS.md`
- 当前项目：`E:/GameTemplate`
- 代码证据：`Client/Assets/Scripts/GameApp/`、`ClientExcel/`、`Client/Packages/manifest.json`、`Client/ProjectSettings/ProjectVersion.txt`

参考文件只用于识别相似的 GF、HybridCLR、UniTask、UI 和 Entity 概念；最终规则以 GameTemplate 当前代码为准。

## Scope

- `.trellis/spec/index.md`
- `.trellis/spec/client/`
- `.trellis/spec/guides/`

覆盖：

- Unity 工程和程序集边界
- C#、UniTask、事件、资源和生命周期
- HybridCLR Loader/Code 分层
- GF UI 与 Entity 开发流程
- ClientExcel、DataTableGenerator、CodeBind 和生成文件
- Unity 编译、Play Mode 和改动类型检查

## Out of Scope

- ET Framework、ETTask、ETUI、ETEntity
- .NET 服务端、MongoDB、服务端部署
- Luban（本项目使用自有 DataTableGenerator）
- Proto/ServerExcel 生成流程（当前只有占位）
- React/TypeScript/ORM/数据库等通用 Web 模板
- 修改产品源码

## Progress

- [x] 分析参考 AGENTS.md 和当前仓库
- [x] 删除不适用的 backend/frontend 占位规范
- [x] 编写 Unity 客户端规范和项目化 thinking guides
- [x] 检查占位文本、索引链接、路径证据和 Git 差异

## Acceptance Criteria

- [x] `.trellis/spec/` 没有模板占位文本。
- [x] 根索引、客户端索引和 guides 索引与实际文件一致。
- [x] 重要规则引用当前项目中的真实文件或重复模式。
- [x] 明确排除 ET、服务端、MongoDB、Luban 和 Web 全栈规范。
- [x] 没有修改业务源码或用户已有的无关变更。
