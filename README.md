# Undefinable\<T>
NuGet package for deserializing JSON's `undefined` correctly in .NET.

## Overview

Sometimes, you need to be able to represent both `undefined` and `null` inside of your C# classes - this library allows you to do exactly that.

My main use case is in ASP.NET Core APIs where I may want to only update properties sent in the JSON body. Let's say you have an `UpdateCustomerRequest` that is used to update a `Customer` entity but you want the option of leaving out certain properties in the request: 

```csharp
public class UpdateCustomerRequest
{
    public Undefinable<string> Name { get; set; }
    public Undefinable<string> InternalName { get; set; }
    public Undefinable<int?> NetTermsInDays { get; set; }
}
```

And then you make your request where you want to change the customer's `Name` and "null out" the `InternalName` but leave `NetTermsInDays` alone:

```http
PUT api/customers/1 HTTP/1.1
Host: localhost
Content-Type: application/json

{
    "Name": "new name",
    "InternalName": null
}
```

You can deserialize the value like normal (requires Newtonsoft.Json while we're all still using it):

```csharp
var customer = DbContext.Customers.Find(1);
var request = JsonConvert.DeserializeObject<UpdateCustomerRequest>(HttpBody);

if (request.Name.IsDefined)             //yes
{
    customer.Name = request.Name;
}
if (request.InternalName.IsDefined)     //yes
{
    customer.InternalName = request.InternalName.Value;
}
if (request.NetTermsInDays.IsDefined)   //no!
{
    customer.NetTermsInDays = request.NetTermsInDays.Value;
}
```

## Why not just use `null` to represent `undefined`?
The problems with representing `undefined` with `null` in C# is very obvious the moment you need to distinguish between the two, like in the example above.

In our `UpdateCustomerRequest`, we want to ONLY update the field if it's part of the JSON body:

```json
{
    "Name": "new name",
    "InternalName": null
}
```

That includes the ability to "null out" the `InternalName` if we want to, or just exclude it from the request entirely if we don't want to overwrite the value at all.

Specifically for me, this library was made to address exactly the use case above.

## TODO

* AutoMapper support (or just library support in general)
* Better tests

## Credit where it's due

This is based on the work done by Alberto Chiesa in his SettableJsonProperties repo here: https://github.com/alberto-chiesa/SettableJsonProperties

The main difference is that I wanted to be able to use it with reference types, so I took his code and adapted it for that use case. Plus, I liked my naming better.

## License

MIT
