using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator
{
    public class ErrorTolerantXmlSerializer : XmlSerializer
    {
        public object Object { get; }

        public ErrorTolerantXmlSerializer(object o)
        {
            Object = o;
        }

        public override bool CanDeserialize(XmlReader xmlReader)
        {
            return base.CanDeserialize(xmlReader);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override XmlSerializationReader CreateReader()
        {
            return base.CreateReader();
        }

        protected override XmlSerializationWriter CreateWriter()
        {
            return new ErrorTolerantXmlWriter(Object);
        }

        protected override object Deserialize(XmlSerializationReader reader)
        {
            return base.Deserialize(reader);
        }

        protected override void Serialize(object o, XmlSerializationWriter writer)
        {
            base.Serialize(o, writer);
        }

        public string Serialize(SimpleXmlWriter xmlWriter, object o)
        {
            var serializer = new ErrorTolerantXmlWriter(o);
            serializer.Write(xmlWriter);
            return xmlWriter.ToString();
        }


        private class ErrorTolerantXmlWriter : XmlSerializationWriter
        {
            public Type Type { get; }

            public object Object { get; }

            public ErrorTolerantXmlWriter(object o)
            {
                Type = o.GetType();
                Object = o;

            }

            public void Write(SimpleXmlWriter writer)
            {
                var properties = Type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                writer.WriteStartElement(Type.Name);

                foreach (var property in properties)
                {
                    string stringValue = "";
                    object value = property.GetValue(Object, null);

                    Type valueType = value?.GetType();

                    if (valueType != null)
                    {

                    }

                    if (value != null)
                    {
                        if (valueType.IsPrimitive || valueType == typeof(string))
                        {
                            stringValue = Convert.ToString(value);
                            writer.WriteElementString(property.Name, stringValue);
                        }


                    }

                }

                writer.WriteFullEndElement();
            }

            protected override void InitCallbacks()
            {

            }
        }
    }
    public class SimpleXmlWriter : XmlTextWriter
    {
        private readonly StringBuilder _sb;
        private XmlWriterSettings _settings;

        public SimpleXmlWriter(StringBuilder sb) : base(new StringWriter(sb))
        {
            _sb = sb;
        }

        public override XmlWriterSettings Settings
        {
            get
            {
                return _settings ??
                       (_settings = new XmlWriterSettings
                       {
                           Indent = true,
                           IndentChars = "  "
                       });
            }
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }


}
