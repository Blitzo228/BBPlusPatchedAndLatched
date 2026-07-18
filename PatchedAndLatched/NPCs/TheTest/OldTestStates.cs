using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches.OldTheTest
{
    public abstract class LookAtGuy_OldBaseState : NpcState
    {
        protected LookAtGuy theTest;
        protected Transform headTransform;

        public LookAtGuy_OldBaseState(LookAtGuy theTest, Transform headTransform) : base(theTest)
        {
            this.theTest = theTest;
            this.headTransform = headTransform;
        }

        public override void Sighted()
        {
            base.Sighted();
            theTest.FreezeNPCs(true);
        }

        public override void Unsighted()
        {
            base.Unsighted();
            theTest.FreezeNPCs(false);
        }
    }

    public class LookAt_OldInactive : LookAtGuy_OldBaseState
    {
        public LookAt_OldInactive(LookAtGuy theTest, Transform headTransform) : base(theTest, headTransform) { }

        public override void Enter()
        {
            base.Enter();
            npc.behaviorStateMachine.ChangeNavigationState(new NavigationState_DoNothing(npc, 127, true));
        }

        public override void Sighted()
        {
            base.Sighted();
            npc.behaviorStateMachine.ChangeState(new LookAt_OldActivating(theTest, headTransform));
        }
    }

    public class LookAt_OldActivating : LookAtGuy_OldBaseState
    {
        protected float time = 5f;

        public LookAt_OldActivating(LookAtGuy theTest, Transform headTransform) : base(theTest, headTransform) { }

        public override void Enter()
        {
            base.Enter();
            theTest.Activate();
        }

        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime;
            if (time <= 0f)
                npc.behaviorStateMachine.ChangeState(new LookAt_OldActive(theTest, headTransform));
        }
    }

    public class LookAt_OldActive : LookAtGuy_OldBaseState
    {
        protected bool charging;
        protected float pressure;
        protected float maxPressure = 20f;
        protected float time;
        protected float maxTime = 10f;
        protected bool seesPlayer;
        protected bool playerSees;
        protected float minHeadHeight = 4f;
        protected float maxHeadHeight = 4.4f;
        protected float num;
        protected bool up;

        public LookAt_OldActive(LookAtGuy theTest, Transform headTransform) : base(theTest, headTransform) { }

        public override void Update()
        {
            base.Update();

            if (PatchedAndLatchedPlugin.EnableNewTestFeatures.Value)
            {
                num = Mathf.Sin(npc.ec.SurpassedRealTime * (1f + pressure / maxPressure * 24f)) / 2f * (pressure / maxPressure) + 0.5f;
                Vector3 pos = headTransform.localPosition;
                pos.y = Mathf.Lerp(minHeadHeight, maxHeadHeight, num);
                headTransform.localPosition = pos;

                var audios = theTest.GetComponentsInChildren<AudioManager>();
                foreach (var am in audios)
                    am.pitchModifier = pressure / 15f + 1f;

                if (num > 0.5f && !up) up = true;
                else if (num <= 0.5f && up) up = false;
            }

            if (!seesPlayer || !playerSees)
            {
                if (pressure > 0f) pressure -= Time.deltaTime * npc.TimeScale * 5f;
                if (time > 0f) time -= Time.deltaTime * npc.TimeScale * 0.75f;
            }
        }

        // Исправленная сигнатура
        public override void OnStateTriggerEnter(Entity otherEntity, Collider other, bool validCollision)
        {
            base.OnStateTriggerEnter(otherEntity, other, validCollision);
            if (!validCollision || other == null) return;

            if (other.CompareTag("Player") && !other.GetComponent<PlayerManager>().Tagged)
            {
                theTest.Blind();
                var audios = theTest.GetComponentsInChildren<AudioManager>();
                foreach (var am in audios)
                    am.pitchModifier = 1f;
            }
        }

        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            seesPlayer = true;
            if (!npc.looker.IsVisible)
            {
                playerSees = false;
                if (!charging && !player.Tagged)
                {
                    charging = true;
                    theTest.FleePlayer(player);
                }
                return;
            }

            if (PatchedAndLatchedPlugin.OldTestFastForward.Value && npc.ec.RemainingTime > 0)
                npc.ec.SetTimeLimit(npc.ec.RemainingTime - Time.deltaTime * 1.35f);

            if (charging)
            {
                charging = false;
                theTest.UpdateHeadPosition(0f);
            }

            if (PatchedAndLatchedPlugin.EnableNewTestFeatures.Value)
            {
                time += Time.deltaTime;
                if (time >= maxTime)
                {
                    pressure += Time.deltaTime * 4f;
                    if (pressure >= maxPressure && !player.Tagged)
                    {
                        var fleeingField = AccessTools.Field(typeof(LookAtGuy), "fleeing");
                        if (fleeingField != null) fleeingField.SetValue(theTest, true);
                        theTest.Blind();
                        var audios = theTest.GetComponentsInChildren<AudioManager>();
                        foreach (var am in audios)
                            am.pitchModifier = 1f;
                        pressure -= 1f;
                    }
                }
            }
        }

        public override void PlayerLost(PlayerManager player)
        {
            base.PlayerLost(player);
            seesPlayer = false;

            foreach (var bob in Object.FindObjectsOfType<PickupBobValue>())
                if (bob.speed == 0f) bob.speed = 5f;

            foreach (var ent in Object.FindObjectsOfType<Entity>())
                if (ent.Frozen && ent.GetComponent<PlayerManager>() == null)
                    ent.SetFrozen(false);

            if (PatchedAndLatchedPlugin.OldTestDisappear.Value)
            {
                var hall = npc.ec.mainHall;
                var cell = hall.cells[Random.Range(0, hall.cells.Count)];
                npc.Navigator.Entity.Teleport(cell.CenterWorldPosition);
                var audios = theTest.GetComponentsInChildren<AudioManager>();
                foreach (var am in audios)
                    am.FlushQueue(true);
            }
        }
    }

    public class LookAt_OldBlinding : LookAtGuy_OldBaseState
    {
        protected HudGauge gauge;
        protected float time;
        protected float totalTime;

        public LookAt_OldBlinding(LookAtGuy theTest, Transform headTransform, HudGauge gauge, float time) : base(theTest, headTransform)
        {
            this.gauge = gauge;
            this.time = time;
            totalTime = time;
        }

        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime * npc.TimeScale;
            gauge.SetValue(totalTime, time);
            if (time <= 0f)
                theTest.Respawn();
        }

        public override void Sighted() { }
        public override void Unsighted() { }

        public override void Exit()
        {
            base.Exit();
            gauge.Deactivate();
        }
    }
}