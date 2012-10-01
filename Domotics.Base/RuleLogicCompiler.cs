using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            else
            {
                var rl = "using Domotics.Base;" +
                         "public class " + Path.GetRandomFileName().Replace(".", "") + " : IRuleLogic" +
                         "{ " +
                         "    public IState GetNewState(IState input) " +
                         "    { " +
                         "         return " + logic + "" +
                         "    }" +
                         "}";

                var ass = CodeDomProvider.CreateProvider ("CSharp")
                                     .CompileAssemblyFromSource (
                                        new CompilerParameters
                                        {
                                            GenerateInMemory = false,
                                            GenerateExecutable = false,
                                            IncludeDebugInformation = false,
                                            OutputAssembly = "RuleLogic",
                                        },
                                        new[]
                                        {
	                                        rl
	                                    }).CompiledAssembly;

                var irl = ass.GetTypes().First(t => t.GetInterfaces().Contains(typeof (IRuleLogic)));

                return (IRuleLogic)Activator.CreateInstance(irl);
            }
        }
    }
}