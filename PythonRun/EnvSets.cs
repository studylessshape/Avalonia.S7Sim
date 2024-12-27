namespace PythonRun
{
    public class EnvSets
    {
        public string FilePath { get; set; }
        public string[] EnvDirectories { get; set; }
        public string NamedPipe { get; set; }

        public EnvSets(string[] args)
        {
            var filePathOptionIndex = -1;
            var envDirectoriesOptionIndex = -1;
            var namePipeOptionIndex = -1;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-f" || args[i] == "--file")
                {
                    filePathOptionIndex = i;
                }
                else if (args[i] == "-s" || args[i] == "--search-paths")
                {
                    envDirectoriesOptionIndex = i;
                }
                else if (args[i] == "-n" || args[i] == "--name-pipe")
                {
                    namePipeOptionIndex = i;
                }

                if (filePathOptionIndex != -1 && envDirectoriesOptionIndex != -1 && namePipeOptionIndex != -1)
                {
                    break;
                }
            }

            if (filePathOptionIndex + 1 >= args.Length || filePathOptionIndex == -1)
            {
                throw new ArgumentException("No target file to execute", "-f / --file");
            }
            if (namePipeOptionIndex + 1 >= args.Length || namePipeOptionIndex == -1)
            {
                throw new ArgumentException("Need name of the name pipe", "-n / --name-pipe");
            }

            FilePath = args[filePathOptionIndex + 1];
            NamedPipe = args[namePipeOptionIndex + 1];

            List<string> list = [];

            foreach (var arg in args.Skip(envDirectoriesOptionIndex))
            {
                if (arg == "-f" || arg == "--file" || arg == "-n" || arg == "--name-pipe")
                {
                    break;
                }

                if (arg == "-s" || arg == "--search-paths")
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(arg))
                {
                    list.Add(arg);
                }
            }

            EnvDirectories = [.. list];
        }
    }
}
