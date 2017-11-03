using System;

namespace MyTenantWorld.Droid
{
	public class TemporalObject : Java.Lang.Object
	{
		public object Item { get; private set;}
		public Android.Views.View View { get; private set;}

		public TemporalObject (Android.Views.View v, object i)
		{
			View = v;
			Item = i;
		}
	}
}

