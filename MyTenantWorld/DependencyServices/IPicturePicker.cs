using System;
using System.IO;
using System.Threading.Tasks;

namespace MyTenantWorld
{
	public interface IPicturePicker
	{
		Task<Stream> GetImageStreamAsync();
	}

}