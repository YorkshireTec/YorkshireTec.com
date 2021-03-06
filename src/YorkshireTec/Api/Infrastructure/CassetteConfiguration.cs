namespace YorkshireTec.Api.Infrastructure
{
    using Cassette;
    using Cassette.Scripts;
    using Cassette.Stylesheets;

    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            bundles.AddPerSubDirectory<StylesheetBundle>("Styles");
            bundles.AddPerSubDirectory<ScriptBundle>("Scripts");
        }
    }
}