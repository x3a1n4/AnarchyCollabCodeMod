using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.AnarchyCollab
{
  [CustomEntity("AnarchyCollab/NPC_Elevator")]
  public class NPCElevator : NPC
  {
    public bool ElevatorFunctional;
    private EntityID id;

    public NPCElevator(EntityData data, Vector2 offset, EntityID id)
      : base(data.Position + offset)
    {
      this.id = id;
      ElevatorFunctional = data.Bool("functional");
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Session session = (base.Scene as Level).Session;

      Add(Talker = new TalkComponent(
        new Rectangle(-48, -16, 96, 32),
        new Vector2(-0.5f, -16f),
        OnTalk));
    }

    private void OnTalk(Player player)
    {
      Audio.Play("event:/game/03_resort/deskbell_again", Position);
      Session session = (base.Scene as Level).Session;
      if (ElevatorFunctional && !session.GetFlag("elevator_" + id.ToString() + "_used"))
      {
        session.SetFlag("elevator_" + id.ToString() + "_used");
        Talker.Enabled = false;
        base.Scene.Add(new ElevatorCutscene(player));
      }
    }

    private IEnumerator ElevatorRoutine(Player player)
    {
      Level level = base.Scene as Level;
      Session session = level.Session;
      session.SetFlag("elevator_" + id.ToString() + "_used");
      Talker.Enabled = false;
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
