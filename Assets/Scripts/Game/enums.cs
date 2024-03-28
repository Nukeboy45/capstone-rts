using System;

namespace Capstone
{
    public enum SquadState
    {
        stationary,
        attackstationary,
        moving,
        attackmove,
        retreating
    }

    public enum UIState
    {
        defaultMenu,
        squad,
        group,
        vehicle,
        other
    }

    [Serializable]
    public enum FactionType
    {
        centralPowers,
        alliedForces
    }

    [Serializable]
    public enum GameActorType
    {
        player,
        aiEasy,
        aiHard
    }

    public enum IconStatus
    {
        queued,
        building, 
        active
    }

    public enum UnitIconRender
    {
        player,
        playerTeam,
        enemy
    }
    
}