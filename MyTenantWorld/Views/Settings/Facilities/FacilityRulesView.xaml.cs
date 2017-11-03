using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyTenantWorld
{
	class SlotsPluralConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			int count = int.Parse(value.ToString());
			if (count != 1)
				return value.ToString() + " "+ parameter.ToString() + "s";
			else
				return value.ToString() + " " + parameter.ToString();
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException ();
		} 
	}


	class SliderColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			string status = value.ToString();
			int r = 0, g = 0, b = 0;
			
			r = (int)(255 * 0.6);
			g = (int)(255 * 0.76);
			b = (int)(255 * 0.35);
			
			string hex = String.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
			return hex;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	
	public partial class FacilityRulesView : ContentView
	{
		public static readonly BindableProperty advance_Booking_MinProperty = BindableProperty.Create("advance_Booking_Min", typeof(int), typeof(Stepper), 0, BindingMode.Default);
		public int advance_Booking_Min { get { return (int)GetValue(advance_Booking_MinProperty); } set { SetValue(advance_Booking_MinProperty, value); } }

		public static readonly BindableProperty advance_Booking_MaxProperty = BindableProperty.Create("advance_Booking_Max", typeof(int), typeof(Stepper), 0, BindingMode.Default);
		public int advance_Booking_Max { get { return (int)GetValue(advance_Booking_MaxProperty); } set { SetValue(advance_Booking_MaxProperty, value); } }

		public static readonly BindableProperty cancel_BookingProperty = BindableProperty.Create("cancel_Booking", typeof(int), typeof(Stepper), 0, BindingMode.Default);
		public int cancel_Booking { get { return (int)GetValue(cancel_BookingProperty); } set { SetValue(cancel_BookingProperty, value); } }

		public static readonly BindableProperty advance_Booking_MinTypeProperty = BindableProperty.Create("advance_Booking_MinType", typeof(string), typeof(Label), null, BindingMode.Default);
		public string advance_Booking_MinType { get { return (string)GetValue(advance_Booking_MinTypeProperty); } set { SetValue(advance_Booking_MinTypeProperty, value); } }

		public static readonly BindableProperty advance_Booking_MaxTypeProperty = BindableProperty.Create("advance_Booking_MaxType", typeof(string), typeof(Label), null, BindingMode.Default);
		public string advance_Booking_MaxType { get { return (string)GetValue(advance_Booking_MaxTypeProperty); } set { SetValue(advance_Booking_MaxTypeProperty, value); } }

		public static readonly BindableProperty cancel_BookingTypeProperty = BindableProperty.Create("cancel_BookingType", typeof(string), typeof(Label), "", BindingMode.Default);
		public string cancel_BookingType { get { return (string)GetValue(cancel_BookingTypeProperty); } set { SetValue(cancel_BookingTypeProperty, value); } }

		public static readonly BindableProperty max_BookingWithinGrpProperty = BindableProperty.Create("max_BookingWithinGrp", typeof(bool), typeof(Switch), false, BindingMode.Default);
		public bool max_BookingWithinGrp { get { return (bool)GetValue(max_BookingWithinGrpProperty); } set { SetValue(max_BookingWithinGrpProperty, value); } }

		public static readonly BindableProperty enable_Max_BookingProperty = BindableProperty.Create("enable_Max_Booking", typeof(bool), typeof(Switch), false, BindingMode.Default);
		public bool enable_Max_Booking { get { return (bool)GetValue(enable_Max_BookingProperty); } set { SetValue(enable_Max_BookingProperty, value); } }

		public static readonly BindableProperty max_Prime_BookingWithinGrpProperty = BindableProperty.Create("max_Prime_BookingWithinGrp", typeof(bool), typeof(Label), false, BindingMode.Default);
		public bool max_Prime_BookingWithinGrp { get { return (bool)GetValue(max_Prime_BookingWithinGrpProperty); } set { SetValue(max_Prime_BookingWithinGrpProperty, value); } }

		public static readonly BindableProperty max_BookingProperty = BindableProperty.Create("max_Booking", typeof(int), typeof(Label), 0, BindingMode.Default);
		public int max_Booking { get { return (int)GetValue(max_BookingProperty); } set { SetValue(max_BookingProperty, value); } }

		public static readonly BindableProperty max_BookingTypeProperty = BindableProperty.Create("max_BookingType", typeof(string), typeof(Label), null, BindingMode.Default);
		public string max_BookingType { get { return (string)GetValue(max_BookingTypeProperty); } set { SetValue(max_BookingTypeProperty, value); } }

		public static readonly BindableProperty max_Prime_BookingProperty = BindableProperty.Create("max_Prime_Booking", typeof(int), typeof(Label), 0, BindingMode.Default);
		public int max_Prime_Booking { get { return (int)GetValue(max_Prime_BookingProperty); } set { SetValue(max_Prime_BookingProperty, value); } }

		public static readonly BindableProperty max_Prime_BookingTypeProperty = BindableProperty.Create("max_Prime_BookingType", typeof(string), typeof(Label), null, BindingMode.Default);
		public string max_Prime_BookingType { get { return (string)GetValue(max_Prime_BookingTypeProperty); } set { SetValue(max_Prime_BookingTypeProperty, value); } }

		public static readonly BindableProperty slot_limit_BookingProperty = BindableProperty.Create("slot_Limit", typeof(int), typeof(Label), 0, BindingMode.Default);
		public int slot_Limit { get { return (int)GetValue(slot_limit_BookingProperty); } set { SetValue(slot_limit_BookingProperty, value); } }

		public static readonly BindableProperty allow_ConsecutiveProperty = BindableProperty.Create("allow_Consecutive", typeof(bool), typeof(Switch), false, BindingMode.Default);
		public bool allow_Consecutive { get { return (bool)GetValue(allow_ConsecutiveProperty); } set { SetValue(allow_ConsecutiveProperty, value); } }

		public static readonly BindableProperty booking_SameTypeProperty = BindableProperty.Create("booking_SameType", typeof(string), typeof(Label), null, BindingMode.Default);
		public string booking_SameType { get { return (string)GetValue(booking_SameTypeProperty); } set { SetValue(booking_SameTypeProperty, value); } }

		public static readonly BindableProperty booking_SameProperty = BindableProperty.Create("booking_Same", typeof(bool), typeof(Switch), false, BindingMode.Default);
		public bool booking_Same { get { return (bool)GetValue(booking_SameProperty); } set { SetValue(booking_SameProperty, value); } }

		public static readonly BindableProperty booking_SameWithinGrpProperty = BindableProperty.Create("booking_SameWithinGrp", typeof(bool), typeof(Label), false, BindingMode.Default);
		public bool booking_SameWithinGrp { get { return (bool)GetValue(booking_SameWithinGrpProperty); } set { SetValue(booking_SameWithinGrpProperty, value); } }

		public static readonly BindableProperty auto_Cancellation_BookingProperty = BindableProperty.Create("auto_Cancellation", typeof(int), typeof(Label), 0, BindingMode.Default);
		public int auto_Cancellation { get { return (int)GetValue(auto_Cancellation_BookingProperty); } set { SetValue(auto_Cancellation_BookingProperty, value); } }

		public static readonly BindableProperty auto_CancellationTypeProperty = BindableProperty.Create("auto_CancellationType", typeof(string), typeof(Label), null, BindingMode.Default);
		public string auto_CancellationType { get { return (string)GetValue(auto_CancellationTypeProperty); } set { SetValue(auto_CancellationTypeProperty, value); } }


		public FacilityRulesView(ContentPage parent)
		{
			InitializeComponent();

            Color[] gradientColors = new Color[] { Color.FromHex("f2c40f"), Color.FromHex("E84d3d") };

			GradientModel g = new GradientModel()
            {
                GradientColors = gradientColors,
                ViewWidth = 400,
                ViewHeight = 4,
                RoundCorners = false,
				CornerRadius = 0,
				LeftToRight = true
			};

			gradientView.SetBinding(GradientViewRender.GradientColorsProperty, "GradientColors");
			gradientView.SetBinding(GradientViewRender.CornerRadiusProperty, "CornerRadius");
			gradientView.SetBinding(GradientViewRender.ViewWidthProperty, "ViewWidth");
			gradientView.SetBinding(GradientViewRender.ViewHeightProperty, "ViewHeight");
			gradientView.SetBinding(GradientViewRender.RoundCornersProperty, "RoundCorners");
			gradientView.SetBinding(GradientViewRender.LeftToRightProperty, "LeftToRight");

			gradientView.BindingContext = g;
           
		}
	}
}
