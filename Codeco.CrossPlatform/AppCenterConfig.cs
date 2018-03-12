namespace Codeco.CrossPlatform
{
    /// <summary>
    /// This file contains the VS App Center keys for UWP and Android.
    /// It should be marked as --skip-worktree in git, allowing it to be
    /// safely modified locally. It should never be checked in to source control after
    /// being modified.
    /// In the CI build, the ReplaceMe strings will be swapped out as a build step.
    /// </summary>
    public static class AppCenterConfig
    {
        public static string UwpKey => "<UwpReplaceMe>";
        public static string AndroidKey => "<AndroidReplaceMe>";
    }
}
