JSON vs. XML
====================

The LinkedIn API replies with XML or JSON but is originally built for XML. Structures have both children and attributes: this does not map easily to JSON.

https://developer.linkedin.com/documents/api-requests-json

    x-li-format: json

Current state
-------------------

This lib now uses JSON.


Previous state
-------------------

This lib was using XML because:

* `XmlSerializer` is in the framework since 1.0 and ia reliable
* `XmlSerializer` does not require external references

I knew JSON is lightweight but there are issues:

* Using Json.NET will add an external reference which may be incompatible with some projects
* `DataContractJsonSerializer` is slower than `XmlSerializer`  
* `DataContractJsonSerializer` is not available in 3.5

Right now XML works quite well. I may later add a provider pattern to allow a different serializer to be used. Maybe create a Sparkle.LinkedInNET.JsonNet assembly with a dependency to Json.NET... 

