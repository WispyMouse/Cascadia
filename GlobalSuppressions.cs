// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0031:Use null propagation", Justification = "Linter incorrectly identifies GameObjects for null propagation, but they're compatible. Disabling this check.", Scope = "member", Target = "~M:Cascadia.Unity.UI.SetupScene.ChooseEmojiCanvas.SelectEmoji(Cascadia.Unity.UI.SetupScene.ChooseEmojiIcon)")]
