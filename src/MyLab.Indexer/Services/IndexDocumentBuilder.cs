using System;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace MyLab.Indexer.Services
{
    class IndexDocumentBuilder
    {
        private readonly DbEntity _initial;

        /// <summary>
        /// Initializes a new instance of <see cref="IndexDocumentBuilder"/>
        /// </summary>
        public IndexDocumentBuilder(DbEntity initial)
        {
            _initial = initial ?? throw new ArgumentNullException(nameof(initial));
        }

        public XDocument BuildXml()
        {
            var doc = new XDocument();
            var root = new XElement("root");
            doc.Add(root);

            root.Add(new XElement(nameof(DbEntity.Id), _initial.Id));

            if(_initial.ExtendedProperties != null)
            {
                foreach (var property in _initial.ExtendedProperties)
                {
                    if (property.Value != null)
                        root.Add(new XElement(property.Key, property.Value.ToString()));
                }
            }

            return doc;
        }

        public string BuildJson(Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeXNode(BuildXml(), formatting, true);
        }
    }
}