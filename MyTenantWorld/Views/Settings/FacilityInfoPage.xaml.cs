using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace MyTenantWorld
{
	public class FacilitySlotDetails
	{
		//Used for passing data to carousel inside FacilitySlotView
		public string Title { get; set; }
        public ObservableCollection<FacilityTimeSlot> obscollection { get; set; }	
	}

	public partial class FacilityInfoPage : ContentPage
	{
		FacilityInfoView infoView;
		FacilityRulesView rulesView;
		FacilitySlotView slotView;
		Facility selectedFacility;
		public bool updateMode = false;
		public string facilityID;
		public ObservableCollection<FacilitySlotDetails> tFacilityTimeSlots { get; set; }
		RestService service;
		List<string> typeList;
		public bool initialLoad = true;
		Dictionary<string,string> bookingTypes;
		public FacilityInfoPage(bool updateMode,string FID=null)
		{
			InitializeComponent();
			this.updateMode = updateMode;
			this.facilityID = FID;
			NavigationPage.SetHasNavigationBar(this, false);
			infoView = new FacilityInfoView(this);
			rulesView = new FacilityRulesView(this);
			slotView = new FacilitySlotView(this);
			service = new RestService();
			typeList = new List<string>();
			typeList.Add("None");
			typeList.Add("A");
			typeList.Add("B");
			typeList.Add("C");
			typeList.Add("D");
			bookingTypes = new Dictionary<string, string>();
			bookingTypes.Add("m", "month");
			bookingTypes.Add("d", "day");
			bookingTypes.Add("w", "week");

			carouselView.ItemsSource = new List<DataTemplate>()
			{
				new DataTemplate(() => { return infoView; }),
				new DataTemplate(() => { return slotView; }),
				new DataTemplate(() => { return rulesView; })
			};
			selectedFacility = new Facility();
			tFacilityTimeSlots = new ObservableCollection<FacilitySlotDetails>(); //Storing FacilityTimeSlots
		}

		async protected override void OnAppearing()
		{
			base.OnAppearing();
			if (initialLoad)
			{
				if (updateMode)
				{
					Debug.WriteLine("Facility ID----------" + facilityID);
					var facility = await service.GetSpecificFacility(App.Current.Properties["defaultPid"].ToString(), facilityID);
					if (facility != null)
					{
                        if (facility.status_code == System.Net.HttpStatusCode.OK)
                        {
                            //Setting Info Page
                            infoView.name = facility.name;
                            infoView.description = facility.description;
                            if (!string.IsNullOrEmpty(facility.effectiveDate))
                                infoView.inOperationStart = DateTime.Parse(facility.effectiveDate);
                            if (!string.IsNullOrEmpty(facility.endDate))
                                infoView.inOperationEnd = DateTime.Parse(facility.endDate);
                            infoView.terms = facility.guideLines;
                            infoView.photo = facility.photo;
                            infoView.equipment = facility.equipment;
                            infoView.deposit = facility.deposit.ToString();
                            infoView.capacity = facility.capacity;
                            infoView.contactPerson = facility.contactPerson;
                            infoView.email = facility.contactEmail;
                            infoView.fax = facility.contactFax;
                            infoView.phone = facility.contactNumber;

                            int selectedSegment = 0;
                            if (!string.IsNullOrEmpty(facility.type))
                                selectedSegment = typeList.FindIndex((obj) => obj.ToLower() == facility.type.ToLower());
                            Debug.WriteLine("selectedSegment =" + selectedSegment.ToString());
                            infoView.type = selectedSegment;

                            var slotDictionary = new Dictionary<string, ObservableCollection<FacilityTimeSlot>>();
                            //Setting Slot Page


                            slotDictionary.Add("Monday", new ObservableCollection<FacilityTimeSlot>());
                            slotDictionary.Add("Tuesday", new ObservableCollection<FacilityTimeSlot>());
                            slotDictionary.Add("Wednesday", new ObservableCollection<FacilityTimeSlot>());
                            slotDictionary.Add("Thursday", new ObservableCollection<FacilityTimeSlot>());
                            slotDictionary.Add("Friday", new ObservableCollection<FacilityTimeSlot>());
                            slotDictionary.Add("Saturday", new ObservableCollection<FacilityTimeSlot>());
                            slotDictionary.Add("Sunday", new ObservableCollection<FacilityTimeSlot>());

                            foreach (var item in facility.tFacilityTimeSlots)
                            {
                                slotDictionary[item.slotDay].Add(item);
                            }

                            foreach (var item in slotDictionary)
                            {
                                if (item.Value.Count > 0)
                                {
                                    tFacilityTimeSlots.Add(new FacilitySlotDetails
                                    {
                                        Title = item.Key,
                                        obscollection = item.Value
                                    });
                                }
                            }

                            slotView.BindingContext = this;
                            slotView.SetBinding(FacilitySlotView.tFacilityTimeSlotsProperty, "tFacilityTimeSlots");
                            //Setting
                            rulesView.advance_Booking_Min = facility.tFacilityOtherSetting.advance_Booking_Min;
                            rulesView.advance_Booking_Max = facility.tFacilityOtherSetting.advance_Booking_Max;
                            rulesView.cancel_Booking = facility.tFacilityOtherSetting.cancel_Booking;

                            if (bookingTypes.ContainsKey(facility.tFacilityOtherSetting.advance_Booking_MinType))
                            {
                                rulesView.advance_Booking_MinType = bookingTypes[facility.tFacilityOtherSetting.advance_Booking_MinType];
                                if (facility.tFacilityOtherSetting.advance_Booking_Min != 1)
                                    rulesView.advance_Booking_MinType += "s";
                            }

                            if (bookingTypes.ContainsKey(facility.tFacilityOtherSetting.advance_Booking_MaxType))
                            {
                                rulesView.advance_Booking_MaxType = bookingTypes[facility.tFacilityOtherSetting.advance_Booking_MaxType];
                                if (facility.tFacilityOtherSetting.advance_Booking_Max != 1)
                                    rulesView.advance_Booking_MaxType += "s";
                            }

                            if (bookingTypes.ContainsKey(facility.tFacilityOtherSetting.cancel_BookingType))
                            {
                                rulesView.cancel_BookingType = bookingTypes[facility.tFacilityOtherSetting.cancel_BookingType];
                                if (facility.tFacilityOtherSetting.cancel_Booking != 1)
                                    rulesView.cancel_BookingType += "s";
                            }
                            rulesView.max_BookingWithinGrp = facility.tFacilityOtherSetting.max_BookingWithinGrp;
                            rulesView.enable_Max_Booking = facility.tFacilityOtherSetting.enable_Max_Booking;
                            rulesView.max_Prime_BookingWithinGrp = facility.tFacilityOtherSetting.max_Prime_BookingWithinGrp;
                            rulesView.max_Booking = facility.tFacilityOtherSetting.max_Booking;

                            if (bookingTypes.ContainsKey(facility.tFacilityOtherSetting.max_BookingType))
                                rulesView.max_BookingType = bookingTypes[facility.tFacilityOtherSetting.max_BookingType];
                            rulesView.max_Prime_Booking = facility.tFacilityOtherSetting.max_Prime_Booking;

                            if (bookingTypes.ContainsKey(facility.tFacilityOtherSetting.max_Prime_BookingType))
                                rulesView.max_Prime_BookingType = bookingTypes[facility.tFacilityOtherSetting.max_Prime_BookingType];

                            rulesView.slot_Limit = facility.tFacilityOtherSetting.slot_Limit;
                            rulesView.allow_Consecutive = facility.tFacilityOtherSetting.allow_Consecutive;
                            if (!string.IsNullOrEmpty(facility.tFacilityOtherSetting.booking_SameType))
                            {
                                if (bookingTypes.ContainsKey(facility.tFacilityOtherSetting.booking_SameType))
                                {
                                    rulesView.booking_SameType = facility.tFacilityOtherSetting.booking_SameType;
                                }
                            }
                            rulesView.booking_Same = facility.tFacilityOtherSetting.booking_Same;
                            rulesView.booking_SameWithinGrp = facility.tFacilityOtherSetting.booking_SameWithinGrp;
                            rulesView.auto_Cancellation = facility.tFacilityOtherSetting.auto_Cancellation;

                            if (bookingTypes.ContainsKey(facility.tFacilityOtherSetting.auto_CancellationType))
                            {
                                if (bookingTypes.ContainsKey(facility.tFacilityOtherSetting.auto_CancellationType))
                                {
                                    rulesView.auto_CancellationType = bookingTypes[facility.tFacilityOtherSetting.auto_CancellationType];
                                    if (facility.tFacilityOtherSetting.auto_Cancellation != 1)
                                        rulesView.auto_CancellationType += "s";
                                }
                            }
                        }
					}
				}
				initialLoad = false;
			}
		}

		void InfoPage(object sender, System.EventArgs e)
		{
			carouselView.Position = 0;
		}

		void SlotPage(object sender, System.EventArgs e)
		{
			carouselView.Position = 1;
		}

		void RulesPage(object sender, System.EventArgs e)
		{
			carouselView.Position = 2;
		}

		async void Back_Clicked(object sender, System.EventArgs e)
		{
			var close = await DisplayAlert("Confirmation", "Do you want to close the window?", "Yes", "No");
			if (close)
				await Navigation.PopAsync();
		}

		async void SaveData(object sender, System.EventArgs e)
		{
			
			var result = new Facility();
			Debug.WriteLine("current page index ---- " + carouselView.Position.ToString());
			FacilityRequest facilityRequest = new FacilityRequest();

			List<FacilityTimeSlot> updatedSlots = new List<FacilityTimeSlot>();
			FacilityOtherSetting otherSetting = new FacilityOtherSetting();

			Debug.WriteLine(rulesView.allow_Consecutive.ToString());
			if (infoView.inOperationStart > infoView.inOperationEnd)
			{
				await DisplayAlert("Error", Config.DateValidatinMsg, "OK");
				return;
			}
			if (!Utils.IsValidEmail(infoView.email) || Utils.IsAlaphabetContained(infoView.fax) )
			{
				await DisplayAlert(Config.InvalidEmailFormatTitle, Config.InvalidEmailFormatMsg, "OK");
				return;
			}
			Debug.WriteLine("infoviewtype" + infoView.type.ToString());

			if (infoView.inOperationStart.Date > infoView.inOperationEnd.Date)
			{
				await DisplayAlert("Error", "The Operation Start date cannot be later than the Operation End Date.", "OK");
				return;
			}

			facilityRequest = new FacilityRequest
			{
				name = infoView.name,
				description = infoView.description,
				equipment = infoView.equipment,
				capacity = infoView.capacity,
				deposit = Convert.ToInt32(infoView.deposit),
				contactPerson = infoView.contactPerson,
				contactNumber = infoView.phone,
				contactFax = infoView.fax,
				contactEmail = infoView.email,
				guideLines = infoView.terms,
				status = true,
				effectiveDate = infoView.inOperationStart.Date.ToString(),
				endDate = infoView.inOperationEnd.Date.ToString(),
				photo = infoView.photo
			};

			if (infoView.newPhoto != null)
			{
				string filePath = App.Current.Properties["defaultPid"].ToString() + "images/facility/";
				var name = Guid.NewGuid().ToString();
				facilityRequest.photo = await AzureStorage.UploadFileAsync(ContainerType.condo, infoView.newPhoto, filePath+name);
			}
			if (infoView.type > 0)
				facilityRequest.type = typeList[infoView.type];


            if (slotView.tFacilityTimeSlots != null)
			{
				foreach (var item in slotView.tFacilityTimeSlots)
				{
					foreach (var detailItem in item.obscollection)
					{
                        if(DateTime.Parse(detailItem.startTime)>DateTime.Parse(detailItem.endTime))
                        {
                            await DisplayAlert("Error", Config.DateValidatinMsg,"OK");
                            return;
                        }
                        updatedSlots.Add(detailItem);
					}
				}
			}
			facilityRequest.tFacilityTimeSlots = updatedSlots;

			otherSetting = new FacilityOtherSetting
			{
				advance_Booking_Min = rulesView.advance_Booking_Min,
				advance_Booking_Max = rulesView.advance_Booking_Max,
				cancel_Booking = rulesView.cancel_Booking,
				advance_Booking_MinType = rulesView.advance_Booking_MinType.Substring(0,1).ToLower(),
				advance_Booking_MaxType = rulesView.advance_Booking_MaxType.Substring(0,1).ToLower(),
				cancel_BookingType = rulesView.cancel_BookingType.Substring(0,1).ToLower(),
				max_BookingWithinGrp = rulesView.max_BookingWithinGrp,
				enable_Max_Booking = rulesView.enable_Max_Booking,
				max_Prime_BookingWithinGrp = rulesView.max_Prime_BookingWithinGrp,
				max_Booking = rulesView.max_Booking,
				max_BookingType = rulesView.max_BookingType.Substring(0,1).ToLower(),
				max_Prime_Booking = rulesView.max_Prime_Booking,
				max_Prime_BookingType = rulesView.max_Prime_BookingType.Substring(0,1).ToLower(),
				slot_Limit = rulesView.slot_Limit,
				allow_Consecutive = rulesView.allow_Consecutive,
				booking_SameType = rulesView.booking_SameType,
				booking_Same = rulesView.booking_Same,
				booking_SameWithinGrp = rulesView.booking_SameWithinGrp,
				auto_Cancellation = rulesView.auto_Cancellation,
				auto_CancellationType = rulesView.auto_CancellationType.Substring(0,1).ToLower()
			};

			facilityRequest.tFacilityOtherSetting = otherSetting;
					
			if(updateMode)
				result = await service.UpdateFacility(App.Current.Properties["defaultPid"].ToString(), facilityRequest, facilityID);
			else
				result = await service.InsertFacility(App.Current.Properties["defaultPid"].ToString(), facilityRequest);
			if (result != null)
			{
				if (result.status_code == System.Net.HttpStatusCode.OK)
				{
					await DisplayAlert("Success", Config.SuccessfulSaveMsg, "OK");
					await Navigation.PopAsync();
				}
				else
					await DisplayAlert("Error", result.message, "OK");
			}
		}

	}
}
