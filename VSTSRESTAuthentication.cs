using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

public static class AuthenticateSource
{
	private static VssConnection connection = null;
	private static WorkItemTrackingHttpClient _witClient = null;

	public static void Login(string personalAccessToken, string collectionUrl)
	{
		string collectionUri = collectionUrl;
		VssConnection connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(string.Empty, personalAccessToken));
		_witClient = connection.GetClient<WorkItemTrackingHttpClient>();
	}

	public static void Logoff()
	{
		connection.Disconnect();
		_witClient = null;
	}

	public static WorkItemTrackingHttpClient WitClient { get { return _witClient; } }
}