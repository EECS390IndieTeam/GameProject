using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//this is the common interface for all player objects
//every player will have an implementation of this attached
public interface IPlayer {
    /// <summary>
    /// Kills the player
    /// </summary>
    /// <param name="killer">Who (or what) killed the player</param>
    /// <param name="weaponID">The weapon the killer used to kill the player (will be defined by an enum later)</param>
    void Die(string killer, int weaponID);
    /// <summary>
    /// Respawns the player at the given location
    /// </summary>
    /// <param name="location"></param>
    void RespawnAt(Transform location);
    /// <summary>
    /// Respawns the player at the given location
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    void RespawnAt(Vector3 position, Quaternion rotation);
    /// <summary>
    /// Moves the player to the given location
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    void MoveTo(Vector3 position, Quaternion rotation);
    /// <summary>
    /// Moves the player to the given location
    /// </summary>
    /// <param name="location"></param>
    void MoveTo(Transform location);

    /// <summary>
    /// Deals damage to the player
    /// </summary>
    /// <param name="Damage">How much damage was dealt</param>
    /// <param name="DamageSource">Who delt the damage to the player (will usually be another player's username)</param>
    /// <param name="direction">The direction in world space that the damage came from</param>
    /// <param name="weaponID">The ID of the weapon that dealt the damage, will be provided by an enum later</param>
    void TakeDamage(float Damage, string DamageSource, Vector3 direction, int weaponID);

    /// <summary>
    /// The username of the player
    /// </summary>
    string Username {
        get;
    }
    /// <summary>
    /// What team this player is on (0-7)
    /// </summary>
    int Team {
        get;
    }

    /// <summary>
    /// A list of the IWeapons that this player has
    /// </summary>
    List<IWeapon> AvailableWeapons {
        get;
    }
    /// <summary>
    /// A reference to the player's currently held IWeapon
    /// </summary>
    IWeapon CurrentWeapon {
        get;
    }

    /// <summary>
    /// The ID of the currently held weapon
    /// [This shouldnt be needed, right? coultn't we just do CurrentWeapon.ID?]
    /// </summary>
    int CurrentWeaponID {
        get;
    }

    /// <summary>
    /// The player's current health
    /// </summary>
    float Health {
        get;
    }

    /// <summary>
    /// The maximum health for this player
    /// </summary>
	float MaxHealth {
		get;
	}

    /// <summary>
    /// Where this player's grapple beam ends in world space
    /// </summary>
    Vector3 GrappleEndpoint {
        get;
    }

    /// <summary>
    /// true if this player's grapple beam is currently visible (fired)
    /// </summary>
    bool GrappleVisible {
        get;
    }

	/// <summary>
	/// Where the player's fired laser ends in world space
	/// </summary>
	Vector3 LaserEndpoint {
		get;
	}

	/// <summary>
	/// true if this player is firing their gun
	/// </summary>
	bool LaserVisible {
		get;
	}

	Vector3 MuzzlePoint {
		get;
	}

	bool HoldingFlag {
		get;
	}

    int HeldFlagTeam {
        get;
    }

    Color HeldFlagColor {
        get;
    }

    bool IsDead {
        get;
    }
}
	
/// <summary>
/// An IWeapon will be attached to every weapon.  Using the Editor, we can then assign the various properties of
/// the weapon, or make a new implementation if needed
/// </summary>
public interface IWeapon {
    /// <summary>
    /// Fires the weapon
    /// </summary>
    //void Fire();
    /// <summary>
    /// How fast the weapon will cool down in units/second
    /// </summary>
    float CooldownRate {
        get;
    }
    /// <summary>
    /// How much the weapon's temperature will increase every time it is fired
    /// </summary>
    float EnergyPerShot {
        get;
    }
    /// <summary>
    /// How much damage a player will take when hit by this weapon
    /// </summary>
    float DamagePerShot {
        get;
    }
    /// <summary>
    /// This field will be true if the weapon is in the overheated state.  A weapon enters the overheated 
    /// state when it has exceeded its maximum temperature.  Once the weapon has cooled down all the way, it will
    /// leave the overheated state.  A weapon cannot be fired while it is in the overheated state.  The weapon
    /// will cooldown at a slightly faster rate when it is overheated.  
    /// </summary>
    bool IsOverheating {
        get;
    }
    /// <summary>
    /// When the weapon is in the Overheated state, it will cool down at a rate equal to CoolDownRate * OverheatedCooldownMultiplier
    /// </summary>
    float OverheatedCooldownMultiplier {
        get;
    }
    /// <summary>
    /// The weapon will wait this many seconds after each shot before starting to cool down
    /// </summary>
    float CooldownDelay {
        get;
    }
    /// <summary>
    /// The ID number of this weapon
    /// </summary>
    int WeaponID {
        get;
    }

    /// <summary>
    /// The maximum temperature that this weapon can reach before it will overheat
    /// </summary>
    float MaxTemperature {
        get;
    }
}

/// <summary>
/// The interface for grenades, it is used very simularly to IWeapon
/// </summary>
public interface IGrenade {
    /// <summary>
    /// The 'strength' of the grenade.  Depending on the implementation, this could be the damage, 
    /// or maybe knockback, or maybe something else
    /// </summary>
    float Strength {
        get;
    }
    /// <summary>
    /// When the grenade goes off, only objects within Radius of the grenade are effected
    /// </summary>
    float Radius {
        get;
    }
    /// <summary>
    /// How long this grenade will take to explode in seconds.  This value will never change
    /// during the lifetime of the IGrenade object
    /// </summary>
    float FuseTime {
        get;
    }
    /// <summary>
    /// How many seconds are left on the grenade's fuse.  When this value reaches zero, the grenade will
    /// explode.  This value starts off equal to FuseTime.
    /// </summary>
    float FuseTimeRemaining {
        get;
    }
    /// <summary>
    /// This is the name of the player who threw the Grenade
    /// </summary>
    string Thrower {
        get;
    }

}

//IGameMode moved to Assets/Scripts/Game Modes/IGameMode.cs
// -SM
