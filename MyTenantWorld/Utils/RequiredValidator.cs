
using Xamarin.Forms;

namespace MyTenantWorld
{
	public class RequiredValidator : Behavior<Entry>
	{
		static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(RequiredValidator), false);

		public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

		public bool IsValid
		{
			get { return (bool)base.GetValue(IsValidProperty); }
			private set { base.SetValue(IsValidPropertyKey, value); }
		}

		protected override void OnAttachedTo(Entry bindable)
		{
		bindable.TextChanged += HandleTextChanged;
		}

		void HandleTextChanged(object sender, TextChangedEventArgs e)
		{
			IsValid = !string.IsNullOrEmpty(e.NewTextValue);
	
			((Entry)sender).BackgroundColor = IsValid ? Color.Default : Color.Red;
		}

		protected override void OnDetachingFrom(Entry bindable)
		{
			bindable.TextChanged -= HandleTextChanged;
		}
	}
}
