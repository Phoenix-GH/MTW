using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyTenantWorld.Droid;
using Newtonsoft.Json;
using RestSharp;

[assembly: Xamarin.Forms.Dependency(typeof(AzureRest))]
namespace MyTenantWorld.Droid
{
    public class AzureRest : IAzureRest
    {
        public HomeProfile GetHomeProfile(string authorisation)
        {
            var client = new RestClient("http://mtw.azurewebsites.net//api/home");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", authorisation);
            request.AddHeader("api-version", "2");
            IRestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject<HomeProfile>(response.Content);
        }
    }
}