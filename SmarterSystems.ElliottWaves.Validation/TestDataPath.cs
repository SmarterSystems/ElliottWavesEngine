using System;
using System.IO;

namespace SmarterSystems.ElliottWaves.Validation
{
    /// <summary>
    /// Resolves the on-disk path to the test data directory in a portable way.
    /// Resolution order:
    ///   1. Environment variable ELLIOTTWAVES_TESTDATA_PATH (absolute path), if set.
    ///   2. Walk upward from the test assembly location looking for a *.sln file;
    ///      use {sln-folder}/testdata.
    ///   3. Fall back to {current working directory}/testdata.
    /// The fetch-test-data.ps1 script in the repo root downloads the JSON files
    /// expected by the regression tests into this directory.
    /// </summary>
    public static class TestDataPath
    {
        private static readonly Lazy<string> _basePath = new(ResolveBasePath);

        /// <summary>Absolute path to the test data directory (may not exist on disk yet).</summary>
        public static string BasePath => _basePath.Value;

        /// <summary>Absolute path to the OHLCV JSON file for a given symbol.</summary>
        public static string ChartDataFile(string symbol) =>
            Path.Combine(BasePath, $"chartdata_{symbol}.json");

        /// <summary>Absolute path to a file inside the test output directory.</summary>
        public static string OutputFile(string fileName) =>
            Path.Combine(OutputPath, fileName);

        /// <summary>Absolute path to the test output directory, created on first access.</summary>
        public static string OutputPath
        {
            get
            {
                var path = Path.Combine(RepoRoot, "testoutput");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary>Absolute path to the repository root (the folder containing the .sln).</summary>
        public static string RepoRoot => _repoRoot.Value;

        private static readonly Lazy<string> _repoRoot = new(ResolveRepoRoot);

        private static string ResolveBasePath()
        {
            var env = Environment.GetEnvironmentVariable("ELLIOTTWAVES_TESTDATA_PATH");
            if (!string.IsNullOrWhiteSpace(env))
                return env;

            return Path.Combine(RepoRoot, "testdata");
        }

        private static string ResolveRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (dir.GetFiles("*.sln").Length > 0)
                    return dir.FullName;
                dir = dir.Parent;
            }
            return Directory.GetCurrentDirectory();
        }
    }
}
