using Discord;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordWebhookDispatcher
{
    public class WebhookRequestManager
    {
        private static WebhookRequestManager instance;
        private ObservableCollection<EmbedBuilder> embeds = new ObservableCollection<EmbedBuilder>();
        private string webhookLink = "";
        private string content = "";

        public ObservableCollection<EmbedBuilder> Embeds { get => embeds; set => embeds = value; }
        public string WebhookLink { get => webhookLink; set => webhookLink = value; }
        public string Content { get => content; set => content = value; }

        public static WebhookRequestManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WebhookRequestManager();
                }
                return instance;
            }
        }

        private WebhookRequestManager()
        {

        }

        public void AddEmbed(EmbedBuilder embed)
        {
            this.embeds.Add(embed);
        }
    }
}
