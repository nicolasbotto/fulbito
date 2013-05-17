using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MvcWebRole.Services
{
    public class JsonpFormatter : JsonMediaTypeFormatter
    {
        private JsonSerializerSettings settings;
        public JsonpFormatter(JsonSerializerSettings settings)
        {
            this.settings = settings;

            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            JsonpParameterName = "callback";
        }

        /// <summary>
        ///  Name of the query string parameter to look for
        ///  the jsonp function name
        /// </summary>
        public string JsonpParameterName { get; set; }

        /// <summary>
        /// Captured name of the Jsonp function that the JSON call
        /// is wrapped in. Set in GetPerRequestFormatter Instance
        /// </summary>
        private string JsonpCallbackFunction;


        public override bool CanWriteType(Type type)
        {
            return true;
        }

        /// <summary>
        /// Override this method to capture the Request object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="request"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, System.Net.Http.HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var formatter = new JsonpFormatter(jsonSerializerSettings)
            {
                JsonpCallbackFunction = GetJsonCallbackFunction(request)
            };

            // You have to reapply any JSON.NET default serializer Customizations here    
            formatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            formatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

            return formatter;
        }


        public override Task WriteToStreamAsync(Type type, object value,
                                        Stream stream,
                                        HttpContent content,
                                        TransportContext transportContext)
        {
            // Create a serializer
            JsonSerializer serializer = JsonSerializer.Create(settings);

            if (string.IsNullOrEmpty(JsonpCallbackFunction))
            {
                //   return base.WriteToStreamAsync(type, value, stream, content, transportContext);
                return Task.Factory.StartNew(() =>
                {
                    using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8)) { CloseOutput = false })
                    {
                        serializer.Serialize(jsonTextWriter, value);
                        jsonTextWriter.Flush();
                    }
                });
            }

          

            //StreamWriter writer = null;

            //// write the pre-amble
            //try
            //{
            //    writer = new StreamWriter(stream);
            //    writer.Write(JsonpCallbackFunction + "(");
            //    writer.Flush();
            //}
            //catch (Exception ex)
            //{
            //    try
            //    {
            //        if (writer != null)
            //            writer.Dispose();
            //    }
            //    catch { }

            //    var tcs = new TaskCompletionSource<object>();
            //    tcs.SetException(ex);
            //    return tcs.Task;
            //}

    
           // Create task writing the serialized content
           return Task.Factory.StartNew(() =>
           {
               using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8)) { CloseOutput = false })
               {
                   jsonTextWriter.WriteRawValue(JsonpCallbackFunction + "(");
                   serializer.Serialize(jsonTextWriter, value);
                   jsonTextWriter.WriteRawValue(")");
                   jsonTextWriter.Flush();
               }
           });


            //return base.WriteToStreamAsync(type, value, stream, content, transportContext)
            //           .ContinueWith(innerTask =>
            //           {
            //               if (innerTask.Status == TaskStatus.RanToCompletion)
            //               {
            //                   writer.Write(")");
            //                   writer.Flush();
            //               }

            //           }, TaskContinuationOptions.ExecuteSynchronously)
            //            .ContinueWith(innerTask =>
            //            {
            //                writer.Dispose();
            //                return innerTask;

            //            }, TaskContinuationOptions.ExecuteSynchronously)
            //            .Unwrap();
        }

        /// <summary>
        /// Retrieves the Jsonp Callback function
        /// from the query string
        /// </summary>
        /// <returns></returns>
        private string GetJsonCallbackFunction(HttpRequestMessage request)
        {
            if (request.Method != HttpMethod.Get)
                return null;

            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var queryVal = query[this.JsonpParameterName];

            if (string.IsNullOrEmpty(queryVal))
                return null;

            return queryVal;
        }
    }
}