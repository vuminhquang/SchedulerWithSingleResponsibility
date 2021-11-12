#SchedulerWithSingleResponsibility

##Test:
WeatherForecastController - Get:
	Send a job directly to the queue
	Using Hangfire for scheduler
	
##Background service:
Thanks to Semaphore, the background service can go to sleep each time a job is done.
When there are new task (the task queue enqueued), the background service will resume and continue to run.

The task then is running with the background service context. Hangfire should not run the task, only use to wake up the background service.