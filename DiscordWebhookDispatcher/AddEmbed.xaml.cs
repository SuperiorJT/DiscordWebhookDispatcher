using Discord;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DiscordWebhookDispatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEmbed : Page
    {
        private ObservableCollection<EmbedFieldBuilder> embedFields = new ObservableCollection<EmbedFieldBuilder>();

        public AddEmbed()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<EmbedFieldBuilder> EmbedFields { get => embedFields; set => embedFields = value; }

        private void buttonEmbedFieldDelete_Click(object sender, RoutedEventArgs e)
        {
            EmbedFieldBuilder field = (sender as FrameworkElement).Tag as EmbedFieldBuilder;
            this.EmbedFields.Remove(field);
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

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
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

            WebhookRequestManager.Instance.AddEmbed(builder);

            this.Frame.Navigate(typeof(MainPage), builder);
        }
    }
}
