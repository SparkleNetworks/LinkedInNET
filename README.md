
![](https://raw.githubusercontent.com/SparkleNetworks/LinkedInNET/master/src/LiNET-200.png)

LinkedInNET
===========

Sparkle.LinkedInNET will help you query the LinkedIn API :)

By using the LinkedIn APIs you agree to the [LinkedIn APIs Terms of Use](https://developer.linkedin.com/documents/linkedin-apis-terms-use).  
This project is released under the LGPL v3 license.  
This is NOT an official client library.

Motivation
------------

Bring the .NET world a nice LinkedIn client library.

Usage
------------

### 1. Installation

[Via NuGet](https://www.nuget.org/packages/Sparkle.LinkedInNET/)

````powershell
PM> Install-Package Sparkle.LinkedInNET
````

Or build the sources... You have to create your own .snk file.

### 2. Create API client with configuration

The `LinkedInApi` class is the entry point for all API calls. You must instantiate it with a configuration object. The minimum configuration is the API key and secret.  

````csharp
// create from config file
var config = LinkedInApiConfiguration.FromAppSettings("MyDemo.LinkedInConnect");
// or manually
var config = LinkedInApiConfiguration("api key", "api secret key");

// get the APIs client
var api = new LinkedInApi(config);
````

````xml
<configuration>
  <appSettings>
    <add key="MyDemo.LinkedInConnect.ApiKey" value="•••••••" />
    <add key="MyDemo.LinkedInConnect.ApiSecretKey" value="•••••••••••••" />
  </appSettings>
</configuration>
````

### 3. Create OAuth2 authorize url

The OAuth2 authentication process is fully supported. The `GetAUthorizationUrl` method will generate the OAuth2 url to navigate the user to.

````csharp
var scope = AuthorizationScope.ReadBasicProfile | AuthorizationScope.ReadEmailAddress;
var state = Guid.NewGuid().ToString();
var redirectUrl = "http://mywebsite/LinkedIn/OAuth2";
var url = api.OAuth2.GetAuthorizationUrl(scope, state, redirectUrl);
// https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=...
````

### 4. Get access token

When redirected to your own website, you can get an access code.

````csharp
// http://mywebsite/LinkedIn/OAuth2?code=...&state=...
var userToken = api.OAuth2.GetAccessToken(code, redirectUrl);
````

### 5. Example call: fetch user profile

````csharp
var user = new UserAuthorization(userToken.AccessToken);
var profile = api.Profiles.GetMyProfile(user);
````

Yes, you have to pass the token for each call. This might seem redundant for some but we prefer stateless objects for multi-threaded contexts. 

### 6. Field selectors

The API uses [field lists](https://developer.linkedin.com/documents/field-selectors) to fetch the desired data. Simple extension methods will allow you to make strongly-typed field selection.

````csharp
var profile = api.Profiles.GetMyProfile(
    user,
    FieldSelector.For<Person>().WithFirstname().WithLastname().WithLocationName());
// https://api.linkedin.com/v1/people/~:(first-name,last-name,location:(name))
````

The `.WithAllFields()` method will generate the list of all available fields. LinkedIn recommends not to do that.

````csharp
var profile = api.Profiles.GetMyProfile(
    user,
    FieldSelector.For<Person>().WithAllFields());
// https://api.linkedin.com/v1/people/~:(all available fields here)
// however it is not recommended to specify all fields
````

### 7. Errors

API error results throw `LinkedInApiException`s. You can find extra info in the Data collection.

````csharp
try
{
    var profile = this.api.Profiles.GetMyProfile(user);
}
catch (LinkedInApiException ex) // one exception type to handle
{
    // ex.Message
    // ex.InnerException // WebException
    // ex.Data["ResponseStream"]
    // ex.Data["HttpStatusCode"]
    // ex.Data["Method"]
    // ex.Data["UrlPath"]
    // ex.Data["ResponseText"]
}
////catch (Exception ex) { } //  bad, don't do that

````

Library internal errors throw `LinkedInNetException`s. You should not catch them as they do not represent a normal behavior. This may be usefull when waiting for a fix.

You should not catch `WebException`s as they are wrapped into `LinkedInApiException`s.

### 8. Explore

Code documentation is quite present. You auto-completion to discover stuff.

The MVC demo app has a /Explore page that demonstrates most API calls. Have a look at it.

Contribute
------------

We are generating code based on a [XML file](DefinitionFile.md). 
This XML file is manually filled to represent the API. 
We worked hard to bring something reliable. 
The API coverage should be implemented by expanding the XML file and enhancing code generation.

To generate the API code, build the "ServiceDefinition" project in Debug mode, then "Run custom tool" on the `Service.tt` file. The XML file will be read and most of the code will be updated automagically. 
  
To alter code generation, search for `CSharpGenerator.cs`. Different methods are responsible of generating different parts of C# code (return types, api groups, selectors).
  
To add/alter API methods and return types, search for `LinkedInApi.xml`. This file [describes the API in a human-readable and machine-readable way](DefinitionFile.md). Don't forget to re-generate the code (Service.tt).


References
------------

https://developer.linkedin.com/apis  
https://developer.linkedin.com/documents/authentication  


.NET version
------------

Supported .NET Framework versions:

* .NET 4.0 (dependencies: Newtonsoft.Json ≥ 4.5.8)
* .NET 3.5 (dependencies: Newtonsoft.Json ≥ 4.5.8)

We are using a lot of code generation so it won't be difficult to target 4.5 or any other framework. Implementing the async pattern won't be hard either.


Status
------------

See our [internal to-do list](src/ToDo.md).
