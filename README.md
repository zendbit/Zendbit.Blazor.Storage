# Zendbit.Blazor.Storage
Blazor extension for local web storage and session implementation using JSInterop

MIT License

```
Install-Package Zendbit.Blazor.Storage -Version 1.0.2
```

add to Startup.cs

```
using Zendbit.Blazor.Storage

// then register to service
services.AddStorage(
    (localStorage) =>
    {
        // your secret key for local storage and session encryption
        localStorage.EncryptionKey = "s3cr3t";
    }
);
```

add to _ViewImports

```
@using Zendbit.Blazor.Storage
```

Usage example

```
@page "/"
@inject IComponentContext ComponentContext
@inject ILocalStorage Storage
@inject ILocalSession Session

<h1>Hello, world!</h1>

Welcome to your new app.

@code
{
    protected override async Task OnAfterRenderAsync()
    {
        if (!ComponentContext.IsConnected) return;

        // supported value type is
        // string, int, uint, double, float, decimal. DateTime, bool
        // add value async
        // isSuccess will true if success
        // errMsg is empty string if success
        // --> for session use Session.AddAsync
        // --> for synchronous call use Storage.Add(key, val)
        var (isSuccess1, errMsg1) = await Storage.AddAsync("A", "a");

        // or you can just do
        // this will update value A to 1
        await Storage.AddAsync("A", 1);

        // get value async
        // isSuccess will true if operation success
        // value is dynamic value for string, int, uint, double, float, decimal. DateTime, bool
        // --> for session use Session.GetValueAsync
        // --> for synchronous call use Storage.GetValue(key)
        var (isSuccess2, value2) = await Storage.GetValueAsync("A");

        // will remove the value with key as parameter
        // isSuccess will return true if success removed
        // value is return value of removed item
        // --> for session use Session.RemoveAsync
        // -->for synchronous call use Storage.Remove(key)
        var (isSuccess3, value3) = await Storage.RemoveAsync("A");

        // check if storage contain key
        // --> for session use Session.ContainAsync
        // --> for synchronous call use Storage.Contain(key)
        var isContainKey = await Storage.ContainAsync("A");

        // clear all storage data
        // --> for session use Session.ClearStorageAsync
        // --> for synchronous call use Storage.ClearStorage()
        await Storage.ClearStorageAsync();
    }
}

```
