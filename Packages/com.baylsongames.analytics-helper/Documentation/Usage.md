# Usage

This package records Unity Analytics custom events from local `AnalyticsEventDefinition` assets. Unity Cloud Dashboard remains the source of truth for schemas. The asset exists to keep game-side event calls consistent and designer friendly.

## Setup

1. Install this package in the Unity project.
2. Confirm the project is linked to Unity Gaming Services.
3. Install or resolve the package dependency for `com.unity.services.analytics`.
4. Decide where the host game initializes Unity Services and activates Analytics collection.
5. Create matching Analytics event schemas in Unity Cloud Dashboard.
6. Create local `AnalyticsEventDefinition` assets that match those Dashboard schemas exactly.

## Creating Events

Create the Dashboard schema before relying on a local event:

1. Open Unity Cloud Dashboard.
2. Go to Analytics > Event Manager.
3. Select Add New > Custom Event.
4. Enter the event name and description.
5. Add each parameter with the expected type.
6. Enable the event.
7. In Unity, create an event definition asset from Assets > Create > Baylson Games > Analytics > Event Definition.
8. Copy the Dashboard event name into the asset.
9. Add matching parameter names and types.
10. Use Tools > Baylson Games > Analytics > Validate Event Definitions before testing.

Unity Analytics event and parameter names are case-sensitive. Names must start with a letter and contain only English letters, numbers, and underscores. A local typo can still compile, but the backend can reject or ignore the event if the Dashboard schema does not match.

Dashboard events and parameters should be planned carefully. Unity allows existing custom events to be enabled or disabled, but event cleanup is limited once schemas exist. Use a test environment for schema experiments, then copy stable definitions to production.

## Activation Responsibility

This package does not initialize Unity Services and does not silently activate Analytics collection. Put that responsibility in the host game's startup/bootstrap flow, near other Unity Gaming Services initialization.

For Unity 6.2 and later with the current Analytics SDK, Unity documents the Developer Data consent path through `EndUserConsent` and `AnalyticsIntent`. If your project policy is that Analytics collection is allowed, grant that intent during startup after `UnityServices.InitializeAsync()` or before it, depending on your broader UGS boot sequence.

Keep this rule in mind: events recorded before Analytics collection is active are ignored by the SDK. They are not buffered and replayed later.

This package intentionally omits consent UI and data deletion helpers because this project assumes no PII collection. If that assumption changes, add consent and deletion flows in the host project before recording events.

## Recording From Designers

Use `AnalyticsEventEmitter` when an event can be triggered from a scene object:

1. Add `AnalyticsEventEmitter` to a GameObject.
2. Assign an `AnalyticsEventDefinition`.
3. Add parameter overrides for values that are specific to that scene object.
4. Enable Record On Start or Record On Enable only when the game bootstrap has already activated Analytics.
5. Otherwise, call the component's `Record()` method from UnityEvents, Timeline signals, UI Buttons, or other gameplay scripts.

Parameter overrides are strict. The override name must exist in the event definition, and the override type must match the definition type.

## Recording From Code

Use `AnalyticsRecorder` when gameplay code supplies runtime values:

```csharp
levelStarted.Record(
    AnalyticsParameterValueOverride.String("level_id", levelId),
    AnalyticsParameterValueOverride.Int("attempt", attempt));
```

or:

```csharp
AnalyticsRecorder.Record(
    levelStarted,
    AnalyticsParameterValueOverride.String("level_id", levelId));
```

Use defaults in the asset for stable values such as event source, mode, screen name, or feature flag. Use overrides for runtime values such as level ID, score bucket, attempt number, or selected option.

## Parameter Types

The helper supports the Unity Analytics custom event value types:

- `String`
- `Int`
- `Long`
- `Float`
- `Double`
- `Bool`
- `Timestamp`

Timestamp defaults can use the current UTC time or a specific ISO 8601 value. Prefer UTC timestamps.

## Validation And Export

Use Tools > Baylson Games > Analytics > Validate Event Definitions to scan all local event assets.

Use Tools > Baylson Games > Analytics > Export Event Schema Markdown to generate a Dashboard handoff document. This is useful when designers configure local definitions and another teammate mirrors the schema into Unity Cloud Dashboard.

Validation is intentionally local and lightweight. It catches naming rules, duplicate parameters, common reserved names, unsupported defaults, and required parameters that need runtime values. The Unity backend still performs final validation when events are uploaded.

## Debugging

Use Unity's Analytics Debug Panel while testing in the Editor. The Debug Panel only shows events recorded while the panel is open.

Use the Unity Cloud Dashboard Event Browser to inspect valid and invalid events that reach the service. Invalid events usually mean one of these is wrong:

- The custom event does not exist in Dashboard.
- The event name differs by case.
- A parameter name differs by case.
- A parameter value type does not match the Dashboard schema.
- Analytics collection was not active when the event was recorded.

## Data Hygiene

Do not put PII in event names, parameter names, or parameter values. Prefer enums, IDs that are not user identifiers, short categories, and stable buckets over free-form text. Avoid high-cardinality strings unless the Dashboard analysis genuinely needs them.
