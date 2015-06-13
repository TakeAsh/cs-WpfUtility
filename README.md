# WpfUtility
- Utility classes for WPF

## Items
- ResourceHelper returns resource from the CallingAssembly.
- RibbonExtensionMethods add helper methods to Ribbon.
- SoftBreak realize 'Word Break' and 'Soft Hyphen' in Label of RibbonButton.

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

