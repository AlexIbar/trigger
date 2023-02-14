using Quartz;

namespace pruebaCronTrigger.ClasePrueba
{
    public class HelloJob:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("En ejecución");
        }    
    }
}
