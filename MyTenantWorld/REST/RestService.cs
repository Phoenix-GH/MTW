using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyTenantWorld
{
	public class RestService : IRestService
	{
		HttpClient client;
		public RestService()
		{
			client = new HttpClient();
			client.BaseAddress = new Uri(Config.BaseURL);
			client.MaxResponseContentBufferSize = 2560000;
		}

		public async Task<LoginResponse> Login(Login user)
		{
            var uri = new Uri(Config.LoginURL);
			try {
				var formContent = new FormUrlEncodedContent(new[]
			  	{
					new KeyValuePair<string, string>("grant_type", user.grant_type),
					new KeyValuePair<string, string>("username", user.username),
					new KeyValuePair<string, string>("password", user.password),
                    new KeyValuePair<string, string>("client_id", user.client_id)
				});
                Debug.WriteLine("Login url------" + Config.LoginURL);
				HttpResponseMessage response = null;
				response = await client.PostAsync(uri, formContent);
				Debug.WriteLine("Login response------" + response);
				var result = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("Login result------" + result);
				var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(result);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					App.Current.Properties["access_token"] = loginResponse.access_token;
					App.Current.Properties["token_type"] = loginResponse.token_type;
				}

				loginResponse.status_code = response.StatusCode;
				return loginResponse;

			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"             Login ERROR {0}", ex.Message);
			}
			return null;
		}

		public async Task<BaseResponse> ResetPassword(string email)
		{   
			var uri = new Uri(Config.BaseURL + "/api/account/resetpassword");
			try {
				var json = JsonConvert.SerializeObject(email);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync(uri, content);
				var resetResponse = new BaseResponse
				{
					status_code = response.StatusCode
				};
				return resetResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"             ResetPassword ERROR {0}", ex.Message);
			}
			return null;
		}

		public async Task<BaseResponse> ActivateAccount(ActivateInfo info)
		{
			var uri = new Uri(Config.BaseURL + "/api/account/activate");
            setHeaders();
			try {
				var json = JsonConvert.SerializeObject(info);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				HttpResponseMessage response = null;
				response = await client.PostAsync(uri, content);
				Debug.WriteLine("ActivateAccount response------" + response);
				var activateResponse = new BaseResponse
				{
					status_code = response.StatusCode
				};
				return activateResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"            ActivateAccount ERROR {0}", ex.Message);
			}
			return null;
		}

		public async Task<HomeProfile> GetHomeProfile(bool fromlogin = false)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/home");
            if (fromlogin)
                uri = new Uri(Config.BaseURL + "/api/home?fromlogin=true");
			try {
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("profileresponse statuscode--------------" + response.StatusCode.ToString());
				var profileResponse = new HomeProfile();
                Debug.WriteLine("GetHomeProfile response--------------" + content);
				if (!string.IsNullOrEmpty(content))
				{
					profileResponse = JsonConvert.DeserializeObject<HomeProfile>(content);
				}
				profileResponse.status_code = response.StatusCode;
				return profileResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetHomeProfile ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<HomeProfile> GetHomeProfileWithPID(string pid)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/home/portfolio/" + pid);
			try {
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				var profileResponse = JsonConvert.DeserializeObject<HomeProfile>(content);
				profileResponse.status_code = response.StatusCode;
				return profileResponse;
			}

			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetHomeProfileWithPid ERROR {0}", ex.Message);
			}
			return null;
		}

		//02 Portfolio Setup
		public async Task<PortfolioInfo> GetPortfolioInfo(string Pid)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/"+Pid);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetPortfolioInfo--------------" + content);
				var profileResponse = new PortfolioInfo();
				if (!string.IsNullOrEmpty(content))
				{
					profileResponse = JsonConvert.DeserializeObject<PortfolioInfo>(content);
				}
				profileResponse.status_code = response.StatusCode;
				return profileResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetPortfolioInfo ERROR {0}", ex.StackTrace);
			}
			return null;		
		}

		public async Task<PortfolioInfo> PutPortfolioInfo(string Pid, PortfolioInfoRequest request)
		{
            setHeaders();
			var json = JsonConvert.SerializeObject(request);
			Debug.WriteLine(json);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + Pid);
			try
			{
				var response = await client.PutAsync(uri,content);
				var responseContent = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("PutPortfolioInfo--------------" + responseContent);
				var portfolioResponse = new PortfolioInfo();
				if (!string.IsNullOrEmpty(responseContent))
				{
					portfolioResponse = JsonConvert.DeserializeObject<PortfolioInfo>(responseContent);
				}
				portfolioResponse.status_code = response.StatusCode;
				return portfolioResponse;
			}
			catch (Exception ex)
			{
				
				Debug.WriteLine(@"				PutPortfolioInfo ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Wordpress> GetWordpressInfo(string Pid)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + Pid+"/wordpress");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetWordpressInfo--------------" + content);
				var resultResponse = new Wordpress();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<Wordpress>(content);
				}
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetWordpressInfo ERROR {0}", ex.StackTrace);
			}
			return null;	
		}

		//03 Portfolio - ManagingAgent Setting
		public async Task<BaseResponse> DeleteAgent(string defaultPid, Agent agent)
		{
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/managingagent");
            setHeaders();
			try
			{
				HttpResponseMessage response = null;
				response = await client.DeleteAsync(uri);
				var result = await response.Content.ReadAsStringAsync();
				var agentResponse = new BaseResponse();

				agentResponse.status_code = response.StatusCode;
				return agentResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"            DeleteAgent ERROR {0}", ex.Message);
			}
			return null;
		}

		public async Task<Agent> InsertAgent(string defaultPid, Agent agent)
		{
			var uri = new Uri(Config.BaseURL + "/api/portfolio/"+defaultPid+"/managingagent");
            setHeaders();
			try
			{
				var json = JsonConvert.SerializeObject(agent);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				HttpResponseMessage response = null;
				response = await client.PostAsync(uri, content);
				var result = await response.Content.ReadAsStringAsync();
				var agentResponse = new Agent();
				if (!string.IsNullOrEmpty(result))
				{
					Debug.WriteLine(result);
					agentResponse = JsonConvert.DeserializeObject<Agent>(result);
				}

				agentResponse.status_code = response.StatusCode;
				return agentResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"            InsertAgent ERROR {0}", ex.Message);
			}
			return null;
		}

		public async Task<List<Agent>> GetAgent(string defaultPid)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/managingagent");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("profileresponse--------------" + content);
				var agentResponses = new List<Agent>();
				if (!string.IsNullOrEmpty(content))
				{
					agentResponses = JsonConvert.DeserializeObject<List<Agent>>(content);
				}
				agentResponses[0].status_code = response.StatusCode;
				return agentResponses;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetAgent ERROR {0}", ex.StackTrace);
			}
			return null;
		
		}

		public async Task<Agent> PutAgent(string defaultPid, Agent agent)
		{
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/managingagent");
			setHeaders();
			try
			{
				var json = JsonConvert.SerializeObject(agent);
				Debug.WriteLine("PutAgent request-----", json);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				HttpResponseMessage response = null;
				response = await client.PostAsync(uri, content);
				var result = await response.Content.ReadAsStringAsync();
				var agentResponse = new Agent();
				if (!string.IsNullOrEmpty(result))
				{
					Debug.WriteLine(result);
					agentResponse = JsonConvert.DeserializeObject<Agent>(result);
				}

				agentResponse.status_code = response.StatusCode;
				return agentResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"            PutAgent ERROR {0}", ex.Message);
			}
			return null;
		}

		//07 Committee - Portfolio Setting
		public async Task<List<Committee>> GetCommittee(string defaultPid)
		{
			setHeaders();

			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/committee");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetCommittee--------------" + content);
				var committeeResponse = new List<Committee>();
				if (!string.IsNullOrEmpty(content))
				{
					committeeResponse = JsonConvert.DeserializeObject<List<Committee>>(content);
				}
				return committeeResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetCommittee ERROR {0}", ex.StackTrace);
			}
			return null;
		}

        public async Task<Dictionary<string,List<Committee>>> UpdateCommittee(string defaultPid, CommitteeRequest committees)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/committee");
			var json = JsonConvert.SerializeObject(committees);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("UpdateCommittee--------------" + content);
				var committeeResponse = new Dictionary<string,List<Committee>>();
				if (!string.IsNullOrEmpty(content))
				{
					committeeResponse = JsonConvert.DeserializeObject<Dictionary<string,List<Committee>>>(content);

				}
				return committeeResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				UpdateCommittee ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		//08 Facility - Portfolio Setting
		public async Task<List<FacilityItem>> GetAllFacility(string defaultPid)
		{
			setHeaders();

			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility?frombooking=true");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetAllFacility--------------" + content);
				var facilityItemResponse = new List<FacilityItem>();
				if (!string.IsNullOrEmpty(content))
				{
					facilityItemResponse = JsonConvert.DeserializeObject<List<FacilityItem>>(content);
				}
				return facilityItemResponse;
			}
			catch (Exception ex)
			{
				
				Debug.WriteLine(@"				GetAllFacility ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Facility> GetSpecificFacility(string defaultPid, string defaultFacilityID)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/"+defaultFacilityID);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetSpecificFacility--------------" + content);
				var facilityResponse = new Facility();
				if (!string.IsNullOrEmpty(content))
				{
					facilityResponse = JsonConvert.DeserializeObject<Facility>(content);
				}
				facilityResponse.status_code = response.StatusCode;
				return facilityResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetSpecificFacility ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Facility> InsertFacility(string defaultPid, FacilityRequest facility)
		{
			setHeaders();

			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility");
			var json = JsonConvert.SerializeObject(facility);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri,requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("InsertFacility--------------" + content);
				var facilityResponse = new Facility();
				if (!string.IsNullOrEmpty(content))
				{
					facilityResponse = JsonConvert.DeserializeObject<Facility>(content);
				}
				facilityResponse.status_code = response.StatusCode;
				return facilityResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				InsertFacility ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Facility> GetNewlyInsertedFacility(string defaultPid, string facilityID)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/" + facilityID);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetNewlyInsertedFacility--------------" + content);
				var facilityResponse = new Facility();
				if (!string.IsNullOrEmpty(content))
				{
					facilityResponse = JsonConvert.DeserializeObject<Facility>(content);
				}
				facilityResponse.status_code = response.StatusCode;
				return facilityResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetNewlyInsertedFacility ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Facility> UpdateFacility(string defaultPid, FacilityRequest facility, string facilityID)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/" + facilityID);
			var json = JsonConvert.SerializeObject(facility);
			Debug.WriteLine("UpdateFacilityContent--------------" + json);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("UpdateFacility--------------" + content);
				var facilityResponse = new Facility();
				if (!string.IsNullOrEmpty(content))
				{
					facilityResponse = JsonConvert.DeserializeObject<Facility>(content);
				}
				facilityResponse.status_code = response.StatusCode;
				return facilityResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				UpdateFacility ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Facility> DeleteFacility(string defaultPid, string facilityID)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/"+ facilityID);
			try
			{
				var response = await client.DeleteAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("DeleteFacility--------------" + content);
				var facilityResponse = new Facility();
				facilityResponse.status_code = response.StatusCode;
				return facilityResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				DeleteFacility ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Facility> GetFacilityGroup(string defaultPid)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facilitygroup/A");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetFacilityGroup--------------" + content);
				var facilityResponse = new Facility();
				if (!string.IsNullOrEmpty(content))
				{
					facilityResponse = JsonConvert.DeserializeObject<Facility>(content);
				}
				facilityResponse.status_code = response.StatusCode;
				return facilityResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetFacilityGroup ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		//12 Customisation - Setup
		public async Task<BaseResponse> DeleteCustomization(string defaultPid)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/customisation");
			try
			{
				var response = await client.DeleteAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("DeleteCustomization--------------" + content);
				var resultResponse = new BaseResponse();
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				DeleteCustomization ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Customization> InsertCustomization(string defaultPid, Customization customization)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/customisation");
			var json = JsonConvert.SerializeObject(customization);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("InsertCustomization--------------" + content);
				var resultResponse = new Customization();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<Customization>(content);
				}
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				InsertCustomization ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Customization> GetCustomization(string defaultPid)
		{
		    setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/customisation");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetCustomization--------------" + content);
				var customizationResponse = new Customization();
				if (!string.IsNullOrEmpty(content))
				{
					customizationResponse = JsonConvert.DeserializeObject<Customization>(content);
				}
				customizationResponse.status_code = response.StatusCode;
				return customizationResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetCustomization ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Customization> UpdateCustomization(string defaultPid, Customization customization)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/customisation");
			var json = JsonConvert.SerializeObject(customization);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("UpdateCustomization--------------" + content);
				var resultResponse = new Customization();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<Customization>(content);
				}
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				UpdateCustomization ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		void setHeaders()
		{
			client.DefaultRequestHeaders.Add("api-version", "2");
			if (App.Current.Properties.ContainsKey("access_token"))
			{
				Debug.WriteLine("Token-----------" + App.Current.Properties["access_token"]);
				client.DefaultRequestHeaders.Remove("Authorization");
				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + App.Current.Properties["access_token"]);
			}
		}

		//01 User - General Setup
		public async Task<List<Unit>> GetAllUnit(string pid)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + pid + "/unit");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetAllResident--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new List<Unit>();
					resultResponse = JsonConvert.DeserializeObject<List<Unit>>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetAllUnit ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		async public Task<List<Resident>> GetAllResident(string pid, string defaultUnitId)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + pid + "/unit/" + defaultUnitId+"/resident");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetAllResident--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new List<Resident>();
					resultResponse = JsonConvert.DeserializeObject<List<Resident>>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetAllResident ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<SpecificResident> GetSpecificResident(string pid, string defaultUnitId, string defaultUserId)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + pid + "/unit/" + defaultUnitId + "/resident/" + defaultUserId);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetSpecificResident--------------" + content);
				var resultResponse = new SpecificResident();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<SpecificResident>(content);
				}
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetSpecificResident ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<SpecificResident> UpdateResident(string Pid, string defaultUnitId, string defaultUserId, SpecificResident request)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + Pid + "/unit/" + defaultUnitId + "/resident/" + defaultUserId);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("UpdateResident--------------" + content);
				var resultResponse = new SpecificResident();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<SpecificResident>(content);
				}
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				UpdateResident ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<ResidentUser> InsertResident(string Pid, string defaultUnitId, string defaultUserId, ResidentUser request)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + Pid + "/unit/" + defaultUnitId + "/resident/" + defaultUserId);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("InsertResident--------------" + content);
				var resultResponse = new SpecificResident();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<SpecificResident>(content);
				}
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				InsertResident ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<SpecificResident> InsertResident(string Pid, string defaultUnitId, SpecificResident request)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + Pid + "/unit/" + defaultUnitId+"/resident");
			Debug.WriteLine("RequestURL---"+uri.OriginalString);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("InsertResident--------------" + content);
				var resultResponse = new SpecificResident();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<SpecificResident>(content);
				}
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				InsertResident ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<BaseResponse> DeleteResident(string Pid, string defaultUnitId, string defaultUserId)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + Pid + "/unit/" + defaultUnitId + "/resident/" + defaultUserId);
			try
			{
				var response = await client.DeleteAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("DeleteResident--------------" + content);
								var deleteResponse = new BaseResponse();
				deleteResponse.status_code = response.StatusCode;
				return deleteResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				DeleteResident ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		//06 Staff Setup
		public async Task<List<Staff>> GetAllStaff(string defaultPid)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/staff");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetAllStaff--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new List<Staff>();
					resultResponse = JsonConvert.DeserializeObject<List<Staff>>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetAllUnit ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<SpecificStaff> GetSpecificStaff(string defaultPid, string staffId)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/staff/" + staffId);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetSpecificStaff--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new SpecificStaff();
					resultResponse = JsonConvert.DeserializeObject<SpecificStaff>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetSpecificResident ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<StaffResponse> InsertStaff(string defaultPid, SpecificStaff request)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/staff");
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("InsertStaff--------------" + content);
                var resultResponse = new StaffResponse();
                if (!string.IsNullOrEmpty(content))
                    resultResponse = JsonConvert.DeserializeObject<StaffResponse>(content);
                resultResponse.status_code = response.StatusCode;
                return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				InsertStaff ERROR {0}", ex.StackTrace);
			}
			return null;
		}

        public async Task<StaffResponse> UpdateStaff(string defaultPid, SpecificStaff request, string staffId)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/staff/" + staffId);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("UpdateStaff--------------" + content);
                var resultResponse = new StaffResponse();
                if(!string.IsNullOrEmpty(content))
                    resultResponse = JsonConvert.DeserializeObject<StaffResponse>(content);
                resultResponse.status_code = response.StatusCode;
                return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				UpdateStaff ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<BaseResponse> DeleteStaff(string defaultPid, string staffId)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/staff/" + staffId);
			try
			{
				var response = await client.DeleteAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("DeleteStaff--------------" + content);
				var deleteResponse = new BaseResponse();
				deleteResponse.status_code = response.StatusCode;
				return deleteResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				DeleteStaff ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<List<SearchUnit>> SearchUserName(string Pid, string searchterm)
		{
            setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + Pid + "/unit?searchterm=" + searchterm);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("SearchUserName--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = JsonConvert.DeserializeObject<List<SearchUnit>>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				SearchUserName ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<List<Folder>> GetFolder(string defaultPid)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/folder");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetFolder--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new List<Folder>();
					resultResponse = JsonConvert.DeserializeObject<List<Folder>>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetFolder ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Folder> InsertFolder(string defaultPid, FolderRequest folder)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/folder");
			var json = JsonConvert.SerializeObject(folder);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("InsertFolder--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.Created)
				{
					var resultResponse = new Folder();
					resultResponse = JsonConvert.DeserializeObject<Folder>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				InsertFolder ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<Folder> UpdateFolder(string defaultPid, string folderID, FolderRequest folder)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/folder/" + folderID);
			var json = JsonConvert.SerializeObject(folder);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("UpdateFolder--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new Folder();
					resultResponse = JsonConvert.DeserializeObject<Folder>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				UpdateFolder ERROR {0}", ex.StackTrace);
			}
			return null;
		}

        public async Task<FileResponse> InsertFile(string defaultPid, string folderID, FileRequest file)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/folder/" + folderID+"/file");
			var json = JsonConvert.SerializeObject(file);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("InsertFile--------------" + content);
                var resultResponse = new FileResponse();
                if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<FileResponse>(content);
				}
                resultResponse.status_code = response.StatusCode;
                return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				InsertFile ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<File> UpdateFile(string defaultPid, string folderID, string fileID, FileRequest file)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/folder/" + folderID + "/file/" + fileID);
			var json = JsonConvert.SerializeObject(file);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("UpdateFile--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new File();
					resultResponse = JsonConvert.DeserializeObject<File>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				UpdateFile ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<BaseResponse> DeleteFile(string defaultPid, string folderID, string fileID)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/folder/" + folderID + "/file/" + fileID);
			try
			{
				var response = await client.DeleteAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("DeleteFile--------------" + content);
				var deleteResponse = new BaseResponse();
				deleteResponse.status_code = response.StatusCode;
				return deleteResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				DeleteFile ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<BaseResponse> DeleteFolder(string defaultPid, string folderID)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/folder/" + folderID);
			try
			{
				var response = await client.DeleteAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("DeleteFolder--------------" + content);
				var deleteResponse = new BaseResponse();
				deleteResponse.status_code = response.StatusCode;
				return deleteResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				DeleteFolder ERROR {0}", ex.StackTrace);
			}
			return null;
		}

		public async Task<List<File>> GetFile(string defaultPid, string folderID)
		{
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/folder/" + folderID+"/file");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetFile--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new List<File>();
					resultResponse = JsonConvert.DeserializeObject<List<File>>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"				GetFile ERROR {0}", ex.StackTrace);
			}
			return null;
		}

        public async Task<List<string>> SelectBlockNo(string defaultPid)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid +"/blockno");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("SelectBlockNo--------------" + content);
				var responseResult = new List<string>();
				if (!string.IsNullOrEmpty(content))
				{
					responseResult = JsonConvert.DeserializeObject<List<string>>(content);
				}
				return responseResult;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              SelectBlockNo ERROR {0}", ex.StackTrace);
			}
			return null;
		}

        public async Task<List<BaseUnit>> SelectUnit(string defaultPid, string blockNo)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/blockno/" + blockNo);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("SelectBlockNo--------------" + content);
                var responseResult = new List<BaseUnit>();
				if (!string.IsNullOrEmpty(content))
				{
                    responseResult = JsonConvert.DeserializeObject<List<BaseUnit>>(content);
				}
				return responseResult;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              SelectBlockNo ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<List<Tenant>> SelectResident(string defaultPid, string blockNo, string unitId)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/blockno/" + blockNo + "/unit/" + unitId);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("SelectResident--------------" + content);
				var responseResult = new List<Tenant>();
				if (!string.IsNullOrEmpty(content))
				{
					responseResult = JsonConvert.DeserializeObject<List<Tenant>>(content);
				}
				return responseResult;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              SelectResident ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        async public Task<FacilityBookingInfo> FacilityScreen(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/" + defaultFaciltyId + "/unit/" + unitId + "/tenant/" + tenantId + "/booking/" + defaultBookingDate);
            Debug.WriteLine(uri.ToString());
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("FacilityScreen--------------" + content);
				var facilityResponse = new FacilityBookingInfo();
				if (!string.IsNullOrEmpty(content))
				{
					facilityResponse = JsonConvert.DeserializeObject<FacilityBookingInfo>(content);
				}
				facilityResponse.status_code = response.StatusCode;
				return facilityResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              FacilityScreen ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BookingDetails> FacilityBookingDetailsScreen(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate, BookingRequest request)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/" + defaultFaciltyId + "/unit/" + unitId + "/tenant/" + tenantId + "/booking/" + defaultBookingDate);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("FacilityBookingDetailsScreen--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new BookingDetails();
					resultResponse = JsonConvert.DeserializeObject<BookingDetails>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              FacilityBookingDetailsScreen ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public Task<List<Tenant>> Reminder(string defaultPid, string unitId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaidBookingResponse> ConfirmBooking(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate, PaidBookingRequest request)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/" + defaultFaciltyId + "/unit/" + unitId + "/tenant/" + tenantId + "/booking/" + defaultBookingDate);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("PaidBookingResponse--------------" + content);
                var resultResponse = new PaidBookingResponse();
                if(!string.IsNullOrEmpty(content))
				    resultResponse = JsonConvert.DeserializeObject<PaidBookingResponse>(content);
                resultResponse.status_code = response.StatusCode;
                return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              PaidBookingResponse ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BaseResponse> AddNotifyResident(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate, NotifyResidentRequest request)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/" + defaultFaciltyId + "/unit/" + unitId + "/tenant/" + tenantId + "/notify/" + defaultBookingDate);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("AddNotifyResident--------------" + content);
		
				var resultResponse = new BaseResponse();
                if (!string.IsNullOrEmpty(content))
                    resultResponse = JsonConvert.DeserializeObject<BaseResponse>(content);
                resultResponse.status_code = response.StatusCode;
                return resultResponse;
				
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              AddNotifyResident ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public Task<BookingDetails> DeleteNotifyResident(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate)
        {
            throw new NotImplementedException();
        }

        public async Task<FacilityBookingInfo> FacilityScreen(string defaultPid, string defaultFaciltyId, string defaultBookingDate)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/" + defaultFaciltyId + "/booking/" + defaultBookingDate);
			Debug.WriteLine(uri.ToString());
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("FacilityScreen--------------" + content);
				var facilityResponse = new FacilityBookingInfo();
				if (!string.IsNullOrEmpty(content))
				{
					facilityResponse = JsonConvert.DeserializeObject<FacilityBookingInfo>(content);
				}
				facilityResponse.status_code = response.StatusCode;
				return facilityResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              FacilityScreen ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BookingResponse> ConfirmBooking(string defaultPid, string defaultFaciltyId, string defaultBookingDate, ReservedBookingRequest request)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/facility/" + defaultFaciltyId + "/booking/" + defaultBookingDate);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PostAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("ConfirmBooking--------------" + content);
				var resultResponse = new BookingResponse();
				if (!string.IsNullOrEmpty(content))
					resultResponse = JsonConvert.DeserializeObject<BookingResponse>(content);
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              ConfirmBooking ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<List<Booking>> GetBookingList(string defaultPid, string status)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/booking");

            if(!string.IsNullOrEmpty(status))
                uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/booking?status=" + status);    

			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetBookingList--------------" + content);
				var resultResponse = new List<Booking>();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<List<Booking>>(content);
				}
				
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              GetBookingList ERROR {0}", ex.StackTrace);
			}
			return null;
        }


		public async Task<Receipt> GetReceipt(string defaultPID, string receiptID)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/transaction/"+receiptID);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetReceipt--------------" + content);
				var resultResponse = new Receipt();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<Receipt>(content);
				}
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              GetReceipt ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BaseResponse> CancelBooking(string defaultPID, string defaultBookingID, CancelBookingRequest request)
        {
			
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/booking/" + defaultBookingID);
            if (request != null)
            {
                string uriString = Config.BaseURL + "/api/portfolio/" + defaultPID + "/booking/" + defaultBookingID + "?refundmethod=" + request.refundMethod;
                if (!string.IsNullOrEmpty(request.chequeNo))
                    uriString += "&chequeno=" + request.chequeNo;
                if(!string.IsNullOrEmpty(request.bank))
                    uriString +="&bank=" + request.bank;
                if (!string.IsNullOrEmpty(request.remarks))
					uriString += "&remarks=" + request.remarks;
                uri = new Uri(uriString);
                Debug.WriteLine(uriString);
            }
			setHeaders();
			try
			{
				HttpResponseMessage response = null;
                response = await client.DeleteAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("CancelBooking--------------" + content);
				var agentResponse = new BaseResponse();
                if(!string.IsNullOrEmpty(content))
                {
                    agentResponse = JsonConvert.DeserializeObject<BaseResponse>(content); 
                }
                agentResponse.status_code = response.StatusCode;
				return agentResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"            CancelBooking ERROR {0}", ex.Message);
			}
			return null;
        }

        public async Task<BaseResponse> ConfirmPayment(string defaultPID, string defaultBookingID, PaidBookingRequest request)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/booking/" + defaultBookingID );
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("ConfirmPayment--------------" + content);
				var resultResponse = new BaseResponse();
                if(!string.IsNullOrEmpty(content))
				    resultResponse = JsonConvert.DeserializeObject<BaseResponse>(content);
				return resultResponse;
				
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              UpdateFeedback ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BaseResponse> CancelReservation(string defaultPID, string defaultBookingID)
        {
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/booking/" + defaultBookingID);
			setHeaders();
			try
			{
				HttpResponseMessage response = null;
				response = await client.DeleteAsync(uri);
				var result = await response.Content.ReadAsStringAsync();
				var agentResponse = new BaseResponse();

				agentResponse.status_code = response.StatusCode;
				return agentResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"            CancelBooking ERROR {0}", ex.Message);
			}
			return null;
        }

        public async Task<List<Transaction>> ReceiptsRefundsScreen(string defaultPid)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/transaction");

			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("ReceiptsRefundsScreen--------------" + content);
				var resultResponse = new List<Transaction>();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<List<Transaction>>(content);
				}
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              ReceiptsRefundsScreen ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<List<Feedback>> GetFeedback(string defaultPID, string status, string startDate="", string endDate="")
        {
            setHeaders();
            bool added = false;

            string uriString = Config.BaseURL + "/api/portfolio/" + defaultPID + "/feedback";
            if (!string.IsNullOrEmpty(status))
            {
                if (!added)
                {
                    uriString += "?status=" + status;
                    added = true;
                }
                else
                    uriString += "&status=" + status;
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                if (!added)
                {
                    uriString += "?startdate=" + startDate;
                    added = true;
                }
                else
                    uriString += "&startdate=" + startDate;

            }
            if (!string.IsNullOrEmpty(endDate))
            {
                if (!added)
                {
                    uriString += "?enddate=" + endDate;
                    added = true;
                }
                else
                    uriString += "&enddate=" + endDate;

            }
            Debug.WriteLine(uriString);
            var uri = new Uri(uriString);

            try
            {
                var response = await client.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("GetFeedback--------------" + content);
                var resultResponse = new List<Feedback>();
                if (!string.IsNullOrEmpty(content))
                {
                    resultResponse = JsonConvert.DeserializeObject<List<Feedback>>(content);
                }
                return resultResponse;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              GetFeedback ERROR {0}", ex.StackTrace);
            }
            return null;
        }

        public async Task<FeedbackDetail> GetFeedbackDetails(string defaultPID, string feedbackID)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/feedback/" + feedbackID);

			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetFeedbackDetails--------------" + content);
				var resultResponse = new FeedbackDetail();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<FeedbackDetail>(content);
				}
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              GetFeedbackDetails ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<UpdateFeedbackResponse> UpdateFeedback(string defaultPID, string feedbackID, UpdateFeedbackRequest request)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/feedback/" + feedbackID);
			var json = JsonConvert.SerializeObject(request);
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("UpdateFeedback--------------" + content);
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var resultResponse = new UpdateFeedbackResponse();
                    resultResponse = JsonConvert.DeserializeObject<UpdateFeedbackResponse>(content);
					return resultResponse;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              UpdateFeedback ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BaseResponse> ForfeitDeposit(string defaultPID, string defaultBookingID, string reason)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/forfeit/" + defaultBookingID);
			var json = JsonConvert.SerializeObject(reason);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, requestContent);
                var resultResponse = new BaseResponse();
                resultResponse.status_code = response.StatusCode;
				return resultResponse;
				
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              ForfeitDeposit ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<ReceiptResponse> RefundDeposit(string defaultPID, string defaultBookingID)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/refund/" + defaultBookingID);
		
			//var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var response = await client.PutAsync(uri, null);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("RefundDeposit--------------" + content);
                Debug.WriteLine("Stautuscode--------------" + response.StatusCode.ToString());
                var resultResponse = new ReceiptResponse();
                if (!string.IsNullOrEmpty(content))
				    resultResponse = JsonConvert.DeserializeObject<ReceiptResponse>(content);
				
                resultResponse.status_code = response.StatusCode;
                return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              RefundDeposit ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<ConfirmResponse> ConfirmPayment(string defaultPID, string defaultBookingID)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/booking/" + defaultBookingID);

			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("ConfirmPayment--------------" + content);
                var resultResponse = new ConfirmResponse();
				if (!string.IsNullOrEmpty(content))
				{
                    resultResponse = JsonConvert.DeserializeObject<ConfirmResponse>(content);
				}
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              ConfirmPayment ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BaseResponse> LogOut(string defaultPID)
        {
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/account");
			setHeaders();
			try
			{
				HttpResponseMessage response = null;
				response = await client.DeleteAsync(uri);
				var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("LogOut--------------" + result);
				var resultResponse = new BaseResponse();
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"            LogOut ERROR {0}", ex.Message);
			}
			return null;
        }

        public async Task<List<Booking>> SearchBookingList(string defaultPid, string status, string searchTerm, string startDate, string endDate)
        {
			setHeaders();
            bool added = false;
            string uriString = Config.BaseURL + "/api/portfolio/" + defaultPid + "/booking";
            if(!string.IsNullOrEmpty(status))
            {
                if (!added)
                {
                    uriString += "?status=" + status;
                    added = true;
                }
                else
                    uriString += "&status="+status;
            }
            if (!string.IsNullOrEmpty(searchTerm))
			{
                if (!added)
                {
                    uriString += "?searchTerm=" + searchTerm;
                    added = true;
                }
				else
                    uriString += "&searchTerm=" + searchTerm;
			}
            if (!string.IsNullOrEmpty(startDate))
			{
                if (!added)
                {
                    uriString += "?startdate=" + startDate;
                    added = true;
                }
				else
					uriString += "&startdate=" + startDate;

			}
            if (!string.IsNullOrEmpty(endDate))
			{
				if (!added)
				{
					uriString += "?enddate=" + endDate;
					added = true;
				}
				else
					uriString += "&enddate=" + endDate;

			}
            Debug.WriteLine(uriString);
            var uri = new Uri(uriString);

			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("SearchBookingList--------------" + content);
				var resultResponse = new List<Booking>();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<List<Booking>>(content);
				}

				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              SearchBookingList ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<List<Transaction>> SearchByTransactionNo(string defaultPid, string transaction)
        {
			setHeaders();
			var	uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPid + "/transaction?searchterm=" + transaction);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("SearchByTransactionNo--------------" + content);
				var resultResponse = new List<Transaction>();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<List<Transaction>>(content);
				}
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              SearchByTransactionNo ERROR {0}", ex.StackTrace);
			}
			return null;
		}

        public async Task<TransactionHeader> GetTransactionHeader(string defaultPID)
        {
            setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/transactionheader");
            try
            {
                var response = await client.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("GetTransactionHeader--------------" + content);
                var resultResponse = new TransactionHeader();
                if (!string.IsNullOrEmpty(content))
                {
                    resultResponse = JsonConvert.DeserializeObject<TransactionHeader>(content);
                }
                return resultResponse;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"              GetTransactionHeader ERROR {0}", ex.StackTrace);
            }
            return null;
        }

        public async Task<BaseResponse> CreateAdhocRefund(string defaultPID, string unitID, string tenantID, Refund refund)
        {
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID+"/unit/"+unitID+"/tenant/"+tenantID+"/transaction");

			try
			{
				var json = JsonConvert.SerializeObject(refund);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
                Debug.WriteLine("CreateAdhocRefund--------------" + content);
				HttpResponseMessage response = await client.PostAsync(uri, content);
                var result = await response.Content.ReadAsStringAsync();
                var resultResponse = new BaseResponse();
                if (!string.IsNullOrEmpty(result))
				{
					resultResponse = JsonConvert.DeserializeObject<BaseResponse>(result);
				}
                resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"             CreateAdhocRefund ERROR {0}", ex.Message);
			}
			return null;
        }

        public async Task<List<Audit>> GetAuditListing(string defaultPID,string startDate = null, string endDate = null)
        {
			setHeaders();
            string uriString = Config.BaseURL + "/api/portfolio/" + defaultPID + "/audit";
            bool added = false;
			if (!string.IsNullOrEmpty(startDate))
			{
				if (!added)
				{
					uriString += "?startdate=" + startDate;
					added = true;
				}
				else
					uriString += "&startdate=" + startDate;

			}
			if (!string.IsNullOrEmpty(endDate))
			{
				if (!added)
				{
					uriString += "?enddate=" + endDate;
					added = true;
				}
				else
					uriString += "&enddate=" + endDate;

			}
            var uri = new Uri(uriString);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetAuditListing--------------" + content);
				var resultResponse = new List<Audit>();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<List<Audit>>(content);
				}
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              GetAuditListing ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BaseResponse> InsertNews(string defaultPID, NewsRequest news)
        {
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/news");

			try
			{
				var json = JsonConvert.SerializeObject(news);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				Debug.WriteLine("InsertNews--------------" + content);
				HttpResponseMessage response = await client.PostAsync(uri, content);
				var result = await response.Content.ReadAsStringAsync();
				var resultResponse = new BaseResponse();
				if (!string.IsNullOrEmpty(result))
				{
					resultResponse = JsonConvert.DeserializeObject<BaseResponse>(result);
				}
				resultResponse.status_code = response.StatusCode;
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"             InsertNews ERROR {0}", ex.Message);
			}
			return null;
        }

        public async Task<List<News>> GetNews(string defaultPID)
        {
			setHeaders();
			var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/news");
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetNews--------------" + content);
                var resultResponse = new List<News>();
				if (!string.IsNullOrEmpty(content))
				{
                    resultResponse = JsonConvert.DeserializeObject<List<News>>(content);
				}
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              GetNews ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<News> GetNewsDetails(string defaultPID, string newsID)
        {
			setHeaders();
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/news/"+newsID);
			try
			{
				var response = await client.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("GetNewsDetails--------------" + content);
				var resultResponse = new News();
				if (!string.IsNullOrEmpty(content))
				{
					resultResponse = JsonConvert.DeserializeObject<News>(content);
				}
				return resultResponse;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"              GetNewsDetails ERROR {0}", ex.StackTrace);
			}
			return null;
        }

        public async Task<BaseResponse> DeleteNews(string defaultPID, string newsID)
        {
            var uri = new Uri(Config.BaseURL + "/api/portfolio/" + defaultPID + "/news/"+newsID);
            setHeaders();
            try
            {
                HttpResponseMessage response = null;
                response = await client.DeleteAsync(uri);
                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("DeleteNews--------------" + result);
                var resultResponse = new BaseResponse();
                resultResponse.status_code = response.StatusCode;
                return resultResponse;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"            DeleteNews ERROR {0}", ex.Message);
            }
            return null;
        }
    }
}
