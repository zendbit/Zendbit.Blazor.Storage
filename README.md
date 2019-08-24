# Zendbit.Blazor.Storage
Blazor server side extension for local web storage and session implementation using JSInterop

MIT License

```
Install-Package Zendbit.Blazor.Storage
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
@using System.Threading.Tasks

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
        var (isSuccess1, errMsg1) = await Storage.AddAsync("A", "a");

        // or you can just do
        // this will update value A to 1
        await Storage.AddAsync("A", 1);

        // get value async
        // isSuccess will true if operation success
        // value is dynamic value for string, int, uint, double, float, decimal. DateTime, bool
        // --> for session use Session.GetValueAsync
        var (isSuccess2, value2) = await Storage.GetValueAsync("A");
        Console.WriteLine(value2);

        // will remove the value with key as parameter
        // isSuccess will return true if success removed
        // value is return value of removed item
        // --> for session use Session.RemoveAsync
        var (isSuccess3, value3) = await Storage.RemoveAsync("A");

        // check if storage contain key
        // --> for session use Session.ContainAsync
        var isContainKey = await Storage.ContainAsync("A");
        Console.WriteLine(isContainKey);

        // clear all storage data
        // --> for session use Session.ClearStorageAsync
        await Storage.ClearStorageAsync();

        // =====================
        // GENERIC STORAGE BASED
        // new feature using generic
        // NOTE: WHEN STORE USING GENERIC MUST BE RETRIEVE USING GENERIC
        // ALSO THE RemoveAsync<T> or Remove<T> should use generic
        var (isSuccess4, errMsg4) = await Storage.AddAsync<string>("A", "a");
        var (isSuccess5, value5) = await Storage.GetValueAsync<string>("A");
        await Storage.RemoveAsync<string>("A");
        Console.WriteLine(value5);

        var (isSuccess6, errMsg6) = await Storage.AddAsync<int>("A", 1);
        var (isSuccess7, value7) = await Storage.GetValueAsync<int>("A");
        await Storage.RemoveAsync<int>("A");
        Console.WriteLine(value7);

        var (isSuccess8, errMsg8) = await Storage.AddAsync<DateTime>("A", DateTime.UtcNow);
        var (isSuccess9, value9) = await Storage.GetValueAsync<DateTime>("A");
        await Storage.RemoveAsync<DateTime>("A");
        Console.WriteLine(value9);

        // you now can using object model and will be serialize as json string
        var (isSuccess16, errMsg16) = await Storage.AddAsync<Dictionary<string, string>>(
            "A",
            new Dictionary<string, string>{
                { "test", "test"}
            }
        );

        var (isSuccess17, value17) = await Storage.GetValueAsync<Dictionary<string, string>>("A");
        await Storage.RemoveAsync<Dictionary<string, string>>("A");
        value17.TryGetValue("test", out string val1);
        Console.WriteLine(val1);

        var (isSuccess18, errMsg18) = await Storage.AddAsync<List<Dictionary<string, string>>>(
            "A",
            new List<Dictionary<string, string>>{
                new Dictionary<string, string>{
                    { "test", "test"}
                }
            }
        );

        var (isSuccess19, value19) = await Storage.GetValueAsync<List<Dictionary<string, string>>>("A");
        await Storage.RemoveAsync<List<Dictionary<string, string>>>("A");
        foreach(var l in value19)
        {
            l.TryGetValue("test", out string val2);
            Console.WriteLine(val2);
        }

        // or if you only want to remove and skip the result just do
        //Storage.JustRemove("A");
        await Storage.JustRemoveAsync("A");
    }
}

```
