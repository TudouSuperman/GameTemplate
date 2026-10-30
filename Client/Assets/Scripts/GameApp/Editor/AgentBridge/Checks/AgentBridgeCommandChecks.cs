using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AgentBridge;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameApp.Editor
{
    public static class AgentBridgeCommandChecks
    {
        private static Task WaitForEditorUpdate()
        {
            var completion = new TaskCompletionSource<bool>();
            // AgentCallable buttons invoke methods from OnGUI. Scene/Undo operations must wait until its layout scopes close.
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                {
                    completion.SetException(new InvalidOperationException("Run AgentBridge checks in Edit Mode after compilation finishes."));
                    return;
                }
                completion.SetResult(true);
            };
            return completion.Task;
        }

        private static void Require(bool condition, string step)
        {
            if (!condition) throw new InvalidOperationException("AgentBridge check failed: " + step);
        }

        private static async Task<JObject> Call(ICommandHandler command, object parameters)
        {
            return JObject.FromObject(await command.ExecuteAsync(JObject.FromObject(parameters)));
        }

        private static async Task ExpectError(string code, Func<Task> operation)
        {
            try { await operation(); }
            catch (CommandException exception)
            {
                Require(exception.Code == code, "expected error " + code + ", actual " + exception.Code);
                return;
            }
            throw new InvalidOperationException("Expected command error: " + code);
        }

        [AgentCallable("验证 Excel 查询、预演、原子写入、版本/旧值冲突及模板追加；只使用临时工作簿并清理", 60)]
        public static async Task ExcelRoundTrip()
        {
            await WaitForEditorUpdate();
            string path = Path.Combine(Application.dataPath, "../Temp/AgentBridgeChecks-" + Guid.NewGuid().ToString("N") + ".xlsx");
            path = Path.GetFullPath(path);
            var command = new ExcelCommand();
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Data");
                    sheet.Cells[1, 1].Value = "Id";
                    sheet.Cells[1, 2].Value = "Name";
                    sheet.Cells[1, 3].Value = "Amount";
                    sheet.Cells[1, 4].Value = "Total";
                    sheet.Cells[2, 1].Value = 1;
                    sheet.Cells[2, 2].Value = "Old";
                    sheet.Cells[2, 3].Value = 2;
                    sheet.Cells[2, 3].Style.Numberformat.Format = "0.00";
                    sheet.Cells[2, 4].Formula = "C2*2";
                    package.SaveAs(new FileInfo(path));
                }
                JObject inspected = await Call(command, new { action = "inspect", filePath = path });
                string originalVersion = inspected.Value<string>("version");
                Require(inspected.Value<int>("sheetCount") == 1, "inspect sheet count");
                JObject read = await Call(command, new { action = "read_range", filePath = path, sheet = "Data", range = "A1:D2" });
                Require(read.Value<int>("returnedCellCount") == 8, "range contains eight cells");
                JObject found = await Call(command, new { action = "find_rows", filePath = path, sheet = "Data", where = new { Id = 1 } });
                Require(found.Value<int>("matchedCount") == 1, "typed numeric key query");
                JObject preview = await Call(command, new { action = "set_cells", filePath = path, sheet = "Data", dryRun = true,
                    changes = new[] { new { cell = "B2", value = "New", expectedValue = "Old" } } });
                Require(!preview.Value<bool>("saved") && preview.Value<int>("changedCount") == 1, "dry run reports unsaved change");
                Require((await Call(command, new { action = "inspect", filePath = path })).Value<string>("version") == originalVersion,
                    "dry run preserves workbook bytes");
                JObject saved = await Call(command, new { action = "set_cells", filePath = path, sheet = "Data", expectedVersion = originalVersion,
                    changes = new[] { new { cell = "B2", value = "New", expectedValue = "Old" } } });
                Require(saved.Value<bool>("saved"), "set_cells saves");
                await ExpectError("EXCEL_CONFLICT", () => Call(command, new { action = "set_cells", filePath = path, sheet = "Data",
                    expectedVersion = originalVersion, changes = new[] { new { cell = "B2", value = "Unexpected" } } }));
                string savedVersion = saved.Value<string>("versionAfter");
                await ExpectError("EXCEL_CONFLICT", () => Call(command, new { action = "set_cells", filePath = path, sheet = "Data",
                    changes = new[] { new { cell = "B2", value = "Staged", expectedValue = "New" },
                        new { cell = "D2", value = "Bad", expectedValue = "Wrong" } } }));
                Require((await Call(command, new { action = "inspect", filePath = path })).Value<string>("version") == savedVersion,
                    "later cell conflict leaves all disk changes unapplied");
                JObject upsert = await Call(command, new { action = "upsert_rows", filePath = path, sheet = "Data", keyColumns = new[] { "Id" },
                    rows = new[] { new { Id = 1, Name = "First", Amount = 3 }, new { Id = 2, Name = "Second", Amount = 4 } } });
                Require(upsert.Value<int>("updatedCount") == 1 && upsert.Value<int>("insertedCount") == 1, "upsert updates and appends");
                using (var package = new ExcelPackage(new FileInfo(path)))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];
                    Require(sheet.Cells[3, 1].GetValue<int>() == 2 && sheet.Cells[3, 2].Text == "Second", "append values persisted");
                    Require(sheet.Cells[3, 4].Formula == "C3*2", "template formula translated");
                    Require(sheet.Cells[3, 3].Style.Numberformat.Format == "0.00", "template number format preserved");
                }
                JObject noOp = await Call(command, new { action = "upsert_rows", filePath = path, sheet = "Data", keyColumns = new[] { "Id" },
                    rows = new[] { new { Id = 2, Name = "Second", Amount = 4 } } });
                Require(!noOp.Value<bool>("saved") && noOp.Value<int>("unchangedCount") == 1, "unchanged upsert preserves file");
                await ExpectError(ErrorCodes.InvalidParams, () => Call(command, new { action = "upsert_rows", filePath = path,
                    sheet = "Data", keyColumns = new[] { "Id" }, rows = new[] { new { Id = 3 }, new { Id = 3 } } }));
                await ExpectError(ErrorCodes.InvalidParams, () => Call(command, new { action = "read_range", filePath = path, sheet = "Data", range = "XFE1" }));
                await ExpectError(ErrorCodes.InvalidParams, () => Call(command, new { action = "inspect", filePath = "../outside.xlsx" }));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [AgentCallable("验证 CodeBind 重命名、现有序列化绑定和 Undo；仅使用隔离的临时场景", 60)]
        public static async Task CodeBindRoundTrip()
        {
            await WaitForEditorUpdate();
            Scene previous = SceneManager.GetActiveScene();
            Scene temporary = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            GameObject root = null;
            GameObject child = null;
            GameApp.Hotfix.MainMenuFormView probe = null;
            try
            {
                SceneManager.SetActiveScene(temporary);
                root = new GameObject("AgentBridgeCodeBindCheck");
                probe = root.AddComponent<GameApp.Hotfix.MainMenuFormView>();
                child = new GameObject("Original", typeof(RectTransform));
                child.transform.SetParent(root.transform);
                UnityEngine.UI.Button button = child.AddComponent<UnityEngine.UI.Button>();
                var command = new CodeBindCommand();
                child.name = "NewGame-Button";
                JObject result = await Call(command, new { action = "set_serialization", instanceId = root.GetInstanceID() });
                Require(new SerializedObject(probe).FindProperty("m_NewGameButton").objectReferenceValue == button && result["serialized"].Count() == 1,
                    "current CodeBind binder sets serialized reference");
                child.name = "Original";
                Undo.IncrementCurrentGroup();
                await Call(command, new { action = "rename_node", instanceId = child.GetInstanceID(), bindName = "Target" });
                Require(child.name == "Target_*", "rename applies bind marker");
                Undo.FlushUndoRecordObjects();
                Undo.PerformUndo();
                Require(child.name == "Original", "rename supports Undo");
                Type binder = typeof(CodeBindCommand).Assembly.GetReferencedAssemblies().Any(name => name.Name == "CodeBind.Editor")
                    ? AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "CodeBind.Editor").GetType("CodeBind.Editor.MonoBehaviourBinder") : null;
                Require(binder?.GetMethod("GenerateBindingSource") != null, "current code generation entry point exists");
            }
            finally
            {
                if (probe != null) Undo.ClearUndo(probe);
                if (child != null) Undo.ClearUndo(child);
                if (child != null) Undo.ClearUndo(child.transform);
                if (root != null) Undo.ClearUndo(root);
                SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(temporary, true);
            }
        }

        [AgentCallable("验证状态组/状态创建、布尔绑定、状态切换和 Prefab 保存；仅使用临时场景与临时 Prefab 并清理", 60)]
        public static async Task StateControllerRoundTrip()
        {
            await WaitForEditorUpdate();
            Scene previous = SceneManager.GetActiveScene();
            Scene temporary = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            string prefabPath = "Assets/AgentBridgeCheck-" + Guid.NewGuid().ToString("N") + ".prefab";
            GameObject root = null;
            try
            {
                SceneManager.SetActiveScene(temporary);
                root = new GameObject("AgentBridgeStateCheck");
                root.AddComponent<StateController.StateController>();
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                var command = new StateControllerCommand();
                await Call(command, new { action = "add_data", assetPath = prefabPath, dataName = "Visibility" });
                await Call(command, new { action = "add_state", assetPath = prefabPath, dataName = "Visibility", stateName = "Visible" });
                await Call(command, new { action = "add_state", assetPath = prefabPath, dataName = "Visibility", stateName = "Hidden" });
                await Call(command, new { action = "add_state_node", assetPath = prefabPath, dataName = "Visibility", stateType = "GameObjectActiveBinding" });
                JObject nodes = await Call(command, new { action = "list_nodes", assetPath = prefabPath, dataName = "Visibility" });
                Require(nodes["nodes"].Count() == 1, "boolean binding discovery uses primary group fields");
                JObject switched = await Call(command, new { action = "set_state", assetPath = prefabPath, dataName = "Visibility", stateName = "Visible" });
                Require(switched.Value<string>("stateName") == "Visible", "requested state selected");
                Require(!AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath).activeSelf, "boolean mapping applied and saved to prefab");
                JObject listed = await Call(command, new { action = "list", assetPath = prefabPath });
                Require(listed["controllers"][0]["datas"][0]["states"].Count() == 2, "prefab retains both states");
                await Call(command, new { action = "add_data", instanceId = root.GetInstanceID(), dataName = "Preview", states = "Off" });
                await Call(command, new { action = "add_state_node", instanceId = root.GetInstanceID(), dataName = "Preview", stateType = "GameObjectActiveBinding" });
                await Call(command, new { action = "set_state", instanceId = root.GetInstanceID(), dataName = "Preview", stateName = "Off" });
                Require(!root.activeSelf, "new scene binding applies even after controller runtime cache initialized");
            }
            finally
            {
                if (root != null) Undo.ClearUndo(root);
                AssetDatabase.DeleteAsset(prefabPath);
                SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(temporary, true);
            }
        }
    }
}
