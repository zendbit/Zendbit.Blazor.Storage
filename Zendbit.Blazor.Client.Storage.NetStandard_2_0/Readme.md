# Zendbit.Blazor.Storage
Blazor server side extension for local web storage and session implementation using JSInterop

MIT License

```
Install-Package Zendbit.Blazor.Client.Storage.NetStandard_2_0
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
@inject ILocalStorage Storage
@inject ILocalSession Session

<h1>Hello, world!</h1>

Welcome to your new app.

<p>Current count: @TestStorage</p>

@foreach(var item in ListTest){
    item.TryGetValue("test", out string val2);
    <p>This is from serialized List of Dictionary: @val2</p>
}

<SurveyPrompt Title="How is Blazor working for you?" />

@code
{
    private int TestStorage { get; set; }
    private List<Dictionary<string, string>> ListTest { get; set; }

    protected override void OnInitialized()
    {
        // supported value type is
        // string, int, uint, double, float, decimal. DateTime, bool
        // add value async
        // isSuccess will true if success
        // errMsg is empty string if success
        // --> for session use Session.Add
        var (isSuccess1, errMsg1) = Storage.Add("A", "a");

        // or you can just do
        // this will update value A to 1
        Storage.Add("A", 1);

        // get value async
        // isSuccess will true if operation success
        // value is dynamic value for string, int, uint, double, float, decimal. DateTime, bool
        // --> for session use Session.GetValue
        var (isSuccess2, value2) = Storage.GetValue("A");

        // update value of TestStorage
        TestStorage = value2;

        // will remove the value with key as parameter
        // isSuccess will return true if success removed
        // value is return value of removed item
        // --> for session use Session.RemoveAsync
        var (isSuccess3, value3) = Storage.Remove("A");

        // check if storage contain key
        // --> for session use Session.Contain
        var isContainKey = Storage.Contain("A");

        // clear all storage data
        // --> for session use Session.ClearStorage
        Storage.ClearStorage();

        // =====================
        // GENERIC STORAGE BASED
        // new feature using generic
        // NOTE: WHEN STORE USING GENERIC MUST BE RETRIEVE USING GENERIC
        // ALSO THE Remove<T> or Remove<T> should use generic
        var (isSuccess4, errMsg4) = Storage.Add<string>("A", "a");
        var (isSuccess5, value5) = Storage.GetValue<string>("A");
        Storage.Remove<string>("A");

        var (isSuccess6, errMsg6) = Storage.Add<int>("A", 1);
        var (isSuccess7, value7) = Storage.GetValue<int>("A");
        Storage.Remove<int>("A");

        var (isSuccess8, errMsg8) = Storage.Add<DateTime>("A", DateTime.UtcNow);
        var (isSuccess9, value9) = Storage.GetValue<DateTime>("A");
        Storage.Remove<DateTime>("A");

        // you now can using object model and will be serialize as json string
        var (isSuccess16, errMsg16) = Storage.Add<Dictionary<string, string>>(
            "A",
            new Dictionary<string, string>{
                { "test", "test"}
            }
        );

        var (isSuccess17, value17) = Storage.GetValue<Dictionary<string, string>>("A");
        Storage.Remove<Dictionary<string, string>>("A");
        value17.TryGetValue("test", out string val1);

        var (isSuccess18, errMsg18) = Storage.Add<List<Dictionary<string, string>>>(
            "A",
            new List<Dictionary<string, string>>{
                new Dictionary<string, string>{
                    { "test", "test"}
                }
            }
        );

        var (isSuccess19, value19) = Storage.GetValue<List<Dictionary<string, string>>>("A");
        Storage.Remove<List<Dictionary<string, string>>>("A");

        // update value of ListTest
        ListTest = value19;

        foreach(var l in value19)
        {
            l.TryGetValue("test", out string val2);
        }

        // or if you only want to remove and skip the result just do
        //Storage.JustRemove("A");
        Storage.JustRemove("A");
    }
}
```
