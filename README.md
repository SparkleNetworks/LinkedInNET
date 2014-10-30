
LinkedInNET
===========

Sparkle.LinkedInNET will help you query the LinkedIn API :)

By using the LinkedIn APIs you agree to the [LinkedIn APIs Terms of Use](https://developer.linkedin.com/documents/linkedin-apis-terms-use). 

Motivation
------------

Bring the .NET world a nice LinkedIn client library.

Usage
------------

### Create API client with configuration

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

### Create OAuth2 authorize url

The OAuth2 authentication process is fully supported. The `GetAUthorizationUrl` method will generate the OAuth2 url to navigate the user to.

````csharp
var scope = AuthorizationScope.ReadBasicProfile | AuthorizationScope.ReadEmailAddress;
var state = Guid.NewGuid().ToString();
var redirectUrl = "http://mywebsite/LinkedIn/OAuth2";
var url = api.OAuth2.GetAuthorizationUrl(scope, state, redirectUrl);
// https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=...
````

### Get access token

When redirected to your own website, you can get an access code.

````csharp
// http://mywebsite/LinkedIn/OAuth2?code=...&state=...
var userToken = api.OAuth2.GetAccessToken(code, redirectUrl);
````

### Fetch user profile

````csharp
var user = new UserAuthorization(userToken.AccessToken);
var profile = api.Profiles.GetMyProfile(user);
````

Yes, you have to pass the token for each call. This might seem redundant for some but we prefer stateless objects for multi-threaded contexts. 

### Field selectors

The API uses field lists to fetch the desired data. Simple extension methods will allow you to make strongly-typed field selection.

````csharp
var profile = api.Profiles.GetMyProfile(
    user,
    FieldSelector.For<Person>().WithFirstname().WithLastname().WithLocationName());
// https://api.linkedin.com/v1/people/~:(first-name,last-name,location:(name))
````

The `.WithAllFields()` method will generate the list of al available fields. LinkedIn recommends not to do that.

````csharp
var profile = api.Profiles.GetMyProfile(
    user,
    FieldSelector.For<Person>().WithAllFields());
// https://api.linkedin.com/v1/people/~:(all available fields here)
// however it is not recommended to specify all fields
````

Links: [Field selectors](https://developer.linkedin.com/documents/field-selectors), 

### Errors

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


Contribute
------------

We are generating code based on a XML file. 
This XML file is manually filled to represent the API. 
We are still working hard on bringing something reliable here. 
The API coverage should be implemented by expanding the XML file and enhancing code generation.

To generate the API code, simply "Run custom tool" on the `Service.tt` file.  
To alter code generation, search for `CSharpGenerator.cs`.  
To alter API methods and return types, search for `LinkedInApi.xml`.


References
------------

https://developer.linkedin.com/apis  
https://developer.linkedin.com/documents/authentication  


.NET version
------------

Supported .NET Framework versions:

* .NET 4.0 (no dependencies)
* .NET 3.5 (with a dependency to Newtonsoft.Json 6.0)

We are using a lot of code generation so it won't be difficult to target 4.5 or any other framework. Implementing the async pattern won't be hard neither.


Status
------------

[We are just at the beginning](src/ToDo.md).

* code generation is ok
* api coverage is very low; but rising
* requesting fields: fluid `FieldSelector` syntax is now working
