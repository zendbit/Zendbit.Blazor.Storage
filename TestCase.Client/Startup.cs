using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Zendbit.Blazor.Client.Storage;

namespace TestCase.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStorage(
                (localStorage) =>
                {
                    // your secret key for local storage and session encryption
                    localStorage.EncryptionKey = "s3cr3t";
                }
            );
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
