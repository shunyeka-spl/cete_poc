using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Amazon.Lambda.Core;
using HeadlessChromium.Puppeteer.Lambda.Dotnet;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace cete_poc;

public class Function
{
    public async Task<string> FunctionHandler(InputObject input, ILambdaContext context)
    {
        try
        {
            var browserLauncher = new HeadlessChromiumPuppeteerLauncher(new LoggerFactory());

            using var browser = await browserLauncher.LaunchAsync();
            using var page = await browser.NewPageAsync();
            await page.GoToAsync(input?.url ?? "https://en.wikipedia.org");
            
            Console.WriteLine(input?.url);
            
            var pdf = await page.PdfStreamAsync();
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}-wiki.pdf";
            
            Console.WriteLine(fileName);
            
            var filePath = Path.Combine("/tmp", fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await pdf.CopyToAsync(fileStream);
            
            byte[] pdfBytes = File.ReadAllBytes(filePath);
            string base64Pdf = Convert.ToBase64String(pdfBytes);
            
            Console.WriteLine(base64Pdf);

            var response = new
            {
                success = true,
                message = "PDF read successful",
                pdfBase64 = base64Pdf
            };
            string jsonResponse = JsonSerializer.Serialize(response);
            
            // Sleep for 1 minute
            await Task.Delay(TimeSpan.FromMinutes(1));

            return jsonResponse;
            Console.WriteLine(jsonResponse);
        }
        catch (Exception e)
        {
            var response = new
            {
                success = false,
                message = "Error occurred while reading PDF file",
                error = e.Message
            };

            // Serialize the JSON object to a string
            string jsonResponse = JsonSerializer.Serialize(response);
            return jsonResponse;
        }
    }
    public class InputObject
    {
        public string url { get; set; }
    }
}

