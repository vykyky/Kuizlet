using System;
using System.Collections;
using DocumentFormat.OpenXml;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Kuizlet.Serializers
{
    public class WordDataSerializer : IDataSerializer
    {
        public string Serialize<T>(T obj)
        {
            throw new NotImplementedException("Сериализация в Word пока не поддерживается.");
        }
        public byte[] SerializeToBytes(IEnumerable<object> data)
        {
            using var ms = new MemoryStream();
            using (var wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                var body = new Body();

                foreach (var item in data)
                {
                    var type = item.GetType();
                    var termProp = type.GetProperty("Term");
                    var defProp = type.GetProperty("Definition");

                    if (termProp == null || defProp == null)
                        continue;

                    var term = termProp.GetValue(item)?.ToString() ?? string.Empty;
                    var definition = defProp.GetValue(item)?.ToString() ?? string.Empty;

                    var line = $"{term}, {definition}";
                    body.AppendChild(new Paragraph(new Run(new Text(line))));
                }

                mainPart.Document = new Document(body);
                mainPart.Document.Save();
            }

            // сброс позиции перед чтением массива
            ms.Position = 0;
            return ms.ToArray();

        }

        public T Deserialize<T>(string data)
        {
            throw new NotSupportedException("WordDataSerializer не использует string.");
        }

        public async Task<T> DeserializeAsync<T>(Stream stream)
        {
            var targetType = typeof(T);
            if (!targetType.IsGenericType || targetType.GetGenericTypeDefinition() != typeof(List<>))
                throw new NotSupportedException("Ожидается тип List<T>");

            var itemType = targetType.GetGenericArguments()[0];
            var props = itemType.GetProperties();

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var wordDoc = WordprocessingDocument.Open(memoryStream, false);
            var body = wordDoc.MainDocumentPart.Document.Body;
            var paragraphs = body.Elements<Paragraph>();

            foreach (var para in paragraphs)
            {
                var text = para.InnerText;
                var parts = text.Split(',', 2); // Разделение по запятой

                var obj = Activator.CreateInstance(itemType)!;

                if (parts.Length > 0 && props.Any(p => p.Name.Equals("Term", StringComparison.OrdinalIgnoreCase)))
                    props.First(p => p.Name.Equals("Term", StringComparison.OrdinalIgnoreCase)).SetValue(obj, parts[0].Trim());

                if (parts.Length > 1 && props.Any(p => p.Name.Equals("Definition", StringComparison.OrdinalIgnoreCase)))
                    props.First(p => p.Name.Equals("Definition", StringComparison.OrdinalIgnoreCase)).SetValue(obj, parts[1].Trim());

                list.Add(obj);
            }

            return (T)list;
        }
    }
}
