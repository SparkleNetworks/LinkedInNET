
API definition
---------------------------------

- [x] APIs are grouped as `<ApiGroup />`  
- [x] Extract UrlPathParameters from `<ApiMethod Path="/v1/people/id={MemberToken}" />`  
- [x] Declare `<ReturnType />` and `<ApiMethod ReturnType="Person" />`
- [ ] Add SecureUrls in configuration and use `secure-urls=true`  
- [x] People  
- [ ] Companies  
- [ ] Groups  
- [ ] Jobs  
- [ ] Everything else  

Code generation
---------------------------------

- [x] Setup .tt file and generation class  
- [x] Generate API groups  
- [x] Generate API group  
- [x] Generate basic return types  
- [x] Generate basic methods  
- [x] Support 1 level of field selectors `/v1/people/id=12345:(first-name,last-name)`  
- [x] Support all levels of field selectors   
- [x] Collections of ResultType  
- [x] Fix generation of sub-fields (ex: location:(name))  
- [ ] Support entity selectors `/v1/people::(~,id=123456,id=456789):(first-name)` ([ref](https://developer.linkedin.com/documents/field-selectors))  

API other items
---------------------------------

- [x] Profile: Some members have profiles in multiple languages. To specify the language you prefer, pass an Accept-Language HTTP header.  
- [ ] Pagination: start=0, count=500  
- [ ] `/v1/companies?is-company-admin=true`  
- [ ]  

For developers
---------------------------------

- [x] Success HTTP codes: 200, 201  
- [x] Error HTTP codes: 400, 401, 403, 404, 500  
- [ ] Async pattern by callbacks for Silverlight and Windows Phone 7ish  

Extra / useless / decorative
---------------------------------

- [ ] async/await pattern  



- [ ]   
