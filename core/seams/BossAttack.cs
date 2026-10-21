namespace ScopeCreep.Core.seams;



public record BossAttack(
    string name, 
    int    damage,
    bool   hits_all
);