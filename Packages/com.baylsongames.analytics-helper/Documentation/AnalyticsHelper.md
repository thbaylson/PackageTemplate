# Baylson Games Analytics Helper

Baylson Games Analytics Helper wraps Unity Analytics custom events with designer-friendly ScriptableObject event definitions. The package keeps event names, parameter names, and default values in assets while still recording through the official `com.unity.services.analytics` SDK.

The package is intentionally lightweight:

- Event schemas live in Unity Cloud Dashboard.
- Matching local definitions live in `AnalyticsEventDefinition` assets.
- Scene objects can record events through `AnalyticsEventEmitter`.
- Gameplay code can record events through `AnalyticsRecorder`.
- Editor validation catches common naming and type mistakes before entering Play Mode.

For detailed setup and workflow guidance, see [Usage](Usage.md).
