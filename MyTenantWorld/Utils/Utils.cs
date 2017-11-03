using System;
using System.Text.RegularExpressions;

namespace MyTenantWorld
{
	public static class Utils
	{
		public static bool IsValidEmail(string email)
		{
			const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
			bool IsValid = (Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase));
			if (IsValid)
				return true;    
			return false;
		}

		public static bool IsAlaphabetContained(string fieldText)
		{
			int alphabetCounter = Regex.Matches(fieldText,@"[a-zA-Z]").Count;
			if (alphabetCounter == 0)
				return false;
			return true;
		}
		public static int GetUnitIdType(string text)
		{
			if (!IsAlaphabetContained(text))
			{
				var count = Regex.Matches(text, @"#[0-9]+-[0-9]+").Count;
				if (count == 1)
					return 2;
				var numberCount = Regex.Matches(text, @"#[0-9]+").Count;
				if (numberCount == 1)
					return 1;
			}
			return 0;
		}
	
	}

}
