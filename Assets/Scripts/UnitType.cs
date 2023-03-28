using System;
using System.Collections.Generic;
using System.Linq;

public enum AerialSpecialization {
	None = 0,
	A = 1,
	B = 2,
	C = 3,
	F = 4,
	R = 5,
	UAV = 6,
	UH = 7,
	AH = 8
}

public enum NavalSpecialization {
	None = 0,
	BB = 1,
	BC = 2,
	CG = 3,
	DD = 4,
	FF = 5,
	FS = 6,
	PC = 7,
	PG = 8
}

public enum GroundSpecialization {
	None = 0,
	HQ = 1,
	Infantry = 2,
	Armoured = 3,
	Reconnaissance = 4,
	AT = 5,
	ATM = 6,
	AA = 7,
	SAM = 8,
	SPG = 9,
	MLRS = 10
}

public enum GroundMovementType {
	None = 0,
	Motorized = 1,
	Mechanized = 2,
	Wheeled = 3
}

public enum GroundTransportType {
	None = 0,
	Airborne = 1,
	AirAssault = 2,
	Amphibious = 3,
	Marine = 4
}

public enum BaseType {
	Base = 0,
	Airfield = 1,
	Port = 2,
	Spawn = 3
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

	internal static string GetUnitTier(int tier) {
		switch (tier) {
			case 0:
			return "Ø";
			case int i when i >= 1 && i <= 3:
			return new string('●', i);
			case int i when i >= 4 && i <= 6:
			return new string('I', i - 3);
			case int i when i >= 7:
			return new string('X', i - 6);
			default:
			return "";
		}
	}

	/// <summary>
	/// Transfer int to roman numeral.
	/// </summary>
	/// <param name="unitIdentification">Unit int identification</param>
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
	/// Transfer string to roman numeral if its int.
	/// </summary>
	/// <param name="unitIdentification">Unit identification string.</param>
	/// <returns>Roman numeral string</returns>
	internal static string GetCorps(string unitIdentification) {
		if (unitIdentification == "") {
			return "0";
		}
		return GetCorps(Convert.ToInt16(unitIdentification));
	}

	/// <summary>
	/// Converts a number to a string representation with the appropriate English ordinal suffix (e.g. 1st, 2nd, 3rd, 4th).
	/// </summary>
	/// <param name="number">The number to convert.</param>
	/// <returns>A string representing the ordinal version of the input number.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the input number is less than 1.</exception>
	public static string NumberWithSuffix(int number) {
		switch (number % 10) {
			case 1 when number % 100 != 11:
			return $"{number}st";
			case 2 when number % 100 != 12:
			return $"{number}nd";
			case 3 when number % 100 != 13:
			return $"{number}rd";
			default:
			return $"{number}th";
		}
	}

	public static bool ConvertIntToBool(int value) {
		if (value == 0) {
			return false;
		}
		return true;
	}
}

