using System.Net;

namespace MyTenantWorld
{
	public class FolderRequest
	{
		public string categoryName { get; set; }
		public bool visibility { get; set; }
	}

	public class Folder : FolderRequest
	{
		public string id { get; set; }
	}

	public class FileRequest
	{
		public string fileName { get; set; }
		public string description { get; set; }
		public string filePath { get; set; }
		public bool visibility { get; set; }
	}

	public class File : FileRequest
	{
		public string id { get; set; }
	}

    public class FileResponse : File
    {
        public string message { get; set; }
        public HttpStatusCode status_code { get; set; }
    }
}
