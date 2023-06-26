using Ramsey.Board;
using Ramsey.Graph;
using Ramsey.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using static Ramsey.Gameplayer.BuilderUtils;

using Debug = UnityEngine.Debug;

namespace Ramsey.Gameplayer
{
    public class ExternalAPIInstance
    {
        public ExternalAPIInstance(ExternalAPI api, string consumerFilePath) 
        {
            this.api = api;

            var startinfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "python.exe",
                Arguments = consumerFilePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            process = Process.Start(startinfo);

            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (sender, args) => 
            {
                Debug.Log("python: " + args.Data);
                
                ExternalAPI.FormatIncoming(args.Data, out var name, out var callArgs);

                Debug.Log("'" + name + "'");

                if(name == "response")
                {
                    this.response = callArgs[0];
                    return;
                }

                if(!api.Bound.TryGetValue(name, out var callFn))
                    throw new InvalidOperationException($"Unknown call '{name}'");

                var response = callFn(callArgs);

                Call("response", response ?? "none");
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                Debug.LogError(args.Data);
            };

            process.Exited += (sender, args) =>
            {
                Debug.LogError("Python exited! " + process.ExitCode);
            };
            
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private readonly ExternalAPI api;
        private readonly Process process;
        
        private string response;

        public string Call(string call, params string[] args)
        {
            var txt = ExternalAPI.FormatOutgoing(call, args);

            response = null;
            process.StandardInput.WriteLine(txt);

            if(!Utils.WaitUntil(() => response != null, timeout: 2000))
            {
                Debug.LogError($"Call '{txt}' had no response!");
            }

            return response;
        }

        public void Close() 
        {
            process.Close();
        }
    }

    public class ExternalAPI
    {
        public static ExternalAPI New => new();

        public delegate string CallFunction(string[] parameters);
        public delegate void CallFunctionNoResponse(string[] parameters);

        private Dictionary<string, CallFunction> incomingCalls = new();

        public IReadOnlyDictionary<string, CallFunction> Bound => incomingCalls;

        public ExternalAPI Clone() 
            => new() { incomingCalls = incomingCalls.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) };

        public ExternalAPI Bind(string name, CallFunction call) 
        {
            incomingCalls.Add(name, call);
            return this;
        }
        public ExternalAPI Bind(string name, CallFunctionNoResponse call) 
        {
            incomingCalls.Add(name, o => {call(o); return null;});
            return this;
        }

        public static string FormatOutgoing(string call, params string[] args)
        {
            return args.Length == 0 
                ? call 
                : call + " " + string.Join(' ', args.Select(Escape));
        }
        public static void FormatIncoming(string input, out string call, out string[] args)
        {
            var firstSep = input.IndexOf(' ');

            if(firstSep == -1)
            {
                call = input;
                args = Array.Empty<string>();

                return;
            }

            call = input[..firstSep];

            var argslist = new List<string>();

            var idx = firstSep+1;

            while(idx < input.Length)
            {
                if(input[idx] == ' ')
                    throw new FormatException($"Unexpected ' ' in call '{input}'");
                
                if(input[idx] == '"')
                {
                    var s = "";
                    idx++;

                    while(true)
                    {
                        if(input[idx] == '\\')
                        {
                            if(idx == input.Length - 1) 
                                throw new FormatException($"Unterminated escape in call '{input}'");

                            if(input[idx + 1] == '\\')
                                s += '\\';
                            else if(input[idx + 1] == '"')
                                s += '"';
                            else if(input[idx + 1] == 'n')
                                s += '\n';
                            else 
                                throw new FormatException($"Unknown escape code \\{input[idx]} in call '{input}");
                            
                            idx++;
                        }
                        else if(input[idx] == '"')
                        {
                            break;
                        }
                        else 
                        {
                            s += input[idx];
                        }

                        idx++;

                        if(idx >= input.Length)
                            throw new FormatException($"Unterminated string in call '{input}'");
                    }

                    argslist.Add(s);
                    idx++;
                }
                else 
                {
                    var sidx = idx;
                    while(input[idx] != ' ' && idx < input.Length) idx++;

                    argslist.Add(input[sidx..idx]);
                }

                idx++;
            }

            if(idx != input.Length)
                throw new FormatException($"Unknown format in call '{input}'");

            args = argslist.ToArray();
        }

        public static string Escape(string str)
        {
            str = str
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\\\\\"", "\\\"")
                .Replace("\n", "\\n");

            if(Regex.Matches(str, @"\s").Any())
            {
                str = '"' + str + '"';
            }

            return str;
        }
    }

    public static class GraphAPIs 
    {
        public static GameState OperativeState { get; set; }
        public static readonly ExternalAPI GameStateAPI = ExternalAPI.New
            .Bind("makenode", p => "" + OperativeState.CreateNode().ID);
    }

    public class ExternalBuilder : Builder
    {
        public ExternalBuilder(string program)
        {
            api = new(GraphAPIs.GameStateAPI, program);

            Reset();
        }

        ExternalAPIInstance api;

        public override BuilderMove GetMove(GameState gameState)
        {
            var move = api.Call("getmove");
            
            var split = move.Split(' ');

            return new(gameState.Nodes[int.Parse(split[0])], gameState.Nodes[int.Parse(split[1])]);
        }

        public override void Reset()
        {
            api.Call("reset");
        }
    }

    public class ExternalPainter : Painter
    {
        public ExternalPainter(string program)
        {
            api = new(GraphAPIs.GameStateAPI, program);

            Reset();
        }

        ExternalAPIInstance api;

        public override PainterMove GetMove(GameState gameState)
        {
            var move = api.Call("getmove");

            return new(gameState.NewestEdge, int.Parse(move));
        }

        public override void Reset()
        {
            api.Call("reset");
        }
    }
}
