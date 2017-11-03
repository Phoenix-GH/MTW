
using System.ComponentModel;
using System.Net;

namespace MyTenantWorld
{
	public class Staff : System.ComponentModel.INotifyPropertyChanged
    {
		public string userId { get; set; }
		public string userName { get; set; }
		public string permissionGroup { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
    }
	public class SpecificStaff
	{
		public string permissionGroup { get; set; }
		public string screenName { get; set; }
		public string email { get; set; }
		public string givenName { get; set; }
		public string familyName { get; set; }
		public string phoneNumber { get; set; }
		public string avatar { get; set; }
	}
    public class StaffResponse : SpecificStaff
    {
		public string message { get; set; }
		public HttpStatusCode status_code { get; set; }
    }
}
