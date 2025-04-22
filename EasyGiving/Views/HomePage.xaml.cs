using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics.Metrics;
using Windows.Globalization.NumberFormatting;
using System.Diagnostics;
using static EasyGiving.ViewModel.HomePage.EnchsViewModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;

namespace EasyGiving.Views
{
    public sealed partial class HomePage : Page
    {
        public Model.HomePage.ToolList toolList;
        public ObservableCollection<string> ToolMaterial_List { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ToolList_List { get; set; } = new ObservableCollection<string>();
        private string MaterialSelect_Item = string.Empty; // 选中的原材料
        private string Selected_Tool_ItemID = string.Empty; // 选中的物品id
        private bool ToolIsUnbreakable = false; // 是否在nbt中添加unbreakable
        public Model.HomePage.Enchantments Enchs;
        public ObservableCollection<string> EnchsList = new ObservableCollection<string>();
        public ViewModel.HomePage.EnchsViewModel SelectedEnchs = new ViewModel.HomePage.EnchsViewModel();
        private static bool IsTipShowed = false;
        public HomePage()
        {
            this.InitializeComponent();
            this.Loaded += HomePage_Loaded;
            this.toolList = new Model.HomePage.ToolList();
            this.ToolMaterial_List = new ObservableCollection<string>(this.toolList.GetAllMaterialsDisplayName());
            this.MaterialSelect.ItemsSource = ToolMaterial_List;
            this.Enchs = new Model.HomePage.Enchantments();
            this.EnchsList = new ObservableCollection<string>(this.Enchs.GetEnchsNames());
            this.Enchs_Search.ItemsSource = this.EnchsList;
        }

        private void StartScreenDialog()
        {
            StackPanel _dialogStackPanel = new StackPanel();
            TextBlock _dialogText1 = new TextBlock();
            _dialogText1.Text = "礦藝之中，聊天框之字符之最曰贰佰伍拾陆。若Befehl之长远超聊天框所纳，令郎可敲以下Befehl于聊天框：";
            _dialogText1.IsTextSelectionEnabled = true;
            _dialogText1.TextWrapping = TextWrapping.Wrap;
            TextBox _dialogText2 = new TextBox();
            _dialogText2.Text = "/give @s command_block";
            _dialogText2.IsReadOnly = true;
            _dialogText2.Width = 220;
            _dialogText2.HorizontalAlignment = HorizontalAlignment.Left;
            TextBlock _dialogText3 = new TextBlock();
            _dialogText3.Text = "然后，令郎方可将Befehl放于Befehl方块之中，便可以按钮激活Befehl方块，得之金刚稿等上上品。";
            _dialogText3.IsTextSelectionEnabled = true;
            _dialogText3.TextWrapping = TextWrapping.Wrap;
            _dialogStackPanel.Children.Add(_dialogText1);
            _dialogStackPanel.Children.Add(_dialogText2);
            _dialogStackPanel.Children.Add(_dialogText3);
            var dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Title = "海記憶體知己，天涯若比鄰";
            dialog.Content = _dialogStackPanel;
            dialog.PrimaryButtonText = "取消";
            dialog.CloseButtonText = "OK";
            dialog.DefaultButton = ContentDialogButton.Close;
            dialog.PrimaryButtonClick += (ContentDialog sender, ContentDialogButtonClickEventArgs args) => sender.Hide();
            _ = dialog.ShowAsync();
            IsTipShowed = true;
        }

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.PageTitleIcon.Height = this.PageTitleTextPanel.ActualHeight + 8;
            this.EnchBookIcon.Height = this.EnchBox.ActualHeight + 4;
            this.GiveCmd_CopyBtn.Height = this.GiveCmd.ActualHeight;
            this.AddedEnchs.Width = this.MainScroller.ActualWidth;
            if (IsTipShowed == false) this.StartScreenDialog();
        }

        private void MaterialSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string eName = e.AddedItems[0].ToString();
            this.ToolsSelect.ItemsSource = null;
            this.ToolsSelect.Items.Clear();
            this.MaterialSelect_Item = eName;
            this.ToolList_List = new ObservableCollection<string>(this.toolList.GetAllToolsDisplayNameFromMaterialDisplayName(eName));
            this.ToolsSelect.ItemsSource = this.ToolList_List;
        }

        private void ToolsSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            string eName = e.AddedItems[0].ToString();
            this.Selected_Tool_ItemID = this.toolList.GetItemIDFromSelectedItem(this.MaterialSelect_Item, eName);
            //this.selected_text.Text = this.Selected_Tool_ItemID;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e) => this.ToolIsUnbreakable = (sender as ToggleButton).IsChecked == true;

