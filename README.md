# [WpfUtility](https://github.com/TakeAsh/cs-WpfUtility/wiki)
- Utility classes for WPF

## Items
- [MessageButton](https://github.com/TakeAsh/cs-WpfUtility/wiki/MessageButton) is an alternative 'MessageBox' that can be pinned on window as an icon.
- [ResourceHelper](https://github.com/TakeAsh/cs-WpfUtility/wiki/ResourceHelper) returns resource from the CallingAssembly.
- [RibbonExtensionMethods](https://github.com/TakeAsh/cs-WpfUtility/wiki/RibbonExtensionMethods) add helper methods to Ribbon.
- [SoftBreak](https://github.com/TakeAsh/cs-WpfUtility/wiki/SoftBreak) realize 'Word Break' and 'Soft Hyphen' in Label of RibbonButton.

## MessageButton
- It can be pinned on window as an icon.
- It show a text as popup with system sound.
- A text is closed automatically after several seconds, or opened infinitely.
- The popup can be opened/closed by click.
- MessageButton is not modal but modeless.
- MessageButton DON'T support buttons(OK, Cancel, Yes, No, ...).
- MessageButtons can be placed in Quick Access ToolBar, Help Pane Content, Ribbon Group.

## ResourceHelper
- returns resource from the CallingAssembly.

### GetImage
- returns BitmapImage with specified file name from the CallingAssembly.

### GetText
- returns string with specified file name and encoding from the CallingAssembly.
  - The text file msut be an embedded resource.

## RibbonExtensionMethods
- add helper methods to Ribbon.

### AddHelpItem
- add an item to HelpPaneContent of the Ribbon.

### AddMinimizeButton
- add the minimize button to HelpPaneContent of the Ribbon.

## SoftBreak
- realize 'Word Break' and 'Soft Hyphen' in Label of RibbonButton.
- 'Word Break' tag &#x5b;&#x5b;WBR]] will be space when the button is large, and it will be empty when the button is small.
- 'Soft Hyphen' tag &#x5b;&#x5b;SHY]] will be hyphen when the button is large, and it will be empty when the button is small.

### Constant
- WordBreakTag: string
- SoftHyphenTag: string

