using Discord;
using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DiscordWebhookDispatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public WebhookRequestManager WebhookRequestManager { get => WebhookRequestManager.Instance; }

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public async void ExecuteWebhookAsync()
        {

            String webhookLink = this.textBoxWebhookLink.Text;

            if (webhookLink.Length == 0)
            {
                this.DisplayInvalidWebhookLinkDialog();
                return;
            }

            try
            {
                String[] webhookLinkParts = webhookLink.Split('/');

                if (webhookLinkParts.Length != 7)
                {
                    throw new Exception();
                }

                ulong webhookId = ulong.Parse(webhookLinkParts[webhookLinkParts.Length - 2]);
                String webhookToken = webhookLinkParts[webhookLinkParts.Length - 1];

                DiscordWebhookClient client = new DiscordWebhookClient(webhookId, webhookToken);
                List<Embed> embeds = new List<Embed>();
                foreach(EmbedBuilder builder in this.WebhookRequestManager.Embeds)
                {
                    embeds.Add(builder.Build());
                }
                await client.SendMessageAsync(this.textBoxContent.Text, false, embeds.ToArray());
                this.WebhookRequestManager.Embeds.Clear();
                this.WebhookRequestManager.Content = "";
                this.textBoxContent.Text = "";
            }
            catch
            {
                this.DisplayInvalidWebhookLinkDialog();
                return;
            }
        }

        private async void DisplayInvalidWebhookLinkDialog()
        {
            ContentDialog noWebhookLinkDialog = new ContentDialog
            {
                Title = "Invalid Webhook Link",
                Content = "A valid Webhook link is required to send a webhook request. Enter it in the field at the top and try again.",
                CloseButtonText = "Yes Daddy"
            };

            ContentDialogResult result = await noWebhookLinkDialog.ShowAsync();
        }

        private void buttonSendRequest_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteWebhookAsync();
        }

        private void buttonAddEmbed_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddEmbed));
        }

        private void buttonEmbedListItem_Click(object sender, RoutedEventArgs e)
        {
            EmbedBuilder embed = (sender as FrameworkElement).Tag as EmbedBuilder;
            this.WebhookRequestManager.Embeds.Remove(embed);
        }
    }
}
