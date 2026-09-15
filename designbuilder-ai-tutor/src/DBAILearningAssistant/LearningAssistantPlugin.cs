using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using DB.Extensibility.Contracts;

namespace DBAILearningAssistant
{
    [Export(typeof(IPlugin2))]
    public sealed class LearningAssistantPlugin : PluginBase2, IPlugin2
    {
        private const string RootKey = "dbAiTutorRoot";
        private const string OpenKey = "dbAiTutorOpen";
        private AssistantForm form;

        public override bool HasMenu { get { return true; } }

        public override string MenuLayout
        {
            get
            {
                var menu = new StringBuilder();
                menu.AppendFormat("*DB AI 学习助手,{0}", RootKey);
                menu.AppendFormat("*>打开助手,{0}", OpenKey);
                return menu.ToString();
            }
        }

        public override bool IsMenuItemVisible(string key) { return true; }
        public override bool IsMenuItemEnabled(string key) { return true; }

        public override void OnMenuItemPressed(string key)
        {
            if (key != OpenKey) return;
            if (form == null || form.IsDisposed)
                form = new AssistantForm(ReadContext);
            form.Show();
            form.BringToFront();
            form.RefreshContext();
        }

        public override void ModelLoaded()
        {
            if (form != null && !form.IsDisposed) form.RefreshContext();
        }

        public override void ModelUnloaded()
        {
            if (form != null && !form.IsDisposed) form.RefreshContext();
        }

        public override void ScreenChanged(ScreenCode screenCode)
        {
            if (form != null && !form.IsDisposed) form.RefreshContext();
        }

        private string ReadContext()
        {
            try
            {
                var e = ApiEnvironment;
                var modelName = String.IsNullOrWhiteSpace(e.DsbFilePath)
                    ? "(未保存或未知)"
                    : Path.GetFileName(e.DsbFilePath);

                var s = new StringBuilder();
                s.AppendLine("DesignBuilder 版本: " + e.DesignBuilderVersion);
                s.AppendLine("项目已打开: " + e.IsActiveProject);
                s.AppendLine("模型文件: " + modelName);
                s.AppendLine("应用模式: " + e.ApplicationMode);
                s.AppendLine("视图: " + e.ViewType);
                s.AppendLine("模型层级: " + e.DecompositionLevel);
                s.AppendLine("当前对象类型: " + e.CurrentObjectType);
                s.AppendLine("当前命令: " + e.CurrentCommand);
                s.AppendLine("Building index: " + e.CurrentBuildingIndex);
                s.AppendLine("Block index: " + e.CurrentBuildingBlockIndex);
                s.AppendLine("Zone index: " + e.CurrentZoneIndex);
                s.AppendLine("Surface index: " + e.CurrentSurfaceIndex);
                s.AppendLine("Opening index: " + e.CurrentOpeningIndex);
                return s.ToString();
            }
            catch (Exception ex)
            {
                return "读取 DB 状态失败: " + ex.Message;
            }
        }
    }
}