# Bootstrap GameTemplate Unity Client Specs

## Goal

为 GameTemplate 建立基于真实代码的 Trellis 项目规范，使后续 Codex 会话按当前 Unity 客户端架构开发，而不是套用其他项目中当前仓库不存在的架构规则。

## Inputs

- 代码证据：`Client/Assets/Scripts/GameApp/`、`ClientExcel/`、`Client/Packages/manifest.json`、`Client/ProjectSettings/ProjectVersion.txt`

外部参考只用于识别可复用的规范结构；最终规则以 GameTemplate 当前代码为准，不保留其他项目或个人机器路径。

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

- 当前仓库不存在的服务端框架、数据库和部署流程
- 当前仓库未采用的第三方配置生成工具（本项目使用自有 DataTableGenerator）
- 当前仓库不存在的协议或服务端配置生成流程
- 与当前 Unity 客户端无关的通用全栈模板
- 修改产品源码

## Progress

- [x] 分析当前仓库和可借鉴的规范结构
- [x] 删除不适用的 backend/frontend 占位规范
- [x] 编写 Unity 客户端规范和项目化 thinking guides
- [x] 检查占位文本、索引链接、路径证据和 Git 差异

## Acceptance Criteria

- [x] `.trellis/spec/` 没有模板占位文本。
- [x] 根索引、客户端索引和 guides 索引与实际文件一致。
- [x] 重要规则引用当前项目中的真实文件或重复模式。
- [x] 明确排除当前仓库不存在的技术栈、工具链和部署假设。
- [x] 没有修改业务源码或用户已有的无关变更。
