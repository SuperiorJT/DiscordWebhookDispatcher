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
        private ObservableCollection<EmbedBuilder> embeds = new ObservableCollection<EmbedBuilder>();
        private EmbedBuilder currentEmbed = new EmbedBuilder();
        private ObservableCollection<EmbedFieldBuilder> embedFields = new ObservableCollection<EmbedFieldBuilder>();

        public ObservableCollection<EmbedFieldBuilder> EmbedFields { get => embedFields; set => embedFields = value; }
        public ObservableCollection<EmbedBuilder> Embeds { get => embeds; set => embeds = value; }

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
                foreach(EmbedBuilder builder in this.Embeds)
                {
                    embeds.Add(builder.Build());
                }
                await client.SendMessageAsync(this.textBoxContent.Text, false, embeds.ToArray());
                this.embeds.Clear();
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

        private async void DisplayInvalidUrlDialog(string fieldName)
        {
            ContentDialog invalidUrlDialog = new ContentDialog
            {
                Title = "Invalid " + fieldName,
                Content = "A valid " + fieldName + " is required to send a webhook request. Enter a valid URL in the field and try again.",
                CloseButtonText = "Yes Daddy"
            };

            ContentDialogResult result = await invalidUrlDialog.ShowAsync();
        }

        private void buttonSendRequest_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteWebhookAsync();
        }

        private void buttonAddEmbedFieldCancel_Click(object sender, RoutedEventArgs e)
        {
            this.buttonAddEmbedFieldFlyout.Flyout.Hide();
        }

        private void flyoutAddEmbedField_Closed(object sender, object e)
        {
            this.textBlockFieldError.Visibility = Visibility.Collapsed;
            this.textBoxEmbedFieldName.Text = "";
            this.textBoxEmbedFieldValue.Text = "";
        }

        private void buttonAddEmbedField_Click(object sender, RoutedEventArgs e)
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();
            try
            {
                builder.Name = this.textBoxEmbedFieldName.Text;
                builder.Value = this.textBoxEmbedFieldValue.Text;
            }
            catch
            {
                this.textBlockFieldError.Visibility = Visibility.Visible;
                return;
            }
            this.EmbedFields.Add(builder);
            this.textBlockFieldError.Visibility = Visibility.Collapsed;
            this.buttonAddEmbedFieldFlyout.Flyout.Hide();
        }

        private void buttonAddEmbed_Click(object sender, RoutedEventArgs e)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = this.textBoxEmbedTitle.Text;
            builder.Description = this.textBoxEmbedDescription.Text;
            try
            {
                if (this.textBoxEmbedUrl.Text.Length > 0)
                {
                    builder.Url = this.textBoxEmbedUrl.Text;
                }
            }
            catch
            {
                this.DisplayInvalidUrlDialog("URL");
                return;
            }
            builder.Footer = new EmbedFooterBuilder();
            try
            {
                if (this.textBoxEmbedFooterIconUrl.Text.Length > 0)
                {
                    builder.Footer.IconUrl = this.textBoxEmbedFooterIconUrl.Text;
                }
            }
            catch
            {
                this.DisplayInvalidUrlDialog("Footer Icon URL");
                return;
            }
            builder.Footer.Text = this.textBoxEmbedFooterText.Text;
            try
            {
                if (this.textBoxEmbedThumbnailUrl.Text.Length > 0)
                {
                    builder.ThumbnailUrl = this.textBoxEmbedThumbnailUrl.Text;
                }
            }
            catch
            {
                this.DisplayInvalidUrlDialog("Thumbnail URL");
                return;
            }
            try
            {
                if (this.textBoxEmbedImageUrl.Text.Length > 0)
                {
                    builder.ImageUrl = this.textBoxEmbedImageUrl.Text;
                }
            }
            catch
            {
                this.DisplayInvalidUrlDialog("Image URL");
                return;
            }
            builder.Author = new EmbedAuthorBuilder();
            builder.Author.Name = this.textBoxEmbedAuthorName.Text;
            try
            {
                if (this.textBoxEmbedAuthorUrl.Text.Length > 0)
                {
                    builder.Author.Url = this.textBoxEmbedAuthorUrl.Text;
                }
            }
            catch
            {
                this.DisplayInvalidUrlDialog("Author URL");
                return;
            }
            builder.Author.IconUrl = this.textBoxEmbedAuthorIconUrl.Text;
            builder.Fields = this.EmbedFields.ToList();
            this.Embeds.Add(builder);
            this.currentEmbed = new EmbedBuilder();
            this.textBoxEmbedTitle.Text = "";
            this.textBoxEmbedDescription.Text = "";
            this.textBoxEmbedUrl.Text = "";
            this.textBoxEmbedTimestamp.Text = "";
            this.textBoxEmbedFooterIconUrl.Text = "";
            this.textBoxEmbedFooterText.Text = "";
            this.textBoxEmbedThumbnailUrl.Text = "";
            this.textBoxEmbedImageUrl.Text = "";
            this.textBoxEmbedAuthorName.Text = "";
            this.textBoxEmbedAuthorUrl.Text = "";
            this.textBoxEmbedAuthorIconUrl.Text = "";
            this.embedFields.Clear();
        }

        private void buttonEmbedFieldDelete_Click(object sender, RoutedEventArgs e)
        {
            EmbedFieldBuilder field = (sender as FrameworkElement).Tag as EmbedFieldBuilder;
            this.EmbedFields.Remove(field);
        }

        private void buttonEmbedListItem_Click(object sender, RoutedEventArgs e)
        {
            EmbedBuilder embed = (sender as FrameworkElement).Tag as EmbedBuilder;
            this.Embeds.Remove(embed);
        }
    }
}
