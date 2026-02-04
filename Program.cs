
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using API_PEDIDOS.ModelsBD2Prueba;
using Microsoft.EntityFrameworkCore;
using Quartz;
using API_PEDIDOS.Jobs;
using static Quartz.Logging.OperationName;
using API_PEDIDOS.funciones;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionStringBD2 = builder.Configuration.GetConnectionString("DB2");
var connectionStringBD2P = builder.Configuration.GetConnectionString("DB2P");

builder.Services.AddDbContext<DBPContext>(options => options.UseSqlServer(connectionString))
    .AddDbContext<BD2Context>(options => options.UseSqlServer(connectionStringBD2))
    .AddDbContext<BD2ContextPrueba>(options => options.UseSqlServer(connectionStringBD2P));

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddScoped<FuncionesPedidos>();

// Configurar Quartz
builder.Services.AddQuartz(q =>
{
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmailJob");
    q.AddJob<JobEmail>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailJob-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)  
        // 0 0 9 ? * MON *
        .WithCronSchedule("0 0 8 ? * MON *")
    );
});

builder.Services.AddQuartz(q =>
{
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmailMesJob");
    q.AddJob<JobEmailMes>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailMesJob-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)  
        .WithCronSchedule("0 10 8 1 * ?")
    );
});


//builder.Services.AddQuartz(q =>
//{
//    // Just use the name of your job that you created in the Jobs folder.
//    var jobKey = new JobKey("SendEmailJobMermas");
//    q.AddJob<JobEmailMermasAla>(opts => opts.WithIdentity(jobKey));

//    q.AddTrigger(opts => opts
//        .ForJob(jobKey)
//        .WithIdentity("SendEmailJobMermas-trigger")
//        //This Cron interval can be described as "run every minute" (when second is zero)  
//        .WithCronSchedule("0 45 8 * * ?")
//    );
//});

//builder.Services.AddQuartz(q =>
//{
//    // Just use the name of your job that you created in the Jobs folder.
//    var jobKey = new JobKey("SendEmailJobMermasB");
//    q.AddJob<JobEmailMermasBoneless>(opts => opts.WithIdentity(jobKey));

//    q.AddTrigger(opts => opts
//        .ForJob(jobKey)
//        .WithIdentity("SendEmailJobMermasB-trigger")
//        //This Cron interval can be described as "run every minute" (when second is zero)  
//        .WithCronSchedule("0 50 8 * * ?")
//    );
//});

//builder.Services.AddQuartz(q =>
//{
//    // Just use the name of your job that you created in the Jobs folder.
//    var jobKey = new JobKey("SendEmail25pts");
//    q.AddJob<JobEmail25pts>(opts => opts.WithIdentity(jobKey));

//    q.AddTrigger(opts => opts
//        .ForJob(jobKey)
//        .WithIdentity("SendEmail25pts-trigger")
//        // This Cron interval can be described as "run every minute"(when second is zero)
//        .WithCronSchedule("0 55 8 * * ?")
//    );
//});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseCors();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    app.UseSwagger().UseDeveloperExceptionPage();
#if DEBUG
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API PLANEACION v1");
#else
    c.SwaggerEndpoint("/back/api_planeacion/swagger/v1/swagger.json", "API_PEDIDOS v1");
#endif
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
