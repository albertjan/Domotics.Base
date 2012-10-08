using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Domotics.Base
{
    /// <summary>
    /// compiles a string containing a swithing story to compiled c# code that implements a rulelogic and instantiates it.
    /// </summary>
    public static class RuleLogicCompiler
    {
        static RuleLogicCompiler()
        {
            CompiledStories = new Dictionary<string, IRuleLogic>();
        }

        private static Dictionary<string, IRuleLogic> CompiledStories { get; set; }

        public static IRuleLogic Compile(string logic)
        {
            if (CompiledStories.ContainsKey (logic)) return CompiledStories[logic];
            
            var rl = "using Domotics.Base;" +
                     "public class " + Path.GetRandomFileName().Replace(".", "") + " : IRuleLogic" +
                     "{ " +
                     "    public State GetNewState(State input, Connection connection, IEnumerable<Connection> connections) " +
                     "    { " +
                     "         return new Logic(input, connection, connections)." + logic + ";" +
                     "    }" +
                     "}";

            var cdp = CodeDomProvider.CreateProvider ("CSharp")
                .CompileAssemblyFromSource (
                    new CompilerParameters
                        {
                            GenerateInMemory = false,
                            GenerateExecutable = false,
                            IncludeDebugInformation = true,
                            OutputAssembly = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mydll.dll")
                        },
                    new[]
                        {
                            rl
                        });

            
            var ass = cdp.CompiledAssembly;

            var irl = ass.GetTypes().First(t => t.GetInterfaces().Contains(typeof (IRuleLogic)));

            return (IRuleLogic)Activator.CreateInstance(irl);
        }
    }
}