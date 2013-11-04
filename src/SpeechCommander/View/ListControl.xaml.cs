using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpeechCommander.View
{
    /// <summary>
    /// Interaction logic for ListControl.xaml
    /// </summary>
    public partial class ListControl : UserControl
    {

        #region Dependency Properties
        public string ListName
        {
            get { return (string)GetValue(ListNameProperty); }
            set { SetValue(ListNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ListName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListNameProperty = DependencyProperty.Register("ListName", typeof(string), typeof(ListControl));

        public System.Collections.IEnumerable List
        {
            get { return (System.Collections.IEnumerable)GetValue(ListProperty); }
            set { SetValue(ListProperty, value); }
        }
        // Using a DependencyProperty as the backing store for List.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListProperty = DependencyProperty.Register("List", typeof(System.Collections.IEnumerable), typeof(ListControl));

        public object SelectedItem
        {
            get 
            {
                Console.WriteLine("Get");
                return (object)GetValue(SelectedItemProperty); 
            }
            set 
            { 
                Console.WriteLine("SET");
                SetValue(SelectedItemProperty, value);
            }
        }
        // Using a DependencyProperty as the backing store for CurrentItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(ListControl));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ListControl));

        public string ItemName
        {
            get { return (string)GetValue(ItemNameProperty); }
            set { SetValue(ItemNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ItemName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemNameProperty = DependencyProperty.Register("ItemName", typeof(string), typeof(ListControl));
        #endregion

        #region Commands
        public ActionCommand AddItemCommand
        {
            get { return (ActionCommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for AddItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddItemCommandProperty = DependencyProperty.Register("AddItemCommand", typeof(ActionCommand), typeof(ListControl));

        public ActionCommand RemoveItemCommand
        {
            get { return (ActionCommand)GetValue(RemoveItemCommandProperty); }
            set { SetValue(RemoveItemCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for RemoveItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveItemCommandProperty = DependencyProperty.Register("RemoveItemCommand", typeof(ActionCommand), typeof(ListControl));

        public ActionCommand RenameItemCommand
        {
            get { return (ActionCommand)GetValue(RenameItemCommandProperty); }
            set { SetValue(RenameItemCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for RenameItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenameItemCommandProperty = DependencyProperty.Register("RenameItemCommand", typeof(ActionCommand), typeof(ListControl));
        #endregion

        public ListControl()
        {
            InitializeComponent();
        }
    }
}
