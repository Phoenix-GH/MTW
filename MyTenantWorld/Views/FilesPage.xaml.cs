using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.FilePicker;
using Xam.Plugin;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
namespace MyTenantWorld
{
	//public class SampleViewModel
	//{
	//	#region singleton
	//	public static SampleViewModel Instance => _instance ?? (_instance = new SampleViewModel());
	//	static SampleViewModel _instance;
	//	SampleViewModel()
	//	{
	//		ListItems.Add("Item 1");
	//		ListItems.Add("This is the second item");
	//		ListItems.Add("3rd Item <3");
	//	}
	//	#endregion

	//	#region fields
	//	IList<string> _listItems = new List<string>();
	//	#endregion

	//	#region properties
	//	public IList<string> ListItems
	//	{
	//		get { return _listItems; }
	//		set { _listItems = value; }
	//	}
	//	#endregion
	//}

	//[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FilesPage : ContentPage
	{
		RestService service = new RestService();
		public static readonly BindableProperty isBusyProperty =
		BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }
		private string folderID;
		public bool isLoaded;
        //public SampleViewModel ViewModel => SampleViewModel.Instance;
        //public PopupMenu Popup;
        private string path;
        private string fileName;
        private byte[] fileArray;
        public Dictionary<string, Folder> folderList;
        public Dictionary<string, File> fileList;

        public FilesPage(byte[] array = null, string fileName = null)
		{
            //BindingContext = ViewModel;

			InitializeComponent();
            this.fileName = fileName;
            NavigationPage.SetHasNavigationBar(this, false);
			service = new RestService();
			folderID = null;
			isLoaded = false;
            if (array != null)
                fileArray = array;
			//Popup = new PopupMenu()
			//{
			//	BindingContext = ViewModel
			//};

			//Popup.SetBinding(PopupMenu.ItemsSourceProperty, "ListItems");
           
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			if (!isLoaded)
			{
				if(await LoadFolder())
					isLoaded = true;
			}
		}

