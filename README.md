LinkedInNET
===========

Sparkle.LinkedInNET will help you query the LinkedIn API :)

Motivation
------------

Bring the .NET world a nice LinkedIn client library.

Usage
------------

### Create API client with configuration

    // create from config file
    var config = LinkedInApiConfiguration.FromAppSettings("MyDemo.LinkedInConnect");
    // or manually
    var config = LinkedInApiConfiguration("api key", "api secret key");
    
    var api = new LinkedInApi(config);

### Create OAuth2 authorize url

    var scope = AuthorizationScope.ReadBasicProfile | AuthorizationScope.ReadEmailAddress;
    var state = Guid.NewGuid().ToString();
    var redirectUrl = "http://mywebsite/LinkedIn/OAuth2";
    var url = api.OAuth2.GetAuthorizationUrl(scope, state, redirectUrl);
    // https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=...


### Get access token

    // http://mywebsite/LinkedIn/OAuth2?code=...&state=...
    var userToken = api.OAuth2.GetAccessToken(code, redirectUrl);

### Fetch user profile

    var user = new UserAuthorization(userToken.AccessToken);
    var profile = api.Profiles.GetMyProfile(user);


Contribute
------------

We are generating code based on a XML file we fill manually. 
We are still working hard on bringing something reliable here.
The API coverage should be implemented by expanding the XML file and enhancing code generation.


References
------------

https://developer.linkedin.com/apis  
https://developer.linkedin.com/documents/authentication  


.NET version
------------

The initial library targets 3.5. 

We are using a lot of code generation so it won't be difficult to target 4.5 or any other framework.


Status
------------

We are just at the beginning.

* code generation is ok but still changing a lot
* api coverage is very low
* requesting fields: nothing yet, important thing to do
