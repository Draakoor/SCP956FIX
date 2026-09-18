using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using UnityEngine;

namespace SCP956Plugin.SCP956
{
    /// <summary>Audio owned by one SCP-956 instance; cleaned up with its schematic.</summary>
    internal sealed class PinataAudio : IDisposable
    {
        private static readonly HashSet<string> loaded = new HashSet<string>();
        private readonly List<AudioPlayer> players = new List<AudioPlayer>();
        private readonly int instance;
        private int sequence;
        private AudioPlayer loop;
        internal PinataAudio(int instanceId) { instance = instanceId; }

        internal static void Load(Config config)
        {
            loaded.Clear();
            foreach (string clip in new[] { "Pinata_attack", "Pinata_loop", "Pinata_trigger_35", "Pinata_trigger_200" })
            {
                string path = Path.Combine(Paths.Configs, config.AudioDirectory, clip + ".ogg");
                string key = "technikbudde.scp956." + clip;
                try
                {
                    if (File.Exists(path) && (AudioClipStorage.AudioClips.ContainsKey(key) || AudioClipStorage.LoadClip(path, key))) loaded.Add(clip);
                    else Log.Warn("SCP-956 audio missing: " + path);
                }
                catch (Exception ex) { Log.Error("SCP-956 audio " + clip + ": " + ex.Message); }
            }
            Log.Info("SCP-956 audio loaded: " + loaded.Count + "/4 clips.");
        }

        internal void StartLoop(Vector3 position)
        {
            StopLoop();
            loop = Play("Pinata_loop", position, repeat: true);
        }
        internal void StopLoop() { if (loop != null) { players.Remove(loop); loop.Destroy(); loop = null; } }
        internal void Move(Vector3 position)
        {
            if (loop != null) foreach (var speaker in loop.SpeakersByName.Values) speaker.Position = position;
        }
        internal AudioPlayer Play(string clip, Vector3 position, List<ReferenceHub> owners = null, bool repeat = false)
        {
            if (!loaded.Contains(clip)) return null;
            players.RemoveAll(p => p == null);
            var player = AudioPlayer.CreateOrGet("SCP956-" + instance + "-" + (++sequence), destroyWhenAllClipsPlayed: !repeat,
                sendSoundGlobally: owners == null, owners: owners,
                onIntialCreation: p => {
                    var speaker = p.AddSpeaker("Pinata", position, 1f, owners == null, 3f, 24f);
                    speaker.Base.IsStatic = false;
                });
            player.AddClip("technikbudde.scp956." + clip, volume: Mathf.Clamp01(SCP956Plugin.Instance.Config.AudioVolume) * (repeat ? .4f : 1f), loop: repeat);
            players.Add(player);
            return player;
        }
        public void Dispose()
        {
            foreach (var player in players) if (player != null) player.Destroy();
            players.Clear(); loop = null;
        }
    }
}
