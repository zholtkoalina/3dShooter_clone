using UnityEngine.UIElements;

namespace AutomationTests.TestFramework.Support
{
    /// <summary>Page object for the player's UI Toolkit HUD</summary>
    internal sealed class UIElements
    {
        private const string HealthBarName = "player-health-bar";
        private const string StaminaBarName = "player-stamina-bar";
        private const string WeaponNameLabel = "weapon-type-label";
        private const string WeaponIconName = "weapon-icon";
        private const string AmmoBarName = "player-ammo-bar";

        private readonly VisualElement root;

        public UIElements(VisualElement root)
        {
            this.root = root ?? throw new System.ArgumentNullException(nameof(root));
        }

        public ProgressBar HealthBar => root.Q<ProgressBar>(HealthBarName);
        public ProgressBar StaminaBar => root.Q<ProgressBar>(StaminaBarName);
        public Label WeaponName => root.Q<Label>(WeaponNameLabel);
        public Image WeaponIcon => root.Q<Image>(WeaponIconName);
        public ProgressBar AmmoBar => root.Q<ProgressBar>(AmmoBarName);
    }
}
