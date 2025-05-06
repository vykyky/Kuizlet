using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kuizlet.Serializers
{
    public class TxtDataSerializer : IDataSerializer
    {
        public string Serialize<T>(T obj)
        {
            if (obj is IEnumerable<object> list)
            {
                return string.Join('\n', list.Select(item =>
                {
                    var term = item.GetType().GetProperty("Term")?.GetValue(item)?.ToString();
            
                    var definition = item.GetType().GetProperty("Definition")?.GetValue(item)?.ToString();
                    return $"{term}, {definition}";
                }));
            }
            return string.Empty;
        }

        public T Deserialize<T>(string data)
        {
            var targetType = typeof(T);

            if (targetType.IsGenericType &&
                targetType.GetGenericTypeDefinition() == typeof(List<>))
            {

                var itemType = targetType.GetGenericArguments()[0]; // тип элементов списка

                // пустой список нужного типа
                var resultList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;

                var lines = data.Split(new[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var parts = line.Split(',', 2); // разделитель — запятая
                    if (parts.Length < 2) continue; // если частей меньше 2, пропускаем

                    var term = parts[0].Trim();
                    var definition = parts[1].Trim();

                    var item = Activator.CreateInstance(itemType); //новый экземпляр элемента списка
                    itemType.GetProperty("Term")?.SetValue(item, term);
                    itemType.GetProperty("Definition")?.SetValue(item, definition);

                    resultList.Add(item);
                }

                return (T)resultList;
            }

            throw new NotSupportedException("Тип должен быть List<T> с полями Term и Definition");
        }
    }
}
