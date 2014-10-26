LinkedInNET
===========

Sparkle.LinkedInNET will help you query the LinkedIn API :)

Motivation
------------

Bring the .NET world a nice LinkedIn client library.

Usage
------------

### Create API client with configuration

````csharp
// create from config file
var config = LinkedInApiConfiguration.FromAppSettings("MyDemo.LinkedInConnect");
// or manually
var config = LinkedInApiConfiguration("api key", "api secret key");

// get the APIs client
var api = new LinkedInApi(config);
````

### Create OAuth2 authorize url

````csharp
var scope = AuthorizationScope.ReadBasicProfile | AuthorizationScope.ReadEmailAddress;
var state = Guid.NewGuid().ToString();
var redirectUrl = "http://mywebsite/LinkedIn/OAuth2";
var url = api.OAuth2.GetAuthorizationUrl(scope, state, redirectUrl);
// https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=...
````


### Get access token

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
    FieldSelector.For<Person>().WithAllFields());

var profile = api.Profiles.GetMyProfile(
    user,
    FieldSelector.For<Person>().WithFirstname().WithLastname());
````

Contribute
------------

We are generating code based on a XML file we fill manually. 
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

* .NET 4.0
* .NET 3.5 (dependency to Newtonsoft.Json 6.0)

We are using a lot of code generation so it won't be difficult to target 4.5 or any other framework. Implementing the async pattern won't be hard too.


Status
------------

[We are just at the beginning](src/ToDo.txt).

* code generation is ok but still changing a lot
* api coverage is very low
* requesting fields: nothing yet, important thing to do
