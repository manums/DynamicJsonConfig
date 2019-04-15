using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SampleGitHubCode
{
    public class JsonConfigManager
    {
        public static SampleType UpdateSampleType(
           SampleType document)
        {
            JToken jsonObject = JToken.FromObject(document,
                JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));

            RecurseConfiguration(jsonObject, prop =>
            {
                if (prop.Type == JTokenType.Property && prop.Value.Type == JTokenType.String)
                {
                    var value = prop.Value?.Value<string>();
                    if (value.StartsWith("ENV[", StringComparison.InvariantCulture) && value.Contains("]"))
                    {
                        string propertyValue = value;

                        //take variable name from first index of [ to ]. Expected something like ENV[VariableName] as variable name
                        string variableName = value.Substring(
                            value.IndexOf("ENV[", StringComparison.InvariantCulture),
                            value.IndexOf("]", value.IndexOf("[", StringComparison.InvariantCulture),
                                StringComparison.InvariantCulture));

                        if (!string.IsNullOrWhiteSpace(variableName))
                        {
                            variableName = variableName.Replace("ENV[", "").Replace("]", "");

                            string environmentValue = Environment.GetEnvironmentVariable(variableName) ?? string.Empty;

                            prop.Value = propertyValue.Replace($"ENV[{variableName}]", environmentValue);
                        }
                    }
                }
            });

            var deserializedObject = jsonObject.ToObject<SampleType>(
                JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));

            return deserializedObject;
        }

        private static void RecurseConfiguration(JToken node,
            Action<JProperty> updatePropertyAction = null)
        {
            if (node.Type == JTokenType.Object)
            {
                foreach (JProperty child in node.Children<JProperty>())
                {
                    updatePropertyAction?.Invoke(child);
                    RecurseConfiguration(child.Value, updatePropertyAction);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (JToken child in node.Children())
                {
                    RecurseConfiguration(child, updatePropertyAction);
                }
            }
        }
    }

    public class SampleType
    {
        public List<Source> Sources;

    }

    public class Source
    {
        public HttpDataSource DataSource;
    }

    public class HttpDataSource
    {
        public HttpConnection Connection;

    }

    public class HttpConnection
    {
        public HttpConfiguration Configuration;

        public ConnectionType Type;

    }

    public enum ConnectionType
    {
        CertAuthHttp = 0,
        ApiKey
    }

    public class HttpConfiguration
    {
        public string HttpEndpoint;
        public string CertName;
    }
}
