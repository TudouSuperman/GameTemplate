# GameTemplate 项目规范

本目录记录 GameTemplate 当前代码库的实际开发约定，供 Trellis 主会话、实现代理和检查代理共同使用。

## 项目定位

GameTemplate 是一个以 UnityGameFramework（GF）为基础的纯 Unity 客户端模板：

- Unity 工程位于 `Client/`，版本固定为 `2022.3.15f1c1`。
- 业务基础程序集为 `GameApp.Runtime`，热更新使用 HybridCLR 和 `GameApp.Hotfix.Runtime`。
- 异步统一使用 UniTask。
- UI 与 Entity 基于 GF，并使用 CodeBind 生成引用绑定代码。
- 配置源位于 `ClientExcel/`，通过项目内 Unity Editor 工具生成运行时数据和枚举。

## 规范导航

- [客户端规范](./client/index.md)：目录、程序集、热更新、UI、Entity、配置生成和质量检查。
- [跨层思考指南](./guides/cross-layer-thinking-guide.md)：修改资源、表格、生成代码和运行时代码时的完整链路。
- [代码复用指南](./guides/code-reuse-thinking-guide.md)：优先复用 GF 扩展、容器、基类和项目工具。

## 当前明确不包含的范围

以下内容没有真实实现，不应生成或套用对应开发规范：

- ET Framework、ETTask、ETUI、ETEntity。
- .NET 服务端、MongoDB、服务端热更新和服务端部署。
- `Server/`、`ServerExcel/` 和 `Proto/` 当前只有占位文件，不能据此推断服务端或协议工作流。
- React、TypeScript、Web 前端、ORM、数据库迁移等 Web 全栈模板内容。

如果上述目录未来出现真实工程，应先重新分析源码，再新增独立规范；不要提前复制其他项目的约定。

## 规范使用原则

1. 修改代码前先读与目标目录对应的规范。
2. 以当前源码、`.asmdef`、预制体和 Editor 工具为最终依据。
3. `Client/Assets/Scripts/Core/` 与 `Client/Assets/Scripts/Library/` 视为框架/第三方边界，业务功能优先写在 `Client/Assets/Scripts/GameApp/`。
4. 生成文件只通过对应工具更新，不直接手改。
5. 新模式只有在项目中形成稳定实践后，才写回本目录。
