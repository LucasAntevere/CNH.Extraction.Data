using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Computer.Vision.Sample
{
    class Program
    {
        static void Main()
        {
            var result = MakeAnalysisRequest(@"C:\Users\lucas\Desktop\hilario.JPG");




            foreach (var region in result.regions)
            {
                foreach (var line in region.lines)
                {
                    var dates = line.Match(new Regex(@"([0-9]{2}[\/]{1}[0-9]{2}[\/]{1}[0-9]{4})"));
                    var cpf = line.Match(new Regex(@"(?<CPF>[0-9]{3}\s*[\.]{1}[0-9]{3}[\.]{1}[0-9]{3}[\-]{1}[0-9]{2})"));

                    foreach (var date in dates)
                        Console.WriteLine(date);

                    foreach (var c in cpf)
                        Console.WriteLine(c);
                }
            }










            var group = new Group();

            foreach (var region in result.regions)
            {
                var groupRegion = new Group();
                groupRegion.BoundingBox = region.boundingBox;

                foreach (var line in region.lines)
                {
                    Console.WriteLine(line.line);

                    var groupLine = new Group();
                    groupLine.IsLine = true;
                    groupLine.BoundingBox = region.boundingBox;

                    foreach (var word in line.words)
                    {
                        var groupWord = new Group();
                        groupWord.Text = word.text;
                        groupWord.BoundingBox = word.boundingBox;

                        groupLine.Groups.Add(groupWord);
                    }

                    groupRegion.Groups.Add(groupLine);
                }

                group.Groups.Add(groupRegion);
            }



            Show(group);

            Console.ReadKey();
        }

        private static void Show(Group g)
        {
            foreach (var item in g.Groups)
            {
                Console.WriteLine(item.Line);
                Show(item);
            }
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static OCRResult MakeAnalysisRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "de5f234535a04c3894c591269d40d66c");

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "detectOrientation=true&language=pt";
            string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + requestParameters;
            Console.WriteLine(uri);

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = client.PostAsync(uri, content).Result;
                return JsonConvert.DeserializeObject<OCRResult>(response.Content.ReadAsStringAsync().Result);
            }

            return null;
        }
    }
}
