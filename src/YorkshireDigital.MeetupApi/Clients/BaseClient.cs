﻿namespace YorkshireDigital.MeetupApi.Clients
{
    using Newtonsoft.Json;
    using RestSharp;
    using YorkshireDigital.MeetupApi.Models;
    using YorkshireDigital.MeetupApi.Requests;

    public class BaseClient
    {
        private const string ApiRoot = @"http://api.meetup.com/2/";
        internal readonly string ApiKey;
        internal readonly string MemberId;
        internal readonly IRestClient Client;

        public BaseClient(string apiKey, string memberId)
        {
            ApiKey = apiKey;
            MemberId = memberId;
            Client = new RestClient(ApiRoot);
        }

        public BaseClient(IRestClient restClient)
        {
            Client = restClient;
        }

        protected ApiResponse<T> Execute<T>(RestRequest request)
        {
            var response = Client.Execute(request);
            var json = response.Content;

            var content = JsonConvert.DeserializeObject<ApiResponse<T>>(json);

            return content;
        }

        public ApiResponse<T> Get<T>(BaseRequest request)
        {
            var restRequest = request.ToRestRequest(Method.GET, ApiKey);

            return Execute<T>(restRequest);
        }
    }
}
