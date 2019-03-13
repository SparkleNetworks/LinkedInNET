
![](https://raw.githubusercontent.com/SparkleNetworks/LinkedInNET/master/src/LiNET-200.png)

LinkedInNET
===========

[Sparkle.LinkedInNET](https://github.com/SparkleNetworks/LinkedInNET/) ([NuGet](https://www.nuget.org/packages/Sparkle.LinkedInNET/)) will help you query the LinkedIn API :)

Have any question? You may reach the authors on the dedicated chat room: [![Join the chat at https://gitter.im/SparkleNetworks/LinkedInNET](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/SparkleNetworks/LinkedInNET?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Before you start - Forks of this project (2018)
----------------------------------------------------------------

This project will **not support the V2 API** for the reason described below.

The following forks may be of interest:

- [247GradLabs/LinkedInNET](https://github.com/247GradLabs/LinkedInNET/) is making support for V2 (no nuget)
- [gianni-rg/LinkedInNET](https://github.com/gianni-rg/LinkedInNET/tree/netstandard2) is working on netstandard2.0 support (no nuget)

Before you start - About the new LinkedIn's V2 API (2018)
----------------------------------------------------------------

As discussed in [issue 23](https://github.com/SparkleNetworks/LinkedInNET/issues/23), **this project might not be able to implement the new API specifications**. Too much stuff needs to change. 

Please [Join the chat](https://gitter.im/SparkleNetworks/LinkedInNET) to organize a new project or discuss the future of this project in the [issue 23](https://github.com/SparkleNetworks/LinkedInNET/issues/23).

It has been a cool adventure to make this library. Is it not the time to have a new one? I will continue to support the V1 API so that those who will create the V2 code/SDK won't have to support V1 *and* V2.


Before you start - About LinkedIn API recent changes (2015)
----------------------------------------------------------------

**[LinkedIn recently changed a lot of things in its developer program](https://developer.linkedin.com/blog/posts/2015/developer-program-changes). When using this API, your applications might break on May 12, 2015**. 

Many documented URLs in this project are broken because LinkedIn changed the documentation pages. Here is the [old documentation via the WaybackMachine](https://web.archive.org/web/20140719025807/http://developer.linkedin.com/documents/people).

> Starting on May 12, 2015, we will be limiting the open APIs to only support the following uses:
>
> - Allowing members to represent their professional identity via their LinkedIn profile using our Profile API.
> - Enabling members to post certifications directly to their LinkedIn profile with our Add to Profile tools.
> - Enabling members to share professional content to their LinkedIn network from across the Web leveraging our Share API.
> - Enabling companies to share professional content to LinkedIn with our Company API.
>
> All other APIs will require developers to become a member of one of our partnership programs.
>
> For many developers, we understand that today’s changes may be disappointing and disruptive, but we believe these changes will provide further clarity and focus on which types of integrations will be supported by LinkedIn.
>
> -- [Changes to our Developer Program](https://developer.linkedin.com/blog/posts/2015/developer-program-changes), February 12, 2015

See also [Transition FAQ](https://developer.linkedin.com/blog/posts/2015/transition-faq), [D-Day's changes](https://developer.linkedin.com/blog/posts/2015/todays-changes).


Before you start - Notice
----------------------------------------------------------------

By using the LinkedIn APIs you agree to the [LinkedIn APIs Terms of Use](https://developer.linkedin.com/documents/linkedin-apis-terms-use).  
This project is released under the LGPL v3 license.  
This is NOT an official client library.

Motivation (2015-2016)
------------

Bring the .NET world a nice LinkedIn client library.

Usage
------------

**If you still think this project may help you**, here is how to start.

### 1. Installation

[Via NuGet](https://www.nuget.org/packages/Sparkle.LinkedInNET/)

````powershell
PM> Install-Package Sparkle.LinkedInNET
````

Or build the sources... You have to create your own .snk file.

Supported frameworks: 3.5 (sync), 4.0 (sync), 4.5 (sync and task async).

### 2. Create API client with configuration

The `LinkedInApi` class is the entry point for all API calls. You must instantiate it with a configuration object. The minimum configuration is the API key and secret.  [Get a LinkedIn API key](https://www.linkedin.com/secure/developer).

````csharp
// create a configuration object
var config = new LinkedInApiConfiguration("•api•key•••", "•api•secret•key••••••");

// get the APIs client
var api = new LinkedInApi(config);
````

### 3. Create OAuth2 authorize url

The OAuth2 authentication process is fully supported. The `GetAuthorizationUrl` method will generate the OAuth2 url to navigate the user to.

````csharp
var scope = AuthorizationScope.ReadBasicProfile | AuthorizationScope.ReadEmailAddress;
var state = Guid.NewGuid().ToString();
var redirectUrl = "http://mywebsite/LinkedIn/OAuth2";
var url = api.OAuth2.GetAuthorizationUrl(scope, state, redirectUrl);
// https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=...
// now redirect your user there
````

### 4. Get access token

When the user is redirected back to your website, you can get an access code.

````csharp
// http://mywebsite/LinkedIn/OAuth2?code=...&state=...
public async Task<ActionResult> OAuth2(string code, string state, string error, string error_description)
{
    if (!string.IsNullOrEmpty(error) || !string.IsNullOrEmpty(error_description))
    {
        // handle error and error_description
    }
    else
    {
        var redirectUrl = "http://mywebsite/LinkedIn/OAuth2";
        var userToken = await api.OAuth2.GetAccessTokenAsync(code, redirectUrl);
        // keep this token for your API requests
    }

    // ...
}
````

You will find in the source codes a nicer way to build the redirect url.

````csharp
var redirectUrl = this.Request.Compose() + this.Url.Action("OAuth2");
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

The `.WithAllFields()` method will generate the list of all available fields. It is not recommended to do that.

````csharp
var profile = api.Profiles.GetMyProfile(
    user,
    FieldSelector.For<Person>().WithAllFields());
// https://api.linkedin.com/v1/people/~:(all available fields here)
// however it is not recommended to specify all fields
````

You can create your own extension methods when you desire many fields. Check the source code to see how it works.

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

Code documentation is quite present. Use the auto-completion to discover stuff.

The MVC demo app has a /Explore page that demonstrates most API calls. Have a look at it.

### 9. Raw queries

A method is missing in the library? You may contribute to create it... Or you may use the raw query methods `RawGetJsonQuery`, `RawPostJsonQuery` or `RawQuery`.

This example shows how to fetch a company share.

````csharp
string shareAsJson = this.api.RawGetJsonQuery("/v1/companies/" + CompanyId + "/updates/key=" + Uri.EscapeDataString(ShareId) + "?format=json", user);
````

More details [here](https://github.com/SparkleNetworks/LinkedInNET/blob/dffaea840eac044654c7cee59df50a8db6f101a3/src/NET45.Sparkle.LinkedInNET.DemoMvc5/Controllers/ExploreController.cs#L363).

Contribute
------------

We welcome contributions. Especially if you can **suggest/access/test methods with a partner API key**.

We are generating code based on a [XML file](DefinitionFile.md).  
This XML file is manually filled to represent the API.  
The API coverage should be implemented by modifying the XML file and enhancing code generation.

To generate the API code, build the "ServiceDefinition" project in Debug mode, edit `LinkedInApi.xml`, then use "Run custom tool" on the `Service.tt` file. The XML file will be read and most of the code will be updated automagically. 
  
To alter code generation, search for `CSharpGenerator.cs`. Different methods are responsible of generating different parts of C# code (return types, api groups, selectors).
  
To add/alter API methods and return types, search for `LinkedInApi.xml`. This file [describes the API in a human-readable and machine-readable way](DefinitionFile.md). Don't forget to re-generate the code (Service.tt).


References
------------

https://developer.linkedin.com/apis  
https://developer.linkedin.com/documents/authentication  


.NET version
------------

Supported .NET Framework versions:

* .NET 4.5 (dependencies: Newtonsoft.Json ≥ 6.0.8, Microsoft.Net.Http ≥ 2.2.29)
* .NET 4.0 (dependencies: Newtonsoft.Json ≥ 6.0.8)
* .NET 3.5 (dependencies: Newtonsoft.Json ≥ 6.0.8)

We are using a lot of code generation so it won't be difficult to target any other framework. 


Status
------------

Because of the API policy changes, most API calls are now reserved to the partners LinkedIn chose. The core team will try to keep up using basic API key.

Check our [internal to-do list](src/ToDo.md) to track missing/done things.
