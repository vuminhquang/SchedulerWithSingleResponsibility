using System;
using System.Threading;
using System.Threading.Tasks;
using AddinEngine.Abstract;
using Microsoft.Extensions.Configuration;

namespace WebApplication
{
    public class AnyTaskExecutor:MultiThreadsTaskBase<string>
    {
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        
        public AnyTaskExecutor(IServiceProvider services, IConfiguration configuration, int maxThreads = 1) : base(maxThreads)
        {
            _services = services;
            _configuration = configuration;
        }
        
        protected override async Task ExecuteTask(string job, int mainTaskNum, CancellationToken stoppingToken)
        {
            Console.WriteLine($"Task {Task.CurrentId} with job {job} executed");
        }
    }
}