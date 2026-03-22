using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Raw HID state struct for the PDP Afterglow Deluxe+ (vendor 0x0E6F, product 0x0188).
///
/// Format is 'HID' so Unity applies state events from this device directly without any
/// preprocessing — no unsafe code, no output reports sent.
///
/// Raw report layout (8 bytes, no report-ID byte):
///   Byte 0: face + shoulder buttons (Y=bit0, B=bit1, A=bit2, X=bit3, L=bit4, R=bit5, ZL=bit6, ZR=bit7)
///   Byte 1: menu + stick-press buttons (Minus=bit0, Plus=bit1, StickL=bit2, StickR=bit3)
///   Byte 2: hat switch (lower nibble, 0-7 = direction, 8 = neutral) — not mapped (left stick used for movement)
///   Byte 3: left stick X  (0-255, center=128)
///   Byte 4: left stick Y  (0-255, center=128)
///   Byte 5: right stick X (0-255, center=128)
///   Byte 6: right stick Y (0-255, center=128)
///   Byte 7: padding
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct PDPAfterglowHIDState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'I', 'D');

    // ── Byte 0: face buttons & shoulders ─────────────────────────────────────
    [InputControl(name = "buttonWest",    layout = "Button", bit = 0, displayName = "Y", shortDisplayName = "Y", usage  = "SecondaryAction")]
    [InputControl(name = "buttonSouth",   layout = "Button", bit = 1, displayName = "B", shortDisplayName = "B", usages = new[] { "Back", "Cancel" })]
    [InputControl(name = "buttonEast",    layout = "Button", bit = 2, displayName = "A", shortDisplayName = "A", usages = new[] { "PrimaryAction", "Submit" })]
    [InputControl(name = "buttonNorth",   layout = "Button", bit = 3, displayName = "X", shortDisplayName = "X")]
    [InputControl(name = "leftShoulder",  layout = "Button", bit = 4, displayName = "L",  shortDisplayName = "L")]
    [InputControl(name = "rightShoulder", layout = "Button", bit = 5, displayName = "R",  shortDisplayName = "R")]
    [InputControl(name = "leftTrigger",   layout = "Button", bit = 6, displayName = "ZL", shortDisplayName = "ZL")]
    [InputControl(name = "rightTrigger",  layout = "Button", bit = 7, displayName = "ZR", shortDisplayName = "ZR")]
    [FieldOffset(0)] public byte buttons0;

    // ── Byte 1: menu & stick press ────────────────────────────────────────────
    [InputControl(name = "selectButton",    layout = "Button", bit = 0, displayName = "Minus")]
    [InputControl(name = "startButton",     layout = "Button", bit = 1, displayName = "Plus", usage = "Menu")]
    [InputControl(name = "leftStickPress",  layout = "Button", bit = 2, displayName = "Left Stick")]
    [InputControl(name = "rightStickPress", layout = "Button", bit = 3, displayName = "Right Stick")]
    [FieldOffset(1)] public byte buttons1;

    // ── Byte 2: hat switch — skipped (left stick handles movement) ───────────
    [FieldOffset(2)] public byte hat;

    // ── Bytes 3-4: left stick ────────────────────────────────────────────────
    // All left-stick sub-controls are declared here (offset is relative to this field).
    [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "leftStick/x",     offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/left",  offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
    [InputControl(name = "leftStick/y",     offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/up",    offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/down",  offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(3)] public byte leftStickX;
    [FieldOffset(4)] public byte leftStickY;

    // ── Bytes 5-6: right stick ───────────────────────────────────────────────
    [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "rightStick/x",     offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/left",  offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
    [InputControl(name = "rightStick/y",     offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/up",    offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/down",  offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(5)] public byte rightStickX;
    [FieldOffset(6)] public byte rightStickY;

    [FieldOffset(7)] public byte padding;
}

/// <summary>
/// Custom Gamepad layout for the PDP Afterglow Deluxe+ Audio Wired Controller on macOS.
///
/// Why not SwitchProControllerHID:
///   That layout performs a Switch BT/USB handshake on every update by sending HID output
///   reports. This device reports MaxOutputReportSize=0 — the macOS HID driver crashes
///   instantly when an output report is written to such a device. Unity's own code
///   excludes it on macOS with #if !(UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) for exactly
///   this reason.
///
/// This class extends Gamepad directly with a custom state struct (format='HID') so Unity
/// applies the raw 8-byte HID report straight to the controls with no preprocessing and
/// no output ever sent.
/// </summary>
[InputControlLayout(stateType = typeof(PDPAfterglowHIDState), displayName = "PDP Afterglow Switch Controller")]
public class PDPAfterglowGamepadHID : Gamepad
{
    // No handshake, no output reports, no unsafe code.
    // Unity applies HID state events directly via the PDPAfterglowHIDState struct.
}
