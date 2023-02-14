using pruebaCronTrigger.ClasePrueba;
using Quartz;

var builder = WebApplication.CreateBuilder(args);


var nums = Enumerable.Range(0, 100).ToArray();

await Parallel.ForEachAsync(nums, async (i, token) =>
{
    Console.WriteLine($"Starting iteration {i}");
    await Task.Delay(1000, token);
    Console.WriteLine($"Finishing iteration {i}");
});

// Add services to the container.
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});
builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var tiempo = builder.Configuration.GetSection("Tiempo").Value;

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var shedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var sheduler = await shedulerFactory.GetScheduler();
IJobDetail job = JobBuilder.Create<HelloJob>()
    .WithIdentity("myJob", "group1")
    .Build();

ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("myJob", "group1")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(20)
        .RepeatForever())
    .Build();
/*
ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("myTrigger1", "groupTrigger1")
    .ForJob("myJob", "group1")
    .WithCronSchedule(tiempo)
    .Build();*/
/*ITrigger trigger2 = TriggerBuilder.Create()
    .WithIdentity("myTrigger2", "groupTrigger2")
    .ForJob("myJob", "group1")
    .WithCronSchedule("\r\n0 6 8 ? * MON,TUE,WED,THU,FRI,SAT *")
    .Build();
ITrigger trigger3 = TriggerBuilder.Create()
    .WithIdentity("myTrigger3", "groupTrigger3")
    .ForJob("myJob", "group1")
    .WithCronSchedule("\r\n0 7 8 ? * MON,TUE,WED,THU,FRI,SAT *")
    .Build();*/
await sheduler.ScheduleJob(job, trigger);
//await sheduler.ScheduleJob(job, trigger2);
//await sheduler.ScheduleJob(job, trigger3);
app.Run();
