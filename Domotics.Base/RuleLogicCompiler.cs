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
            
            var rl = @"
                     using Domotics.Base;
                     using Domotics.Base.DSL;
                     using System.Collections.Generic;
                     namespace Domotics.Base.Generated
                     {
                         public class " + Path.GetRandomFileName().Replace(".", "") + @" : IRuleLogic
                         { 
                             public StateChangeDirective GetNewState(State input, Connection connection, List<Connection> connections)
                             { 
                                 return new Logic(input, connection, connections)." + logic + @";
                             }
                         }
                     }";

            var cp = new CompilerParameters
                         {
                             GenerateInMemory = false,
                             GenerateExecutable = false,
                             IncludeDebugInformation = true,
                             OutputAssembly = Path.GetRandomFileName ().Replace (".", "") + ".dll"
                         };

            cp.ReferencedAssemblies.Add("Domotics.Base.dll");
            cp.ReferencedAssemblies.Add ("mscorlib.dll");

            var cdp = CodeDomProvider.CreateProvider ("CSharp").CompileAssemblyFromSource (cp, new[] { rl });
            
            var ass = cdp.CompiledAssembly;

            var irl = ass.GetTypes().First(t => t.GetInterfaces().Contains(typeof (IRuleLogic)));

            return (IRuleLogic)Activator.CreateInstance(irl);
        }
    }
}