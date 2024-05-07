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

    public enum BuildingType
    {
        neutralGarrison,
        headquarters,
        production,
        ownedGarrison,
        ownedDefense
    }

    public enum UIState
    {
        defaultMenu,
        productionStructure,
        squad,
        group,
        vehicle,
        other
    }

    [Serializable]
    public enum FactionType
    {
        centralPowers,
        ententeForces
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

    public enum PlayerState
    {
        Default,
        Building
    }
    
    public enum TargetType
    {
        infantry,
        building
    }

    public enum SelectMode
    {
        click,
        drag
    }
}