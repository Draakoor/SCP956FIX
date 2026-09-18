Compatibility release for SCP:SL 14.2.7, EXILED 9.14.2 and ProjectMER 2026.7.6.1.

- Migrated obsolete MapEditorReborn APIs to ProjectMER.
- Added round lifecycle cleanup and corrected model handling.
- Added four operator-provided positional/targeting sound cues; corrected targeting, damage and restoration of temporary effects.

Built against the actual server references. Model loading and round spawning verified on the Linux server; real-player interaction/audio playtesting remains necessary.

Install the ZIP folders relative to the server .config directory. Matching EXILED, ProjectMER and AudioPlayerApi dependencies are required.
