using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.AnarchyCollab2022
{
  [CustomEntity("AnarchyCollab/NPC_Elevator")]
  public class ElevatorCutscene : CutsceneEntity
  {
    private Player player;

    public ElevatorCutscene(Player player)
    {
      this.player = player;
    }

    public override void OnBegin(Level level)
    {

    }

    public override void OnEnd(Level level)
    {
      throw new System.NotImplementedException();
    }

    private IEnumerator Cutscene(Level level)
    {
      Session session = level.Session;
      player.StateMachine.State = 11;
      player.StateMachine.Locked = true;
      session.SetFlag("elevator_door");
      Audio.SetAmbience("event:/env/amb/03_interior");

      yield return 0.5f;

      Input.Rumble(RumbleStrength.Medium, RumbleLength.FullSecond);
      // start long elevator

      yield return 1f;

      player.StateMachine.Locked = false;
      player.StateMachine.State = 0;
      Audio.SetMusic("event:/new_content/music/lv10/golden_room");
      level.Shake();
    }
  }
}
