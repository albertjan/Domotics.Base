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
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        static RuleLogicCompiler()
        {
            CompiledStories = new Dictionary<string, IRuleLogic>();
        }

        private static Dictionary<string, IRuleLogic> CompiledStories { get; set; }

        private static string GenerateClassName()
        {
            var rng = new Random(DateTime.Now.Millisecond);
            var buffer = new char[12];

            for (var i = 0; i < 12; i++)
            {
                buffer[i] = Chars[rng.Next(Chars.Length)];
            }
            return new string(buffer);
        }

        /// <summary>
        /// Compile the user story
        /// </summary>
        /// <param name="logic">the story</param>
        /// <returns>the newly compiled RuleLogic.</returns>
        public static IRuleLogic Compile(string logic)
        {
            if (CompiledStories.ContainsKey (logic)) return CompiledStories[logic];
            
            var rl = @"
                     using Domotics.Base.DSL;
                     using System.Collections.Generic;
                     namespace Domotics.Base.Generated
                     {
                         public class " + GenerateClassName() + @" : IRuleLogic
                         { 
                             public IEnumerable<StateChangeDirective> GetNewState(State input, 
                                                                      Connection connection,
                                                                      List<Connection> connections, 
                                                                      long lastTriggered,
                                                                      long timeOfLastChange)
                             { 
                                 return new Logic(input, connection, connections, lastTriggered, timeOfLastChange)." + logic + @".CollectedStateChanges;
                             }

                             public string Path { get; set; }

                             public string Logic 
                             { 
                                 get 
                                 {
                                     return """ + logic.Replace("\"", "\\\"") + @""";
                                 } 
                             }
                         }
                     }
                     ";

            var cp = new CompilerParameters
                         {
                             GenerateInMemory = false,
                             GenerateExecutable = false,
                             OutputAssembly = Path.GetRandomFileName ().Replace (".", "") + ".dll"
                         };

            cp.ReferencedAssemblies.Add ("Domotics.Base.dll");
            cp.ReferencedAssemblies.Add ("mscorlib.dll");
#if DEBUG
            cp.IncludeDebugInformation = true;
#endif
            var cdp = CodeDomProvider.CreateProvider ("CSharp").CompileAssemblyFromSource (cp, new[] { rl });
            
            var ass = cdp.CompiledAssembly;

            var irl = ass.GetTypes().First(t => t.GetInterfaces().Contains(typeof (IRuleLogic)));

            CompiledStories.Add(logic, (IRuleLogic)Activator.CreateInstance(irl));

            CompiledStories[logic].Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cdp.PathToAssembly);

            return CompiledStories[logic];
        }
    }
}