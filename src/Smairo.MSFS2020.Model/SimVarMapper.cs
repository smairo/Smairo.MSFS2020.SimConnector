using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Smairo.MSFS2020.Model.Enumeration;

namespace Smairo.MSFS2020.Model
{
    public class SimVarMapper
    {
        private readonly Dictionary<string, SimVar> _variables;
        public SimVarMapper()
        {
            var json = File.ReadAllText("simvariables.json", Encoding.UTF8);
            _variables = JsonConvert.DeserializeObject<Dictionary<string, SimVar>>(json);
        }

        public List<SimvarRequest> GetRequestsForStruct<T>()
            where T : struct
        {
            var t = typeof(T);
            var definition = (Definition) Enum.Parse(typeof(Definition), t.Name);
            var members = t
                .GetFields()
                .Select(f => f.Name);

            var result = new List<SimvarRequest>();
            foreach (var member in members)
            {
                var sdkName = member
                    .Replace("__", ":")
                    .Replace("_", " ");

                if (_variables.TryGetValue(sdkName, out var value))
                {
                    result.Add(
                        new SimvarRequest
                        {
                            DataType = value.DataType,
                            Definition = definition,
                            NameUnitTuple = (sdkName, value.Unit),
                            Request = (Request) Enum.Parse(typeof(Request), member)
                        });
                }
                else
                {
                    throw new InvalidOperationException(
                        $"simvariables.json is missing '{sdkName}'");
                }
            }

            return result;
        }

        public List<string> GetSettables()
        {
            return _variables
                .Where(kvp => kvp.Value.Settable)
                .Select(kvp => kvp.Key)
                .ToList();
        }
    }
}