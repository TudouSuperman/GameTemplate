# 资源设置与本地化规范

本规范覆盖运行时图片设置、Owner 级资源生命周期和本地化加载。实际入口以 SpriteCollection、TextureSet、`ResourceContainer` 和 GF Localization 为准。

## 能力选择

| 场景 | 当前入口 | 关键文件 |
| --- | --- | --- |
| `Image` 动态设置 Sprite | `SetSprite` / `SetSpriteAsync` | `GameApp/SpriteCollection/SetSpriteExtension.cs` |
| `RawImage` 设置本地、资源或网络 Texture | `SetTextureByFileSystem` / `SetTextureByResources*` / `SetTextureByNetwork*` | `GameApp/TextureSet/SetTextureExtension.cs` |
| 一组任意资源跟随 UI、Widget 或 Entity 释放 | `ResourceContainer`，优先通过 `*LogicEx` 暴露的方法使用 | `GameApp/Container/ResourceContainer.cs` |
| UIForm / Entity 主资源 | Excel 配置 + `UIExtension` / `EntityExtension` | 对应 UI、Entity 规范 |
| 语言字典 | `$Localization.xlsx` + `LocalizationExtension.LoadLanguageAsync` | `GameApp/Localization/` |

## SpriteCollection 契约

可用签名：

```csharp
UniTask SetSpriteAsync(this Image image, string collectionPath, string spritePath)
UniTask SetSpriteAsync(this Image image, string spritePath)
void SetSprite(this Image image, string collectionPath, string spritePath)
void SetSprite(this Image image, string spritePath)
```

- 只传 `spritePath` 的重载会用 `Path.ChangeExtension(spritePath, ".asset")` 推导收集器路径；只有资源命名满足该约定时才使用。
- 后续逻辑依赖图片已经设置完成时使用 Async 重载；只需触发设置时使用 void 重载。
- 不绕过 `GameEntry.SpriteCollection` 为每个界面重新实现 Sprite 缓存、等待队列和释放逻辑。

## TextureSet 契约

`RawImage` 根据来源选择接口：

- GF Resource：`SetTextureByResources` / `SetTextureByResourcesAsync`。
- 网络：`SetTextureByNetwork` / `SetTextureByNetworkAsync`；需要落盘时显式传 `saveFilePath`。
- TextureSet 文件系统：`SetTextureByFileSystem`。

文件系统入口不是任意磁盘读取 API；调用方必须提供 TextureSet 能识别的键或缓存路径。需要等待结果再继续时只能使用已有 Async 重载，不要用固定帧延迟猜测加载完成。

## Owner 级资源生命周期

`UGuiFormLogicEx`、`UGuiWidgetLogicEx` 和 `UGFEntityLogicEx` 已内置 `ResourceContainer`：

- 首次加载时以当前 Logic 为 Owner 创建 Container。
- 回调式加载用版本号阻止已销毁 Owner 接收迟到结果。
- Async 加载共享 `CancellationTokenSource`，`UnloadAllAssets` 会取消未完成任务。
- 回收前先卸载/取消，再把 Container 归还 `ReferencePool`。

业务子类优先调用基类暴露的 `LoadAsset*` / `UnloadAsset*`，不要另建平行 List 和 Token。若独立创建 `ResourceContainer`，必须保证 `UnloadAllAssets` 与 `ReferencePool.Release` 成对执行；`Clear()` 本身不负责卸载 Unity 资源。

## 本地化链路

```text
ClientExcel/GameHotfix/$Localization.xlsx
  → DataTableLocalizationGenerator
  → Assets/Res/Generate/TableData/Localization/<Language>.xml
  + Hotfix/Code/Runtime/Generate/TableConst/LocalizationKey.cs
  → ProcedureLaunch 选择语言和资源变体
  → LocalizationExtension.LoadLanguageAsync
  → GameEntry.Localization.GetString(...)
```

运行时加载顺序是：清空旧 RawString、恢复 `BuiltinData` 内置字典、读取当前语言 XML。`XmlLocalizationHelper` 遇到重复或非法 Key 会记录 Warning 并返回失败。

- 热更业务优先使用 `HotConstant.LocalizationKey` 的生成常量。
- 修改源表后重新生成 XML 和 Key；禁止直接编辑生成产物。
- 当前语言界面把选择保存到 Setting 后重启应用，语言切换不是即时刷新契约。
- 当前支持流程会把未支持的系统语言回退到 English，并同步资源 Variant；新增语言时同时修改启动选择、Variant、Excel 和资源。

## 错误与验证矩阵

| 场景 | 期望行为 / 检查 |
| --- | --- |
| Sprite 收集器或资源键错误 | Async 不应被当成成功；检查 SpriteCollection 日志与资源收集配置 |
| 网络 Texture 失败 | 不继续使用未完成结果；记录 URL/缓存键并检查 TextureSet 日志 |
| Owner 在加载完成前销毁 | 取消或版本检查阻止回写，已加载资源被卸载 |
| 本地化 Key 重复 | XML 解析失败并记录 Warning，修复 Excel 源后重新生成 |
| 新增语言 | XML、Key、启动语言、资源 Variant 和语言 UI 一起验证 |

错误做法是直接在 UI 中 `GameEntry.Resource.LoadAsset` 后长期保存引用；正确做法是根据目标类型走 SpriteCollection/TextureSet，或让 `*LogicEx` 的 `ResourceContainer` 管理完整生命周期。
