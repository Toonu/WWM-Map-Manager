﻿using System;
using System.Collections.Generic;
using System.Linq;

public enum AerialSpecialization {
	A = 0,
	B = 1,
	C = 2,
	F = 3,
	R = 4,
	UAV = 5,
	UH = 6,
	AH = 7
}

public enum NavalSpecialization {
	TaskElement = 0,
	TaskUnit = 1,
	TaskGroup = 2,
	TaskForce = 3,
	Fleet = 4
}

public enum GroundSpecialization {
	Infantry = 0,
	Armoured = 1,
	Reconnaissance = 2,
	AT = 3,
	ATM = 4,
	AA = 5,
	SAM = 6,
	SPG = 7,
	MLRS = 8,
	HQ = 9
}

public enum GroundProtectionType {
	Motorized = 0,
	MotorizedWheeled = 1,
	Mechanized = 2,
	MechanizedWheeled = 3
}

public enum GroundTransportType {
	None = 0,
	Airborne = 1,
	AirAssault = 2,
	Amphibious = 3
}

public enum BaseType {
	Base = 0,
	Airfield = 1,
	Port = 2,
	//Spawn = 3
}

public enum UnitTier {
	Team = 0,
	Squad = 1,
	Section = 2,
	Platoon = 3,
	Company = 4,
	Battalion = 5,
	Regiment = 6,
	Brigade = 7,
	Division = 8,
	Corps = 9,
	Army = 10,
	ArmyGroup = 11,
	Theatre = 12
}


public static class EnumUtil {
	public static IEnumerable<T> GetValues<T>() {
		return Enum.GetValues(typeof(T)).Cast<T>();
	}

	/// <summary>
	/// Method transforms unit tier int to string symbol.
	/// </summary>
	/// <param name="tier"></param>
	/// <returns></returns>
	internal static string GetUnitTier(int tier) {
		return tier switch {
			0 => "Ø",
			int i when i >= 1 && i <= 3 => new string('●', i),
			int i when i >= 4 && i <= 6 => new string('I', i - 3),
			int i when i >= 7 => new string('X', i - 6),
			_ => "",
		};
	}

	/// <summary>
	/// Method transfers unit ID int to roman numeral.
	/// </summary>
	/// <param name="unitIdentification">Unit int nameUI</param>
	/// <returns>Roman numeral string</returns>
	internal static string GetCorps(int unitIdentification) {
		string[] thousands = { "", "M", "MM", "MMM" };
		string[] hundreds = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
		string[] tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
		string[] ones = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

		return thousands[unitIdentification / 1000] +
			   hundreds[(unitIdentification % 1000) / 100] +
			   tens[(unitIdentification % 100) / 10] +
			   ones[unitIdentification % 10];
	}

	/// <summary>
	/// Method transforms unit number ID string to roman numeral string.
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public static int GetCorpsInt(string s) {
		Dictionary<char, int> romanDict = new() {
		{'I', 1},
		{'V', 5},
		{'X', 10},
		{'L', 50},
		{'C', 100},
		{'D', 500},
		{'M', 1000}
	};

		int result = 0;
		for (int i = 0; i < s.Length; i++) {
			int currentVal = romanDict[s[i]];
			int nextVal = i + 1 < s.Length ? romanDict[s[i + 1]] : 0;
			result += currentVal < nextVal ? -currentVal : currentVal;
		}
		return result;
	}

	/// <summary>
	/// Method transfers unit ID string to roman numeral if its int.
	/// </summary>
	/// <param name="unitIdentification">Unit nameUI string.</param>
	/// <returns>Roman numeral string</returns>
	internal static string GetCorps(string unitIdentification) {
		if (unitIdentification == "") {
			return "0";
		}
		return GetCorps(Convert.ToInt16(unitIdentification));
	}

	/// <summary>
	/// Method converts a number to a string representation with the appropriate English ordinal suffix (e.g. 1st, 2nd, 3rd, 4th).
	/// </summary>
	/// <param name="number">The number to convert.</param>
	/// <returns>A string representing the ordinal version of the input number.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the input number is less than 1.</exception>
	public static string NumberWithSuffix(int number) {
		return (number % 10) switch {
			1 when number % 100 != 11 => $"{number}st",
			2 when number % 100 != 12 => $"{number}nd",
			3 when number % 100 != 13 => $"{number}rd",
			_ => $"{number}th",
		};
	}

	/// <summary>
	/// Method transforms amount of vehicles and unit domain to unit tier int.
	/// </summary>
	/// <param name="domain"></param>
	/// <param name="vehicles"></param>
	/// <returns></returns>
	public static int GetUnitTier(int domain, int vehicles) {
		return domain switch {
			1 => vehicles switch {
				//Air Force
				1 => 1,
				2 => 2,
				> 2 and < 12 => 3,
				> 11 and < 24 => 4,
				> 23 and < 37 => 5,
				> 36 and < 49 => 6,
				> 48 and < 201 => 7,
				_ => 8
			},
			2 => vehicles switch {
				//Navy
				1 => 0,
				> 1 and < 4 => 1,
				> 3 and < 7 => 2,
				> 6 and < 13 => 3,
				_ => 4
			},
			_ => vehicles switch {
				//Army
				1 => 1,
				2 => 2,
				> 2 and < 5 => 3,
				> 4 and < 14 => 4,
				> 13 and < 41 => 5,
				> 40 and < 84 => 6,
				> 83 and < 121 => 7,
				_ => 8
			},
		};
	}

	/// <summary>
	/// Method transforms int to bool. 1 for true, anything else for false.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static bool ConvertIntToBool(int value) {
		return value == 1;
	}

	/// <summary>
	/// Method transforms bool to int. True for 1, false for 0.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static int ConvertBoolToInt(bool value) {
		return value ? 1 : 0;
	}
}

