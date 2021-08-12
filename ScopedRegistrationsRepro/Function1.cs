using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ScopedRegistrationsRepro
{
    public class Function1
    {
        private readonly IValidationService validationService;
        private readonly IMessageService messageService;
        private readonly ILogger log;

        public Function1(IMessageService messageService, IValidationService validationService, ILogger<Function1> logger)
        {
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            this.validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            this.log = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "account/read")] HttpRequest req)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await Task.Delay(100);

            var request = req.Headers.GetCommaSeparatedValues(UserIdentityAccessor.Header);

            var validation = await validationService.Validate();
            
            var message1 = await messageService.Header();
            var message2 = await messageService.Hello();
            

            return new OkObjectResult($"validation: {validation} \r\n message1: {message1} \r\n message2: {message2} \r\n request header: { string.Join(",", request)}");
        }
    }
}
