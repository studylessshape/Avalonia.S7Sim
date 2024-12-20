namespace PythonRun
{
    public class EnvSets
    {
        public string FilePath { get; set; }
        public string[] EnvDirectories { get; set; }

        public EnvSets(string[] args)
        {
            var filePathOptionIndex = -1;
            var envDirectoriesOptionIndex = -1;
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

                if (filePathOptionIndex != -1 && envDirectoriesOptionIndex != -1)
                {
                    break;
                }
            }

            if (filePathOptionIndex + 1 >= args.Length || filePathOptionIndex == -1)
            {
                throw new ArgumentException("No target file to execute", "-f / --file");
            }

            FilePath = args[filePathOptionIndex + 1];
            
            List<string> list = [];

            foreach (var arg in args.Skip(envDirectoriesOptionIndex))
            {
                if (arg == "-f" || arg == "--file")
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
