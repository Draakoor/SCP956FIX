using System.Linq;
using Exiled.API.Features;
using ProjectMER.Events.Handlers;
using MEC;
using SCP956Plugin.Handlers;
using SCP956Plugin.SCP956;
using System;
using UnityEngine;
using Exiled.Events.Handlers;
using SchematicHan = SCP956Plugin.Handlers.SchematicHandler;
using PlayerHan = SCP956Plugin.Handlers.PlayerHandler;

namespace SCP956Plugin
{
    public class SCP956Plugin : Plugin<Config>
    {
        public override string Name => "SCP956FIX";
        public override string Prefix => "scp956";
        public override string Author => "Draakoor / original SCP-956 contributors";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredExiledVersion => new Version(9, 14, 2);
        public static SCP956Plugin Instance { get; private set; }

        private SchematicHandler SchematicHandler;
        private PlayerHandler PlayerHandler;

        public override void OnEnabled()
        {
            Instance = this;
            RegisterEvent();
            PinataAudio.Load(Config);
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Cleanup();
            UnRegisterEvent();
            Instance = null;
            base.OnDisabled();
        }

        void RegisterEvent()
        {
            SchematicHandler = new SchematicHan();
            PlayerHandler = new PlayerHan();

            Schematic.SchematicSpawned += SchematicHandler.OnSpawn;
            Schematic.SchematicDestroyed += SchematicHandler.OnDestroy;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Cleanup;
            Exiled.Events.Handlers.Player.Shot += PlayerHandler.OnShot;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd;
            Exiled.Events.Handlers.Player.ChangingItem += PlayerHandler.ChangeItem;
            Exiled.Events.Handlers.Player.DroppingItem += PlayerHandler.DropItem;
            Exiled.Events.Handlers.Player.InteractingDoor += PlayerHandler.InteractDoor;
            Exiled.Events.Handlers.Player.InteractingElevator += PlayerHandler.InteractElevator;
            Exiled.Events.Handlers.Player.InteractingLocker += PlayerHandler.InteractLocker;
            Exiled.Events.Handlers.Player.Jumping += PlayerHandler.Jump;
            Exiled.Events.Handlers.Player.Left += PlayerHandler.OnQuit;
            Exiled.Events.Handlers.Player.Shooting += PlayerHandler.OnShooting;
            if (Config.Scp956CanHit)
            {
                if (Config.DamagableTypes.Contains(Exiled.API.Enums.DamageType.Explosion))
                {
                    Exiled.Events.Handlers.Map.ExplodingGrenade += PlayerHandler.OnGrenadeExplode;
                }
            }
        }

        void UnRegisterEvent()
        {
            Schematic.SchematicSpawned -= SchematicHandler.OnSpawn;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Cleanup;
            Exiled.Events.Handlers.Player.Shot -= PlayerHandler.OnShot;
            Schematic.SchematicDestroyed -= SchematicHandler.OnDestroy;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
            Exiled.Events.Handlers.Player.ChangingItem -= PlayerHandler.ChangeItem;
            Exiled.Events.Handlers.Player.DroppingItem -= PlayerHandler.DropItem;
            Exiled.Events.Handlers.Player.InteractingDoor -= PlayerHandler.InteractDoor;
            Exiled.Events.Handlers.Player.InteractingElevator -= PlayerHandler.InteractElevator;
            Exiled.Events.Handlers.Player.InteractingLocker -= PlayerHandler.InteractLocker;
            Exiled.Events.Handlers.Player.Jumping -= PlayerHandler.Jump;
            Exiled.Events.Handlers.Player.Left -= PlayerHandler.OnQuit;
            Exiled.Events.Handlers.Player.Shooting -= PlayerHandler.OnShooting;
            if (Config.Scp956CanHit)
            {
                if (Config.DamagableTypes.Contains(Exiled.API.Enums.DamageType.Explosion))
                {
                    Exiled.Events.Handlers.Map.ExplodingGrenade -= PlayerHandler.OnGrenadeExplode;
                }
            }

            SchematicHandler = null;
            PlayerHandler = null;
        }

        void OnRoundStart()
        {
            Cleanup();
            for (int i = 0; i < Math.Max(0, Math.Min(8, Config.MaximumCountOfScp956)); i++)
            {
                if (Config.SchematicName.Length == 0 || !ProjectMER.Features.ObjectSpawner.TrySpawnSchematic(Config.SchematicName[UnityEngine.Random.Range(0, Config.SchematicName.Length)], new Vector3(0f, -300f, 0f), Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), Vector3.one, out _))
                    Log.Error("SCP-956: configured ProjectMER schematic is missing.");
            }
        }

        void Cleanup()
        {
            foreach (var ai in SchematicHan.aIs.ToArray())
                if (ai != null) UnityEngine.Object.Destroy(ai.gameObject);
            SchematicHan.aIs.Clear();
            SCP956AI.DoorList.Clear();
        }

        void OnRoundEnd(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            Cleanup();
        }
    }
}
