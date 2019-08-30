using System;
using System.Text;
using CodeGenerator.XXXX.Implementation;

namespace CodeGenerator
{
    public static class Program
    {
        private static StringBuilder Builder;
        private static ClipboardWriter Writer;

        [STAThread]
        public static void Main()
        {
            Before();

            var templates = TemplateEngine.GetUserTemplates();




        }

        //private static void One()
        //{
        //    var instance = XXX.SolutionFactory.Create();


        //    StringBuilder sb = new StringBuilder();
        //    StringWriter wr = new StringWriter(sb);
        //    XmlWriter xmlWriter = XmlWriter.Create(wr, new XmlWriterSettings { Indent = true });
        //    XmlSerializer serializer = new XmlSerializer(instance.GetType());
        //    serializer.Serialize(xmlWriter, instance);
        //    string text = sb.ToString();

        //    File.WriteAllText(@"c:\stage\vs\repogen\tools\CodeGenerator\Solution.xml", text);
        //}

        private static void After()
        {
            Writer.Clip();
            Console.WriteLine(Builder);
            Console.ReadKey();
        }

        private static void Before()
        {
            Builder = new StringBuilder();
            Writer = ClipboardWriter.CreateForStringBuilder(Builder);
        }

        private static void Write(string value)
        {
            Writer.Write(value);
        }

        private static void WriteLine(string value)
        {
            Writer.WriteLine(value);
        }

        private static void WriteLine()
        {
            Writer.WriteLine();
        }

        private static IDisposable OpenBlock()
        {
            return Writer.OpenBlock();
        }
    }
}
