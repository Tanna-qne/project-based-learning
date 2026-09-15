using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBAILearningAssistant
{
    internal sealed class AssistantForm : Form
    {
        private readonly Func<string> contextReader;
        private readonly TextBox contextBox = new TextBox();
        private readonly TextBox chatBox = new TextBox();
        private readonly TextBox questionBox = new TextBox();
        private readonly Button askButton = new Button();
        private AppSettings settings;
        private DifyClient client;

        public AssistantForm(Func<string> contextReader)
        {
            this.contextReader = contextReader;
            settings = AppSettings.Load();
            client = new DifyClient(settings);

            Text = "DesignBuilder AI 学习助手";
            Width = 560; Height = 720; MinimumSize = new Size(440, 560);
            StartPosition = FormStartPosition.CenterScreen;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(6) };
            var refresh = new Button { Text = "刷新状态", AutoSize = true };
            var config = new Button { Text = "设置", AutoSize = true };
            refresh.Click += (s, e) => RefreshContext();
            config.Click += (s, e) => EditSettings();
            top.Controls.Add(refresh); top.Controls.Add(config);

            contextBox.Dock = DockStyle.Top; contextBox.Height = 175;
            contextBox.Multiline = true; contextBox.ReadOnly = true;
            contextBox.ScrollBars = ScrollBars.Vertical;
            contextBox.Font = new Font("Consolas", 9F);

            chatBox.Dock = DockStyle.Fill; chatBox.Multiline = true; chatBox.ReadOnly = true;
            chatBox.ScrollBars = ScrollBars.Vertical; chatBox.BackColor = Color.White;

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 94, Padding = new Padding(6) };
            askButton.Text = "提问"; askButton.Dock = DockStyle.Right; askButton.Width = 76;
            questionBox.Multiline = true; questionBox.Dock = DockStyle.Fill;
            askButton.Click += async (s, e) => await AskAsync();
            bottom.Controls.Add(questionBox); bottom.Controls.Add(askButton);

            Controls.Add(chatBox); Controls.Add(contextBox); Controls.Add(top); Controls.Add(bottom);
            RefreshContext();
        }

        public void RefreshContext()
        {
            if (InvokeRequired) { BeginInvoke(new Action(RefreshContext)); return; }
            contextBox.Text = contextReader();
        }

        private async Task AskAsync()
        {
            var q = questionBox.Text.Trim();
            if (q.Length == 0) return;
            askButton.Enabled = false;
            chatBox.AppendText("你： " + q + Environment.NewLine + Environment.NewLine);
            questionBox.Clear();
            try
            {
                RefreshContext();
                var answer = await client.AskAsync(q, contextBox.Text);
                chatBox.AppendText("助手： " + answer + Environment.NewLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                chatBox.AppendText("错误： " + ex.Message + Environment.NewLine + Environment.NewLine);
            }
            finally { askButton.Enabled = true; }
        }

        private void EditSettings()
        {
            using (var dialog = new Form { Text = "Dify 设置", Width = 520, Height = 230, StartPosition = FormStartPosition.CenterParent })
            {
                var url = new TextBox { Left = 120, Top = 20, Width = 360, Text = settings.DifyBaseUrl };
                var key = new TextBox { Left = 120, Top = 58, Width = 360, Text = settings.DifyApiKey, UseSystemPasswordChar = true };
                var user = new TextBox { Left = 120, Top = 96, Width = 360, Text = settings.UserId };
                var save = new Button { Left = 390, Top = 135, Width = 90, Text = "保存", DialogResult = DialogResult.OK };
                dialog.Controls.AddRange(new Control[] {
                    new Label { Left=18, Top=24, Text="Dify API 地址", AutoSize=true }, url,
                    new Label { Left=18, Top=62, Text="App API Key", AutoSize=true }, key,
                    new Label { Left=18, Top=100, Text="User ID", AutoSize=true }, user, save
                });
                dialog.AcceptButton = save;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                settings.DifyBaseUrl = url.Text.Trim();
                settings.DifyApiKey = key.Text.Trim();
                settings.UserId = user.Text.Trim();
                settings.Save();
                client = new DifyClient(settings);
            }
        }
    }
}