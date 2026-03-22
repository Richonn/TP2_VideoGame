using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.Layouts;

/// <summary>
/// Registers HID layouts for Switch controllers not natively supported by Unity Input System.
/// Must be initialized before any input is processed.
/// </summary>
public static class SwitchControllerSupport
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
#endif
    public static void Initialize()
    {
        // PowerA NSW Nano Wired Controller (USB, vendor 0x20D6, product 0xA718)
        InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(
            new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x20D6)
                .WithCapability("productId", 0xA718));

        // PDP Afterglow Deluxe+ Audio Wired Controller (vendor 0x0E6F, product 0x0188).
        // Uses a custom layout (PDPAfterglowGamepadHID) instead of SwitchProControllerHID because
        // this device has MaxOutputReportSize=0 — the Switch handshake output reports crash macOS.
        InputSystem.RegisterLayout<PDPAfterglowGamepadHID>(
            matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x0E6F)
                .WithCapability("productId", 0x0188));
    }
}
