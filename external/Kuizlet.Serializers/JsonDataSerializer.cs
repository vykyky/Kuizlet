using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kuizlet.Serializers
{
    public class JsonDataSerializer : IDataSerializer
    {
        public string Serialize<T>(T obj) => JsonSerializer.Serialize(obj);
        public T Deserialize<T>(string data) => JsonSerializer.Deserialize<T>(data);
    }
}
