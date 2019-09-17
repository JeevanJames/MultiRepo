using System;
using LibGit2Sharp;
using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    //internal static class CredentialsProvider
    //{
    //    private static Credentials _credentials;

    //    public static Credentials Provide(string url, string userName, SupportedCredentialTypes types)
    //    {
    //        if (_credentials != null)
    //            return _credentials;

    //        if (string.IsNullOrWhiteSpace(userName))
    //            userName = Prompt($"{Magenta}User name: ", value => !string.IsNullOrWhiteSpace(value));
    //        else
    //            PrintLine($"{Magenta}User name: {Reset}{userName}");

    //        string password = ReadSecret($"{Magenta}Password : ", needValue: true);

    //        _credentials = new UsernamePasswordCredentials
    //        {
    //            Username = userName,
    //            Password = password,
    //        };

    //        return _credentials;
    //    }
    //}

    public sealed class CredentialProvider
    {
        private Credentials _credentials;

        public Credentials Provide(string url, string userName, SupportedCredentialTypes types)
        {
            bool supportsDefaultCreds = (types & SupportedCredentialTypes.Default) == SupportedCredentialTypes.Default;
            bool supportsUserNameCreds = (types & SupportedCredentialTypes.UsernamePassword) ==
                                         SupportedCredentialTypes.UsernamePassword;

            if (supportsDefaultCreds)
                return new DefaultCredentials();

            if (supportsUserNameCreds)
                return _credentials ?? (_credentials = PromptUserNamePassword(userName));

            throw new NotSupportedException("Invalid authentication scheme");
        }

        private static Credentials PromptUserNamePassword(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                userName = Prompt($"{Magenta}User name: ", value => !string.IsNullOrWhiteSpace(value));
            else
                PrintLine($"{Magenta}User name: {Reset}{userName}");

            string password = ReadSecret($"{Magenta}Password : ", needValue: true);

            return new UsernamePasswordCredentials
            {
                Username = userName,
                Password = password,
            };
        }
    }
}
