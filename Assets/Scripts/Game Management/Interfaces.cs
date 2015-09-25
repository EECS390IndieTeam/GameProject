using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IPlayer {
    void Die();
    void RespawnAt(Transform location);
    void MoveTo(Transform location);

    void TakeDamage(float Damage, string OtherUser, Vector3 direction);

    string Username {
        get;
    }
    int Team {
        get;
    }

    List<IWeapon> AvailableWeapons {
        get;
    }
    IWeapon CurrentWeapon {
        get;
    }

    int CurrentWeaponID {
        get;
    }

    float Health {
        get;
    }

	float MaxHealth {
		get;
	}
		
}
	
public interface IWeapon {
    void Fire();
    float CooldownRate {
        get;
    }
    float EnergyPerShot {
        get;
    }
    float DamagePerShot {
        get;
    }
    bool IsOverheating {
        get;
    }
    float CooldownDelay {
        get;
    }
    int WeaponID {
        get;
    }

    float MaxTemperature {
        get;
    }
}

public interface IGrenade {
    float Strength {
        get;
    }

    float Radius {
        get;
    }

    float FuseTime {
        get;
    }

    float FuseTimeRemaining {
        get;
    }

    string Thrower {
        get;
    }

}

public interface IGameMode {
	int minPlayers
	{
		get;
	}
	int maxPlayers
	{
		get;
	}
	IGameLevel level {
		get;
		set; //Not sure if we need to be able to set this or not.
	}
	string gameModeName 
	{
		get;
	}
}
	
public interface IGameLevel {
	string levelName {
		get;
	}
	void loadGameLevel();
}
