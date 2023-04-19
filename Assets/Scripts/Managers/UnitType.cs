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

public enum GroundMovementType {
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
	/// Transfer int to roman numeral.
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
	/// Transfer string to roman numeral if its int.
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
	/// Converts a number to a string representation with the appropriate English ordinal suffix (e.g. 1st, 2nd, 3rd, 4th).
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


	public static int GetUnitTier(int domain, int vehicles) {
		switch (domain) {
			case 1:
			//Air Force
			if (vehicles == 1) {
				return 1;
			} else if (vehicles == 2) {
				return 2;
			} else if (3 < vehicles && vehicles < 12) {
				return 3;
			} else if (11 < vehicles && vehicles < 24) {
				return 4;
			} else if (23 < vehicles && vehicles < 37) {
				return 5;
			} else if (36 < vehicles && vehicles < 49) {
				return 6;
			} else if (48 < vehicles && vehicles < 201) {
				return 7;
			} else {
				return 8;
			}
			case 2:
				//Navy
			if (vehicles == 1) {
				return 1;
			} else if (1 < vehicles && vehicles < 4 ) {
				return 2;
			} else if (3 < vehicles && vehicles < 7) {
				return 3;
			} else if (6 < vehicles && vehicles < 13) {
				return 4;
			} else {
				return 5;
			}
			default:
				//Army
			if (vehicles == 1) {
				return 1;
			} else if (vehicles == 2) {
				return 2;
			} else if (2 < vehicles && vehicles < 5) {
				return 3;
			} else if (4 < vehicles && vehicles < 14) {
				return 4;
			} else if (13 < vehicles && vehicles < 41) {
				return 5;
			} else if (40 < vehicles && vehicles < 84) {
				return 6;
			} else if (83 < vehicles && vehicles < 121) {
				return 7;
			} else {
				return 8;
			}
		}
	}

	public static bool ConvertIntToBool(int value) {
		if (value == 0) {
			return false;
		}
		return true;
	}

	public static int ConvertBoolToInt(bool value) {
		if (value) {
			return 1;
		}
		return 0;
	}
}

