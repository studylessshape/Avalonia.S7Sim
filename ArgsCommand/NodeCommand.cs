using System.Collections.Generic;
using System.Linq;

namespace ArgsCommand
{
    public class NodeCommand
    {
        public string Command { get; set; }
        public NodeCommand SubCommand { get; set; } = null;
        public Dictionary<string, string[]> Options { get; set; } = new Dictionary<string, string[]>();

        public static NodeCommand ReadArgs(string[] args)
        {
            NodeCommand rootCmd = new NodeCommand();

            if (args != null && args.Length > 0)
            {
                var isFirstOption = args[0].TrimStart().StartsWith("-");
                if (!isFirstOption)
                {
                    rootCmd.SubCommand = ReadArgs(args.Skip(1).ToArray());
                    if (rootCmd.SubCommand != null)
                    {
                        rootCmd.SubCommand.Command = args[0];
                    }
                }
                else
                {
                    string option = null;
                    Dictionary<string, List<string>> optionValues = new Dictionary<string, List<string>>();

                    foreach (var arg in args)
                    {
                        if (option == null)
                        {
                            option = arg;
                            continue;
                        }
                        else
                        {
                            if (arg.StartsWith("-"))
                            {
                                option = arg;
                                continue;
                            }
                        }

                        if (!optionValues.ContainsKey(option))
                        {
                            optionValues.Add(option, new List<string>());
                        }
                        optionValues[option].Add(arg);
                    }

                    rootCmd.Options = optionValues.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
                }
            }

            return rootCmd;
        }
    }
}
