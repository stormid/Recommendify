using System;
using Machine.Specifications;
using RedisIntegration;

namespace Recommendify.Specs
{
    public class RedisSetup : IAssemblyContext
    {
        public static Connection RedisConnectionInfo;

        public void OnAssemblyStart()
        {
            RedisConnectionInfo = HostManager.RunInstance(new Random().Next(6000, 7000));
        }

        public void OnAssemblyComplete()
        {
        }
    }
}