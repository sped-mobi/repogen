// -----------------------------------------------------------------------
// <copyright file="IndentedWriter.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public class IndentedWriter : TextWriter
    {
        private bool _needIndent;

        public IndentedWriter(StringBuilder builder, string indentString = "    ")
        {
            IndentString = indentString;
            InnerWriter = new StringWriter(builder);
        }

        public IndentedWriter(TextWriter writer, string indentString = "    ")
        {
            IndentString = indentString;
            InnerWriter = writer;
        }

        public override Encoding Encoding =>
            Encoding.ASCII;

        public int Indent { get; set; }

        public string IndentString { get; }

        public TextWriter InnerWriter { get; }

        public override void Close()
        {
            InnerWriter.Close();
        }

        public override void Flush()
        {
            InnerWriter.Flush();
        }

        public IDisposable OpenBlock()
        {
            return new Block(this);
        }

        public IDisposable OpenBlockString(string expression)
        {
            return new BlockWithString(this, expression);
        }

        public IDisposable OpenBlockSemicolon()
        {
            return new BlockWithSemicolon(this);
        }

        public IDisposable OpenProjectBlock()
        {
            return new ProjectBlock(this);
        }

        public IDisposable OpenPropertyGroupBlock()
        {
            return new PropertyGroupBlock(this);
        }

        public IDisposable OpenItemGroupBlock()
        {
            return new ItemGroupBlock(this);
        }

        public void PopIndent()
        {
            Indent--;
        }

        public void PushIndent()
        {
            Indent++;
        }

        public override void Write(char value)
        {
            WriteIndentIfNeeded();
            InnerWriter.Write(value);
        }

        public override void Write(long value)
        {
            WriteIndentIfNeeded();
            InnerWriter.Write(value);
        }

        public override void Write(object value)
        {
            WriteIndentIfNeeded();
            InnerWriter.Write(value);
        }

        public override void Write(string value)
        {
            WriteIndentIfNeeded();
            InnerWriter.Write(value);
        }

        public void WriteIndent()
        {
            _needIndent = false;
            for (int i = 0; i < Indent; i++)
            {
                InnerWriter.Write(IndentString);
            }
        }

        public override void WriteLine()
        {
            InnerWriter.WriteLine();
            _needIndent = true;
        }

        public override void WriteLine(char value)
        {
            WriteIndentIfNeeded();
            InnerWriter.WriteLine(value);
            _needIndent = true;
        }

        public override void WriteLine(long value)
        {
            WriteIndentIfNeeded();
            InnerWriter.WriteLine(value);
            _needIndent = true;
        }

        public override void WriteLine(object value)
        {
            WriteIndentIfNeeded();
            InnerWriter.WriteLine(value);
            _needIndent = true;
        }

        public override void WriteLine(string value)
        {
            if (_needIndent)
            {
                WriteIndent();
            }

            InnerWriter.WriteLine(value);
            _needIndent = true;
        }

        protected virtual void CloseBlockCore(bool semi = false)
        {
            PopIndent();
            WriteLine(semi ? "};" : "}");
        }

        protected virtual void CloseBlockCore(string expression)
        {
            PopIndent();
            Write("}");
            WriteLine(expression);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                InnerWriter.Dispose();
            }
        }

        protected virtual void OpenBlockCore()
        {
            WriteLine("{");
            PushIndent();
        }

        private void WriteIndentIfNeeded()
        {
            if (_needIndent)
            {
                WriteIndent();
            }
        }

        private class Block : IDisposable
        {
            private readonly IndentedWriter _writer;

            public Block(IndentedWriter writer)
            {
                _writer = writer;
                _writer.OpenBlockCore();
            }

            public void Dispose()
            {
                _writer.CloseBlockCore();
            }
        }

        private class BlockWithString : IDisposable
        {
            private readonly IndentedWriter _writer;
            private readonly string _expression;

            public BlockWithString(IndentedWriter writer, string expression)
            {
                _writer = writer;
                _expression = expression;
                _writer.OpenBlockCore();
            }

            public void Dispose()
            {
                _writer.CloseBlockCore(_expression);
            }
        }

        private class BlockWithSemicolon : IDisposable
        {
            private readonly IndentedWriter _writer;

            public BlockWithSemicolon(IndentedWriter writer)
            {
                _writer = writer;
                _writer.OpenBlockCore();
            }

            public void Dispose()
            {
                _writer.CloseBlockCore(true);
            }
        }

        private class ProjectBlock : IDisposable
        {
            private readonly IndentedWriter _writer;

            public ProjectBlock(IndentedWriter writer)
            {
                _writer = writer;
                _writer.WriteLine("<Project>");
                _writer.PushIndent();
            }

            public void Dispose()
            {
                _writer.PopIndent();
                _writer.WriteLine("</Project>");
            }
        }

        private class PropertyGroupBlock : IDisposable
        {
            private readonly IndentedWriter _writer;

            public PropertyGroupBlock(IndentedWriter writer)
            {
                _writer = writer;
                _writer.WriteLine("<PropertyGroup>");
                _writer.PushIndent();
            }

            public void Dispose()
            {
                _writer.PopIndent();
                _writer.WriteLine("</PropertyGroup>");
            }
        }

        private class ItemGroupBlock : IDisposable
        {
            private readonly IndentedWriter _writer;

            public ItemGroupBlock(IndentedWriter writer)
            {
                _writer = writer;
                _writer.WriteLine("<ItemGroup>");
                _writer.PushIndent();
            }

            public void Dispose()
            {
                _writer.PopIndent();
                _writer.WriteLine("</ItemGroup>");
            }
        }
    }
}
