# GameTemplate 项目规范

本目录记录 GameTemplate 当前代码库的实际开发约定，供 Trellis 主会话、实现代理和检查代理共同使用。

## 项目定位

GameTemplate 是一个以 UnityGameFramework（GF）为基础的纯 Unity 客户端模板：

- Unity 工程位于 `Client/`，版本固定为 `2022.3.15f1c1`。
- 业务基础程序集为 `GameApp.Runtime`，热更新使用 HybridCLR 和 `GameApp.Hotfix.Runtime`。
- 异步统一使用 UniTask。
- UI 与 Entity 基于 GF，并使用 CodeBind 生成引用绑定代码。
- 配置源位于 `ClientExcel/`，通过项目内 Unity Editor 工具生成运行时数据和枚举。
- 动态图片使用 SpriteCollection / TextureSet，批量资源生命周期使用 `ResourceContainer`。
- 构建、HybridCLR、DataTable 和 Toolbar 工具均位于 `GameApp.Editor`。

## 规范导航

- [客户端规范](./client/index.md)：目录、程序集、热更新、UI、Entity、配置生成和质量检查。
- [跨层思考指南](./guides/cross-layer-thinking-guide.md)：修改资源、表格、生成代码和运行时代码时的完整链路。
- [代码复用指南](./guides/code-reuse-thinking-guide.md)：优先复用 GF 扩展、容器、基类和项目工具。

## 规范使用原则

1. 修改代码前先读与目标目录对应的规范。
2. 以当前源码、`.asmdef`、预制体和 Editor 工具为最终依据。
3. `Client/Assets/Scripts/Core/` 与 `Client/Assets/Scripts/Library/` 视为框架/第三方边界，业务功能优先写在 `Client/Assets/Scripts/GameApp/`。
4. 生成文件只通过对应工具更新，不直接手改。
5. 新模式只有在项目中形成稳定实践后，才写回本目录。

## 外部规范融合边界

从其他项目参考规范时，只吸收已经与当前源码、目录、程序集和工具链相符的通用规则。

不得直接迁移其他项目的项目路径、Unity 版本、类名、命名空间、业务系统、第三方库、构建脚本、MCP 或个人机器配置，以及当前项目不存在的技术栈。

任何新规则必须先确认当前项目确实存在对应实现；否则只作为外部参考，不写入项目规范。
