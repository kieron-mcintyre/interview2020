using System.Text.RegularExpressions;

namespace Digbyswift.Interview.UI.Email
{
	public static class EmailExtensions
	{
        public const string Email = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        public static readonly Regex EmailRegex = new Regex(Email, RegexOptions.IgnoreCase);

        public static bool IsEmail(this string value)
        {
            return EmailRegex.IsMatch(value);
        }

	}
}