using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DBAILearningAssistant
{
    internal sealed class DifyClient
    {
        private readonly HttpClient http = new HttpClient();
        private readonly JavaScriptSerializer json = new JavaScriptSerializer();
        private readonly AppSettings settings;

        public string ConversationId { get; private set; } = "";

        public DifyClient(AppSettings settings)
        {
            this.settings = settings;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            http.Timeout = TimeSpan.FromSeconds(90);
        }

        public async Task<string> AskAsync(string question, string dbContext)
        {
            if (String.IsNullOrWhiteSpace(settings.DifyApiKey))
                throw new InvalidOperationException("请先点击“设置”，填写 Dify App API Key。");

            var query =
                "你是 DesignBuilder 7.3 中文学习助手。根据以下由插件读取的当前状态回答。"
                + "状态不能证明的内容请明确询问用户；不要声称看到了输入框；不要自动修改模型。\n\n"
                + "[当前状态]\n" + dbContext + "\n[用户问题]\n" + question;

            var body = new Dictionary<string, object>
            {
                ["inputs"] = new Dictionary<string, object>(),
                ["query"] = query,
                ["response_mode"] = "blocking",
                ["conversation_id"] = ConversationId,
                ["user"] = settings.UserId
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post,
                settings.DifyBaseUrl.TrimEnd('/') + "/chat-messages"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.DifyApiKey.Trim());
                request.Content = new StringContent(json.Serialize(body), Encoding.UTF8, "application/json");
                var response = await http.SendAsync(request).ConfigureAwait(false);
                var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Dify 请求失败 (" + (int)response.StatusCode + "): " + raw);

                var result = json.Deserialize<Dictionary<string, object>>(raw);
                if (result != null && result.ContainsKey("conversation_id"))
                    ConversationId = Convert.ToString(result["conversation_id"]);
                if (result == null || !result.ContainsKey("answer"))
                    throw new InvalidOperationException("Dify 返回中没有 answer 字段。");
                return Convert.ToString(result["answer"]);
            }
        }
    }
}