        private async Task ShowDialog(string Title, string Content)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Title = Title;
            dialog.Content = Content;
            dialog.PrimaryButtonText = "取消";
            dialog.CloseButtonText = "OK";
            dialog.DefaultButton = ContentDialogButton.Close;
            dialog.PrimaryButtonClick += (ContentDialog sender, ContentDialogButtonClickEventArgs args) => sender.Hide();
            await dialog.ShowAsync();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = this.EnchsList.Where(i => i.Contains(sender.Text));
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null) sender.Text = (string)(args.ChosenSuggestion); //从提示框中选择某一项时触发
            else await AddOneEnch(args.QueryText, false); //用户在输入时敲回车或者点击右边按钮确认输入时触发
        }

        private async Task AddOneEnch(string ench, bool? showDialogIfSuccess = true)
        {
            if (this.EnchsList.IndexOf(ench) == -1) await this.ShowDialog("❌错误", "不是正确的minecraft:Enchantments");
            else
            {
                try
                {
                    this.SelectedEnchs.Add(this.Enchs.GetEnchIDFromEnchName(ench), ench, "1");
                    this.UpdateAddedEnchsList();
                    if (showDialogIfSuccess == true) await this.ShowDialog("AutoSuggestBox", EnchsList.IndexOf(ench).ToString());
                }
                catch (EnchAlreadyExistsException eaee)
                {
                    await this.ShowDialog("提示", "已经添加过了对吧");
                }
                catch (Exception ex)
                {
                    Debug.Write($"来自EasyGiving.Views.HomePage.AddOneEnch()的消息：“{ex}”\n");
                    await this.ShowDialog("EasyGiving.Views.HomePage.AddOneEnch()", ex.ToString());
                }
            }
        }

        private void UpdateAddedEnchsList()
        {
            this.AddedEnchs.ItemsSource = null;
            this.AddedEnchs.Items.Clear();
            this.AddedEnchs.ItemsSource = this.SelectedEnchs.AddedEnchs;
            Debug.Write("来自EasyGiving.Views.HomePage.UpdateAddedEnchsList()的消息：“列表更新了”\n");
        }

        private void AddAllEnchsBtn_OK_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedEnchs.Clear();
            this.UpdateAddedEnchsList();
            foreach (KeyValuePair<string, string> en in this.Enchs.GetAllEnchsAndID())
            {
                this.AddAllEnchsBtn_Flyout.Hide();
                if (en.Key == "binding_curse" || en.Key == "vanishing_curse") continue;
                this.SelectedEnchs.Add(en.Key, en.Value, "1");
                this.UpdateAddedEnchsList();
            }
        }

        private void AddAllEnchsBtn_Cancel_Click(object sender, RoutedEventArgs e) => this.AddAllEnchsBtn_Flyout.Hide();

        private void AllEnch255Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            this.AllEnch255Btn_Flyout.Hide();
            this.SelectedEnchs.All255();
            this.UpdateAddedEnchsList();
        }

        private void AllEnch255Btn_Cancel_Click(object sender, RoutedEventArgs e) => this.AllEnch255Btn_Flyout.Hide();

        private void DelAllEnchsBtn_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedEnchs.Clear();
            this.UpdateAddedEnchsList();
        }

        private void GenGiveBtn_Click(object sender, RoutedEventArgs e)
        {
            string cmd = this.MakeCommand();
            this.GiveCmd.Text = cmd;
        }

        private void EnchLvlBox_LostFocus(object sender, RoutedEventArgs e)
        {
            NumberBox numBox = (NumberBox)sender;
            try
            {
                if (numBox.Text == string.Empty) numBox.Text = "1";
                else if (int.Parse(numBox.Text) < 1) numBox.Text = "1";
                else if (int.Parse(numBox.Text) > 255) numBox.Text = "255";
            }
            catch (Exception ex)
            {
                _ = this.ShowDialog("EasyGiving.Views.HomePage.EnchLvlBox_LostFocus()", ex.ToString());
                if (numBox != null) numBox.Text = "1";
            }
            finally
            {
                int EnchIndex = SelectedEnchs.GetEnchGuidAndIndexByID(numBox.Tag.ToString()).Item2;
                if (EnchIndex != -1) this.SelectedEnchs.AddedEnchs[EnchIndex].EnchLvl = Convert.ToByte(numBox.Text);
                UpdateAddedEnchsList();
            }
        }

        private void DelEnchBtn_Click(object sender, RoutedEventArgs e) // 删除一个附魔
        {
            Guid _guid = this.SelectedEnchs.GetEnchGuidAndIndexByID(((Button)sender).Tag.ToString()).Item1;
            this.SelectedEnchs.DelOneEnch(_guid);
            UpdateAddedEnchsList();
        }

        private string MakeCommand()
        {
            if (string.IsNullOrEmpty(this.Selected_Tool_ItemID))
            {
                _ = this.ShowDialog("EasyGiving.Views.HomePage.MakeCommand()", "武器未选择");
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(TargetPlayer.Text))
            {
                _ = this.ShowDialog("EasyGiving.Views.HomePage.MakeCommand()", "目标玩家或实体为空");
                return string.Empty;
            }
            string result = string.Empty;
            result += $"give {this.TargetPlayer.Text} {this.Selected_Tool_ItemID}";
            if (this.ToolIsUnbreakable == false && this.SelectedEnchs.AddedEnchs.Count == 0) return result;
            result += "{";
            if (this.ToolIsUnbreakable == true) result += "Unbreakable:1,";
            if (this.SelectedEnchs.AddedEnchs.Count == 0) return result + "}";
            result += "Enchantments:[";
            for (var i = 0; i < this.SelectedEnchs.AddedEnchs.Count; i++)
            {
                result += $"{{id:{this.SelectedEnchs.AddedEnchs[i].ID},lvl:{this.SelectedEnchs.AddedEnchs[i].EnchLvl.ToString()}}}";
                if (i != this.SelectedEnchs.AddedEnchs.Count - 1) result += ",";
            }
            result += "]}";
            return result;
        }

        private void GiveCmd_CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.GiveCmd.Text)) return;
            DataPackage dataPackage = new();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(GiveCmd.Text);
            Clipboard.SetContent(dataPackage);
            _ = this.ShowDialog("EasyGiving.Views.HomePage.GiveCmd_CopyBtn_Click()", "复制成功");
        }
    }
}
