using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Mobs;
using Fallen_Knight.src.Interface;
using Fallen_Knight.src.PlayerState;
using Microsoft.Xna.Framework;



public class Attack : PlayerState
{
    float attackTime = 0.3f;
    
    public Attack(Player player) : base(player)
    {
        
    }

    public override void HandleInput(GameTime gameTime)
    {
        HandleCommonInput();
        if (attackTime > 0f) 
        {
            InvokeState();
            attackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else if (attackTime <= 0)
        {
            Player.SwitchState(new Idle(Player));
        }
    }

    private void InvokeState()
    {
        foreach (var enemy in Player.Level.enemies)
        {
            Enemy mob = enemy as Enemy;
            if (mob.BoundingRectangle.Intersects(Player.playerWeapon.AttackHitBox))
            {
                mob.KillEnemy();
            }
        }
    }

    public override void UpdateState()
    {
        Player.CurrentAction = PlayerStatus.Attack;
    }
}

