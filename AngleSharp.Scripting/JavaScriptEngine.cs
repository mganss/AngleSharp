﻿namespace AngleSharp.Scripting
{
    using AngleSharp.Infrastructure;
    using AngleSharp.Network;
    using AngleSharp.Tools;
    using Jint;
    using Jint.Runtime.Environments;
    using System;
    using System.IO;
    using System.Text;

    public class JavaScriptEngine : IScriptEngine
    {
        readonly Engine _engine;

        public JavaScriptEngine()
        {
            _engine = new Engine();
            _engine.SetValue("console", new ConsoleInstance(_engine));
        }

        public String Type
        {
            get { return "text/javascript"; }
        }

        public void Evaluate(String source, ScriptOptions options)
        {
            var context = new DomNodeInstance(_engine, options.Context ?? new AnalysisWindow(options.Document));
            var env = LexicalEnvironment.NewObjectEnvironment(_engine, context, _engine.ExecutionContext.LexicalEnvironment, true);
            _engine.EnterExecutionContext(env, env, context);
            _engine.Execute(source);
            _engine.LeaveExecutionContext();
        }

        public void Evaluate(IResponse response, ScriptOptions options)
        {
            var reader = new StreamReader(response.Content, options.Encoding ?? Encoding.UTF8, true);
            var content = reader.ReadToEnd();
            reader.Close();
            Evaluate(content, options);
        }
    }
}
