# 架构与目录规范

## 工程边界

`Client/` 是唯一实际运行的 Unity 工程。Unity 生成的 `Client/Client.sln` 和 `*.csproj` 用于 IDE 浏览，不是程序集依赖的源文件；依赖关系以源码目录和 `*.asmdef` 为准。

核心目录：

```text
Client/
├── Assets/
│   ├── Res/                         # 预制体、表数据、配置和运行时资源
│   └── Scripts/
│       ├── Core/                    # GF、UGF 与 UGF Extensions
│       ├── GameApp/                 # 本项目业务和项目级扩展
│       ├── Library/                 # 第三方库及其扩展
│       └── Test/                    # 当前仅有辅助脚本，没有正式测试程序集
├── Packages/manifest.json
└── ProjectSettings/

ClientExcel/
├── Game/                            # 常驻层配置源
└── GameHotfix/                      # 热更新层配置源
```

## `GameApp/` 所有权

- `Base/`：`GameEntry` 等全局访问入口。
- `Definition/`：常量、枚举和通用数据结构。
- `Container/`：把资源、实体、事件的获取与释放绑定到 Owner 生命周期。
- `UI/`、`Entity/`：GF 的项目级基类和扩展。
- `SpriteCollection/`、`TextureSet/`、`Localization/`：动态图片与本地化的项目级入口。
- `Procedure/`：资源检查、启动和进入热更层之前的常驻流程。
- `Hotfix/Loader/`：加载热更 DLL/资源并创建热更入口，不参与业务迭代。
- `Hotfix/Code/`：可热更新的游戏业务。
- `Editor/`：构建、表格、CodeBind 和项目工具，仅允许 Editor 程序集引用。
- `Generate/`：工具生成的常驻层代码，不手改。

业务代码默认写入 `GameApp/`。除修复框架本身外，不在 `Core/GameFramework`、`Core/UnityGameFramework` 或 `Library/` 中实现项目业务，避免后续框架升级和第三方替换产生冲突。

参考边界：

- `Client/Assets/Scripts/GameApp/GameApp.Runtime.asmdef`
- `Client/Assets/Scripts/GameApp/Editor/GameApp.Editor.asmdef`
- `Client/Assets/Scripts/GameApp/Hotfix/Loader/Runtime/GameApp.Hotfix.Loader.Runtime.asmdef`
- `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/GameApp.Hotfix.Runtime.asmdef`

## 文件与生成内容边界

- 持久文档、命令和规范只使用项目相对路径，不保存个人机器上的绝对路径。
- `Library/`、`Temp/`、`Logs/` 和 `obj/` 等 Unity 生成目录不可手工编辑或提交。
- Unity 生成的 `.sln` 和 `.csproj` 只用于 IDE 浏览，不通过修改它们解决程序集问题。

## 程序集依赖方向

```text
Core / Library
      ↓
GameApp.Runtime
      ↓
GameApp.Hotfix.Loader.Runtime
      ↓
GameApp.Hotfix.Runtime
```

- 常驻层不能引用热更程序集。
- 热更层可以复用 `GameApp.Runtime` 中稳定的基类、扩展和契约。
- 需要被热更层调用的非热更能力，应抽到 `GameApp.Runtime`，而不是用反向引用解决。
- Editor API 只能出现在 Editor 目录/程序集；运行时程序集不得引用 `UnityEditor`。

## 命名空间与代码位置

- 常驻业务使用 `GameApp`，例如 `UGuiFormLogic`、`EntityExtension`、`GameEntry`。
- 热更业务使用 `GameApp.Hotfix`，例如 `HotfixEntry`、`ProcedureGame`、`MainMenuFormLogic`。
- Editor 工具使用 `GameApp.Editor`。
- 新文件跟随相邻模块的命名空间和程序集，不根据文件夹名机械创造新根命名空间。

项目级非目标统一记录在 `../index.md`，专题规范只描述当前客户端已经存在的代码与工具链。
