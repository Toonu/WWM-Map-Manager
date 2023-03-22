using System.Collections.Generic;
using System;
using System.Linq;

public enum UnitType {
    None,
    Infantry,
    Armoured,
    Recon,
    SPG,
    Mortar,
    SPAAG,
    SAM,
    AT,
    ATGM,
    AH,
    UH,
    UAV,
    F,
    SF,
    SOF
}

public enum UnitMobility {
    Foot,
    Motorized,
    Mechanized,
    Armoured,
    Airborne,
    AirAssault,
    Amphibious,
    Marine
}

public enum UnitMobilityModifier {
    Tracked,
    Wheeled,
    WheeledLight
}

public enum UnitTopModifier {
    Command,
    Naval,
    RocketArtillery
}

public enum UnitAffiliation {
    Friendly,
    Enemy,
    Neutral,
    Unknown
}

public enum UnitTier {
    Team,
    Squad,
    Section,
    Platoon,
    Company,
    Battalion,
    Regiment,
    Brigade,
    Division,
    Corps,
    Army,
    ArmyGroup,
    Theatre
}

public static class EnumUtil {
	public static IEnumerable<T> GetValues<T>() {
		return Enum.GetValues(typeof(T)).Cast<T>();
	}
}