		async void AddNew(object sender, System.EventArgs e)
		{
			if (string.IsNullOrEmpty(folderID))
			{
				var promptConfig = new PromptConfig
				{
					Text = "New Folder",
					OnTextChanged = args =>
					{
						args.IsValid = true; // setting this to false will disable the OK/Positive button
					}
				};
				PromptResult dialog = await UserDialogs.Instance.PromptAsync(promptConfig);
				if (dialog.Ok)
				{
					var folder = new FolderRequest
					{
						categoryName = dialog.Value,
						visibility = true
					};
					var result = service.InsertFolder(App.Current.Properties["defaultPid"].ToString(), folder);
					if (result != null)
						await LoadFolder();
				}
			}
			else
			{
				try
				{
					var picker = await CrossFilePicker.Current.PickFile();

					var byteArray = picker.DataArray;
					string filePath = App.Current.Properties["defaultPid"].ToString() + "/files/upload/";
					var name = picker.FileName;
					this.isBusy = true;
					newButton.IsEnabled = false;
					var fileUpload = await AzureStorage.UploadFileAsync(ContainerType.condo, new MemoryStream(byteArray), filePath + name);
					if (!string.IsNullOrEmpty(fileUpload))
					{
						var file = new FileRequest
						{
							filePath = fileUpload,
							fileName = name,    
							visibility = true,
                            description = name,
						};

						var result = await service.InsertFile(App.Current.Properties["defaultPid"].ToString(), folderID, file);
                        if (result != null)
                        {
                            if (result.status_code == System.Net.HttpStatusCode.Created)
                                await LoadFiles(folderID);
                            else
                                await DisplayAlert("Error", result.message, "OK");
                        }
                        else
                            await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
                        
                        newButton.IsEnabled = true;
					}
                    this.isBusy = false;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}
		}

        async Task<bool> AddSharedFile()
        {
			try
			{	
				string filePath = App.Current.Properties["defaultPid"].ToString() + "/files/upload/";
								
				newButton.IsEnabled = false;
                MemoryStream stream = new MemoryStream(fileArray);
                var fileUpload = await AzureStorage.UploadFileAsync(ContainerType.condo, stream, filePath + fileName);
				if (!string.IsNullOrEmpty(fileUpload))
				{
					var file = new FileRequest
                    {
                        filePath = fileUpload,
                        fileName = fileName,
                        description = fileName,
						visibility = true
					};
                    this.isBusy = true;
					var result = await service.InsertFile(App.Current.Properties["defaultPid"].ToString(), folderID, file);
                    isBusy = false;
                    newButton.IsEnabled = true;
                    if (result != null)
                    {
                        await LoadFiles(folderID);
                        return true;
                    }


				}
                return false;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
            return false;
        }

		async void Delete_Clicked(object sender, System.EventArgs e)
		{
			var button = ((Button)sender);
			var delete = await DisplayAlert("Confirmation", "Are you sure to remove?", "OK", "Cancel");
			if (delete)
			{
				BaseResponse result = new BaseResponse();
                this.isBusy = true;
				if (button.CommandParameter.GetType() == typeof(Folder))
				{
					var item = (Folder)button.CommandParameter;
                    isBusy = true;
					result = await service.DeleteFolder(App.Current.Properties["defaultPid"].ToString(), item.id);
                    isBusy = false;

				}
				else if (button.CommandParameter.GetType() == typeof(File))
				{
					var item = (File)button.CommandParameter;
                    isBusy = true;
					result = await service.DeleteFile(App.Current.Properties["defaultPid"].ToString(), folderID, item.id);
                    isBusy = false;
				}

				if (result.status_code == System.Net.HttpStatusCode.NoContent)
				{
                    this.isBusy = false;
                    grid.Children.Clear();
					if (button.CommandParameter.GetType() == typeof(Folder))
						await LoadFolder();
					else
						await LoadFiles(folderID);
		
				}
			}

		}

		async Task<bool> LoadFolder()
		{
            this.isBusy = true;
			var result = await service.GetFolder(App.Current.Properties["defaultPid"].ToString());
            this.isBusy = false;
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped+= Gesture_Tapped;
			if (result != null)
			{
                folderList = new Dictionary<string, Folder>();
                grid.Children.Clear();
                int height = (int)(grid.Width - 150) / 4;
                for (int rowIndex = 0; rowIndex < result.Count / 4 + 1; rowIndex++)
                    grid.RowDefinitions.Add(new RowDefinition { Height = height });
                for (int i = 0; i < result.Count;i++)
                {
                    folderList.Add(result[i].id, result[i]);
                    Button menuButton = new Button()
                    {
                        Image = "ic_hamburger.png",
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions= LayoutOptions.End,
                        WidthRequest=30,
                        HeightRequest= 30,
                        StyleId = result[i].id
                    };
                    menuButton.Clicked+= FolderButton_Clicked; 
                    StackLayout layout = new StackLayout()
                    {
                        Orientation=StackOrientation.Vertical,
                        Children = {
                            new Image(){Source = "ic_folder.png", HorizontalOptions = LayoutOptions.Center},
							new Label(){Text=result[i].categoryName, HorizontalOptions=LayoutOptions.CenterAndExpand,
									TextColor=Color.FromHex("4a4a4a"), FontFamily="Lato-Bold", FontSize=13},
									menuButton
                            },
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };
                    Frame frame = new Frame()
                    {
                        Content = layout,
                        HeightRequest = height
                    };
                    frame.StyleId = result[i].id;
                    frame.GestureRecognizers.Add(tapGesture);

                    grid.Children.Add(frame,i%4, i/4);

                }
				return true;
			}   
			return false;
		}

		async Task<bool> LoadFiles(string folder_id)
		{
            isBusy = true;
			var result = await service.GetFile(App.Current.Properties["defaultPid"].ToString(), folder_id);
            isBusy = false;
			if (result != null)
			{
                fileList = new Dictionary<string, File>();
                grid.Children.Clear();
				int height = (int)(grid.Width - 150) / 4;
				for (int rowIndex = 0; rowIndex < result.Count / 4 + 1; rowIndex++)
					grid.RowDefinitions.Add(new RowDefinition { Height = height });
				for (int i = 0; i < result.Count; i++)
				{
                    fileList.Add(result[i].id, result[i]);
					Button menuButton = new Button()
					{
						Image = "ic_hamburger.png",
						BackgroundColor = Color.Transparent,
						HorizontalOptions = LayoutOptions.End,
						WidthRequest = 22,
						HeightRequest = 22,
						StyleId = result[i].id
					};
					menuButton.Clicked += FileButton_Clicked;
					StackLayout layout = new StackLayout()
					{
						Orientation = StackOrientation.Vertical,
						Children = {
							new Image(){Source="ic_file.png", HorizontalOptions = LayoutOptions.Center},
                            new Label(){Text=result[i].description, HorizontalOptions = LayoutOptions.Center,
								TextColor=Color.FromHex("4a4a4a"), FontFamily="Lato-Bold", FontSize=13},
                            menuButton
						},
						VerticalOptions = LayoutOptions.CenterAndExpand
					};
					Frame frame = new Frame()
					{
						Content = layout,
						HeightRequest = height
					};

					grid.Children.Add(frame, i % 4, i / 4);

				}
				return true;
			}

			return false;
		}

        async void Back_Clicked(object sender, System.EventArgs e)
        {
			if (!string.IsNullOrEmpty(folderID))
			{
				if (await LoadFolder())
				{
					folderID = null;
                    newButton.Text = "Add";
				}
			}
            else
            {
                await Back();
            }

        }

		void Edit(object sender, System.EventArgs e)
		{

		}

        async Task<bool> Back()
        {
			var res = await DisplayAlert("Confirm", "Are you going to close?", "OK", "Cancel");
            if (res)
            {
                if (fileArray == null)
                    await Navigation.PopAsync(true);
                else
                    await Navigation.PopModalAsync(true);
                return true;
            }
            return false;
            
        }

        protected override bool OnBackButtonPressed()
        {
            Back();
            return true;
        }
        async void Gesture_Tapped(object sender, EventArgs e)
        {
			this.isBusy = true;
            string selectedFolderId = ((Frame)sender).StyleId;
            if (fileArray==null)
            {
                if (await LoadFiles(selectedFolderId))
                {
                    folderID = selectedFolderId;
                    newButton.Text = "Upload";
                }
            }
            else
            {
                folderID = selectedFolderId;
                bool result = await AddSharedFile();
                if (!result)
                    
                    await DisplayAlert("Error", "File Uploading failed, please try again", "OK");
            }
			this.isBusy = false;
        }
         
        async void FolderButton_Clicked(object sender, EventArgs e){
            Button button = sender as Button;
			var promptConfig = new PromptConfig
			{
				OnTextChanged = args =>
				{
					args.IsValid = true; // setting this to false will disable the OK/Positive button
				}
			};

            if (folderList.ContainsKey(button.StyleId))
            {
                string lockButtonName = "Lock";
                if (folderList[button.StyleId].visibility == false)
                    lockButtonName = "Unlock";
                var res = await DisplayActionSheet("Select Action", "OK", "Cancel", "Rename", lockButtonName, "Delete");
                if (res == "Rename")
                {
                    promptConfig.Text = "New folder name";
                    PromptResult dialog = await UserDialogs.Instance.PromptAsync(promptConfig);
                    if (dialog.Ok)
                    {
                        var folder = new FolderRequest
                        {
                            categoryName = dialog.Value
                        };
                        isBusy = true;
                        var result = await service.UpdateFolder(App.Current.Properties["defaultPid"].ToString(), button.StyleId, folder);
                        isBusy = false;
                        if (result != null)
                            await LoadFolder();
                    }
                }
                else if (res == "Lock")
                {
                    var folder = new FolderRequest
                    {
                        visibility = false
                    };
                    isBusy = true;
                    var result = await service.UpdateFolder(App.Current.Properties["defaultPid"].ToString(), button.StyleId, folder);
                    isBusy = false;
                    if (result != null)
                        await LoadFolder();
                }
				else if (res == "Unlock")
				{
					var folder = new FolderRequest
					{
						visibility = true
					};
                    isBusy = true;
					var result = await service.UpdateFolder(App.Current.Properties["defaultPid"].ToString(), button.StyleId, folder);
                    isBusy = false;
					if (result != null)
						await LoadFolder();
				}
				else if (res == "Delete")
				{
                    var alert = await DisplayAlert("Confirm", "Are you sure to remove?", "OK", "Cancel");
                    if (alert)
                    {
                        isBusy = true;
                        var result = await service.DeleteFolder(App.Current.Properties["defaultPid"].ToString(), button.StyleId);
                        isBusy = false;
                        if (result != null)
                            await LoadFolder();
                    }
				}
            }
        }
		async void FileButton_Clicked(object sender, EventArgs e)
		{
			Button button = sender as Button;
			var promptConfig = new PromptConfig
			{
				OnTextChanged = args =>
				{
					args.IsValid = true; // setting this to false will disable the OK/Positive button
				}
			};

            if (fileList.ContainsKey(button.StyleId))
			{
				string lockButtonName = "Lock";
                if (fileList[button.StyleId].visibility == false)
					lockButtonName = "Unlock";
				var res = await DisplayActionSheet("Select Action", "OK", "Cancel", "Update File Description", lockButtonName    , "Delete");
				if (res == "Update File Description")
				{
					promptConfig.Text = "New file name";
					PromptResult dialog = await UserDialogs.Instance.PromptAsync(promptConfig);
					if (dialog.Ok)
					{
                        var file = new FileRequest 
						{
                            description = dialog.Value
						};
                        isBusy = true;
                        var result = await service.UpdateFile(App.Current.Properties["defaultPid"].ToString(), folderID, button.StyleId, file);
                        isBusy = false;
						if (result != null)
							await LoadFiles(folderID);
					}
				}
				else if (res == "Lock")
				{
					var file = new FileRequest
					{
						visibility = false
					};
                    isBusy = true;
                    var result = await service.UpdateFile(App.Current.Properties["defaultPid"].ToString(), folderID, button.StyleId, file);
                    isBusy = false;
					if (result != null)
						await LoadFiles(folderID);
				}
				else if (res == "Unlock")
				{
					var file = new FileRequest
					{
						visibility = true
					};
                    isBusy = true;
					var result = await service.UpdateFile(App.Current.Properties["defaultPid"].ToString(), folderID, button.StyleId, file);
                    isBusy = false;
					if (result != null)
                        await LoadFiles(folderID);
				}
				else if (res == "Delete")
				{
					var delete = await DisplayAlert("Confirmation", "Are you sure to remove?", "OK", "Cancel");
                    if (delete)
                    {
                        isBusy = true;
                        var result = await service.DeleteFile(App.Current.Properties["defaultPid"].ToString(), folderID, button.StyleId);
                        isBusy = false;
                        if (result != null)
                            await LoadFiles(folderID);
                    }
				}
			}
		}
    }
}