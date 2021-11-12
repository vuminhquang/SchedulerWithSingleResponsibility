using System;
using System.Threading;
using System.Threading.Tasks;
using AddinEngine.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication
{
    public class AnyBackgroundService: BackgroundService2
    {
        private static AnyBackgroundService _instance = null;
        public static AnyBackgroundService Instance
        {
            get => _instance;
            set => _instance ??= value;//only set once if _instance = null
        }
        
        private readonly AnyTaskExecutor _taskExecutor;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        private readonly ObservableConcurrentQueue<string> _tasksQueue;
        
        public AnyBackgroundService(IServiceProvider services, IConfiguration configuration)
        {
            Instance = this;
            
            _taskExecutor = new AnyTaskExecutor(services, configuration/*,Convert.ToInt32(configuration["abc:MaxThreads"]*/);
            _services = services;
            _configuration = configuration;
            _tasksQueue = services.GetRequiredService<ObservableConcurrentQueue<string>>();
            _tasksQueue.ContentChanged += TasksQueueOnContentChanged;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingCts.Token.Register(() => _taskExecutor.Cancel());
            return _taskExecutor.Run(stoppingToken);
        }
        
        private void TasksQueueOnContentChanged(object sender, ObservableConcurrentQueue<string>.NotifyConcurrentQueueChangedEventArgs<string> args)
        {
            if (!_tasksQueue.TryDequeue(out var taskDesc)) return;

            _taskExecutor.JobQueue.Enqueue(taskDesc);
        }
        
        public static class RegisterWrapper
        {
            public static void RegisterToRun(string job)
            {
                AnyBackgroundService.Instance.RegisterToRun(job);
            }
        }
        
        public void RegisterToRun(string job)
        {
            _tasksQueue.Enqueue(job);
        }
    }
}