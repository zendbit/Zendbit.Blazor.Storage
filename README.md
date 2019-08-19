# Zendbit.Blazor.Storage
Blazor extension for local web storage and session implementation using JSInterop

MIT License

```
Install-Package Zendbit.Blazor.Storage -Version 1.0.2
```

add to _ViewImports

```
@using Zendbit.Blazor.Storage
```

Example using async

```
@page "/"
@inject IJSRuntime JsRuntime
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

        //System.Console.WriteLine(Storage);
        await Storage.AddAsync("Test", "OK");
        await Storage.AddAsync("Helo", 1);
        System.Console.WriteLine(
            await Storage.GetValueAsync("Test")
        );
        await Storage.RemoveAsync("Test");
        System.Console.WriteLine(
            await Storage.GetValueAsync("Test")
        );
        await Session.AddAsync("Test", DateTime.UtcNow);
        System.Console.WriteLine(
            await Session.GetValueAsync("Test")
        );
        System.Console.WriteLine(
            await Session.ContainAsync("Test")
        );
    }
}
```

example using sync

```
@page "/"
@inject IJSRuntime JsRuntime
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

        //System.Console.WriteLine(Storage);
        Storage.Add("Test", "OK");
        Storage.Add("Helo", 1);
        System.Console.WriteLine(
            Storage.GetValue("Test")
        );
        Storage.RemoveAsync("Test");
        System.Console.WriteLine(
            Storage.GetValue("Test")
        );
        await Session.Add("Test", DateTime.UtcNow);
        System.Console.WriteLine(
            Session.GetValue("Test")
        );
        System.Console.WriteLine(
            Session.Contain("Test")
        );
    }
}
```
