using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	class AttachmentCountConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
            if (value != null)
            {
                var list = (List<FeedbackDetailImage>)value;
                return list.Count;
            }
            return 0;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
    public partial class FeedbackDetailPage : ContentPage
    {
        public static readonly BindableProperty feedbackProperty = BindableProperty.Create("feedback", typeof(FeedbackDetail), typeof(FeedbackDetail), null, BindingMode.Default);
		public FeedbackDetail feedback { get { return (FeedbackDetail)GetValue(feedbackProperty); } set { SetValue(feedbackProperty, value); } }

        RestService service;
        string feedbackID;
        List<string> statusList;
        TapGestureRecognizer tapGesture;
        public FeedbackDetailPage(string id)
        {
            InitializeComponent();
            this.feedbackID = id;
            service = new RestService();
            NavigationPage.SetHasNavigationBar(this, false);
            statusList = new List<string>();
            statusList.Add("Pending");
            statusList.Add("Assigned");
            statusList.Add("Closed");
            statusPicker.ItemsSource = statusList;
            statusPicker.SelectedIndex = 0;
			tapGesture = new TapGestureRecognizer();
			tapGesture.Tapped += TapGesture_Tapped;
        }

        protected async override void OnAppearing()
        {
           
            base.OnAppearing();
            feedback = await service.GetFeedbackDetails(App.Current.Properties["defaultPid"].ToString(), feedbackID);
            if (feedback!=null)
            {
                var index = statusList.FindIndex(x=>x.ToLower().Equals(feedback.status.ToLower()));
                if (index >= 0)
                    statusPicker.SelectedIndex = index;
               
                foreach (var item in feedback.tFeedBackDetails)
                {
                    
                    Image newImage = new Image()
                    {
                        Margin = new Thickness(0, 0, 20, 0),
                        HeightRequest = 118,
                        Source = item.imagePath,
                        StyleId = item.imagePath,
                    };
                    newImage.GestureRecognizers.Add(tapGesture);
                    galleryLayout.Children.Add(newImage);
                }

            }
        }

		async void Back_Clicked(object sender, System.EventArgs e)
		{
            await OnBack();
		}

        protected override bool OnBackButtonPressed()
        {
            OnBack();
            return true;

        }

		async void PostReply(object sender, System.EventArgs e)
		{
            string status = statusPicker.SelectedItem as string;
            if (status.Equals("Pending") && replyText.Text.Length>0)
                status = "Acknowledged";
            var request = new UpdateFeedbackRequest()
            {
                status = status,
                replyremark = replyText.Text
            };
            var result = await service.UpdateFeedback(App.Current.Properties["defaultPid"].ToString(), feedbackID, request);
            if(result!=null)
            {
                await DisplayAlert("Success", "Your reply has been successfully posted!", "OK");
                await Navigation.PopAsync(true);
            }
		}

        async Task<bool> OnBack()
        {
            var result = await DisplayAlert("Close", "Are you going to close this window?", "OK", "Cancel");
            if(result)
            {
                await Navigation.PopAsync();
                return true;
            }
            return false;

        }

        void TapGesture_Tapped(object sender, EventArgs e)
        {
            string path = ((Image)sender).StyleId;
            fullImage.IsVisible = true;
            fullImage.Source = path;
        }

		void CloseFullImage(object sender, EventArgs e)
		{
            fullImage.IsVisible = false;
		}
    }
}
