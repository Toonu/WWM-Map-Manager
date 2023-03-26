using System;
using System.Collections.Generic;
using System.Linq;

public enum AerialSpecialization {
	None = 0, A = 1, B = 2, C = 3, F = 4, R = 5, UAV = 6, UH = 7, AH = 8
}

public enum NavalSpecialization {
	None = 0, BB = 1, BC = 2, CG = 3, DD = 4, FF = 5, FS = 6, PC = 7, PG = 8
}

public enum GroundSpecialization {
	None = 0, HQ = 1, Infantry = 2, Armoured = 3, Reconnaissance = 4, AT = 5, ATM = 6, AA = 7, SAM = 8, SPG = 9, MLRS = 10
}

public enum GroundMovementType { 
	None = 0, Motorized = 1, Mechanized = 2, Wheeled = 3
}

public enum GroundTransportType {
	None = 0, Airborne = 1,	AirAssault = 2, Amphibious = 3, Marine = 4
}

public enum BaseType {
	Base = 0, Airfield = 1, Port = 2, Spawn = 3
}

public enum UnitTier {
	Team = 0,
	Squad = 1,
	Section = 2,
	Platoon = 3,
	Company = 4,
	Battalion = 5,
	Regiment = 6,
	Brigade	= 7,
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
}


