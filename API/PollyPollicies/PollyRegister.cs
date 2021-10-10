using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public static class PollyRegister
    {
        public static readonly PolicyRegistry registry = new();

        public static void ConfigurePollyPolicies(this IServiceCollection services, Dictionary<string, Policy> policies)
        {
            foreach (KeyValuePair<string, Policy> policy in policies)
                registry.Add(policy.Key, policy.Value);

            services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
        }
    }
}