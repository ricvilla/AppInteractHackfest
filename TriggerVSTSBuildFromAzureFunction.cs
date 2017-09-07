using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
  var definitionId = -1;

  dynamic data = await req.Content.ReadAsAsync<object>();
  var branch = data?.push?.changes[0]?.@new?.name;

  if(branch == "master") definitionId = 6; // TODO update the branch name and definition Id to match your settings
  else if(branch == "ci/website-staging") definitionId = 7; // TODO update the branch name and definition Id to match your settings

  if (definitionId >= 0) // Known branch
  {
      string accessToken = GetEnvironmentVariable("PersonalAccessToken"); // TODO add your personal token to your app settings or paste it here
      const string instance = "instance_name"; // TODO put the instance name
      const string project = "project_name"; // TODO put the project name
      const string version = "api-version=2.0";

      var url = $"https://{instance}.visualstudio.com/DefaultCollection/{project}/_apis/build/builds?{version}";
      var authorizationToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{accessToken}"));
      var body = "{\"definition\" : {\"id\" : " + definitionId + "}}";

      return await PostAsync(url, body, authorizationToken);
  }

  return req.CreateResponse(HttpStatusCode.OK);
}

private static async Task<HttpResponseMessage> PostAsync(string url, string jsonBody, string authorizationToken = null)
{
  using (var client = new HttpClient())
  {
      if (!string.IsNullOrEmpty(authorizationToken))
          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationToken);
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      client.BaseAddress = new Uri(url);
      var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
      return await client.PostAsync("", content);
  }
}

private static string GetEnvironmentVariable(string name)
{
  return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}