# 资源设置与本地化规范

本规范覆盖运行时图片设置、Owner 级资源生命周期和本地化加载。单张图片通过 AssetSet 设置；一组资源跟随 UI、Widget 或 Entity 生命周期时使用 `ResourceContainer`。

## 图片接口

| 场景 | API | 资源类型 |
| --- | --- | --- |
| `Image` 加载 Sprite | `SetSprite` / `SetSpriteAsync` | GF Resource，Sprite 完整路径 |
| `RawImage` 加载 Resource 纹理 | `SetTextureByResource` / `SetTextureByResourceAsync` | GF Resource，Texture2D 完整路径 |
| `RawImage` 读取 AssetSet 缓存 | `SetTextureByFileSystem` / `SetTextureByFileSystemAsync` | AssetSet 文件系统键 |
| `RawImage` 下载并缓存图片 | `SetTextureByWebRequest` / `SetTextureByWebRequestAsync` | URL；默认持久化 |

只需触发加载时使用同步返回的接口；后续逻辑依赖资源设置完成时使用 Async。Async 被目标换图、加载失败或等待项清理取消时会抛出 `OperationCanceledException`；它不提供底层失败原因，具体资源错误查看 GF 日志。

## AssetSet 生命周期

通过 `GameEntry.AssetSet` 使用统一组件。它按“路径 + Unity 类型”合并 Resource 加载和缓存键，因此同一图片路径可分别作为 Sprite 和 Texture2D 使用。

同一个目标发起新设置时，组件会移除旧等待项；已完成的旧资源在定期释放检查中回收。目标销毁、贴图替换或主动调用 `ReleaseUnused()` 也会触发回收。Resource 资源最终通过 GF `UnloadAsset` 释放；文件系统和下载生成的 Unity 对象通过 `Destroy` 释放。

AssetSet 的网络缓存使用 URL 作为下载地址和磁盘缓存键。图片更新应使用版本化 URL。`DeleteByFileSystem(url)` 只删除磁盘缓存，内存对象池中的同键对象仍可能命中。

`SetTextureByFileSystem` 只读取 AssetSet 自己的虚拟文件系统，不支持任意磁盘路径。文件系统初始容量、缓冲区、池容量和过期时间由 `AssetSetComponent` 配置。

自定义目标继承 `AssetSet<T>`，实现赋值、释放判断和清理。实例使用 `ReferencePool.Acquire/Release`。从文件系统读取还需实现 `ISerializeAssetSet`；使用 WebRequest 需实现 `ISerializeAssetSet` 和 `ISaveAbleAssetSet`。

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
| Sprite 或 Texture 资源键错误 | Async 任务取消并检查 GF Resource 日志 |
| WebRequest 下载失败 | 等待任务取消并检查 URL 与 GF WebRequest 日志 |
| 目标在加载完成前销毁或换图 | 旧设置取消或回收，不能覆盖新图片 |
| Owner 销毁 | `ResourceContainer` 取消等待并卸载已加载资源 |
| 本地化 Key 重复 | XML 解析失败并记录 Warning，修复 Excel 源后重新生成 |
| 新增语言 | XML、Key、启动语言、资源 Variant 和语言 UI 一起验证 |

不要在 UI 中直接调用 `GameEntry.Resource.LoadAsset` 后长期持有引用；按目标类型使用 AssetSet，Owner 级资源使用 `ResourceContainer`。
