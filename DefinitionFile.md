XML definition file
===================

`<ApiGroup>`
------------

The API definition file helps us generate client API code. All elements are grouped within `<ApiGroup>`s. This helps organize ourselves.

A `<ApiGroup>` contains API calls and return type definitions.

A Name attribute is required `<ApiGroup Name="">` in order to build different classes to put API calls into as methods.

`<ApiMethod>`
-------------

A `ApiMethod` defines a possible call to the API and a method/function to generate.

Attribute `ApiMethod.MethodName`: Required. The name of the method to create.  
Attribute `ApiMethod.HttpMethod`: Required. GET/POST/PUT/...   
Attribute `ApiMethod.Path`: Required. The HTTP path.  
Attribute `ApiMethod.ReturnType`: The `ReturnType.Name` or `ReturnType.ClassName` of the `<ReturnType>` to use as a response format.  
Attribute `ApiMethod.PostReturnType`: Optional, default empty. The `ReturnType.Name` or `ReturnType.ClassName` of the `<ReturnType>` to use as a request format.  
Attribute `ApiMethod.RequiresUserAuthentication`: Optional, default 0. Indicates whether a user token must be specified for this call. 1/0.  
Attribute `ApiMethod.UsesAcceptLanguage`: Optional, default 0. Indicates whether the method supports specifying a culture name. 1/0.  
Attribute `ApiMethod.UseFieldSelectors`: Optional, default 1. Indicates whether field selectors are used for this call. 1/0.  
Attribute `ApiMethod.Title`: Optional, default empty. A short documentation.

The `ApiMethod.Path` attribute supports special and custom variable names.

    <ApiMethod Path="/v1/company-search{FieldSelector}?start={int start}&amp;count={int count}&amp;keywords={keywords}" />

The `{FieldSelector}` variable is a placeholder to insert field selectors (`:(first-name,last-name)`).  
The `{int start}` variable is a custom variable with integer type.  
The `{keywords}` variable is a custom variable with string type.

Here is a piece of generated code related to this XML element. 

    public Companies.CompanySearch Search(
          UserAuthorization user // RequiresUserAuthentication=1
        , int start              // {int start}
        , int count              // {int count}
        , string keywords        // {keywords}
        , FieldSelector<Companies.CompanySearch> fields = null // UseFieldSelectors=1
    )
    {
        const string urlFormat = "/v1/company-search{FieldSelector}?start={int start}&count={int count}&keywords={keywords}";
        var url = FormatUrl(urlFormat, fields, "int start", start, "int count", count, "keywords", keywords);

        var context = new RequestContext();
        context.UserAuthorization = user;
        context.Method =  "GET";
        context.UrlPath = this.LinkedInApi.Configuration.BaseApiUrl + url;

        if (!this.ExecuteQuery(context))
            this.HandleJsonErrorResponse(context);
        
        var result = this.HandleJsonResponse<Companies.CompanySearch>(context);
        return result;
    }


`<ReturnType>`
--------------

A `<ReturnType>` defines the structure of an expected HTTP response body. It contains `<Field>`s and `<Header>`s.

Attribute `ReturnType.Name`: Required. The unique name of the return type structure.  
Attribute `ReturnType.ClassName`: Optional, default empty. Override the generated class name.  
Attribute `ReturnType.AutoGenerateFieldSelectors`: Optional. Indicates whether to generate strongly-type field selectors from all defined fields.

`<Field>`
---------

A `<Field>` defines a value in a `<ReturnType>`. It may contain sub-`<Field>`, `<FieldSelector>`s.

Attribute `Field.Name`: Required. The unique name of the value.   
Attribute `Field.PropertyName`: Optional, default empty. Override the generated property name.  
Attribute `Field.Type`: Optional, default empty. Contains the name of a `<ReturnType>` or the name of a type in a programming language (int, DateTime...)
Attribute `Field.Title`: Optional. A short description.  
Attribute `Field.Remark`: Optionel. A long description.  
Attribute `Field.Ignore`: Optional. Default 0. Ignores this field.  
Attribute `Field.IsDefault`: Optional. Unused. Indicates this field is included when no field selector is specified.  
Attribute `Field.IsAttribute`: Optional. When deserializing XML, indicates the field is an attribute and not a child element. 

`<Header>`
-----------------

A `<Header>` defines a HTTP header to capture in a HTTP response when child of a `<ReturnType>`.

A `<Header>` defines a HTTP header to write in a HTTP request when child of a `<ApiMethod>` (not implemented yet).

It shares the same attributes as `<Field>`.

This element should not have been created. We should have used a `<Field IsHeader="1">` instead.


`<FieldSelector>`
-----------------

TODO




Basic example
-------------



    <Root>
      <ApiGroup Name="Profiles">
        <ApiMethod MethodName="GetMyProfile" ReturnType="Person" />
        <ReturnType Name="person">
          <Field Name="first-name" PropertyName="Firstname" Title="the member's first name" />
          <Field Name="last-name" PropertyName="Lastname" Title="the member's last name" />
        </ReturnType>
      </ApiGroup>
    </Root>



















