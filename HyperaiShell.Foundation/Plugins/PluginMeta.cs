namespace HyperaiShell.Foundation.Plugins
{
    public struct PluginMeta
    {
        public string Identity { get; private set; }
        public string FileBase { get; private set; }
        public string SpaceDirectory { get; private set; }

        public PluginMeta(string identity, string fileBase, string configDirectory)
        {
            Identity = identity;
            FileBase = fileBase;
            SpaceDirectory = configDirectory;
        }
    }
}